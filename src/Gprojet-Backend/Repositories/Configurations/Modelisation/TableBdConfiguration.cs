// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class TableBdConfiguration : ConfigurationEntiteAuditable<TableBd>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<TableBd> builder)
    {
        builder.ToTable("table_bd", "modelisation");

        builder.Property(e => e.Nom).HasMaxLength(63);
        builder.Property(e => e.SchemaBd).HasMaxLength(63);
        builder.Property(e => e.EstTableAssociation).HasDefaultValueSql("false");
        builder.Property(e => e.EstSurchargee).HasDefaultValueSql("false");

        builder.HasOne(e => e.Modele).WithMany(p => p.TableBds).HasForeignKey(e => e.ModeleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.EntiteSource).WithMany().HasForeignKey(e => e.EntiteSourceId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(e => e.AssociationSource).WithMany().HasForeignKey(e => e.AssociationSourceId).OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => new { e.ModeleId, e.SchemaBd, e.Nom }, "ux_table_bd").IsUnique().HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.EntiteSourceId, "ix_table_bd_entite");
        builder.HasIndex(e => e.AssociationSourceId, "ix_table_bd_association");
    }
}
