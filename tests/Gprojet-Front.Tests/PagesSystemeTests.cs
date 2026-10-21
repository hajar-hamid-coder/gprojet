using System.Globalization;
using Bunit;
using FluentAssertions;
using Gprojet.Front.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Gprojet.Front.Tests;

[Trait("Category", "PagesSysteme")]
public sealed class PagesSystemeTests : BunitContext
{
    private readonly CultureInfo _avant = CultureInfo.CurrentUICulture;

    public PagesSystemeTests() => Services.AddLocalization();

    protected override void Dispose(bool disposing)
    {
        CultureInfo.CurrentUICulture = _avant;
        base.Dispose(disposing);
    }

    private static void Culture(string culture) => CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);

    [Fact]
    public void Accueil_se_rend_avec_son_titre()
    {
        Culture("fr-MA");
        Render<Accueil>().Find("h1").TextContent.Should().Be("GestionProjet");
    }

    [Theory]
    [InlineData("fr-MA", "Page introuvable", "Retour à l'accueil")]
    [InlineData("en", "Page not found", "Back to home")]
    public void Introuvable_se_rend_avec_son_titre_et_un_retour_accueil(string culture, string titre, string retour)
    {
        Culture(culture);
        var cut = Render<Introuvable>();

        cut.Find("h1").TextContent.Should().Be(titre);
        cut.Find("a.toolbar-btn").TextContent.Should().Be(retour);
    }

    [Theory]
    [InlineData("fr-MA", "Accès refusé")]
    [InlineData("en", "Access denied")]
    public void Acces_refuse_se_rend_avec_son_titre_traduit(string culture, string titre)
    {
        Culture(culture);
        Render<AccesRefuse>().Find("h1").TextContent.Should().Be(titre);
    }

    [Fact]
    public void Erreur_affiche_un_identifiant_de_correlation_sans_detail_technique()
    {
        Culture("fr-MA");
        var cut = Render<Erreur>();

        cut.Find("h1").TextContent.Should().Be("Erreur inattendue");
        Guid.TryParse(cut.Find(".identifiant-correlation").TextContent, out _).Should().BeTrue();
        cut.Markup.Should().NotContain("Exception").And.NotContain("StackTrace");
    }

    [Fact]
    public void Erreur_reprend_l_identifiant_fourni_par_l_appelant()
    {
        Culture("en");
        var cut = Render<Erreur>(p => p.Add(e => e.Identifiant, "abc-123"));

        cut.Find(".identifiant-correlation").TextContent.Should().Be("abc-123");
    }
}
