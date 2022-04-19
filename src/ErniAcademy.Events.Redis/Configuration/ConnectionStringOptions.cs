using System.ComponentModel.DataAnnotations;

namespace ErniAcademy.Events.Redis.Configuration;

public class ConnectionStringOptions
{
    /// <summary>
    /// The connection string to use for connecting to the Service Bus namespace.
    /// </summary>
    [Required] 
    public string ConnectionString { get; set; }
}

