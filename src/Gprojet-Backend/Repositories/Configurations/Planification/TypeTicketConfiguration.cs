// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Planification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Planification;

public sealed class TypeTicketConfiguration : ConfigurationEntiteAuditable<TypeTicket>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<TypeTicket> builder)
    {
        builder.ToTable("type_ticket", "planif");

        builder.Property(e => e.Code).HasMaxLength(30);
        builder.Property(e => e.Libelle).HasMaxLength(100);
        builder.Property(e => e.Icone).HasMaxLength(50);
        builder.Property(e => e.Actif).HasDefaultValueSql("true");

        builder.HasOne(e => e.Workflow).WithMany().HasForeignKey(e => e.WorkflowId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.Code, "ux_type_ticket_code").IsUnique().HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.WorkflowId, "ix_type_ticket_workflow");
    }
}
