// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Planification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Planification;

public sealed class StatutWorkflowConfiguration : ConfigurationEntiteAuditable<StatutWorkflow>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<StatutWorkflow> builder)
    {
        builder.ToTable("statut_workflow", "planif", t =>
        {
            t.HasCheckConstraint("statut_workflow_categorie_check", "categorie IN ('A_FAIRE','EN_COURS','TERMINE')");
            t.HasCheckConstraint("statut_workflow_couleur_check", "couleur ~ '^#[0-9A-Fa-f]{6}$'");
        });

        builder.Property(e => e.Code).HasMaxLength(30);
        builder.Property(e => e.Libelle).HasMaxLength(100);
        builder.Property(e => e.Categorie).HasMaxLength(10);
        builder.Property(e => e.Couleur).HasMaxLength(7);
        builder.Property(e => e.EstInitial).HasDefaultValueSql("false");

        builder.HasOne(e => e.Workflow).WithMany(p => p.StatutWorkflows).HasForeignKey(e => e.WorkflowId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.WorkflowId, e.Code }, "ux_statut_workflow_code").IsUnique().HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.WorkflowId, "ux_statut_workflow_initial").IsUnique().HasFilter("est_initial AND NOT est_supprime");
    }
}
