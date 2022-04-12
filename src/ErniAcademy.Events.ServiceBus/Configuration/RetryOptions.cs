namespace ErniAcademy.Events.ServiceBus.Configuration;

public class RetryOptions
{
    public RetryOptions()
    {
        MaxRetries = 3;
        Delay = TimeSpan.FromSeconds(0.8);
        MaxDelay = TimeSpan.FromSeconds(60);
        TryTimeout = TimeSpan.FromMilliseconds(60);
    }

    /// <summary>
    /// The maximum number of retry attempts before considering the associated operation to have failed. The default retry limit is 3.
    /// </summary>
    public int MaxRetries { get; set; }

    /// <summary>
    /// The delay between retry attempts for a fixed approach or the delay on which to base calculations for a backoff-based approach. The default delay is 0.8 seconds.
    /// </summary>
    public TimeSpan Delay { get; set; }

    /// <summary>
    /// The maximum permissible delay between retry attempts. The default maximum delay is 60 seconds.
    /// </summary>
    public TimeSpan MaxDelay { get; set; }

    /// <summary>
    /// The maximum duration to wait for completion of a single attempt, whether the initial attempt or a retry. The default timeout is 60 seconds.
    /// </summary>
    public TimeSpan TryTimeout { get; set; }
}
