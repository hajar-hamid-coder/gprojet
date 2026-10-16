using System.Net;
using System.Text;
using Bunit;
using FluentAssertions;
using Gprojet.Backend.Contracts.Commun;
using Gprojet.Front.Api;
using Gprojet.Front.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Gprojet.Front.Tests;

[Trait("Category", "Auth")]
public sealed class BffAuthenticationTests : BunitContext
{
    private sealed class Simule(Func<HttpRequestMessage, HttpResponseMessage> f) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage r, CancellationToken ct) => Task.FromResult(f(r));
    }

    private IRenderedComponent<AuthorizeView> Rendre(Func<HttpRequestMessage, HttpResponseMessage> f)
    {
        var client = new BffApiClient(new HttpClient(new Simule(f)) { BaseAddress = new Uri("http://localhost/") });
        Services.RemoveAll<IAuthorizationService>(); // vraie autorisation, pas le service factice de bUnit
        Services.AddAuthorizationCore();
        Services.AddCascadingAuthenticationState();
        Services.AddSingleton<AuthenticationStateProvider>(new BffAuthenticationStateProvider(client));
        return Render<AuthorizeView>(p => p
            .Add(v => v.Authorized, (RenderFragment<AuthenticationState>)(c => b => b.AddContent(0, "bonjour " + c.User.Identity!.Name)))
            .Add(v => v.NotAuthorized, (RenderFragment<AuthenticationState>)(_ => b => b.AddContent(0, "anonyme"))));
    }

    [Fact]
    public void User_200_donne_le_contenu_autorise()
    {
        var json = $$"""{"id":"{{Guid.NewGuid()}}","matricule":"A0001","entiteId":"{{Guid.NewGuid()}}","roles":["DIRECTEUR"]}""";
        var cut = Rendre(r =>
        {
            r.RequestUri!.AbsolutePath.Should().Be("/bff/user");
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json, Encoding.UTF8, "application/json") };
        });

        cut.WaitForAssertion(() => cut.Markup.Should().Contain("bonjour A0001"));
    }

    [Fact]
    public void User_401_donne_le_contenu_non_autorise()
    {
        var cut = Rendre(_ => new HttpResponseMessage(HttpStatusCode.Unauthorized));

        cut.WaitForAssertion(() => cut.Markup.Should().Contain("anonyme"));
    }

    [Fact]
    public async Task Une_requete_mutante_porte_l_en_tete_anti_csrf_et_pas_un_GET()
    {
        var vues = new List<HttpRequestMessage>();
        var client = new BffApiClient(new HttpClient(new Simule(r => { vues.Add(r); return new HttpResponseMessage(HttpStatusCode.NoContent); }))
        { BaseAddress = new Uri("http://localhost/") });

        await client.DeconnecterAsync();
        await client.LoginDevDisponibleAsync();

        vues[0].Headers.GetValues(ProtectionCsrf.EnTete).Should().Equal(ProtectionCsrf.Valeur);
        vues[1].Headers.Contains(ProtectionCsrf.EnTete).Should().BeFalse();
    }
}
