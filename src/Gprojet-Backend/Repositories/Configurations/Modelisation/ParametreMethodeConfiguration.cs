// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class ParametreMethodeConfiguration : ConfigurationEntiteAuditable<ParametreMethode>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<ParametreMethode> builder)
    {
        builder.ToTable("parametre_methode", "modelisation");

        builder.Property(e => e.Nom).HasMaxLength(100);
        builder.Property(e => e.Type).HasMaxLength(150);
        builder.Property(e => e.Ordre).HasDefaultValueSql("1");
        builder.Property(e => e.EstOptionnel).HasDefaultValueSql("false");
        builder.Property(e => e.ValeurDefaut).HasMaxLength(200);

        builder.HasOne(e => e.Methode).WithMany(p => p.ParametreMethodes).HasForeignKey(e => e.MethodeId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.MethodeId, e.Nom }, "ux_parametre_methode").IsUnique().HasFilter("NOT est_supprime");
    }
}
