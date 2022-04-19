
using Azure.Storage.Queues;

namespace ErniAcademy.Events.StorageQueues.ClientProvider;

/// <summary>
/// Contract to provide an instance of QueueClient
/// </summary>
public interface IQueueClientProvider
{
    /// <summary>
    /// Get QueueClient
    /// </summary>
    /// <returns>QueueClient instance</returns>
    /// <param name="queueName">The name of the queue</param>
    QueueClient GetClient(string queueName);
}
