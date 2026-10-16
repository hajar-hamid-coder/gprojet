// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Planification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Planification;

public sealed class LienTicketConfiguration : ConfigurationEntiteAuditable<LienTicket>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<LienTicket> builder)
    {
        builder.ToTable("lien_ticket", "planif", t =>
        {
            t.HasCheckConstraint("lien_ticket_type_lien_check", "type_lien IN ('BLOQUE','DUPLIQUE','LIE_A','CAUSE')");
            t.HasCheckConstraint("ck_lien_ticket_different", "source_id <> cible_id");
        });

        builder.Property(e => e.TypeLien).HasMaxLength(15);

        builder.HasOne(e => e.Source).WithMany().HasForeignKey(e => e.SourceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Cible).WithMany().HasForeignKey(e => e.CibleId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.SourceId, e.CibleId, e.TypeLien }, "ux_lien_ticket").IsUnique().HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.CibleId, "ix_lien_ticket_cible");
    }
}
