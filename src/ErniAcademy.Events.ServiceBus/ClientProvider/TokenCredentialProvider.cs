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
    private readonly ServiceBusClientOptions _busOptions;

    /// <summary>
    /// Initializes a new instance of the TokenCredentialProvider class.
    /// </summary>
    /// <param name="options">IOptionsMonitor of FullyQualifiedNamespaceOptions settings</param>
    /// <param name="tokenCredential">The Azure managed identity credential to use for authorization. Access controls may be specified by the Service Bus namespace.</param>
    /// /// <param name="busOptions">The set of Azure.Messaging.ServiceBus.ServiceBusClientOptions to use for configuring the Azure.Messaging.ServiceBus.ServiceBusClient.</param>
    public TokenCredentialProvider(
        IOptionsMonitor<FullyQualifiedNamespaceOptions> options,
        TokenCredential tokenCredential,
        ServiceBusClientOptions busOptions = null)
    {
        _options = options;
        _tokenCredential = tokenCredential;
        _busOptions = busOptions;
    }

    public ServiceBusClient GetClient() => new ServiceBusClient(_options.CurrentValue.FullyQualifiedNamespace, _tokenCredential, _busOptions);
}
