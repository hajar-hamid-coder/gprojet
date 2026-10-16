// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class EntiteConfiguration : ConfigurationEntiteAuditable<Entite>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Entite> builder)
    {
        builder.ToTable("entite", "modelisation", t =>
        {
            t.HasCheckConstraint("entite_nom_technique_check", "nom_technique ~ '^[a-z][a-z0-9_]*$'");
            t.HasCheckConstraint("entite_stereotype_check", "stereotype IN ('ENTITE','REFERENTIEL','VALEUR','INTERFACE')");
            t.HasCheckConstraint("entite_strategie_heritage_check", "strategie_heritage IN ('TPH','TPT','TPC')");
            t.HasCheckConstraint("ck_entite_heritage", "entite_parent_id IS NULL OR entite_parent_id <> id");
        });

        builder.Property(e => e.Nom).HasMaxLength(100);
        builder.Property(e => e.NomTechnique).HasMaxLength(63);
        builder.Property(e => e.NomPluriel).HasMaxLength(100);
        builder.Property(e => e.LibelleUi).HasMaxLength(150);
        builder.Property(e => e.Stereotype).HasMaxLength(15).HasDefaultValueSql("'ENTITE'");
        builder.Property(e => e.EstAbstraite).HasDefaultValueSql("false");
        builder.Property(e => e.StrategieHeritage).HasMaxLength(5);
        builder.Property(e => e.EstRacineAgregat).HasDefaultValueSql("true");
        builder.Property(e => e.AvecAudit).HasDefaultValueSql("true");
        builder.Property(e => e.AvecSuppressionLogique).HasDefaultValueSql("true");
        builder.Property(e => e.AvecConcurrence).HasDefaultValueSql("true");
        builder.Property(e => e.GenererApi).HasDefaultValueSql("true");
        builder.Property(e => e.GenererPages).HasDefaultValueSql("true");

        builder.HasOne(e => e.Module).WithMany(p => p.Entites).HasForeignKey(e => e.ModuleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.EntiteParent).WithMany().HasForeignKey(e => e.EntiteParentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.AttributLibelle).WithMany().HasForeignKey(e => e.AttributLibelleId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("fk_entite_attribut_libelle");

        builder.HasIndex(e => new { e.ModuleId, e.NomTechnique }, "ux_entite_nom_technique").IsUnique().HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.EntiteParentId, "ix_entite_parent");
    }
}
