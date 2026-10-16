using System.Diagnostics;
using Gprojet.Backend.Common;
using Gprojet.Backend.Repositories;
using Gprojet.Backend.Repositories.Audit;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Gprojet.Backend.Tests.Integration;

[CollectionDefinition(Nom)]
public sealed class PgTestGroupe : ICollectionFixture<PgTestFixture>
{
    public const string Nom = "pg-test";
}

/// <summary>
/// Base jetable dédiée aux tests, créée sur l'instance <c>pg-test</c> (port 55432, jamais <c>gprojet</c>) puis migrée par EF.
/// Si l'instance n'est pas démarrée, la fixture lance <c>python scripts/pg-test.py demarrer --vide</c> et l'arrête à la fin ;
/// si elle l'est déjà (<c>python scripts/pg-test.py demarrer --vide</c> lancé à la main), elle la laisse en place.
/// </summary>
public sealed class PgTestFixture : IAsyncLifetime
{
    private static readonly string Port = Environment.GetEnvironmentVariable("PG_TEST_PORT") ?? "55432";
    private readonly string _base = "gp_it_" + Guid.NewGuid().ToString("N")[..12];
    private bool _demarreeParNous;
    private string _script = null!;

    public string Chaine => $"Host=localhost;Port={Port};Database={_base};Username=postgres;Pooling=false";

    /// <summary>Connexion superutilisateur à la base de test (manipulations SQL directes).</summary>
    public NpgsqlConnection Ouvrir() => new(Chaine);

    public GpDbContext Contexte(ICurrentUser? utilisateur = null, IHorloge? horloge = null) =>
        new(new DbContextOptionsBuilder<GpDbContext>().UseNpgsql(Chaine)
            .AddInterceptors(new AuditInterceptor(utilisateur ?? new UtilisateurFactice(), horloge ?? new HorlogeFactice(), new ContexteRequete()))
            .Options);

    public async Task InitializeAsync()
    {
        var dossier = new DirectoryInfo(AppContext.BaseDirectory);
        while (dossier is not null && !File.Exists(Path.Combine(dossier.FullName, "scripts", "pg-test.py")))
        {
            dossier = dossier.Parent;
        }

        _script = Path.Combine(dossier?.FullName ?? throw new InvalidOperationException("scripts/pg-test.py introuvable."), "scripts", "pg-test.py");

        if (!await InstanceJoignableAsync())
        {
            Python("demarrer", "--vide");
            _demarreeParNous = true;
        }

        await using (var admin = new NpgsqlConnection($"Host=localhost;Port={Port};Database=postgres;Username=postgres;Pooling=false"))
        {
            await admin.OpenAsync();
            await using var cmd = new NpgsqlCommand($"CREATE DATABASE {_base}", admin);
            await cmd.ExecuteNonQueryAsync();
        }

        await using var db = new GpDbContext(new DbContextOptionsBuilder<GpDbContext>().UseNpgsql(Chaine).Options);
        await db.Database.MigrateAsync();
    }

    public Task DisposeAsync()
    {
        if (_demarreeParNous)
        {
            Python("arreter");
        }
        else
        {
            using var admin = new NpgsqlConnection($"Host=localhost;Port={Port};Database=postgres;Username=postgres;Pooling=false");
            admin.Open();
            using var cmd = new NpgsqlCommand($"DROP DATABASE IF EXISTS {_base} WITH (FORCE)", admin);
            cmd.ExecuteNonQuery();
        }

        return Task.CompletedTask;
    }

    private static async Task<bool> InstanceJoignableAsync()
    {
        try
        {
            await using var c = new NpgsqlConnection($"Host=localhost;Port={Port};Database=postgres;Username=postgres;Timeout=3;Pooling=false");
            await c.OpenAsync();
            return true;
        }
        catch (NpgsqlException)
        {
            return false;
        }
    }

    private void Python(params string[] args)
    {
        var psi = new ProcessStartInfo("python") { RedirectStandardOutput = true, RedirectStandardError = true };
        psi.ArgumentList.Add(_script);
        foreach (var a in args)
        {
            psi.ArgumentList.Add(a);
        }

        using var p = Process.Start(psi)!;
        var sortie = p.StandardOutput.ReadToEnd() + p.StandardError.ReadToEnd();
        p.WaitForExit();
        if (p.ExitCode != 0)
        {
            throw new InvalidOperationException($"pg-test.py {string.Join(' ', args)} a échoué : {sortie}");
        }
    }
}

public sealed class UtilisateurFactice : ICurrentUser
{
    public Guid Id { get; init; } = Guid.Parse("00000000-0000-7000-8000-000000000001");

    public string Matricule => "T001";

    public Guid EntiteId => Guid.Empty;

    public IReadOnlyCollection<string> Roles => [];
}

public sealed class HorlogeFactice : IHorloge
{
    public DateTimeOffset Maintenant { get; set; } = new DateTimeOffset(2026, 10, 16, 12, 30, 45, 123, TimeSpan.Zero).AddTicks(4560);

    public DateOnly Aujourdhui => DateOnly.FromDateTime(Maintenant.UtcDateTime);
}
