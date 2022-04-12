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
    /// <returns>A task that represents the asynchronous publish operation</returns>
    Task PublishAsync<TEvent>(TEvent @event) 
        where TEvent : class, IEvent, new();

    /// <summary>
    /// Publisher of events
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="event">The events to be publish</param>
    /// <returns>A task that represents the asynchronous publish operation</returns>
    Task PublishAsync<TEvent>(TEvent[] @event)
        where TEvent : class, IEvent, new();
}
