using Azure.Messaging.ServiceBus;

namespace ErniAcademy.Events.ServiceBus.ClientProvider;

/// <summary>
/// Contract to provide an instance of ServiceBusClient
/// </summary>
public interface IServiceBusClientProvider
{
    /// <summary>
    /// Get ServiceBusClient
    /// </summary>
    /// <returns>ServiceBusClient instance</returns>
    ServiceBusClient GetClient();
}
