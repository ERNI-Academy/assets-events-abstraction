using ErniAcademy.Events.StorageQueues.Configuration;
using Microsoft.Extensions.Options;
using Azure.Storage.Queues;

namespace ErniAcademy.Events.StorageQueues.ClientProvider;

/// <summary>
/// Provider to build a StorageQueuesClient based on connection string settings
/// </summary>
public class ConnectionStringProvider : IQueueClientProvider
{
    private readonly IOptionsMonitor<ConnectionStringOptions> _options;
    private readonly QueueClientOptions _queueOptions;

    /// <summary>
    /// Initializes a new instance of the TokenCredentialProvider class.
    /// </summary>
    /// <param name="options">IOptionsMonitor of ConnectionStringOptions settings</param>
    /// <param name="queueOptions">The set of Azure.Storage.Queues.QueueClientOptions to use for configuring the Azure.Storage.Queues.QueueClient.</param>
    public ConnectionStringProvider(IOptionsMonitor<ConnectionStringOptions> options, QueueClientOptions queueOptions = null)
    {
        _options = options;
        _queueOptions = queueOptions;
    }

    public QueueClient GetClient(string queueName) => new QueueClient(_options.CurrentValue.ConnectionString, queueName, _queueOptions);
}
