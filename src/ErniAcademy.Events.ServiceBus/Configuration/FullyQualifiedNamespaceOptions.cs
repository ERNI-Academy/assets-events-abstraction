namespace ErniAcademy.Events.ServiceBus.Configuration;

public class FullyQualifiedNamespaceOptions
{
    /// <summary>
    /// The fully qualified Service Bus namespace to connect to. This is likely to be similar to {yournamespace}.servicebus.windows.net.
    /// </summary>
    public string FullyQualifiedNamespace { get; set; }

}
