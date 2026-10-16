// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class ValeurEnumerationConfiguration : ConfigurationEntiteAuditable<ValeurEnumeration>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<ValeurEnumeration> builder)
    {
        builder.ToTable("valeur_enumeration", "modelisation");

        builder.Property(e => e.Code).HasMaxLength(50);
        builder.Property(e => e.Libelle).HasMaxLength(150);
        builder.Property(e => e.Ordre).HasDefaultValueSql("1");

        builder.HasOne(e => e.Enumeration).WithMany(p => p.ValeurEnumerations).HasForeignKey(e => e.EnumerationId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.EnumerationId, e.Code }, "ux_valeur_enumeration").IsUnique().HasFilter("NOT est_supprime");
    }
}
