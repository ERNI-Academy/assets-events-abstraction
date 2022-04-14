namespace ErniAcademy.Events.EventGrid.Configuration;

public class PublisherOptions
{
    public PublisherOptions()
    {
        DataVersion = "v1";
    }

    /// <summary>
    /// A resource path relative to the topic path.
    /// </summary>
    public string SubjectBasePath { get; set; }

    /// <summary>
    /// The schema version of the data object. Default v1
    /// </summary>
    public string DataVersion { get; internal set; }
}
