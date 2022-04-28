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
        for (int i = 0; i < 10; i++)
        {
            var @event = new DummyEvent
            {
                MyCustomProperty = "hi from event " + i
            };

            Console.WriteLine($"Publishing event: {@event.MyCustomProperty}");

            await _eventPublisher.PublishAsync(@event);
        }
    }
}
