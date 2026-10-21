// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class EntiteInterfaceConfiguration : ConfigurationEntiteAuditable<EntiteInterface>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<EntiteInterface> builder)
    {
        builder.ToTable("entite_interface", "modelisation", t =>
        {
            t.HasCheckConstraint("ck_entite_interface", "entite_id <> interface_id");
        });

        builder.HasOne(e => e.Entite).WithMany().HasForeignKey(e => e.EntiteId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Interface).WithMany().HasForeignKey(e => e.InterfaceId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.EntiteId, e.InterfaceId }, "ux_entite_interface").IsUnique().HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.InterfaceId, "ix_entite_interface_itf");
    }
}
