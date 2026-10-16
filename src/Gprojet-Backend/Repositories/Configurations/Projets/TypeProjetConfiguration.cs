// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Projets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Projets;

public sealed class TypeProjetConfiguration : ConfigurationEntiteAuditable<TypeProjet>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<TypeProjet> builder)
    {
        builder.ToTable("type_projet", "projet");

        builder.Property(e => e.Code).HasMaxLength(30);
        builder.Property(e => e.Libelle).HasMaxLength(150);
        builder.Property(e => e.Actif).HasDefaultValueSql("true");

        builder.HasIndex(e => e.Code, "ux_type_projet_code").IsUnique().HasFilter("NOT est_supprime");
    }
}
