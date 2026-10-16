// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class MethodeConfiguration : ConfigurationEntiteAuditable<Methode>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Methode> builder)
    {
        builder.ToTable("methode", "modelisation", t =>
        {
            t.HasCheckConstraint("methode_visibilite_check", "visibilite IN ('PUBLIC','PROTEGE','PRIVE')");
            t.HasCheckConstraint("methode_couche_cible_check", "couche_cible IN ('ENTITE','SERVICE','ORCHESTRATION')");
        });

        builder.Property(e => e.Nom).HasMaxLength(100);
        builder.Property(e => e.TypeRetour).HasMaxLength(150).HasDefaultValueSql("'void'");
        builder.Property(e => e.Visibilite).HasMaxLength(10).HasDefaultValueSql("'PUBLIC'");
        builder.Property(e => e.CoucheCible).HasMaxLength(15).HasDefaultValueSql("'SERVICE'");
        builder.Property(e => e.EstAsync).HasDefaultValueSql("true");
        builder.Property(e => e.EstStatique).HasDefaultValueSql("false");
        builder.Property(e => e.Ordre).HasDefaultValueSql("1");

        builder.HasOne(e => e.Entite).WithMany(p => p.Methodes).HasForeignKey(e => e.EntiteId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.EntiteId, "ix_methode_entite");
    }
}
