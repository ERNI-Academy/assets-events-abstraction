using Azure.Messaging.ServiceBus;

namespace ErniAcademy.Events.ServiceBus.Extensions;

internal static class RetryOptionsExtensions
{
    internal static ServiceBusClientOptions ToServiceBusClientOptions(this Configuration.RetryOptions options)
    {
        var serviceBusOptions = new ServiceBusClientOptions()
        {
            RetryOptions = new ServiceBusRetryOptions
            {
                MaxRetries = options.MaxRetries,
                Delay = options.Delay,
                MaxDelay = options.MaxDelay,
                TryTimeout = options.TryTimeout
            }
        };

        return serviceBusOptions
    }
}
