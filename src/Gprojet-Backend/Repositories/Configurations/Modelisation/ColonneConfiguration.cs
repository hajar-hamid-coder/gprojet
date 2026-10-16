// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class ColonneConfiguration : ConfigurationEntiteAuditable<Colonne>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Colonne> builder)
    {
        builder.ToTable("colonne", "modelisation");

        builder.Property(e => e.Nom).HasMaxLength(63);
        builder.Property(e => e.TypePhysique).HasMaxLength(100);
        builder.Property(e => e.EstNullable).HasDefaultValueSql("true");
        builder.Property(e => e.EstClePrimaire).HasDefaultValueSql("false");
        builder.Property(e => e.ValeurDefaut).HasMaxLength(200);
        builder.Property(e => e.Ordre).HasDefaultValueSql("1");
        builder.Property(e => e.EstSurchargee).HasDefaultValueSql("false");

        builder.HasOne(e => e.Table).WithMany(p => p.Colonnes).HasForeignKey(e => e.TableId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.AttributSource).WithMany().HasForeignKey(e => e.AttributSourceId).OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => new { e.TableId, e.Nom }, "ux_colonne").IsUnique().HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.AttributSourceId, "ix_colonne_attribut");
    }
}
