using ErniAcademy.Events.ServiceBus.Extensions;
using ErniAcademy.Events.StorageQueues.Extensions;
using ErniAcademy.Events.Redis.Extensions;
using ErniAcademy.Serializers.Contracts;
using ErniAcademy.Serializers.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ErniAcademy.Events.Samples;

public static class SampleConfiguration
{
    /// <summary>
    /// this is a sample of how you can configure ServiceBus pub/sub, so later you can use dependency injection
    /// </summary>
    /// <param name="services">your service collection</param>
    /// <param name="configuration">your configuration</param>
    public static void ConfigureServiceBus(IServiceCollection services, IConfiguration configuration)
    {
        ISerializer serializer = new JsonSerializer(); //please note that ISerializer serializer is not in the IoC thats why it may be the case that you want diferents serializers impl within your app


        ///please note that it is mandatory for you to have a configuration like this to match your sectionKey "Events:ServiceBus"
        ///Events:{
        ///"ServiceBus": {
        ///    "ConnectionString": "[put here your ServiceBus connection string]"
        ///}
        services.AddEventsPublisherServiceBus(configuration, serializer, sectionKey: "Events:ServiceBus");
        services.AddEventsSubscriberTopicServiceBus<DummyEvent>(configuration, serializer, sectionKey: "Events:ServiceBus", subscriptionName: "testprocessor"); //change testprocessor with your subscription name
    }

    /// <summary>
    /// this is a sample of how you can configure StorageQueues pub/sub, so later you can use dependency injection
    /// </summary>
    /// <param name="services">your service collection</param>
    /// <param name="configuration">your configuration</param>
    public static void ConfigureStorageQueues(IServiceCollection services, IConfiguration configuration)
    {
        ISerializer serializer = new JsonSerializer(); //please note that ISerializer serializer is not in the IoC thats why it may be the case that you want diferents serializers impl within your app

        ///please note that it is mandatory for you to have a configuration like this to match your sectionKey "Events:StorageQueues"
        ///Events:{
        ///"StorageQueues": {
        ///    "ConnectionString": "[put here your StorageQueues connection string]"
        ///}
        services.AddEventsPublisherStorageQueues(configuration, serializer, sectionKey: "Events:StorageQueues");
        services.AddEventsSubscriberStorageQueues<DummyEvent>(configuration, serializer, sectionKey: "Events:StorageQueues");
    }

    /// <summary>
    /// this is a sample of how you can configure Redis pub/sub, so later you can use dependency injection
    /// </summary>
    /// <param name="services">your service collection</param>
    /// <param name="configuration">your configuration</param>
    public static void ConfigureRedis(IServiceCollection services, IConfiguration configuration)
    {
        ISerializer serializer = new JsonSerializer(); //please note that ISerializer serializer is not in the IoC thats why it may be the case that you want diferents serializers impl within your app

        ///please note that it is mandatory for you to have a configuration like this to match your sectionKey "Events:Redis"
        ///Events:{
        ///"Redis": {
        ///    "ConnectionString": "[put here your Redis connection string]"
        ///}
        services.AddEventsPublisherRedis(configuration, serializer, sectionKey: "Events:Redis");
        services.AddEventsSubscriberRedis<DummyEvent>(configuration, serializer, sectionKey: "Events:Redis");
    }
}
