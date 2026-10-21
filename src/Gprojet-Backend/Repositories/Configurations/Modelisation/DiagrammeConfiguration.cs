// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class DiagrammeConfiguration : ConfigurationEntiteAuditable<Diagramme>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Diagramme> builder)
    {
        builder.ToTable("diagramme", "modelisation", t =>
        {
            t.HasCheckConstraint("diagramme_type_check", "type IN ('MCD','CLASSES','MLD','SEQUENCE','CAS_UTILISATION','ACTIVITE','ETATS','COMPOSANTS','DEPLOIEMENT','BPMN','C4')");
            t.HasCheckConstraint("diagramme_statut_validation_check", "statut_validation IN ('BROUILLON','EN_VALIDATION','VALIDE','REJETE')");
        });

        builder.Property(e => e.Type).HasMaxLength(20);
        builder.Property(e => e.Nom).HasMaxLength(150);
        builder.Property(e => e.Contenu).HasColumnType("jsonb");
        builder.Property(e => e.StatutValidation).HasMaxLength(15).HasDefaultValueSql("'BROUILLON'");
        builder.Property(e => e.Version).HasDefaultValueSql("1");

        builder.HasOne(e => e.Modele).WithMany(p => p.Diagrammes).HasForeignKey(e => e.ModeleId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.ModeleId, e.Type }, "ix_diagramme_modele");
    }
}
