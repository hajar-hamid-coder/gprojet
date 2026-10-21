// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class ModuleConfiguration : ConfigurationEntiteAuditable<Module>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Module> builder)
    {
        builder.ToTable("module", "modelisation", t =>
        {
            t.HasCheckConstraint("module_schema_bd_check", "schema_bd ~ '^[a-z][a-z0-9_]*$'");
        });

        builder.Property(e => e.Code).HasMaxLength(50);
        builder.Property(e => e.Nom).HasMaxLength(150);
        builder.Property(e => e.SchemaBd).HasMaxLength(63);
        builder.Property(e => e.Namespace).HasMaxLength(100);
        builder.Property(e => e.Ordre).HasDefaultValueSql("1");

        builder.HasOne(e => e.Modele).WithMany(p => p.Modules).HasForeignKey(e => e.ModeleId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.ModeleId, e.Code }, "ux_module_code").IsUnique().HasFilter("NOT est_supprime");
    }
}
