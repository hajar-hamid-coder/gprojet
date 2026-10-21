// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Organisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Organisation;

public sealed class AbsenceConfiguration : ConfigurationEntiteAuditable<Absence>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Absence> builder)
    {
        builder.ToTable("absence", "org", t =>
        {
            t.HasCheckConstraint("absence_type_check", "type IN ('CONGE','MALADIE','FORMATION','MISSION','AUTRE')");
            t.HasCheckConstraint("ck_absence_dates", "date_fin >= date_debut");
        });

        builder.Property(e => e.Type).HasMaxLength(20);
        builder.Property(e => e.DemiJournee).HasDefaultValueSql("false");
        builder.Property(e => e.Commentaire).HasMaxLength(300);

        builder.HasOne(e => e.Agent).WithMany(p => p.Absences).HasForeignKey(e => e.AgentId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.AgentId, e.DateDebut, e.DateFin }, "ix_absence_agent_dates");
    }
}
