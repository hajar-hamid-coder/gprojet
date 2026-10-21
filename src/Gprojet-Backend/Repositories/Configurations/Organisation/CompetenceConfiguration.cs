// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Organisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Organisation;

public sealed class CompetenceConfiguration : ConfigurationEntiteAuditable<Competence>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Competence> builder)
    {
        builder.ToTable("competence", "org");

        builder.Property(e => e.Code).HasMaxLength(30);
        builder.Property(e => e.Libelle).HasMaxLength(150);
        builder.Property(e => e.Categorie).HasMaxLength(100);

        builder.HasIndex(e => e.Code, "ux_competence_code").IsUnique().HasFilter("NOT est_supprime");
    }
}
