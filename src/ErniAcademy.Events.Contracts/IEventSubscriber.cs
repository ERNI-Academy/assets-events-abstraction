namespace ErniAcademy.Events.Contracts;

/// <summary>
/// Subscriber of events
/// </summary>
public interface IEventSubscriber<TEvent>
    where TEvent : class, IEvent, new()
{
    Task SubscribeAsync(Func<TEvent, Task> handler, CancellationToken cancellationToken = default);

    Task UnSubscribeAsync(Func<TEvent, Task> handler, CancellationToken cancellationToken = default);

    Task StarProcessingAsync(CancellationToken cancellationToken = default);

    Task StopProcessingAsync(CancellationToken cancellationToken = default);
}