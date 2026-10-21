// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Temps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Temps;

public sealed class CategorieActiviteConfiguration : ConfigurationEntiteAuditable<CategorieActivite>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<CategorieActivite> builder)
    {
        builder.ToTable("categorie_activite", "temps");

        builder.Property(e => e.Code).HasMaxLength(30);
        builder.Property(e => e.Libelle).HasMaxLength(100);
        builder.Property(e => e.EstProjet).HasDefaultValueSql("true");
        builder.Property(e => e.Actif).HasDefaultValueSql("true");

        builder.HasIndex(e => e.Code, "ux_categorie_activite_code").IsUnique().HasFilter("NOT est_supprime");
    }
}
