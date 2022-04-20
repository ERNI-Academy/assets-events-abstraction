using ErniAcademy.Events.Contracts;

namespace ErniAcademy.Events.Samples;

public class MyEvent : EventBase
{
    public MyEvent()
    {
    }

    public MyEvent(Guid correlationId) 
        : base(correlationId)
    {
    }

    public MyEvent(string eventType) 
        : base(eventType)
    {
    }

    public MyEvent(Guid eventId, Guid correlationId, string eventType, DateTimeOffset createdAt) 
        : base(eventId, correlationId, eventType, createdAt)
    {
    }

    public string MyCustomProperty { get; set; }
}
