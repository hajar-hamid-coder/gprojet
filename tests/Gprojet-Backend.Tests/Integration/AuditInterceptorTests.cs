using FluentAssertions;
using Gprojet.Backend.Entities.Commun;
using Gprojet.Backend.Entities.Organisation;
using Gprojet.Backend.Repositories.Audit;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Gprojet.Backend.Tests.Integration;

/// <summary>Lancer : <c>dotnet test --filter Category=Integration</c> (démarre pg-test si besoin, voir PgTestFixture).</summary>
[Collection(PgTestGroupe.Nom)]
[Trait("Category", "Integration")]
public sealed class AuditInterceptorTests(PgTestFixture pg)
{
    private static Competence Nouvelle(string code) => new() { Code = code, Libelle = "Libellé " + code };

    private static async Task<List<Historique>> HistoriqueDeAsync(PgTestFixture pg, Guid id)
    {
        await using var db = pg.Contexte();
        return await db.Set<Historique>().Where(h => h.CibleId == id).OrderBy(h => h.Id).ToListAsync();
    }

    [Fact]
    public async Task Creation_remplit_audit_et_historique()
    {
        var utilisateur = new UtilisateurFactice();
        var horloge = new HorlogeFactice();
        var c = Nouvelle("CRE");
        await using (var db = pg.Contexte(utilisateur, horloge))
        {
            db.Add(c);
            await db.SaveChangesAsync();
        }

        await using var lecture = pg.Contexte();
        var relu = await lecture.Set<Competence>().SingleAsync(x => x.Id == c.Id);
        relu.CreeLe.Should().Be(horloge.Maintenant);
        relu.CreePar.Should().Be(utilisateur.Id);

        var h = (await HistoriqueDeAsync(pg, c.Id)).Should().ContainSingle().Subject;
        h.Action.Should().Be(ActionHistorique.Creation);
        h.TypeCible.Should().Be("competence");
        h.AgentId.Should().Be(utilisateur.Id);
        h.CorrelationId.Should().NotBeNull();
        h.Empreinte.Should().MatchRegex("^[0-9a-f]{64}$");
    }

    [Fact]
    public async Task Suppression_est_logique_et_masquee_par_le_filtre_global()
    {
        var c = Nouvelle("SUP");
        await using (var db = pg.Contexte())
        {
            db.Add(c);
            await db.SaveChangesAsync();
        }

        await using (var db = pg.Contexte())
        {
            db.Remove(await db.Set<Competence>().SingleAsync(x => x.Id == c.Id));
            await db.SaveChangesAsync();
        }

        await using var lecture = pg.Contexte();
        (await lecture.Set<Competence>().AnyAsync(x => x.Id == c.Id)).Should().BeFalse();
        var brut = await lecture.Set<Competence>().IgnoreQueryFilters().SingleAsync(x => x.Id == c.Id);
        brut.EstSupprime.Should().BeTrue();
        brut.SupprimeLe.Should().NotBeNull();
        brut.SupprimePar.Should().NotBeNull();
        (await HistoriqueDeAsync(pg, c.Id)).Select(h => h.Action)
            .Should().Equal(ActionHistorique.Creation, ActionHistorique.Suppression);
    }

    [Fact]
    public async Task Modification_historise_chaque_champ_change_avec_chaine_valide()
    {
        var c = Nouvelle("MOD");
        await using (var db = pg.Contexte())
        {
            db.Add(c);
            await db.SaveChangesAsync();
        }

        await using (var db = pg.Contexte())
        {
            var x = await db.Set<Competence>().SingleAsync(y => y.Id == c.Id);
            x.Libelle = "Nouveau";
            x.Categorie = "Cat";
            x.Code = "MOD"; // inchangé : aucune ligne
            await db.SaveChangesAsync();
        }

        var lignes = await HistoriqueDeAsync(pg, c.Id);
        lignes.Where(h => h.Action == ActionHistorique.Modification).Select(h => (h.Champ, h.AncienneValeur, h.NouvelleValeur))
            .Should().BeEquivalentTo([("libelle", (string?)"Libellé MOD", (string?)"Nouveau"), ("categorie", null, "Cat")]);

        await using var verif = pg.Contexte();
        (await new VerificateurChaineHistorique(verif).PremiereLigneInvalideAsync()).Should().BeNull();
    }

    [Fact]
    public async Task Alteration_en_sql_direct_est_detectee()
    {
        var c = Nouvelle("ALT");
        await using (var db = pg.Contexte())
        {
            db.Add(c);
            await db.SaveChangesAsync();
            c.Libelle = "Autre";
            await db.SaveChangesAsync();
        }

        await using var admin = pg.Ouvrir();
        await admin.OpenAsync();
        var cible = await Sql<long>(admin, $"SELECT id FROM commun.historique WHERE cible_id = '{c.Id}' AND champ = 'libelle'");
        await Sql(admin, "ALTER TABLE commun.historique DISABLE TRIGGER tg_historique_lecture_seule");
        await Sql(admin, $"UPDATE commun.historique SET nouvelle_valeur = 'Falsifie' WHERE id = {cible}");
        await Sql(admin, "ALTER TABLE commun.historique ENABLE TRIGGER tg_historique_lecture_seule");

        try
        {
            await using var verif = pg.Contexte();
            var invalide = await new VerificateurChaineHistorique(verif).PremiereLigneInvalideAsync();
            invalide.Should().NotBeNull();
            invalide!.Id.Should().Be(cible);
        }
        finally
        {
            // La base est partagée par les autres tests : on rétablit la valeur d'origine.
            await Sql(admin, "ALTER TABLE commun.historique DISABLE TRIGGER tg_historique_lecture_seule");
            await Sql(admin, $"UPDATE commun.historique SET nouvelle_valeur = 'Autre' WHERE id = {cible}");
            await Sql(admin, "ALTER TABLE commun.historique ENABLE TRIGGER tg_historique_lecture_seule");
        }
    }

    [Fact]
    public async Task Ecritures_concurrentes_gardent_une_chaine_valide()
    {
        async Task Ecrire(string prefixe)
        {
            for (var i = 0; i < 10; i++)
            {
                await using var db = pg.Contexte();
                db.Add(Nouvelle($"{prefixe}{i}"));
                await db.SaveChangesAsync();
            }
        }

        await Task.WhenAll(Ecrire("CA"), Ecrire("CB"), Ecrire("CC"));

        await using var verif = pg.Contexte();
        (await new VerificateurChaineHistorique(verif).PremiereLigneInvalideAsync()).Should().BeNull();
        var precedentes = await verif.Set<Historique>().Select(h => h.EmpreintePrecedente).ToListAsync();
        precedentes.Where(p => p is null).Should().HaveCount(1); // une seule racine : pas de fourche
    }

    [Fact]
    public async Task Rollback_n_ecrit_aucun_historique()
    {
        var c = Nouvelle("RB");
        await using (var db = pg.Contexte())
        {
            db.Add(c);
            db.Add(Nouvelle("RB")); // code unique en doublon : échec de la sauvegarde
            await db.Invoking(d => d.SaveChangesAsync()).Should().ThrowAsync<DbUpdateException>();
        }

        (await HistoriqueDeAsync(pg, c.Id)).Should().BeEmpty();
    }

    [Fact]
    public async Task Type_cible_tient_dans_30_caracteres_et_les_noms_de_table_sont_uniques()
    {
        await using var db = pg.Contexte();
        var tables = db.Model.GetEntityTypes().Select(e => e.GetTableName()!).ToList();
        tables.Should().OnlyContain(t => t.Length <= 30);
        tables.Should().OnlyHaveUniqueItems(); // type_cible sans schéma : doit rester non ambigu
    }

    private static async Task Sql(NpgsqlConnection c, string sql)
    {
        await using var cmd = new NpgsqlCommand(sql, c);
        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task<T> Sql<T>(NpgsqlConnection c, string sql)
    {
        await using var cmd = new NpgsqlCommand(sql, c);
        return (T)(await cmd.ExecuteScalarAsync())!;
    }
}
