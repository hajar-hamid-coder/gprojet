// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Planification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Planification;

public sealed class DerogationOrdreConfiguration : ConfigurationEntiteAuditable<DerogationOrdre>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<DerogationOrdre> builder)
    {
        builder.ToTable("derogation_ordre", "planif");

        builder.Property(e => e.DateDerogation).HasDefaultValueSql("now()");

        builder.HasOne(e => e.Tache).WithMany().HasForeignKey(e => e.TacheId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Predecesseur).WithMany().HasForeignKey(e => e.PredecesseurId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Agent).WithMany().HasForeignKey(e => e.AgentId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.TacheId, "ix_derogation_tache");
        builder.HasIndex(e => e.PredecesseurId, "ix_derogation_predecesseur");
        builder.HasIndex(e => e.AgentId, "ix_derogation_agent");
    }
}
