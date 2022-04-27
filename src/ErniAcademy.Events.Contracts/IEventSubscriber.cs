namespace ErniAcademy.Events.Contracts;

/// <summary>
/// Subscriber of events
/// </summary>
public interface IEventSubscriber<TEvent>
    where TEvent : class, IEvent, new()
{
    event Func<TEvent, Task> ProcessEventAsync;

    Task StartProcessingAsync(CancellationToken cancellationToken = default);

    Task StopProcessingAsync(CancellationToken cancellationToken = default);
}