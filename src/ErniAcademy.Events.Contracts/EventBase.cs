namespace ErniAcademy.Events.Contracts;

public abstract class EventBase : IEvent
{
    protected EventBase()
        : this(eventType: null)
    {

    }

    protected EventBase(Guid correlationId)
        : this(Guid.NewGuid(), correlationId, null, DateTimeOffset.UtcNow)
    {
    }

    protected EventBase(string eventType)
        : this(Guid.NewGuid(), Guid.NewGuid(), eventType, DateTimeOffset.UtcNow)
    {
    }

    protected EventBase(Guid eventId, Guid correlationId, string eventType, DateTimeOffset createdAt)
    {
        EventId = eventId;
        CorrelationId = correlationId;
        EventType = eventType ?? GetType().FullName;
        CreatedAt = createdAt;
        Metadata = new Metadata();
    }

    public Guid EventId { get; set; }
    public Guid CorrelationId { get; set; }
    public string EventType { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Metadata Metadata { get; set; }

    public void AddMetadata(string key, string value) => Metadata.Values.Add(key, value);
}
