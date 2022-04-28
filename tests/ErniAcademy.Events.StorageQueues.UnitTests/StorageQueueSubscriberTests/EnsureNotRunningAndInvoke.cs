using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ErniAcademy.Events.StorageQueues.UnitTests.StorageQueueSubscriberTests;

public class EnsureNotRunningAndInvoke : IClassFixture<StorageQueueSubscriberFixture>
{
    private readonly StorageQueueSubscriberFixture _fixture;

    public EnsureNotRunningAndInvoke(StorageQueueSubscriberFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void With_not_null_active_task_Throws_InvalidOperationException()
    {
        //Arrange
        _fixture.Sut._activeReceiveTask = Task.CompletedTask;

        //Act
        var actual = () => _fixture.Sut.EnsureNotRunningAndInvoke(()=> { });

        //Assert
        actual.Should().Throw<InvalidOperationException>().WithMessage("Already processing");
    }
}
