// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class EnumerationConfiguration : ConfigurationEntiteAuditable<Enumeration>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Enumeration> builder)
    {
        builder.ToTable("enumeration", "modelisation", t =>
        {
            t.HasCheckConstraint("enumeration_stockage_check", "stockage IN ('ENTIER','TEXTE')");
        });

        builder.Property(e => e.Nom).HasMaxLength(100);
        builder.Property(e => e.NomTechnique).HasMaxLength(63);
        builder.Property(e => e.Stockage).HasMaxLength(10).HasDefaultValueSql("'TEXTE'");

        builder.HasOne(e => e.Module).WithMany(p => p.Enumerations).HasForeignKey(e => e.ModuleId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.ModuleId, e.NomTechnique }, "ux_enumeration_nom").IsUnique().HasFilter("NOT est_supprime");
    }
}
