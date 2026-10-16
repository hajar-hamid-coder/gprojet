using System.Reflection;
using FluentAssertions;
using Gprojet.Backend.Common;
using Gprojet.Backend.Entities;
using Gprojet.Backend.Entities.Commun;
using Gprojet.Backend.Entities.Projets;

namespace Gprojet.Backend.Tests.Unitaires.Entites;

/// <summary>
/// Les entités reproduisent database/01-schema.sql : une classe par table (Pascal du nom, espace de noms du module),
/// une énumération par CHECK IN texte dont les valeurs (via <see cref="ValeurEnumSql"/>) sont exactement celles du SQL.
/// « Type entité » = classe concrète de l'espace de noms Gprojet.Backend.Entities (ni abstraite, ni énumération).
/// </summary>
[Trait("Category", "Entites")]
public class EntitesTests
{
    private static readonly Assembly Backend = typeof(EntiteAuditable).Assembly;

    private static Type Entite(string schema, string table) =>
        Backend.GetType($"Gprojet.Backend.Entities.{SchemaSql.Modules[schema]}.{SchemaSql.Pascal(table)}")
        ?? throw new InvalidOperationException($"Aucune entité pour {schema}.{table}");

    public static TheoryData<string, string, string, string[]> ChecksIn()
    {
        var data = new TheoryData<string, string, string, string[]>();
        foreach (var (schema, table, colonne, valeurs) in SchemaSql.ChecksIn())
        {
            data.Add(schema, table, colonne, valeurs);
        }

        return data;
    }

    [Fact]
    public void Soixante_types_entites()
    {
        var entites = Backend.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsNested: false }
                        && t.Namespace?.StartsWith("Gprojet.Backend.Entities", StringComparison.Ordinal) == true)
            .ToList();

        entites.Should().HaveCount(60);
        SchemaSql.Tables().Should().HaveCount(60);
        SchemaSql.Tables().Select(t => Entite(t.Schema, t.Table)).Should().BeEquivalentTo(entites);
    }

    [Fact]
    public void Toutes_heritent_d_EntiteAuditable_sauf_Historique()
    {
        foreach (var (schema, table, _) in SchemaSql.Tables())
        {
            var type = Entite(schema, table);
            type.IsSubclassOf(typeof(EntiteAuditable)).Should().Be(type != typeof(Historique), type.Name);
        }

        typeof(Historique).GetProperty(nameof(Historique.Id))!.PropertyType.Should().Be<long>();
    }

    [Theory]
    [MemberData(nameof(ChecksIn))]
    public void Enumeration_identique_au_check_sql(string schema, string table, string colonne, string[] valeurs)
    {
        var propriete = Entite(schema, table).GetProperty(SchemaSql.Pascal(colonne));
        propriete.Should().NotBeNull($"{schema}.{table}.{colonne} doit avoir une propriété");

        var type = Nullable.GetUnderlyingType(propriete!.PropertyType) ?? propriete.PropertyType;
        type.IsEnum.Should().BeTrue($"{schema}.{table}.{colonne} a un CHECK IN : sa propriété doit être une énumération");

        Enum.GetValues(type).Cast<Enum>().Select(ValeurEnumSql.VersSql)
            .Should().Equal(valeurs, $"l'énumération {type.Name} doit reproduire le CHECK de {schema}.{table}.{colonne}");
    }

    [Fact]
    public void Conversion_sql_aller_retour()
    {
        ValeurEnumSql.VersSql(StatutProjet.EnAffectation).Should().Be("EN_AFFECTATION");
        ValeurEnumSql.DepuisSql<StatutProjet>("EN_AFFECTATION").Should().Be(StatutProjet.EnAffectation);
        ValeurEnumSql.VersSql(Entities.Modelisation.CardinaliteMax.Un).Should().Be("1");
        ValeurEnumSql.DepuisSql<Entities.Modelisation.StackBack>("DOTNET_10").Should().Be(Entities.Modelisation.StackBack.Dotnet10);
        FluentActions.Invoking(() => ValeurEnumSql.DepuisSql<StatutProjet>("INCONNU")).Should().Throw<ArgumentOutOfRangeException>();
    }
}
