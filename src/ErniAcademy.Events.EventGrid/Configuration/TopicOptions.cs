namespace ErniAcademy.Events.EventGrid.Configuration;

public class TopicOptions
{
    /// <summary>
    /// The topic endpoint. For example, "https://TOPIC-NAME.REGION-NAME-1.eventgrid.azure.net/api/events".
    /// </summary>
    public Uri Endpoint { get; set; }
}