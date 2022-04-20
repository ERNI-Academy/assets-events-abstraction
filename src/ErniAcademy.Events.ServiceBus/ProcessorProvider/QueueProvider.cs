using Azure.Messaging.ServiceBus;
using ErniAcademy.Events.ServiceBus.ClientProvider;

namespace ErniAcademy.Events.ServiceBus.ProcessorProvider;

/// <summary>
/// Provider to build a ServiceBusProcessor for ServiceBus queues
/// </summary>
public class QueueProvider : IServiceBusProcessorProvider
{
    private readonly IServiceBusClientProvider _serviceBusClientProvider;
    private readonly ServiceBusProcessorOptions _serviceBusProcessorOptions;

    public QueueProvider(
        IServiceBusClientProvider serviceBusClientProvider, 
        ServiceBusProcessorOptions serviceBusProcessorOptions = null)
    {
        _serviceBusClientProvider = serviceBusClientProvider;
        _serviceBusProcessorOptions = serviceBusProcessorOptions;
    }

    public ServiceBusProcessor GetProcessor(string eventName)
    {
        var client = _serviceBusClientProvider.GetClient();
        return _serviceBusProcessorOptions == null
             ? client.CreateProcessor(queueName: eventName)
             : client.CreateProcessor(queueName: eventName, _serviceBusProcessorOptions);
    }
}
