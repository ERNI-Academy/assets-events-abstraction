namespace ErniAcademy.Events.Contracts;

/// <summary>
/// Subscriber of events
/// </summary>
public interface IEventSubscriber<TEvent>
    where TEvent : class, IEvent, new()
{
    void Subscribe(Func<TEvent, Task> handler);

    void UnSubscribe(Func<TEvent, Task> handler);

    Task StarProcessingAsync(CancellationToken cancellationToken = default);

    Task StopProcessingAsync(CancellationToken cancellationToken = default);
}