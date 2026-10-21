using System.Text.RegularExpressions;
using Gprojet.Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Gprojet.Backend.Repositories;

/// <summary>
/// Contexte EF Core unique (6 schémas). Le détail de chaque table est dans <c>Configurations/&lt;Module&gt;/</c> ;
/// ici ne vivent que les conventions transverses, alignées sur <c>database/01-schema.sql</c>.
/// </summary>
public sealed partial class GpDbContext(DbContextOptions<GpDbContext> options) : DbContext(options)
{
    // Tous les index sont déclarés dans les configurations, comme dans le SQL : pas d'index FK implicite
    // (sinon EF ajoute IX_entite_attribut_libelle_id, absent du schéma de référence).
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) =>
        configurationBuilder.Conventions.Remove<ForeignKeyIndexConvention>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("ltree");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GpDbContext).Assembly);

        foreach (var entite in modelBuilder.Model.GetEntityTypes())
        {
            var table = entite.GetTableName()!;
            entite.FindPrimaryKey()!.SetName($"{table}_pkey");

            foreach (var propriete in entite.GetProperties())
            {
                if (propriete.Name != nameof(EntiteAuditable.VersionLigne)
                    && propriete.FindAnnotation(RelationalAnnotationNames.ColumnName) is null)
                {
                    propriete.SetColumnName(SnakeCase(propriete.Name));
                }

                var type = Nullable.GetUnderlyingType(propriete.ClrType) ?? propriete.ClrType;
                if (type.IsEnum)
                {
                    propriete.SetValueConverter((ValueConverter)Activator.CreateInstance(typeof(ConvertisseurEnumSql<>).MakeGenericType(type))!);
                }

                // Le DEFAULT SQL reste dans le schéma, mais EF envoie toujours la valeur C# : sinon false / 0 / premier membre
                // d'enum seraient pris pour « non renseigné » et remplacés par le défaut (ex. Actif = false => true).
                // Exceptions : PK uuid (toujours fixée côté C#) et horodatages now().
                if (propriete.GetDefaultValueSql() is not null && type != typeof(Guid) && type != typeof(DateTimeOffset))
                {
                    propriete.ValueGenerated = ValueGenerated.Never;
                }
            }

            // Nom explicite : sinon EF suffixe « 1 » les homonymes de schémas différents (ix_entite_parent dans org et modelisation).
            foreach (var index in entite.GetIndexes())
            {
                index.SetDatabaseName(index.Name);
            }

            foreach (var cle in entite.GetForeignKeys())
            {
                if (cle.FindAnnotation(RelationalAnnotationNames.Name) is null)
                {
                    cle.SetConstraintName($"{table}_{cle.Properties[0].GetColumnName()}_fkey"); // nom par défaut de PostgreSQL
                }
            }
        }
    }

    private static string SnakeCase(string nom) => Majuscule().Replace(nom, "_").ToLowerInvariant();

    [GeneratedRegex("(?<!^)(?=[A-Z])")]
    private static partial Regex Majuscule();
}
