using ErniAcademy.Events.Contracts;

namespace ErniAcademy.Events.IntegrationTests;

public class DummyEvent : EventBase
{
    public string Title { get; set; }
}
