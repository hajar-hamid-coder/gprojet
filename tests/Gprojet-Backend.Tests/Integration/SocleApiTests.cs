using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using FluentAssertions;
using Gprojet.Backend.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Gprojet.Backend.Tests.Integration;

// Les actions MVC ne peuvent pas être statiques.
[SuppressMessage("Performance", "CA1822")]
[ApiController]
[Route("test-erreurs")]
public sealed class ControleurDeTest : ControllerBase
{
    [HttpGet("introuvable")]
    public IActionResult Introuvable() => throw new IntrouvableException("Projet inconnu.");

    [HttpGet("conflit")]
    public IActionResult Conflit() => throw new ConflitException("Déjà modifié.");

    [HttpGet("explose")]
    public IActionResult Explose() => throw new InvalidOperationException("secret-interne Host=db;Password=xyz");
}

[Collection(PgTestGroupe.Nom)]
[Trait("Category", "Integration")]
public sealed class SocleApiTests(PgTestFixture pg)
{
    // Écrase explicitement ConnectionStrings:Gp (même si la variable d'environnement vise gprojet) par la base jetable pg-test.
    private WebApplicationFactory<Program> Fabrique(string environnement) =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment(environnement);
            b.UseSetting("ConnectionStrings:Gp", pg.Chaine);
            b.ConfigureServices(s => s.AddControllers().AddApplicationPart(typeof(ControleurDeTest).Assembly));
        });

    [Fact]
    public async Task La_chaine_utilisee_est_celle_de_pg_test_et_jamais_gprojet()
    {
        using var f = Fabrique("Development");
        var chaine = f.Services.GetRequiredService<IConfiguration>().GetConnectionString("Gp")!;

        new NpgsqlConnectionStringBuilder(chaine).Database.Should().StartWith("gp_it_").And.NotBe("gprojet");
    }

    [Theory]
    [InlineData("/health")]
    [InlineData("/health/ready")]
    [InlineData("/openapi/v1.json")]
    public async Task Les_points_d_entree_techniques_repondent_200(string chemin)
    {
        using var f = Fabrique("Development");

        var reponse = await f.CreateClient().GetAsync(chemin);

        reponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("introuvable", HttpStatusCode.NotFound)]
    [InlineData("conflit", HttpStatusCode.Conflict)]
    public async Task Une_exception_metier_donne_un_problem_json(string route, HttpStatusCode attendu)
    {
        using var f = Fabrique("Production");

        var reponse = await f.CreateClient().GetAsync("/test-erreurs/" + route);

        reponse.StatusCode.Should().Be(attendu);
        reponse.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
        using var json = JsonDocument.Parse(await reponse.Content.ReadAsStringAsync());
        json.RootElement.GetProperty("status").GetInt32().Should().Be((int)attendu);
    }

    [Fact]
    public async Task Une_erreur_inattendue_ne_fuit_pas_en_production()
    {
        using var f = Fabrique("Production");

        var reponse = await f.CreateClient().GetAsync("/test-erreurs/explose");
        var corps = await reponse.Content.ReadAsStringAsync();

        reponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        reponse.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
        corps.Should().NotContain("secret-interne").And.NotContain("Password").And.NotContain("InvalidOperationException");
    }

    [Fact]
    public async Task Le_detail_est_visible_en_development()
    {
        using var f = Fabrique("Development");

        var corps = await (await f.CreateClient().GetAsync("/test-erreurs/explose")).Content.ReadAsStringAsync();

        corps.Should().Contain("InvalidOperationException");
    }
}
