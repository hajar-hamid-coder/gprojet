using Bunit;
using FluentAssertions;
using Gprojet.Front.Pages;

namespace Gprojet.Front.Tests;

public sealed class AccueilTests : BunitContext
{
    [Fact]
    public void L_accueil_affiche_le_titre()
    {
        var cut = Render<Accueil>();

        cut.Find("h1").TextContent.Should().Be("GestionProjet");
    }
}
