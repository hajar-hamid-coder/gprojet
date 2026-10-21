// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Planification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Planification;

public sealed class BaselineTacheConfiguration : ConfigurationEntiteAuditable<BaselineTache>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<BaselineTache> builder)
    {
        builder.ToTable("baseline_tache", "planif");

        builder.Property(e => e.ChargeH).HasPrecision(8, 2);

        builder.HasOne(e => e.Baseline).WithMany(p => p.BaselineTaches).HasForeignKey(e => e.BaselineId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Tache).WithMany(p => p.BaselineTaches).HasForeignKey(e => e.TacheId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.BaselineId, e.TacheId }, "ux_baseline_tache").IsUnique().HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.TacheId, "ix_baseline_tache_tache");
    }
}
