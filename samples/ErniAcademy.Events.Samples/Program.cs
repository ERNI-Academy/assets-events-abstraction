using ErniAcademy.Events.Samples;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

IServiceCollection services = new ServiceCollection();

var configuration = new ConfigurationBuilder()
    .SetBasePath(Environment.CurrentDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

services.AddSingleton<SampleEventProducer>();
services.AddSingleton<SampleEventProcessor>();

SampleConfiguration.ConfigureServiceBus(services, configuration);
//SampleConfiguration.ConfigureRedis(services, configuration);
//SampleConfiguration.ConfigureStorageQueues(services, configuration);

var provider = services.BuildServiceProvider();

var producer = provider.GetRequiredService<SampleEventProducer>();
var processor = provider.GetRequiredService<SampleEventProcessor>();

await producer.RunAsync();
await processor.RunAsync();