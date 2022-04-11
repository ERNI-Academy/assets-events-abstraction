namespace ErniAcademy.Events.Contracts;

public abstract class EventBase : IEvent
{
    protected EventBase()
        : this(eventType: null)
    {

    }

    protected EventBase(string eventType)
        : this(Guid.NewGuid(), eventType, DateTimeOffset.UtcNow)
    {
    }

    protected EventBase(Guid eventId, string eventType, DateTimeOffset createdAt)
    {
        EventId = eventId;
        EventType = eventType ?? GetType().FullName;
        CreatedAt = createdAt;
        Metadata = new Metadata();
    }

    public Guid EventId { get; set; }
    public string EventType { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Metadata Metadata { get; set; }

    public void AddMetadata(string key, string value) => Metadata.Values.Add(key, value);
}
