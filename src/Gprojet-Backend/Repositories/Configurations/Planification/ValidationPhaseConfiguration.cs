// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Planification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Planification;

public sealed class ValidationPhaseConfiguration : ConfigurationEntiteAuditable<ValidationPhase>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<ValidationPhase> builder)
    {
        builder.ToTable("validation_phase", "planif", t =>
        {
            t.HasCheckConstraint("validation_phase_decision_check", "decision IN ('EN_ATTENTE','APPROUVEE','REJETEE')");
            t.HasCheckConstraint("ck_validation_rejet", "decision <> 'REJETEE' OR commentaire IS NOT NULL");
        });

        builder.Property(e => e.Tour).HasDefaultValueSql("1");
        builder.Property(e => e.Decision).HasMaxLength(15).HasDefaultValueSql("'EN_ATTENTE'");

        builder.HasOne(e => e.Phase).WithMany(p => p.ValidationPhases).HasForeignKey(e => e.PhaseId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Valideur).WithMany().HasForeignKey(e => e.ValideurId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.PhaseId, e.Tour, e.OrdreNiveau }, "ux_validation_phase").IsUnique().HasFilter("NOT est_supprime");
        builder.HasIndex(e => new { e.ValideurId, e.Decision }, "ix_validation_valideur");
    }
}
