using Azure.Core;
using Azure.Messaging.ServiceBus;
using ErniAcademy.Events.ServiceBus.Configuration;
using Microsoft.Extensions.Options;

namespace ErniAcademy.Events.ServiceBus.ClientProvider;

/// <summary>
/// Provider to build a ServiceBusClient based on fully qualified namespace settings and token credentials
/// </summary>
public class TokenCredentialProvider : IServiceBusClientProvider
{
    private readonly IOptionsMonitor<FullyQualifiedNamespaceOptions> _options;
    private readonly TokenCredential _tokenCredential;
    private readonly IOptionsMonitor<Configuration.RetryOptions> _retryOptions;

    /// <summary>
    /// Initializes a new instance of the TokenCredentialProvider class.
    /// </summary>
    /// <param name="options">IOptionsMonitor of FullyQualifiedNamespaceOptions settings</param>
    /// <param name="tokenCredential">The Azure managed identity credential to use for authorization. Access controls may be specified by the Service Bus namespace.</param>
    /// <param name="retryOptions">IOptionsMonitor of RetryOptions settings</param>
    public TokenCredentialProvider(
        IOptionsMonitor<FullyQualifiedNamespaceOptions> options,
        TokenCredential tokenCredential, 
        IOptionsMonitor<Configuration.RetryOptions> retryOptions)
    {
        _options = options;
        _tokenCredential = tokenCredential;
        _retryOptions = retryOptions;
    }

    public ServiceBusClient GetClient()
    {
        var options = _options.CurrentValue;
        return new ServiceBusClient(options.FullyQualifiedNamespace, _tokenCredential);
    }
}
