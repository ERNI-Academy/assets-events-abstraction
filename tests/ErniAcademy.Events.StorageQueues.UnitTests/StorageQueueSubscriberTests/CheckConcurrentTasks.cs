using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using ErniAcademy.Events.StorageQueues.Configuration;
using System;

namespace ErniAcademy.Events.StorageQueues.UnitTests.StorageQueueSubscriberTests;

public class CheckConcurrentTasks : IClassFixture<StorageQueueSubscriberFixture>
{
    private readonly StorageQueueSubscriberFixture _fixture;

    public CheckConcurrentTasks(StorageQueueSubscriberFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void With_all_completed_tasks_Should_dispose_cts()
    {
        //Arrange
        _fixture.Sut._taskTuples = new();
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken(), CancellationToken.None);
        _fixture.Sut._taskTuples.Add((Process(), linkedCts));

        _fixture.Options.CurrentValue.Returns(new QueueSubscriberOptions { MaxConcurrentCalls = 0 }); //force maxconcurrency condition

        Task Process() { 
            return Task.CompletedTask;
        };

        //Act
        _fixture.Sut.CheckConcurrentTasks();

        //Assert
        _fixture.Sut._taskTuples.Should().BeEmpty();

        var token = () => linkedCts.Token;

        token.Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public void With_pending_tasks_Should_add_to_remaining()
    {
        //Arrange
        _fixture.Sut._taskTuples = new();
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken(), CancellationToken.None);
        _fixture.Sut._taskTuples.Add((Process(), linkedCts));

        _fixture.Options.CurrentValue.Returns(new QueueSubscriberOptions { MaxConcurrentCalls = 0 }); //force maxconcurrency condition

        Task Process()
        {
            return Task.Delay(10000);
        };

        //Act
        _fixture.Sut.CheckConcurrentTasks();

        //Assert
        _fixture.Sut._taskTuples.Should().HaveCount(1);
        linkedCts.Token.Should().NotBeNull();
    }
}
