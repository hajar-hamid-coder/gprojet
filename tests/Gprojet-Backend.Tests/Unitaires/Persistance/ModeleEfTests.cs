using System.Text.RegularExpressions;
using FluentAssertions;
using Gprojet.Backend.Entities;
using Gprojet.Backend.Entities.Commun;
using Gprojet.Backend.Entities.Projets;
using Gprojet.Backend.Repositories;
using Gprojet.Backend.Tests.Unitaires.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Gprojet.Backend.Tests.Unitaires.Persistance;

/// <summary>Le modèle EF (sans base de données) reproduit database/01-schema.sql : tables, colonnes, index, CHECK.</summary>
[Trait("Category", "Persistance")]
public class ModeleEfTests
{
    private static readonly Lazy<IModel> Modele = new(() =>
    {
        using var contexte = new GpDbContext(new DbContextOptionsBuilder<GpDbContext>()
            .UseNpgsql("Host=localhost;Database=factice").Options);
        return contexte.GetService<IDesignTimeModel>().Model;
    });

    private static IEnumerable<IEntityType> Entites => Modele.Value.GetEntityTypes();

    private static string Nom(IEntityType e) => $"{e.GetSchema()}.{e.GetTableName()}";

    private static string Normalise(string? sql) => Regex.Replace(sql ?? "", @"\s+", " ").Trim();

    [Fact]
    public void Soixante_tables_dans_les_six_schemas_identiques_au_sql()
    {
        Entites.Should().HaveCount(60);
        Entites.Select(e => e.GetSchema()).Distinct().Should().BeEquivalentTo(SchemaSql.Modules.Keys);
        Entites.Select(Nom).Should().BeEquivalentTo(SchemaSql.Tables().Select(t => $"{t.Schema}.{t.Table}"));
    }

    [Fact]
    public void Colonnes_identiques_au_sql()
    {
        foreach (var (schema, table, corps) in SchemaSql.Tables())
        {
            var entite = Entites.Single(e => Nom(e) == $"{schema}.{table}");
            var colonnes = entite.GetProperties()
                .Select(p => p.GetColumnName())
                .Where(c => c != "xmin"); // colonne système, absente du CREATE TABLE
            colonnes.Should().BeEquivalentTo(SchemaSql.Colonnes(corps), $"colonnes de {schema}.{table}");
        }
    }

    [Fact]
    public void Index_identiques_au_sql_noms_unicite_et_filtres_partiels()
    {
        var ef = Entites.SelectMany(e => e.GetIndexes().Select(i =>
            (Table: e.GetTableName()!, Nom: i.GetDatabaseName()!, i.IsUnique, Filtre: Normalise(i.GetFilter()))));
        var sql = SchemaSql.Index().Select(i => (i.Table, i.Nom, i.Unique, Filtre: Normalise(i.Filtre)));

        ef.Except(sql).Should().BeEmpty("index du modèle EF absents du SQL");
        sql.Except(ef).Should().BeEmpty("index du SQL absents du modèle EF");
        ef.Count(i => i.IsUnique && i.Filtre.Length > 0).Should().Be(sql.Count(i => i.Unique && i.Filtre.Length > 0));
    }

    [Fact]
    public void Contraintes_check_en_meme_nombre_que_le_sql()
    {
        foreach (var (schema, table, corps) in SchemaSql.Tables())
        {
            var entite = Entites.Single(e => Nom(e) == $"{schema}.{table}");
            entite.GetCheckConstraints().Should().HaveCount(Regex.Count(corps, @"\bCHECK \("), $"CHECK de {schema}.{table}");
        }
    }

    [Fact]
    public void Suppression_logique_concurrence_et_conversions()
    {
        foreach (var entite in Entites.Where(e => typeof(EntiteAuditable).IsAssignableFrom(e.ClrType)))
        {
            entite.GetDeclaredQueryFilters().Should().NotBeEmpty($"{entite.ClrType.Name} filtre est_supprime");
            var version = entite.FindProperty(nameof(EntiteAuditable.VersionLigne))!;
            version.GetColumnName().Should().Be("xmin");
            version.IsConcurrencyToken.Should().BeTrue();
            entite.FindProperty(nameof(EntiteAuditable.Id))!.GetDefaultValueSql().Should().Be("uuidv7()");
        }

        Modele.Value.FindEntityType(typeof(Historique))!.GetDeclaredQueryFilters().Should().BeEmpty();

        var raid = Modele.Value.FindEntityType(typeof(ElementRaid))!;
        raid.FindProperty(nameof(ElementRaid.Criticite))!.GetComputedColumnSql().Should().Be("probabilite * impact");
        raid.FindProperty(nameof(ElementRaid.Criticite))!.GetIsStored().Should().BeTrue();
        raid.FindProperty(nameof(ElementRaid.Statut))!.GetValueConverter()!.ConvertToProvider(StatutElementRaid.EnCours).Should().Be("EN_COURS");

        var projet = Modele.Value.FindEntityType(typeof(Projet))!;
        projet.FindProperty(nameof(Projet.CheminEntite))!.GetColumnType().Should().Be("ltree");
        projet.FindProperty(nameof(Projet.Priorite))!.GetMaxLength().Should().Be(20);
        projet.FindProperty(nameof(Projet.Priorite))!.ValueGenerated.Should().Be(ValueGenerated.Never, "la valeur C# est toujours envoyée");
    }
}
