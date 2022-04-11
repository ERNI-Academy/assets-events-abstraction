namespace ErniAcademy.Events.Contracts;

/// <summary>
/// Generic representation of an Event
/// </summary>
public interface IEvent
{
    /// <summary>
    /// Unique identifier of the event
    /// </summary>
    Guid EventId { get; set; }

    /// <summary>
    /// Event type
    /// </summary>
    string EventType { get; set; }

    /// <summary>
    /// Created at DateTimeOffset
    /// </summary>
    DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Metadata of the event
    /// </summary>
    Metadata Metadata { get; set; }

    /// <summary>
    /// Add metadata to the event
    /// </summary>
    /// <param name="key">key identifier of the metadata</param>
    /// <param name="value">value of the metadata</param>
    void AddMetadata(string key, string value);
}
