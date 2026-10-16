using System.Security.Claims;
using Bunit;
using FluentAssertions;
using Gprojet.Backend.Contracts.Commun;
using Gprojet.Front.Auth;
using Gprojet.Front.Layouts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Gprojet.Front.Tests;

[Trait("Category", "Layout")]
public sealed class MainLayoutTests : BunitContext
{
    private sealed class Fournisseur(ClaimsPrincipal utilisateur) : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync() => Task.FromResult(new AuthenticationState(utilisateur));
    }

    private IRenderedComponent<MainLayout> Rendre(params string[] roles)
    {
        var identite = roles.Length == 0
            ? new ClaimsIdentity()
            : new ClaimsIdentity([new Claim(ClaimTypes.Name, "agent"), .. roles.Select(r => new Claim(ClaimTypes.Role, r))], "test");
        Services.RemoveAll<IAuthorizationService>(); // remplace le service factice de bUnit : on teste les vraies politiques
        Services.AddAuthorizationCore(Politiques.Enregistrer);
        Services.AddSingleton(new Gprojet.Front.Api.BffApiClient(new HttpClient { BaseAddress = new Uri("http://localhost/") }));
        Services.AddCascadingAuthenticationState();
        Services.AddSingleton<AuthenticationStateProvider>(new Fournisseur(new ClaimsPrincipal(identite)));
        return Render<MainLayout>(p => p.Add(l => l.Body, "<p>contenu</p>"));
    }

    [Fact]
    public void Le_rendu_suit_la_structure_de_la_charte()
    {
        var cut = Rendre();

        cut.FindAll(".page .sidebar").Should().HaveCount(1);
        cut.FindAll(".main-area .topbar").Should().HaveCount(1);
        cut.FindAll(".main-area .content-area").Should().HaveCount(1);
        cut.Find(".content-area").TextContent.Should().Contain("contenu");
        cut.Find(".search-shortcut").TextContent.Should().Be("Ctrl+K");
    }

    [Fact]
    public void Un_agent_ne_voit_pas_le_portefeuille_direction()
    {
        var cut = Rendre();

        cut.Markup.Should().NotContain("Portefeuille direction");
        cut.Markup.Should().NotContain("Administration");
        cut.Markup.Should().Contain("Mes tâches");
    }

    [Fact]
    public void Un_directeur_voit_le_portefeuille_direction()
    {
        var cut = Rendre(RolesApplication.Directeur);

        cut.Markup.Should().Contain("Portefeuille direction");
    }

    [Fact]
    public void Un_responsable_d_entite_voit_l_administration_mais_pas_le_portefeuille()
    {
        var cut = Rendre(RolesApplication.ResponsableEntite);

        cut.Markup.Should().Contain("Administration");
        cut.Markup.Should().NotContain("Portefeuille direction");
    }

    [Fact]
    public void Le_bouton_de_repli_ajoute_sidebar_collapsed_et_met_a_jour_aria_expanded()
    {
        var cut = Rendre();
        var bouton = cut.Find("button[aria-controls='menu-principal']");
        bouton.GetAttribute("aria-expanded").Should().Be("true");

        bouton.Click();

        cut.Find(".page").ClassList.Should().Contain("sidebar-collapsed").And.Contain("mobile-open");
        cut.Find("button[aria-controls='menu-principal']").GetAttribute("aria-expanded").Should().Be("false");
    }
}
