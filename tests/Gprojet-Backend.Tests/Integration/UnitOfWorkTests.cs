using FluentAssertions;
using Gprojet.Backend.Common;
using Gprojet.Backend.Entities.Commun;
using Gprojet.Backend.Entities.Organisation;
using Gprojet.Backend.Repositories;
using Gprojet.Backend.Repositories.Organisation;
using Microsoft.EntityFrameworkCore;

namespace Gprojet.Backend.Tests.Integration;

[Collection(PgTestGroupe.Nom)]
[Trait("Category", "Integration")]
public sealed class UnitOfWorkTests(PgTestFixture pg)
{
    private static string Code(string prefixe) => prefixe + Guid.NewGuid().ToString("N")[..8];

    private static Competence Nouvelle(string code) => new() { Code = code, Libelle = "Libellé " + code };

    private async Task<(int Competences, int Historiques)> CompterAsync(params Guid[] ids)
    {
        await using var db = pg.Contexte();
        return (await db.Set<Competence>().CountAsync(c => ids.Contains(c.Id)),
                await db.Set<Historique>().CountAsync(h => ids.Contains(h.CibleId)));
    }

    [Fact]
    public async Task Transaction_validee_ecrit_donnees_et_historique()
    {
        var a = Nouvelle(Code("OK"));
        var b = Nouvelle(Code("OK"));
        await using (var db = pg.Contexte())
        {
            var uow = new UnitOfWork(db);
            var repo = new CompetenceRepository(db);
            await uow.ExecuterDansTransactionAsync(async ct =>
            {
                repo.Ajouter(a);
                await uow.CommitAsync(ct);
                repo.Ajouter(b);
                await uow.CommitAsync(ct);
            });
        }

        (await CompterAsync(a.Id, b.Id)).Should().Be((2, 2)); // historique écrit dans la transaction englobante
    }

    [Fact]
    public async Task Echec_au_milieu_annule_tout_donnees_et_historique()
    {
        var a = Nouvelle(Code("KO"));
        var b = Nouvelle(Code("KO"));
        await using (var db = pg.Contexte())
        {
            var uow = new UnitOfWork(db);
            var repo = new CompetenceRepository(db);
            var act = () => uow.ExecuterDansTransactionAsync(async ct =>
            {
                repo.Ajouter(a);
                await uow.CommitAsync(ct);
                repo.Ajouter(b);
                await uow.CommitAsync(ct);
                throw new InvalidOperationException("échec simulé");
            });
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        (await CompterAsync(a.Id, b.Id)).Should().Be((0, 0));
    }

    [Fact]
    public async Task Echec_dun_commit_seul_nenregistre_rien()
    {
        var code = Code("DUP");
        var a = Nouvelle(code);
        var b = Nouvelle(code); // doublon : violation d'unicité au milieu du même SaveChanges
        await using (var db = pg.Contexte())
        {
            var repo = new CompetenceRepository(db);
            repo.Ajouter(a);
            repo.Ajouter(b);
            var act = () => new UnitOfWork(db).CommitAsync();
            await act.Should().ThrowAsync<DbUpdateException>();
        }

        (await CompterAsync(a.Id, b.Id)).Should().Be((0, 0));
    }

    [Fact]
    public async Task Deux_mises_a_jour_concurrentes_levent_ConflitException()
    {
        var c = Nouvelle(Code("CON"));
        await using (var db = pg.Contexte())
        {
            new CompetenceRepository(db).Ajouter(c);
            await new UnitOfWork(db).CommitAsync();
        }

        await using var db1 = pg.Contexte();
        await using var db2 = pg.Contexte();
        var c1 = (await new CompetenceRepository(db1).GetByIdAsync(c.Id))!;
        var c2 = (await new CompetenceRepository(db2).GetByIdAsync(c.Id))!;

        c1.Libelle = "premier";
        await new UnitOfWork(db1).CommitAsync();

        c2.Libelle = "second";
        var act = () => new UnitOfWork(db2).CommitAsync();
        (await act.Should().ThrowAsync<ConflitException>()).Which.Message.Should().NotContain("xmin").And.NotContain("UPDATE");
    }

    [Fact]
    public async Task GetById_pagination_et_suppression_logique()
    {
        var prefixe = Code("PG");
        var items = Enumerable.Range(1, 5).Select(i => Nouvelle($"{prefixe}-{i}")).ToList();
        await using (var db = pg.Contexte())
        {
            var repo = new CompetenceRepository(db);
            items.ForEach(repo.Ajouter);
            await new UnitOfWork(db).CommitAsync();
        }

        await using var lecture = pg.Contexte();
        var r = new CompetenceRepository(lecture);
        (await r.GetByIdAsync(items[0].Id))!.Code.Should().Be(items[0].Code);
        (await r.GetByIdAsync(Guid.NewGuid())).Should().BeNull();

        // Le repository d'agrégat n'expose pas de filtre : on compte via la base partagée en ciblant nos Id.
        var attendus = items.Select(i => i.Id).ToHashSet();
        var tous = new List<Guid>();
        for (var page = 1; ; page++)
        {
            var p = await r.ListerAsync(new PageRequete { Page = page, Taille = 200 });
            tous.AddRange(p.Elements.Select(e => e.Id).Where(attendus.Contains));
            if (page >= p.NombrePages) break;
        }

        tous.Should().Equal(items.Select(i => i.Id).Order()); // trié par Id
        var p2 = await r.ListerAsync(new PageRequete { Page = 2, Taille = 2 });
        p2.Elements.Should().HaveCount(2);
        p2.Total.Should().BeGreaterThanOrEqualTo(5);
        (await r.ListerAsync(new PageRequete { Page = 2, Taille = 2 })).Elements.Select(e => e.Id)
            .Should().Equal(p2.Elements.Select(e => e.Id)); // déterministe

        await using (var db = pg.Contexte())
        {
            var repo = new CompetenceRepository(db);
            repo.Supprimer((await repo.GetByIdAsync(items[0].Id))!);
            await new UnitOfWork(db).CommitAsync();
        }

        await using var apres = pg.Contexte();
        (await new CompetenceRepository(apres).GetByIdAsync(items[0].Id)).Should().BeNull();
        (await apres.Set<Competence>().IgnoreQueryFilters().SingleAsync(x => x.Id == items[0].Id)).EstSupprime.Should().BeTrue();
    }
}
