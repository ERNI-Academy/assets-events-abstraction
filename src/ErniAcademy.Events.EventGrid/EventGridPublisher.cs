using Azure.Messaging.EventGrid;
using ErniAcademy.Events.Contracts;
using Microsoft.Extensions.Options;
using ErniAcademy.Events.EventGrid.Configuration;
using ErniAcademy.Events.EventGrid.ClientProvider;
using ErniAcademy.Serializers.Contracts;

namespace ErniAcademy.Events.EventGrid;

public class EventGridPublisher : IEventPublisher
{
    private readonly EventGridPublisherClient _client;
    private readonly IEventNameResolver _eventNameResolver;
    private readonly ISerializer _serializer;
    private readonly IOptionsMonitor<PublisherOptions> _options;

    public EventGridPublisher(
        IEventGridPublisherClientProvider eventGridPublisherClientProvider, 
        IEventNameResolver eventNameResolver,
        ISerializer serializer,
        IOptionsMonitor<PublisherOptions> options)
    {
        _client = eventGridPublisherClientProvider.GetClient();
        _eventNameResolver = eventNameResolver;
        _serializer = serializer;
        _options = options;
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent, new() => PublishAsync(new TEvent[] { @event }, cancellationToken);

    public async Task PublishAsync<TEvent>(TEvent[] events,CancellationToken cancellationToken = default)
        where TEvent : class, IEvent, new()
    {
        var messages = new EventGridEvent[events.Length];
        for (int i = 0; i < events.Length; i++)
        {
            messages[i] = await BuildEvent(events[i]);
        }

        await _client.SendEventsAsync(messages, cancellationToken);
    }

    internal async Task<EventGridEvent> BuildEvent<TEvent>(TEvent @event) 
        where TEvent : class, IEvent, new()
    {
        await using var stream = new MemoryStream();
        await _serializer.SerializeToStreamAsync(@event, stream);

        var options = _options.CurrentValue;

        var eventName = _eventNameResolver.Resolve(@event);
        var subject = options.SubjectBasePath == null
                    ? eventName
                    : $"{options.SubjectBasePath}/{eventName}";

        var eventData = new EventGridEvent(subject, eventName, options.DataVersion, await BinaryData.FromStreamAsync(stream))
        {
            Id = @event.EventId.ToString(),
            Topic = eventName,
            EventTime = @event.CreatedAt
        };

        return eventData;
    }
}
