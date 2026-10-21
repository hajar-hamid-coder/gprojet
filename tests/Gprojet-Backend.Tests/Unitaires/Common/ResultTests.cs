using FluentAssertions;
using Gprojet.Backend.Common;

namespace Gprojet.Backend.Tests.Unitaires.Common;

[Trait("Category", "Common")]
public class ResultTests
{
    [Fact]
    public void Succes_porte_la_valeur()
    {
        var r = Result.Succes(42);
        r.IsSuccess.Should().BeTrue();
        r.Valeur.Should().Be(42);
        r.Erreur.Should().BeNull();
    }

    [Fact]
    public void Echec_porte_l_erreur_et_refuse_la_valeur()
    {
        var r = Result.Echec<int>("boom");
        r.IsSuccess.Should().BeFalse();
        r.Erreur.Should().Be("boom");
        r.Invoking(x => x.Valeur).Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Result_simple()
    {
        Result.Succes().IsSuccess.Should().BeTrue();
        Result.Echec("x").IsSuccess.Should().BeFalse();
    }
}
