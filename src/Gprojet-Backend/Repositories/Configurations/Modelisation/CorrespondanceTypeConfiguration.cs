// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class CorrespondanceTypeConfiguration : ConfigurationEntiteAuditable<CorrespondanceType>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<CorrespondanceType> builder)
    {
        builder.ToTable("correspondance_type", "modelisation", t =>
        {
            t.HasCheckConstraint("correspondance_type_cible_check", "cible IN ('POSTGRESQL','CSHARP')");
        });

        builder.Property(e => e.TypeLogique).HasMaxLength(15);
        builder.Property(e => e.Cible).HasMaxLength(15);
        builder.Property(e => e.TypePhysique).HasMaxLength(100);

        builder.HasIndex(e => new { e.TypeLogique, e.Cible }, "ux_correspondance_type").IsUnique().HasFilter("NOT est_supprime");
    }
}
