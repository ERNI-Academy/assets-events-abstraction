using ErniAcademy.Events.Contracts;
using ErniAcademy.Events.Contracts.UnitTests.Utils;
using FluentAssertions;
using Xunit;

namespace ErniAcademy.Events.Contracts.UnitTests.EventNameResolverTests;

public class Resolve
{
    private readonly EventNameResolver _sut;

    public Resolve()
    {
        _sut = new EventNameResolver();
    }

    [Fact]
    public void With_generic_type_Returns_generic_type()
    {
        //Act
        var actual = _sut.Resolve<DummyEvent>();

        //Assert
        actual.Should().Be("dummyevent");
    }

    [Fact]
    public void With_instance_Returns_instance_type()
    {
        //Arrange
        var @event = new DummyEvent();

        //Act
        var actual = _sut.Resolve(@event);

        //Assert
        actual.Should().Be("dummyevent");
    }
}
