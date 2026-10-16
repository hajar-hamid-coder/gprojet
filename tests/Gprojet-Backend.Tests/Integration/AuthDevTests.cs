using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Gprojet.Backend.Common;
using Gprojet.Backend.Infrastructure.Authentification;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Gprojet.Backend.Tests.Integration;

[Collection(PgTestGroupe.Nom)]
[Trait("Category", "Integration")]
public sealed class AuthDevTests(PgTestFixture pg) : IAsyncLifetime
{
    private readonly string _sfx = Guid.NewGuid().ToString("N")[..8];
    private readonly Guid _entite = Guid.CreateVersion7();
    private readonly Guid _chef = Guid.CreateVersion7();
    private readonly Guid _agent = Guid.CreateVersion7();
    private readonly Guid _inactif = Guid.CreateVersion7();

    private string Mat(string p) => p + _sfx;

    public async Task InitializeAsync()
    {
        await using var c = pg.Ouvrir();
        await c.OpenAsync();
        var ent = _entite.ToString("N");
        var aujourdhui = DateOnly.FromDateTime(DateTime.UtcNow);
        var sql = $"""
            INSERT INTO org.entite_organisationnelle (id, code, libelle, niveau, chemin) VALUES ('{_entite}', 'E{_sfx}', 'Service test', 'SERVICE', '{ent}');
            INSERT INTO org.agent (id, matricule, nom, prenom, email, entite_id, actif) VALUES
              ('{_chef}', '{Mat("CHEF")}', 'Chef', 'C', 'chef{_sfx}@x.ma', '{_entite}', true),
              ('{_agent}', '{Mat("AGT")}', 'Agent', 'A', 'agt{_sfx}@x.ma', '{_entite}', true),
              ('{_inactif}', '{Mat("OFF")}', 'Off', 'O', 'off{_sfx}@x.ma', '{_entite}', false);
            UPDATE org.entite_organisationnelle SET responsable_id = '{_chef}' WHERE id = '{_entite}';
            INSERT INTO org.delegation (delegant_id, delegataire_id, entite_id, date_debut, date_fin)
              VALUES ('{_chef}', '{_agent}', '{_entite}', '{aujourdhui.AddDays(-2):yyyy-MM-dd}', '{aujourdhui.AddDays(2):yyyy-MM-dd}');
            """;
        await new NpgsqlCommand(sql, c).ExecuteNonQueryAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private WebApplicationFactory<Program> Fabrique(string environnement) =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment(environnement);
            b.UseSetting("ConnectionStrings:Gp", pg.Chaine);
            b.ConfigureServices(s => s.AddControllers().AddApplicationPart(typeof(ControleurDeTest).Assembly));
        });

    private static HttpClient Client(WebApplicationFactory<Program> f, string? matricule)
    {
        var c = f.CreateClient();
        if (matricule is not null)
        {
            c.DefaultRequestHeaders.Add(DevAuthHandler.EnTete, matricule);
        }

        return c;
    }

    private sealed record Moi(Guid Id, string Matricule, Guid EntiteId, string[] Roles);

    [Fact]
    public async Task Sans_en_tete_401()
    {
        using var f = Fabrique("Development");
        (await Client(f, null).GetAsync("/test-erreurs/moi")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Matricule_inconnu_401()
    {
        using var f = Fabrique("Development");
        (await Client(f, "INCONNU" + _sfx).GetAsync("/test-erreurs/moi")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Agent_inactif_401()
    {
        using var f = Fabrique("Development");
        (await Client(f, Mat("OFF")).GetAsync("/test-erreurs/moi")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Responsable_d_entite_200_avec_son_role()
    {
        using var f = Fabrique("Test");
        var r = await Client(f, Mat("CHEF")).GetAsync("/test-erreurs/moi");

        r.StatusCode.Should().Be(HttpStatusCode.OK);
        var moi = (await r.Content.ReadFromJsonAsync<Moi>())!;
        moi.Should().BeEquivalentTo(new Moi(_chef, Mat("CHEF"), _entite, [RolesApplication.ResponsableEntite]));
    }

    [Fact]
    public async Task Delegataire_actif_recoit_le_role_delegataire()
    {
        using var f = Fabrique("Development");
        var moi = (await Client(f, Mat("AGT")).GetFromJsonAsync<Moi>("/test-erreurs/moi"))!;

        moi.Id.Should().Be(_agent);
        moi.Roles.Should().BeEquivalentTo([RolesApplication.Delegataire]);
    }

    [Fact]
    public async Task Delegation_expiree_ne_donne_pas_le_role()
    {
        await using (var c = pg.Ouvrir())
        {
            await c.OpenAsync();
            await new NpgsqlCommand($"UPDATE org.delegation SET date_fin = current_date - 10, date_debut = current_date - 20 WHERE delegataire_id = '{_agent}'", c).ExecuteNonQueryAsync();
        }

        using var f = Fabrique("Development");
        var moi = (await Client(f, Mat("AGT")).GetFromJsonAsync<Moi>("/test-erreurs/moi"))!;

        moi.Roles.Should().BeEmpty();
    }

    private async Task ExecuterAsync(string sql)
    {
        await using var c = pg.Ouvrir();
        await c.OpenAsync();
        await new NpgsqlCommand(sql, c).ExecuteNonQueryAsync();
    }

    [Fact]
    public async Task Agent_actif_mais_supprime_logiquement_401()
    {
        await ExecuterAsync($"UPDATE org.agent SET est_supprime = true WHERE id = '{_agent}' AND actif");
        using var f = Fabrique("Development");
        (await Client(f, Mat("AGT")).GetAsync("/test-erreurs/moi")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Entite_supprimee_logiquement_ne_donne_pas_le_role_responsable()
    {
        await ExecuterAsync($"UPDATE org.entite_organisationnelle SET est_supprime = true WHERE id = '{_entite}'");
        using var f = Fabrique("Development");
        var moi = (await Client(f, Mat("CHEF")).GetFromJsonAsync<Moi>("/test-erreurs/moi"))!;

        moi.Roles.Should().BeEmpty();
    }

    [Fact]
    public async Task Delegation_supprimee_logiquement_ne_donne_pas_le_role()
    {
        await ExecuterAsync($"UPDATE org.delegation SET est_supprime = true WHERE delegataire_id = '{_agent}'");
        using var f = Fabrique("Development");
        var moi = (await Client(f, Mat("AGT")).GetFromJsonAsync<Moi>("/test-erreurs/moi"))!;

        moi.Roles.Should().BeEmpty();
    }

    [Fact]
    public async Task En_production_le_schema_Dev_est_absent_et_l_en_tete_ignore()
    {
        using var f = Fabrique("Production");
        var schemas = f.Services.GetRequiredService<IAuthenticationSchemeProvider>();

        (await schemas.GetSchemeAsync(DevAuthHandler.Schema)).Should().BeNull();
        (await Client(f, Mat("CHEF")).GetAsync("/test-erreurs/moi")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Les_points_techniques_restent_anonymes_en_production()
    {
        using var f = Fabrique("Production");
        (await f.CreateClient().GetAsync("/health")).StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
