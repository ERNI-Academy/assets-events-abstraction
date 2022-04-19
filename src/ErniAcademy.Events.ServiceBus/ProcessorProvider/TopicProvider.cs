using Azure.Messaging.ServiceBus;
using ErniAcademy.Events.ServiceBus.ClientProvider;

namespace ErniAcademy.Events.ServiceBus.ProcessorProvider;

public class TopicProvider : IServiceBusProcessorProvider
{
    private readonly IServiceBusClientProvider _serviceBusClientProvider;
    private readonly string _subscriptionName;
    private readonly ServiceBusProcessorOptions _serviceBusProcessorOptions;

    public TopicProvider(
        IServiceBusClientProvider serviceBusClientProvider,
        string subscriptionName,
        ServiceBusProcessorOptions serviceBusProcessorOptions = null)
    {
        _serviceBusClientProvider = serviceBusClientProvider;
        _subscriptionName = subscriptionName;
        _serviceBusProcessorOptions = serviceBusProcessorOptions;
    }

    public ServiceBusProcessor GetProcessor(string eventName)
    {
        var client = _serviceBusClientProvider.GetClient();
        return _serviceBusProcessorOptions == null
             ? client.CreateProcessor(topicName: eventName, subscriptionName: _subscriptionName)
             : client.CreateProcessor(topicName: eventName, subscriptionName: _subscriptionName, _serviceBusProcessorOptions);
    }
}
