using System.ComponentModel.DataAnnotations;

namespace ErniAcademy.Events.EventGrid.Configuration;

public class KeyOptions
{
    /// <summary>
    /// Key credential used to authenticate to an Azure Service
    /// </summary>
    [Required] 
    public string Key { get; set; }
}