using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ErniAcademy.Events.StorageQueues.UnitTests.StorageQueueSubscriberTests;

public class ProcessEventAsync : IClassFixture<StorageQueueSubscriberFixture>
{
    private readonly StorageQueueSubscriberFixture _fixture;

    public ProcessEventAsync(StorageQueueSubscriberFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void With_null_assigment_Throws_ArgumentNullException()
    {
        //Act
        var actual = () => _fixture.Sut.ProcessEventAsync += null;

        //Assert
        actual.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void With_more_than_one_assigment_Throws_NotSupportedException()
    {
        //Arrange
        _fixture.Sut.ProcessEventAsync += async e =>
        {
            //firt assigment
            await Task.CompletedTask;
        };

        //Act
        var actual = () => _fixture.Sut.ProcessEventAsync += async e =>
        {
            //second assigment
            await Task.CompletedTask;
        };

        //Assert
        actual.Should().Throw<NotSupportedException>().Which.Message.Should().Be("Handler has already been assigned");
    }

    [Fact]
    public void With_null_unassigment_Throws_ArgumentNullException()
    {
        //Act
        var actual = () => _fixture.Sut.ProcessEventAsync -= null;

        //Assert
        actual.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void With_unassigment_with_no_previus_assigment_Throws_NotSupportedException()
    {
        //Act
        var actual = () => _fixture.Sut.ProcessEventAsync -= async e =>
        {
            await Task.CompletedTask;
        };

        //Assert
        actual.Should().Throw<NotSupportedException>().Which.Message.Should().Be("Handler has not been assigned");
    }
}
