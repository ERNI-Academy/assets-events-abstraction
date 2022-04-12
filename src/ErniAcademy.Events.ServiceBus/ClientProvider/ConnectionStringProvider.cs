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
    private readonly IOptionsMonitor<RetryOptions> _retryOptions;

    // <summary>
    /// Initializes a new instance of the TokenCredentialProvider class.
    /// </summary>
    /// <param name="options">IOptionsMonitor of ConnectionStringOptions settings</param>
    /// <param name="retryOptions">IOptionsMonitor of RetryOptions settings</param>
    public ConnectionStringProvider(IOptionsMonitor<ConnectionStringOptions> options, IOptionsMonitor<RetryOptions> retryOptions)
    {
        _options = options;
        _retryOptions = retryOptions;
    }

    public ServiceBusClient GetClient() => new ServiceBusClient(_options.CurrentValue.ConnectionString, _retryOptions.CurrentValue.ToServiceBusClientOptions());
}
