// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Planification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Planification;

public sealed class TransitionConfiguration : ConfigurationEntiteAuditable<Transition>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Transition> builder)
    {
        builder.ToTable("transition", "planif", t =>
        {
            t.HasCheckConstraint("ck_transition_statuts", "statut_depuis_id <> statut_vers_id");
        });

        builder.Property(e => e.Libelle).HasMaxLength(100);
        builder.Property(e => e.RolesAutorises).HasDefaultValueSql("'{}'");

        builder.HasOne(e => e.Workflow).WithMany(p => p.Transitions).HasForeignKey(e => e.WorkflowId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.StatutDepuis).WithMany().HasForeignKey(e => e.StatutDepuisId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.StatutVers).WithMany().HasForeignKey(e => e.StatutVersId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.StatutDepuisId, e.StatutVersId }, "ux_transition").IsUnique().HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.WorkflowId, "ix_transition_workflow");
        builder.HasIndex(e => e.StatutVersId, "ix_transition_vers");
    }
}
