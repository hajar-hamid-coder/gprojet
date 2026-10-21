// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class IndexAttributConfiguration : ConfigurationEntiteAuditable<IndexAttribut>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<IndexAttribut> builder)
    {
        builder.ToTable("index_attribut", "modelisation");

        builder.Property(e => e.Ordre).HasDefaultValueSql("1");

        builder.HasOne(e => e.Index).WithMany(p => p.IndexAttributs).HasForeignKey(e => e.IndexId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Attribut).WithMany(p => p.IndexAttributs).HasForeignKey(e => e.AttributId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.IndexId, e.AttributId }, "ux_index_attribut").IsUnique().HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.AttributId, "ix_index_attribut_attr");
    }
}
