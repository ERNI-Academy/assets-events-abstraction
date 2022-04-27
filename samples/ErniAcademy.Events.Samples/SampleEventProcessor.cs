using ErniAcademy.Events.Contracts;

namespace ErniAcademy.Events.Samples;

public class SampleEventProcessor
{
    private readonly IEventSubscriber<DummyEvent> _subscriber;

    public SampleEventProcessor(IEventSubscriber<DummyEvent> subscriber)
    {
        _subscriber = subscriber;
    }

    public async Task RunAsync()
    {
        _subscriber.ProcessEventAsync += ProcessEvent;
        
        await _subscriber.StartProcessingAsync();
    }

    private Task ProcessEvent(DummyEvent arg)
    {
        Console.WriteLine($"Processing event: {arg.MyCustomProperty}");

       //Do something with the event
       return Task.CompletedTask;
    }
}