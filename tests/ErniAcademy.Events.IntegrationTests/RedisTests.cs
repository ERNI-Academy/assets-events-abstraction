using System;
using System.Threading.Tasks;
using ErniAcademy.Events.Redis;
using ErniAcademy.Events.Redis.Extensions;
using ErniAcademy.Serializers.Contracts;
using ErniAcademy.Serializers.Json;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace ErniAcademy.Events.IntegrationTests;

public class RedisTests : BaseTests
{
    private readonly ISubscriber _subscriber;
    private readonly ISerializer _serializer = new JsonSerializer();
    private DummyEvent _result = default(DummyEvent);

    public RedisTests()
    {
        var connectionMultiplexerProvider = _provider.GetRequiredService<IConnectionMultiplexerProvider>();
        _subscriber = connectionMultiplexerProvider.Connection.GetSubscriber();

        _subscriber.Subscribe(new RedisChannel("dummyevent", RedisChannel.PatternMode.Auto), (channel, value) => {
            _result = _serializer.DeserializeFromString<DummyEvent>(value.ToString());
        });
    }

    protected override IServiceCollection RegisterSut(IServiceCollection services)
    {
        services.AddRedis(_serializer, sectionKey: "Events:Redis");
        return services;
    }

    protected override async Task<DummyEvent> WaitForReceive()
    {
        await Task.Delay(TimeSpan.FromSeconds(5));

        await _subscriber.UnsubscribeAllAsync();

        return _result;
    }
}
