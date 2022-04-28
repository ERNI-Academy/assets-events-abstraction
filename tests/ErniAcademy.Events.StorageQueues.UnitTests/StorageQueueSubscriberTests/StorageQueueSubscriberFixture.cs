using Azure.Storage.Queues.Models;
using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.StorageQueues.ClientProvider;
using ErniAcademy.Events.StorageQueues.Configuration;
using ErniAcademy.Serializers.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;

namespace ErniAcademy.Events.StorageQueues.UnitTests.StorageQueueSubscriberTests;

public class StorageQueueSubscriberFixture
{
    internal readonly IQueueClientProvider QueueClientProvider = Substitute.For<IQueueClientProvider>();
    internal readonly ISerializer Serializer = Substitute.For<ISerializer>();
    internal readonly IEventNameResolver EventNameResolver = Substitute.For<IEventNameResolver>();
    internal readonly ILoggerFactory LoggerFactory = Substitute.For<ILoggerFactory>();
    internal readonly IOptionsMonitor<QueueSubscriberOptions> Options = Substitute.For<IOptionsMonitor<QueueSubscriberOptions>>();
    internal readonly StorageQueueSubscriberDummy<DummyEvent> Sut;

    public StorageQueueSubscriberFixture()
    {
        Sut = new StorageQueueSubscriberDummy<DummyEvent>(QueueClientProvider, EventNameResolver, Serializer, LoggerFactory, Options);
    }

    internal class StorageQueueSubscriberDummy<TEvent> : StorageQueueSubscriber<TEvent>
        where TEvent : class, IEvent, new()
    {
        public StorageQueueSubscriberDummy(
            IQueueClientProvider queueClientProvider,
            IEventNameResolver eventNameResolver, 
            ISerializer serializer, 
            ILoggerFactory loggerFactory, 
            IOptionsMonitor<QueueSubscriberOptions> options) 
            : base(queueClientProvider, eventNameResolver, serializer, loggerFactory, options)
        {
        }

        protected override Task ProcessQueueMessageAsync(QueueMessage message, CancellationTokenSource cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
