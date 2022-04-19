using System.ComponentModel.DataAnnotations;

namespace ErniAcademy.Events.StorageQueues.Configuration;

public class ConnectionStringOptions
{
    /// <summary>
    /// The connection string to use for connecting to the Storage Queue.
    /// </summary>
    [Required]
    public string ConnectionString { get; set; }
}
