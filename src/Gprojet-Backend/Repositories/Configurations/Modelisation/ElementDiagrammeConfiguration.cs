// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class ElementDiagrammeConfiguration : ConfigurationEntiteAuditable<ElementDiagramme>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<ElementDiagramme> builder)
    {
        builder.ToTable("element_diagramme", "modelisation", t =>
        {
            t.HasCheckConstraint("element_diagramme_type_element_check", "type_element IN ('ENTITE','ASSOCIATION','ENUMERATION','TABLE','NOTE')");
        });

        builder.Property(e => e.TypeElement).HasMaxLength(20);
        builder.Property(e => e.X).HasPrecision(10, 2).HasDefaultValueSql("0");
        builder.Property(e => e.Y).HasPrecision(10, 2).HasDefaultValueSql("0");
        builder.Property(e => e.Largeur).HasPrecision(10, 2);
        builder.Property(e => e.Hauteur).HasPrecision(10, 2);
        builder.Property(e => e.Style).HasColumnType("jsonb");

        builder.HasOne(e => e.Diagramme).WithMany(p => p.ElementDiagrammes).HasForeignKey(e => e.DiagrammeId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.DiagrammeId, e.TypeElement, e.ElementId }, "ux_element_diagramme").IsUnique().HasFilter("NOT est_supprime");
    }
}
