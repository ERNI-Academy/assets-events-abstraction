namespace ErniAcademy.Events.Contracts;

/// <summary>
/// Subscriber of events
/// </summary>
public interface IEventSubscriber<TEvent>
    where TEvent : class, IEvent, new()
{
    public event Func<TEvent, Task> ProcessEventAsync;

    public event Func<Tuple<string, Exception>, Task> ProcessErrorAsync;

    Task StartProcessingAsync(CancellationToken cancellationToken = default);

    Task StopProcessingAsync(CancellationToken cancellationToken = default);
}