namespace ErniAcademy.Events.Contracts;

public class EventNameResolver : IEventNameResolver
{
    public string Resolve<TEvent>()
        where TEvent : class, IEvent, new() => typeof(TEvent).Name.ToLowerInvariant();

    public string Resolve<TEvent>(TEvent @event)
        where TEvent : class, IEvent, new() => @event.GetType().Name.ToLowerInvariant();
}
