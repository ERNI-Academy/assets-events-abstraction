using ErniAcademy.Events.Contracts.UnitTests.Utils;
using FluentAssertions;
using Xunit;

namespace ErniAcademy.Events.Contracts.UnitTests.EventBaseTests;

public class AddMetadata
{
    [Fact]
    public void With_correlationid_ctor()
    {
        //Arrange
        var sut = new DummyEvent();

        //Act
        sut.AddMetadata("key", "value");

        //Assert
        sut.Metadata.Should().NotBeNull();
        sut.Metadata.Values.Should()
            .HaveCount(1)
            .And.ContainKey("key")
            .And.ContainValue("value");
    }
}
