namespace ErniAcademy.Events.StorageQueues.Configuration;

public class QueueSubscriberOptions
{
    public QueueSubscriberOptions()
    {
        MaxConcurrentCalls = 10;
    }

    /// <summary>
    /// The number of maximum concurrent calls. Default value 10
    /// </summary>
    public int MaxConcurrentCalls { get; set; }
}
