using Azure.Messaging.ServiceBus;

namespace ErniAcademy.Events.ServiceBus.ProcessorProvider;

public interface IServiceBusProcessorProvider
{
    ServiceBusProcessor GetProcessor(string eventName);
}
