namespace ErniAcademy.Events.Contracts;

/// <summary>
/// Publisher of events
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publisher of events
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="event">The event to be publish</param>
    /// <param name="cancellationToken">An optional System.Threading.CancellationToken instance to signal the request to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous publish operation</returns>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : class, IEvent, new();

    /// <summary>
    /// Publisher of events
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="events">The events to be publish</param>
    /// <param name="cancellationToken">An optional System.Threading.CancellationToken instance to signal the request to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous publish operation</returns>
    Task PublishAsync<TEvent>(TEvent[] events, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent, new();
}
