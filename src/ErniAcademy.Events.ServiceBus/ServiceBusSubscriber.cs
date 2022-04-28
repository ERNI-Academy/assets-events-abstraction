using Azure.Messaging.ServiceBus;
using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.ServiceBus.ProcessorProvider;
using ErniAcademy.Serializers.Contracts;
using Microsoft.Extensions.Logging;

namespace ErniAcademy.Events.ServiceBus;

public class ServiceBusSubscriber<TEvent> : IEventSubscriber<TEvent>
    where TEvent : class, IEvent, new()
{
    private readonly ServiceBusProcessor _processor;
    private readonly ISerializer _serializer;
    private readonly ILogger _logger;

    public ServiceBusSubscriber(
        IServiceBusProcessorProvider serviceBusProcessorProvider,
        IEventNameResolver eventNameResolver,
        ISerializer serializer,
        ILoggerFactory loggerFactory)
    {
        var eventName = eventNameResolver.Resolve<TEvent>();

        _processor = serviceBusProcessorProvider.GetProcessor(eventName);
        _serializer = serializer;
        _logger = loggerFactory.CreateLogger(nameof(ServiceBusSubscriber<TEvent>));
    }

    public event Func<TEvent, Task> ProcessEventAsync;

    public async Task StartProcessingAsync(CancellationToken cancellationToken = default)
    {
        _processor.ProcessMessageAsync += async args =>
        {
            var @event = await _serializer.DeserializeFromStreamAsync<TEvent>(args.Message.Body.ToStream(), cancellationToken);

            await ProcessEventAsync.Invoke(@event);

            await args.CompleteMessageAsync(args.Message, cancellationToken);
        };

        _processor.ProcessErrorAsync += args =>
        {
            _logger.LogError(args.Exception, "ProcessErrorAsync {ErrorSource} {EntityPath}", args.ErrorSource, args.EntityPath);
            return Task.CompletedTask;
        };

        await _processor.StartProcessingAsync(cancellationToken);
    }

    public Task StopProcessingAsync(CancellationToken cancellationToken = default) => _processor.StopProcessingAsync(cancellationToken);
}
