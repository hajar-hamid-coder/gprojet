using FluentAssertions;
using Gprojet.Backend.Common;

namespace Gprojet.Backend.Tests.Unitaires.Common;

[Trait("Category", "Common")]
public class PaginationTests
{
    [Fact]
    public void Valeurs_par_defaut()
    {
        var p = new PageRequete();
        p.Page.Should().Be(1);
        p.Taille.Should().Be(PageRequete.TailleParDefaut);
        p.Decalage.Should().Be(0);
    }

    [Theory]
    [InlineData(0, 0, 1, 1)]
    [InlineData(-5, -1, 1, 1)]
    [InlineData(3, 500, 3, 200)]
    [InlineData(2, 200, 2, 200)]
    public void Bornes(int page, int taille, int pageAttendue, int tailleAttendue)
    {
        var p = new PageRequete { Page = page, Taille = taille };
        p.Page.Should().Be(pageAttendue);
        p.Taille.Should().Be(tailleAttendue);
    }

    [Fact]
    public void Decalage_ne_deborde_pas_avec_page_enorme()
        => new PageRequete { Page = int.MaxValue, Taille = 200 }.Decalage.Should().BeGreaterThan(0);

    [Fact]
    public void Decalage_calcule()
        => new PageRequete { Page = 3, Taille = 10 }.Decalage.Should().Be(20);

    [Theory]
    [InlineData(0, 10, 0)]
    [InlineData(10, 10, 1)]
    [InlineData(11, 10, 2)]
    public void Nombre_de_pages(int total, int taille, int attendu)
        => new PageResultat<int>([], total, 1, taille).NombrePages.Should().Be(attendu);
}
