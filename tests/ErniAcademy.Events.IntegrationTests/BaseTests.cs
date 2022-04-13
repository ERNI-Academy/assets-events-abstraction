using System;
using System.IO;
using System.Threading.Tasks;
using ErniAcademy.Events.Contracts;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ErniAcademy.Events.IntegrationTests
{
    public abstract class BaseTests
    {
        protected IServiceProvider _provider;

        protected IEventPublisher _sut;

        protected BaseTests()
        {
            var services = new ServiceCollection();

            var tempConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("tests.settings.json", optional: false)
                .Build();

            var isDevelopment = tempConfig.GetValue<string>("Environment") == "__IsAtBuiltTime__";

            services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile(isDevelopment ? "tests.settings.Development.json" : "tests.settings.json", optional: false)
                .AddEnvironmentVariables()
                .Build());

            RegisterSut(services);

            _provider = services.BuildServiceProvider();

            _sut = _provider.GetService<IEventPublisher>();
        }

        protected abstract IServiceCollection RegisterSut(IServiceCollection services);
        protected abstract Task<DummyEvent> WaitForReceive();

        [Fact]
        public virtual async Task Publish_event_should_be_received_by_a_consumer()
        {
            //Arrange
            var @event = new DummyEvent
            {
                Title = "Integration test event" + Guid.NewGuid()
            };

            //Act
            await _sut.PublishAsync(@event);

            //Assert
            var actual = await WaitForReceive();

            actual.Should().BeEquivalentTo(@event);
        }
    }
}
