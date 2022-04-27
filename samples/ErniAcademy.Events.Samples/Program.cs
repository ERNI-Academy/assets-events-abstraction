using ErniAcademy.Events.Samples;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

IServiceCollection services = new ServiceCollection();

var configuration = new ConfigurationBuilder()
    .SetBasePath(Environment.CurrentDirectory)
    .AddJsonFile("appsettings.development.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

services.AddLogging(builder => builder.AddConsole().AddDebug());

services.AddSingleton<SampleEventProducer>();
services.AddSingleton<SampleEventProcessor>();

Console.WriteLine("type '1' for ServiceBus");
Console.WriteLine("type '2' for Redis");
Console.WriteLine("type '3' for StorageQueues");

var line = Console.ReadLine();

switch (line)
{
    case "1": { SampleConfiguration.ConfigureServiceBus(services, configuration); break; }
    case "2": { SampleConfiguration.ConfigureRedis(services, configuration); break; }
    case "3": { SampleConfiguration.ConfigureStorageQueues(services, configuration); break; }
    default: { Console.WriteLine($"invalid type {line}"); break; }
}

var provider = services.BuildServiceProvider();

var producer = provider.GetRequiredService<SampleEventProducer>();
var processor = provider.GetRequiredService<SampleEventProcessor>();

await processor.RunAsync();
await producer.RunAsync();

Console.Read();