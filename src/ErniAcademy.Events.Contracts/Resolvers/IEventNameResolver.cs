namespace ErniAcademy.Events.Contracts;

/// <summary>
/// Resolver for the event name
/// </summary>
public interface IEventNameResolver
{
    /// <summary>
    /// Resolve by the generic type TEvent of the event
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns>event name</returns>
    string Resolve<TEvent>()
        where TEvent : class, IEvent, new();

    /// <summary>
    /// Resolve by the type of the event
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns>event name</returns>
    string Resolve<TEvent>(TEvent @event)
        where TEvent : class, IEvent, new();
}
