// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class AttributConfiguration : ConfigurationEntiteAuditable<Attribut>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Attribut> builder)
    {
        builder.ToTable("attribut", "modelisation", t =>
        {
            t.HasCheckConstraint("attribut_nom_technique_check", "nom_technique ~ '^[a-z][a-z0-9_]*$'");
            t.HasCheckConstraint("attribut_type_logique_check", "type_logique IN ('TEXTE','TEXTE_LONG','ENTIER','DECIMAL','BOOLEEN','DATE','DATE_HEURE','UUID','ENUM','JSON','FICHIER')");
            t.HasCheckConstraint("attribut_longueur_check", "longueur > 0");
            t.HasCheckConstraint("attribut_precision_num_check", "precision_num BETWEEN 1 AND 38");
            t.HasCheckConstraint("attribut_echelle_check", "echelle >= 0");
            t.HasCheckConstraint("attribut_visibilite_check", "visibilite IN ('PUBLIC','PROTEGE','PRIVE')");
            t.HasCheckConstraint("attribut_controle_ui_check", "controle_ui IN ('TEXTE','ZONE','NOMBRE','DATE','LISTE','CASE','FICHIER')");
            t.HasCheckConstraint("ck_attribut_proprietaire", "num_nonnulls(entite_id, association_id) = 1");
            t.HasCheckConstraint("ck_attribut_enum", "(type_logique = 'ENUM') = (enumeration_id IS NOT NULL)");
            t.HasCheckConstraint("ck_attribut_decimal", "type_logique <> 'DECIMAL' OR precision_num IS NOT NULL");
            t.HasCheckConstraint("ck_attribut_echelle", "echelle IS NULL OR precision_num IS NULL OR echelle <= precision_num");
        });

        builder.Property(e => e.Nom).HasMaxLength(100);
        builder.Property(e => e.NomTechnique).HasMaxLength(63);
        builder.Property(e => e.LibelleUi).HasMaxLength(150);
        builder.Property(e => e.TypeLogique).HasMaxLength(15);
        builder.Property(e => e.EstIdentifiant).HasDefaultValueSql("false");
        builder.Property(e => e.EstObligatoire).HasDefaultValueSql("false");
        builder.Property(e => e.EstUnique).HasDefaultValueSql("false");
        builder.Property(e => e.EstCalcule).HasDefaultValueSql("false");
        builder.Property(e => e.ValeurDefaut).HasMaxLength(200);
        builder.Property(e => e.Visibilite).HasMaxLength(10).HasDefaultValueSql("'PUBLIC'");
        builder.Property(e => e.RegexValidation).HasMaxLength(300);
        builder.Property(e => e.ValeurMin).HasMaxLength(50);
        builder.Property(e => e.ValeurMax).HasMaxLength(50);
        builder.Property(e => e.ControleUi).HasMaxLength(10);
        builder.Property(e => e.AfficherDansListe).HasDefaultValueSql("true");
        builder.Property(e => e.EstFiltrable).HasDefaultValueSql("false");
        builder.Property(e => e.EstTriable).HasDefaultValueSql("true");
        builder.Property(e => e.Ordre).HasDefaultValueSql("1");

        builder.HasOne(e => e.Entite).WithMany(p => p.Attributs).HasForeignKey(e => e.EntiteId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Association).WithMany(p => p.Attributs).HasForeignKey(e => e.AssociationId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Enumeration).WithMany().HasForeignKey(e => e.EnumerationId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.EntiteId, e.NomTechnique }, "ux_attribut_entite").IsUnique().HasFilter("entite_id IS NOT NULL AND NOT est_supprime");
        builder.HasIndex(e => new { e.AssociationId, e.NomTechnique }, "ux_attribut_association").IsUnique().HasFilter("association_id IS NOT NULL AND NOT est_supprime");
        builder.HasIndex(e => e.EnumerationId, "ix_attribut_enumeration");
    }
}
