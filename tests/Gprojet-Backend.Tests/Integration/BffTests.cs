using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Gprojet.Backend.Contracts.Commun;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Gprojet.Backend.Tests.Integration;

// Endpoint mutant minimal pour éprouver l'anti-CSRF (le projet n'a pas encore d'API métier).
[ApiController]
[Route("api/test-csrf")]
public sealed class ControleurCsrfDeTest : ControllerBase
{
    [HttpPost]
    public IActionResult Poster() => NoContent();
}

[Collection(PgTestGroupe.Nom)]
[Trait("Category", "Integration")]
public sealed class BffTests(PgTestFixture pg) : IAsyncLifetime
{
    private readonly string _sfx = Guid.NewGuid().ToString("N")[..8];
    private string Mat => "BFF" + _sfx;

    public async Task InitializeAsync()
    {
        await using var c = pg.Ouvrir();
        await c.OpenAsync();
        var e = Guid.CreateVersion7();
        var sql = $"""
            INSERT INTO org.entite_organisationnelle (id, code, libelle, niveau, chemin) VALUES ('{e}', 'B{_sfx}', 'Service bff', 'SERVICE', '{e:N}');
            INSERT INTO org.agent (id, matricule, nom, prenom, email, entite_id, actif) VALUES ('{Guid.CreateVersion7()}', '{Mat}', 'B', 'B', 'bff{_sfx}@x.ma', '{e}', true);
            """;
        await new NpgsqlCommand(sql, c).ExecuteNonQueryAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private (HttpClient Client, WebApplicationFactory<Program> Fabrique) Creer(string environnement)
    {
        var f = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment(environnement);
            b.UseSetting("ConnectionStrings:Gp", pg.Chaine);
            b.ConfigureServices(s => s.AddControllers().AddApplicationPart(typeof(ControleurCsrfDeTest).Assembly));
        });
        // https : le cookie est Secure ; pas de redirection automatique pour inspecter la réponse du login.
        return (f.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false }), f);
    }

    private static Task<HttpResponseMessage> Login(HttpClient c, string matricule) =>
        c.PostAsync("/bff/login", new FormUrlEncodedContent([new("matricule", matricule)]));

    private static HttpRequestMessage PostApi(bool avecEnTete)
    {
        var r = new HttpRequestMessage(HttpMethod.Post, "/api/test-csrf");
        if (avecEnTete)
        {
            r.Headers.Add(ProtectionCsrf.EnTete, ProtectionCsrf.Valeur);
        }

        return r;
    }

    [Fact]
    public async Task User_sans_cookie_401()
    {
        var (c, f) = Creer("Development");
        using var _ = f;

        (await c.GetAsync("/bff/user")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_dev_pose_un_cookie_durci_puis_user_200()
    {
        var (c, f) = Creer("Test");
        using var _ = f;

        var login = await Login(c, Mat);

        login.StatusCode.Should().Be(HttpStatusCode.Redirect);
        login.Headers.Location!.OriginalString.Should().Be("/");
        var cookie = login.Headers.GetValues("Set-Cookie").Single(h => h.StartsWith("gp.session=", StringComparison.Ordinal));
        cookie.ToLowerInvariant().Should().Contain("httponly").And.Contain("secure").And.Contain("samesite=strict");

        var user = await c.GetAsync("/bff/user");
        user.StatusCode.Should().Be(HttpStatusCode.OK);
        (await user.Content.ReadFromJsonAsync<UtilisateurDto>())!.Matricule.Should().Be(Mat);
    }

    [Fact]
    public async Task Matricule_inconnu_est_refuse_sans_cookie()
    {
        var (c, f) = Creer("Development");
        using var _ = f;

        var login = await Login(c, "INCONNU" + _sfx);

        login.Headers.Location!.OriginalString.Should().StartWith("/connexion");
        login.Headers.Contains("Set-Cookie").Should().BeFalse();
        (await c.GetAsync("/bff/user")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_api_avec_cookie_exige_l_en_tete_anti_csrf()
    {
        var (c, f) = Creer("Development");
        using var _ = f;
        await Login(c, Mat);

        (await c.SendAsync(PostApi(false))).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await c.SendAsync(PostApi(true))).StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Logout_exige_l_en_tete_puis_ferme_la_session()
    {
        var (c, f) = Creer("Development");
        using var _ = f;
        await Login(c, Mat);

        (await c.PostAsync("/bff/logout", null)).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        var ok = new HttpRequestMessage(HttpMethod.Post, "/bff/logout");
        ok.Headers.Add(ProtectionCsrf.EnTete, ProtectionCsrf.Valeur);
        (await c.SendAsync(ok)).StatusCode.Should().Be(HttpStatusCode.NoContent);

        (await c.GetAsync("/bff/user")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task En_tete_dev_sans_cookie_reste_accepte_sans_en_tete_csrf()
    {
        var (c, f) = Creer("Development");
        using var _ = f;
        var r = PostApi(false);
        r.Headers.Add("X-Dev-Matricule", Mat);

        (await c.SendAsync(r)).StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task En_production_le_login_n_existe_pas_et_aucun_cookie_n_est_emis()
    {
        var (c, f) = Creer("Production");
        using var _ = f;

        var login = await Login(c, Mat);

        // Non mappé : le POST tombe sur le fallback du front (GET seul), d'où 405 plutôt que 404 ; dans les deux cas rien ne répond au login.
        login.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.MethodNotAllowed);
        login.Headers.Contains("Set-Cookie").Should().BeFalse();
        (await c.GetAsync("/bff/login")).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await c.GetAsync("/bff/user")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("fr-MA")]
    [InlineData("en")]
    [InlineData("ar-MA")]
    public async Task Langue_valide_pose_le_cookie_lisible_par_le_front(string culture)
    {
        var (c, f) = Creer("Development");
        using var _ = f;

        var r = await c.PostAsJsonAsync("/bff/langue", new LangueDto(culture));

        r.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var cookie = r.Headers.GetValues("Set-Cookie").Single(h => h.StartsWith(Langues.NomCookie + "=", StringComparison.Ordinal));
        cookie.Should().StartWith($"{Langues.NomCookie}={culture};");
        cookie.ToLowerInvariant().Should().NotContain("httponly").And.Contain("samesite=lax").And.Contain("max-age=");
    }

    [Theory]
    [InlineData("de-DE")]
    [InlineData("")]
    [InlineData("fr-MA; Path=/evil")]
    public async Task Langue_hors_liste_blanche_400_sans_cookie(string culture)
    {
        var (c, f) = Creer("Development");
        using var _ = f;

        var r = await c.PostAsJsonAsync("/bff/langue", new LangueDto(culture));

        r.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        r.Headers.Contains("Set-Cookie").Should().BeFalse();
    }

    [Fact]
    public async Task Langue_avec_session_exige_l_en_tete_anti_csrf()
    {
        var (c, f) = Creer("Development");
        using var _ = f;
        await Login(c, Mat);

        (await c.PostAsJsonAsync("/bff/langue", new LangueDto("en"))).StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
