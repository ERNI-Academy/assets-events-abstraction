using ErniAcademy.Events.Contracts;

namespace ErniAcademy.Events.Samples;

public class SampleEventProcessor
{
    private readonly IEventSubscriber<MyEvent> _subscriber;

    public SampleEventProcessor(IEventSubscriber<MyEvent> subscriber)
    {
        _subscriber = subscriber;
    }

    public async Task RunAsync()
    {
        _subscriber.Subscribe(ProcessEvent);
        
        await _subscriber.StarProcessingAsync();
    }

    private Task ProcessEvent(MyEvent arg)
    {
       //Do something with the event
       return Task.CompletedTask;
    }
}