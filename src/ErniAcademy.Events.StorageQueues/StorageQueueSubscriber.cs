using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.StorageQueues.ClientProvider;
using ErniAcademy.Events.StorageQueues.Configuration;
using ErniAcademy.Events.StorageQueues.Extensions;
using ErniAcademy.Serializers.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ErniAcademy.Events.StorageQueues;

public class StorageQueueSubscriber<TEvent> : IEventSubscriber<TEvent>
    where TEvent : class, IEvent, new()
{
    private readonly Lazy<QueueClient> _queueClientLazy;
    private readonly ISerializer _serializer;
    private readonly IOptionsMonitor<QueueSubscriberOptions> _options;
    private readonly ILogger _logger;

    private Func<TEvent, Task> _processEventAsync;
    private Task _activeReceiveTask;
    private readonly SemaphoreSlim _processingStartStopSemaphore = new(1, 1);
    private CancellationTokenSource _runningTaskTokenSource;
    private readonly CancellationTokenSource _handlerCts = new();
    private List<(Task Task, CancellationTokenSource Cts)> _taskTuples = new();

    public StorageQueueSubscriber(
        IQueueClientProvider queueClientProvider,
        IEventNameResolver eventNameResolver,
        ISerializer serializer,
        ILoggerFactory loggerFactory,
        IOptionsMonitor<QueueSubscriberOptions> options)
    {
        var eventName = eventNameResolver.Resolve<TEvent>();

        _queueClientLazy = new Lazy<QueueClient>(() =>
        {
            return queueClientProvider.GetClient(eventName);
        });

        _serializer = serializer;
        _options = options;
        _logger = loggerFactory.CreateLogger(nameof(StorageQueueSubscriber<TEvent>));
    }

    public event Func<TEvent, Task> ProcessEventAsync
    {
        add
        {
            ArgumentNullException.ThrowIfNull(value, nameof(ProcessEventAsync));

            if (_processEventAsync != default)
            {
                throw new NotSupportedException("Handler has already been assigned");
            }

            EnsureNotRunningAndInvoke(() => _processEventAsync = value);
        }

        remove
        {
            ArgumentNullException.ThrowIfNull(value, nameof(ProcessEventAsync));

            if (_processEventAsync != value)
            {
                throw new ArgumentException("Handler has not been assigned");
            }

            EnsureNotRunningAndInvoke(() => _processEventAsync = default);
        }
    }

    public async Task StartProcessingAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested<TaskCanceledException>();
        bool releaseGuard = false;
        try
        {
            await _processingStartStopSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            releaseGuard = true;

            if (_activeReceiveTask == null)
            {
                _logger.LogInformation("StartProcessingAsync starts");

                if (_processEventAsync == null) throw new InvalidOperationException("ProcessEventAsync must be assigned");

                cancellationToken.ThrowIfCancellationRequested<TaskCanceledException>();

                _runningTaskTokenSource = new CancellationTokenSource();

                _activeReceiveTask = ReceiveEventAsync(_runningTaskTokenSource.Token);

                _logger.LogInformation("StartProcessingAsync complete");
            }
            else
            {
                throw new InvalidOperationException("Already processing");
            }
        }
        finally
        {
            if (releaseGuard)
            {
                _processingStartStopSemaphore.Release();
            }
        }
    }

    public async Task StopProcessingAsync(CancellationToken cancellationToken = default)
    {
        bool releaseGuard = false;
        try
        {
            await _processingStartStopSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            releaseGuard = true;

            if (_activeReceiveTask != null)
            {
                _logger.LogInformation("StopProcessingAsync starts");

                cancellationToken.ThrowIfCancellationRequested<TaskCanceledException>();

                // Cancel the current running task. Catch any exception that are triggered due to customer token registrations, and
                // log these as warnings as we don't want to forego stopping due to these exceptions.
                try
                {
                    _runningTaskTokenSource.Cancel();
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Processor stoping cancelling");
                }

                _runningTaskTokenSource.Dispose();
                _runningTaskTokenSource = null;

                // Now that a cancellation request has been issued, wait for the running task to finish.  In case something
                // unexpected happened and it stopped working midway, this is the moment we expect to catch an exception.
                try
                {
                    await _activeReceiveTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // Nothing to do here.  These exceptions are expected.
                }
                finally
                {
                    // If an unexpected exception occurred while awaiting the receive task, we still want to dispose and set to null
                    // as the task is complete and there is no use in awaiting it again if StopProcessingAsync is called again.
                    _activeReceiveTask.Dispose();
                    _activeReceiveTask = null;
                }
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Processor stoping cancel");
        }
        finally
        {
            if (releaseGuard)
            {
                _processingStartStopSemaphore.Release();
            }
        }

        _logger.LogInformation("StopProcessingAsync complete");
    }

    protected async Task ReceiveEventAsync(CancellationToken cancellationToken)
    {
        CancellationTokenSource linkedHandlerTcs = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _handlerCts.Token);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (linkedHandlerTcs.IsCancellationRequested)
                {
                    linkedHandlerTcs.Dispose();
                    linkedHandlerTcs = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _handlerCts.Token);
                    break;
                }

                var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, CancellationToken.None);

                var messages = await _queueClientLazy.Value.ReceiveMessagesAsync(cancellationToken: cancellationToken);

                if (messages.Value == null)
                {
                    continue;
                }

                foreach (var message in messages.Value)
                {
                    _taskTuples.Add((ProcessQueueMessageAsync(message, linkedCts), linkedCts));
                }

                CheckConcurrentTasks();
            }
        }
        finally
        {
            try
            {
                await Task.WhenAll(_taskTuples.Select(t => t.Task)).ConfigureAwait(false);
            }
            finally
            {
                foreach (var (_, cts) in _taskTuples)
                {
                    cts.Dispose();
                }

                _taskTuples.Clear();

                linkedHandlerTcs.Dispose();
            }
        }
    }

    private async Task ProcessQueueMessageAsync(QueueMessage message, CancellationTokenSource cancellationToken)
    {
        var @event = await _serializer.DeserializeFromStreamAsync<TEvent>(message.Body.ToStream(), cancellationToken.Token).ConfigureAwait(false);
        _logger.LogInformation("ReceiveEventAsync. Message receive MessageId:'{MessageId}' EventId:'{EventId}'", message.MessageId, @event.EventId);
        await _processEventAsync.Invoke(@event).ConfigureAwait(false);
        await _queueClientLazy.Value.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken.Token).ConfigureAwait(false);
    }

    private void CheckConcurrentTasks()
    {
        if (_taskTuples.Count > _options.CurrentValue.MaxConcurrentCalls)
        {
            List<(Task Task, CancellationTokenSource Cts)> remaining = new();
            foreach (var tuple in _taskTuples)
            {
                if (tuple.Task.IsCompleted)
                {
                    tuple.Cts.Dispose();
                }
                else
                {
                    remaining.Add(tuple);
                }
            }

            _taskTuples = remaining;
        }
    }

    internal void EnsureNotRunningAndInvoke(Action action)
    {
        if (_activeReceiveTask == null)
        {
            try
            {
                _processingStartStopSemaphore.Wait();
                if (_activeReceiveTask == null)
                {
                    action?.Invoke();
                }
                else
                {
                    throw new InvalidOperationException("Already processing");
                }
            }
            finally
            {
                _processingStartStopSemaphore.Release();
            }
        }
        else
        {
            throw new InvalidOperationException("Already processing");
        }
    }
}
