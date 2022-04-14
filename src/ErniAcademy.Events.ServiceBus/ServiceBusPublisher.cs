using Azure.Messaging.ServiceBus;
using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.ServiceBus.ClientProvider;
using ErniAcademy.Serializers.Contracts;
using System.Collections.Concurrent;

namespace ErniAcademy.Events.ServiceBus;

public class ServiceBusPublisher : IEventPublisher
{
    private readonly ServiceBusClient _client;
    private readonly ConcurrentDictionary<string, ServiceBusSender> _senders;
    private readonly IEventNameResolver _eventNameResolver;
    private readonly ISerializer _serializer;

    public ServiceBusPublisher(
        IServiceBusClientProvider serviceBusClientProvider, 
        IEventNameResolver eventNameResolver,
        ISerializer serializer)
    {
        _client = serviceBusClientProvider.GetClient();
        _eventNameResolver = eventNameResolver;
        _serializer = serializer;

        _senders = new ConcurrentDictionary<string, ServiceBusSender>();
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent, new() => PublishAsync(new TEvent[] { @event });

    public async Task PublishAsync<TEvent>(TEvent[] events,CancellationToken cancellationToken = default)
        where TEvent : class, IEvent, new()
    {
        var sender = GetSender(_eventNameResolver.Resolve<TEvent>());

        var messages = new ServiceBusMessage[events.Length];
        for (int i = 0; i < events.Length; i++)
        {
            messages[i] = await BuildMessage(events[i]);
        }

        await sender.SendMessagesAsync(messages, cancellationToken);
    }

    protected virtual ServiceBusSender GetSender(string queueOrTopicName) => _senders.GetOrAdd(queueOrTopicName, s => _client.CreateSender(queueOrTopicName));

    internal async Task<ServiceBusMessage> BuildMessage<TEvent>(TEvent @event) 
        where TEvent : class, IEvent, new()
    {
        await using var stream = new MemoryStream();
        await _serializer.SerializeToStreamAsync(@event, stream);

        var message = new ServiceBusMessage
        {
            ContentType = _serializer.ContentType
        };

        message.Body = await BinaryData.FromStreamAsync(stream);

        foreach (var metadata in @event.Metadata?.Values)
        {
            message.ApplicationProperties.Add(metadata.Key, metadata.Value);
        }

        return message;
    }
}
