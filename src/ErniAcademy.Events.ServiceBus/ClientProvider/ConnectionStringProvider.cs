using Azure.Messaging.ServiceBus;
using ErniAcademy.Events.ServiceBus.Configuration;
using Microsoft.Extensions.Options;
using ErniAcademy.Events.ServiceBus.Extensions;

namespace ErniAcademy.Events.ServiceBus.ClientProvider;

/// <summary>
/// Provider to build a ServiceBusClient based on connection string settings
/// </summary>
public class ConnectionStringProvider : IServiceBusClientProvider
{
    private readonly IOptionsMonitor<ConnectionStringOptions> _options;
    private readonly ServiceBusClientOptions _busOptions;

    /// <summary>
    /// Initializes a new instance of the TokenCredentialProvider class.
    /// </summary>
    /// <param name="options">IOptionsMonitor of ConnectionStringOptions settings</param>
    /// <param name="busOptions">The set of Azure.Messaging.ServiceBus.ServiceBusClientOptions to use for configuring the Azure.Messaging.ServiceBus.ServiceBusClient.</param>
    public ConnectionStringProvider(IOptionsMonitor<ConnectionStringOptions> options, ServiceBusClientOptions busOptions = null)
    {
        _options = options;
        _busOptions = busOptions;
    }

    public ServiceBusClient GetClient() => new ServiceBusClient(_options.CurrentValue.ConnectionString, _busOptions);
}
