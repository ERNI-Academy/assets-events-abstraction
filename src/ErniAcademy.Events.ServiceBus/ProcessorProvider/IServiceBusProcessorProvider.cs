using Azure.Messaging.ServiceBus;

namespace ErniAcademy.Events.ServiceBus.ProcessorProvider;

/// <summary>
/// Contract to provide an instance of ServiceBusProcessor
/// </summary>
public interface IServiceBusProcessorProvider
{
    /// <summary>
    /// Get ServiceBusProcessor
    /// </summary>
    /// <param name="eventName">The name of the event</param>
    /// <returns>ServiceBusProcessor instance</returns>
    ServiceBusProcessor GetProcessor(string eventName);
}
