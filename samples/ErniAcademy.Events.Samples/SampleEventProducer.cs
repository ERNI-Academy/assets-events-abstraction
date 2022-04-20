using ErniAcademy.Events.Contracts;

namespace ErniAcademy.Events.Samples;

public class SampleEventProducer
{
    private readonly IEventPublisher _eventPublisher;

    public SampleEventProducer(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task RunAsync() 
    {
        var @event = new MyEvent
        {
            MyCustomProperty = "hi"
        };

        await _eventPublisher.PublishAsync(@event);
    }
}
