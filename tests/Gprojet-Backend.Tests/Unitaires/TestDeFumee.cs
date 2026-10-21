using FluentAssertions;

namespace Gprojet.Backend.Tests.Unitaires;

public class TestDeFumee
{
    [Fact]
    public void Le_projet_backend_se_charge()
        => typeof(Program).Assembly.GetName().Name.Should().Be("Gprojet-Backend");
}
