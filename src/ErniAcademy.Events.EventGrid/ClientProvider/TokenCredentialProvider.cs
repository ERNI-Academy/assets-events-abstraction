using Azure.Core;
using Azure.Messaging.EventGrid;
using ErniAcademy.Events.EventGrid.Configuration;
using Microsoft.Extensions.Options;

namespace ErniAcademy.Events.EventGrid.ClientProvider;

/// <summary>
/// Provider to build a EventGridPublisherClient based on topic settings and token credentials
/// </summary>
public class TokenCredentialProvider : IEventGridPublisherClientProvider
{
    private readonly IOptionsMonitor<TopicOptions> _options;
    private readonly TokenCredential _tokenCredential;

    /// <summary>
    /// Initializes a new instance of the TokenCredentialProvider class.
    /// </summary>
    /// <param name="options">IOptionsMonitor of TopicOptions settings</param>
    /// <param name="tokenCredential">The token credential used to authenticate with the service</param>
    public TokenCredentialProvider(
        IOptionsMonitor<TopicOptions> options,
        TokenCredential tokenCredential)
    {
        _options = options;
        _tokenCredential = tokenCredential;
    }

    public EventGridPublisherClient GetClient() => new EventGridPublisherClient(_options.CurrentValue.Endpoint, _tokenCredential);
}
