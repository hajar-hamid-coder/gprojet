// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class IndexEntiteConfiguration : ConfigurationEntiteAuditable<IndexEntite>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<IndexEntite> builder)
    {
        builder.ToTable("index_entite", "modelisation");

        builder.Property(e => e.Nom).HasMaxLength(63);
        builder.Property(e => e.EstUnique).HasDefaultValueSql("false");

        builder.HasOne(e => e.Entite).WithMany(p => p.IndexEntites).HasForeignKey(e => e.EntiteId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.EntiteId, "ix_index_entite");
    }
}
