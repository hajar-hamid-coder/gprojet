// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Planification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Planification;

public sealed class WorkflowConfiguration : ConfigurationEntiteAuditable<Workflow>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Workflow> builder)
    {
        builder.ToTable("workflow", "planif", t =>
        {
            t.HasCheckConstraint("workflow_type_element_check", "type_element IN ('TACHE','TICKET')");
        });

        builder.Property(e => e.Code).HasMaxLength(30);
        builder.Property(e => e.Libelle).HasMaxLength(150);
        builder.Property(e => e.TypeElement).HasMaxLength(10);
        builder.Property(e => e.EstDefaut).HasDefaultValueSql("false");

        builder.HasIndex(e => e.Code, "ux_workflow_code").IsUnique().HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.TypeElement, "ux_workflow_defaut").IsUnique().HasFilter("est_defaut AND NOT est_supprime");
    }
}
