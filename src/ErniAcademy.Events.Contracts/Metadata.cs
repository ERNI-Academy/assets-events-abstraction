namespace ErniAcademy.Events.Contracts;

public class Metadata
{
    public Metadata()
    {
        Values = new Dictionary<string, string>();
    }

    public IDictionary<string, string> Values { get; set; }
}
