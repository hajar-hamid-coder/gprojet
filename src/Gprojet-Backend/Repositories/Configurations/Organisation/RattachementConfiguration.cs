// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Organisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Organisation;

public sealed class RattachementConfiguration : ConfigurationEntiteAuditable<Rattachement>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Rattachement> builder)
    {
        builder.ToTable("rattachement", "org", t =>
        {
            t.HasCheckConstraint("ck_rattachement_dates", "date_fin IS NULL OR date_fin >= date_debut");
        });

        builder.Property(e => e.Motif).HasMaxLength(200);

        builder.HasOne(e => e.Agent).WithMany(p => p.Rattachements).HasForeignKey(e => e.AgentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Entite).WithMany().HasForeignKey(e => e.EntiteId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.AgentId, "ux_rattachement_en_cours").IsUnique().HasFilter("date_fin IS NULL AND NOT est_supprime");
        builder.HasIndex(e => e.EntiteId, "ix_rattachement_entite");
    }
}
