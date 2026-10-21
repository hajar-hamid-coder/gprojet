// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Modelisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Modelisation;

public sealed class ModeleDomaineConfiguration : ConfigurationEntiteAuditable<ModeleDomaine>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<ModeleDomaine> builder)
    {
        builder.ToTable("modele_domaine", "modelisation", t =>
        {
            t.HasCheckConstraint("modele_domaine_version_check", "version > 0");
            t.HasCheckConstraint("modele_domaine_statut_check", "statut IN ('BROUILLON','EN_VALIDATION','VALIDE','FIGE')");
            t.HasCheckConstraint("modele_domaine_sgbd_cible_check", "sgbd_cible IN ('POSTGRESQL')");
        });

        builder.Property(e => e.Nom).HasMaxLength(150);
        builder.Property(e => e.Version).HasDefaultValueSql("1");
        builder.Property(e => e.Statut).HasMaxLength(15).HasDefaultValueSql("'BROUILLON'");
        builder.Property(e => e.NamespaceRacine).HasMaxLength(100);
        builder.Property(e => e.SgbdCible).HasMaxLength(20).HasDefaultValueSql("'POSTGRESQL'");

        builder.HasOne(e => e.Projet).WithMany(p => p.ModeleDomaines).HasForeignKey(e => e.ProjetId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.ProjetId, e.Version }, "ux_modele_projet_version").IsUnique().HasFilter("NOT est_supprime");
    }
}
