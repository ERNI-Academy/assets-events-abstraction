using ErniAcademy.Events.EventGrid.Configuration;
using Microsoft.Extensions.Options;
using Azure.Messaging.EventGrid;

namespace ErniAcademy.Events.EventGrid.ClientProvider;

/// <summary>
/// Provider to build a EventGridPublisherClient based on topic settings and key credentials
/// </summary>
public class KeyCredentialProvider : IEventGridPublisherClientProvider
{
    private readonly IOptionsMonitor<TopicOptions> _options;
    private readonly IOptionsMonitor<KeyOptions> _keyOptions;

    // <summary>
    /// Initializes a new instance of the KeyCredentialProvider class.
    /// </summary>
    /// <param name="options">IOptionsMonitor of TopicOptions settings</param>
    /// <param name="keyOptions">IOptionsMonitor of KeyOptions settings</param>
    public KeyCredentialProvider(
        IOptionsMonitor<TopicOptions> options, 
        IOptionsMonitor<KeyOptions> keyOptions)
    {
        _options = options;
        _keyOptions = keyOptions;
    }

    public EventGridPublisherClient GetClient() => new EventGridPublisherClient(_options.CurrentValue.Endpoint, new Azure.AzureKeyCredential(_keyOptions.CurrentValue.Key));
}
