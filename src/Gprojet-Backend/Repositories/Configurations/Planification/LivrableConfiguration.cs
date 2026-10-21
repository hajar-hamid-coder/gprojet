// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Planification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Planification;

public sealed class LivrableConfiguration : ConfigurationEntiteAuditable<Livrable>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Livrable> builder)
    {
        builder.ToTable("livrable", "planif", t =>
        {
            t.HasCheckConstraint("livrable_statut_check", "statut IN ('A_PRODUIRE','EN_COURS','LIVRE','VALIDE','REJETE')");
        });

        builder.Property(e => e.Libelle).HasMaxLength(200);
        builder.Property(e => e.Statut).HasMaxLength(20).HasDefaultValueSql("'A_PRODUIRE'");

        builder.HasOne(e => e.Phase).WithMany(p => p.Livrables).HasForeignKey(e => e.PhaseId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.PhaseId, "ix_livrable_phase");
    }
}
