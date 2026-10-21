// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class AssociationConfiguration : ConfigurationEntiteAuditable<Association>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Association> builder)
    {
        builder.ToTable("association", "modelisation", t =>
        {
            t.HasCheckConstraint("association_nature_check", "nature IN ('ASSOCIATION','COMPOSITION','AGREGATION','POLYMORPHE')");
            t.HasCheckConstraint("association_regle_suppression_check", "regle_suppression IN ('CASCADE','RESTRICT','SET_NULL')");
        });

        builder.Property(e => e.Nom).HasMaxLength(100);
        builder.Property(e => e.NomTechnique).HasMaxLength(63);
        builder.Property(e => e.Nature).HasMaxLength(15).HasDefaultValueSql("'ASSOCIATION'");
        builder.Property(e => e.RegleSuppression).HasMaxLength(10).HasDefaultValueSql("'RESTRICT'");

        builder.HasOne(e => e.Modele).WithMany(p => p.Associations).HasForeignKey(e => e.ModeleId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.ModeleId, "ix_association_modele");
    }
}
