// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class CleEtrangereConfiguration : ConfigurationEntiteAuditable<CleEtrangere>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<CleEtrangere> builder)
    {
        builder.ToTable("cle_etrangere", "modelisation", t =>
        {
            t.HasCheckConstraint("cle_etrangere_on_delete_check", "on_delete IN ('CASCADE','RESTRICT','SET_NULL')");
        });

        builder.Property(e => e.Nom).HasMaxLength(63);
        builder.Property(e => e.OnDelete).HasMaxLength(10).HasDefaultValueSql("'RESTRICT'");

        builder.HasOne(e => e.Table).WithMany().HasForeignKey(e => e.TableId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Colonne).WithMany().HasForeignKey(e => e.ColonneId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.TableReferencee).WithMany().HasForeignKey(e => e.TableReferenceeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.ColonneReferencee).WithMany().HasForeignKey(e => e.ColonneReferenceeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.PatteSource).WithMany().HasForeignKey(e => e.PatteSourceId).OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.TableId, "ix_cle_etrangere_table");
        builder.HasIndex(e => e.ColonneId, "ix_cle_etrangere_colonne");
        builder.HasIndex(e => e.TableReferenceeId, "ix_cle_etrangere_table_ref");
        builder.HasIndex(e => e.ColonneReferenceeId, "ix_cle_etrangere_col_ref");
        builder.HasIndex(e => e.PatteSourceId, "ix_cle_etrangere_patte");
    }
}
