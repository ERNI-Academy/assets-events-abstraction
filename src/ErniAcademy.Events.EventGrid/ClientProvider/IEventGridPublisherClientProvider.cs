using Azure.Messaging.EventGrid;

namespace ErniAcademy.Events.EventGrid.ClientProvider;

/// <summary>
/// Contract to provide an instance of EventGridPublisherClient
/// </summary>
public interface IEventGridPublisherClientProvider
{
    /// <summary>
    /// Get EventGridPublisherClient
    /// </summary>
    /// <returns>EventGridPublisherClient instance</returns>
    EventGridPublisherClient GetClient();
}
