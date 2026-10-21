// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Projets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Projets;

public sealed class AffectationProjetConfiguration : ConfigurationEntiteAuditable<AffectationProjet>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<AffectationProjet> builder)
    {
        builder.ToTable("affectation_projet", "projet", t =>
        {
            t.HasCheckConstraint("affectation_projet_niveau_cible_check", "niveau_cible IN ('DIVISION','SERVICE')");
            t.HasCheckConstraint("affectation_projet_statut_check", "statut IN ('EN_ATTENTE','ACCEPTEE','RENVOYEE','ANNULEE')");
            t.HasCheckConstraint("ck_affectation_entites", "entite_source_id <> entite_cible_id");
            t.HasCheckConstraint("ck_affectation_motif", "statut <> 'RENVOYEE' OR motif_reponse IS NOT NULL");
        });

        builder.Property(e => e.NiveauCible).HasMaxLength(20);
        builder.Property(e => e.Statut).HasMaxLength(20).HasDefaultValueSql("'EN_ATTENTE'");
        builder.Property(e => e.ChargePrevueJh).HasPrecision(10, 2);
        builder.Property(e => e.DateAffectation).HasDefaultValueSql("now()");
        builder.Property(e => e.EstActive).HasDefaultValueSql("true");

        builder.HasOne(e => e.Projet).WithMany(p => p.AffectationProjets).HasForeignKey(e => e.ProjetId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Precedente).WithMany().HasForeignKey(e => e.PrecedenteId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(e => e.EntiteSource).WithMany().HasForeignKey(e => e.EntiteSourceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.EntiteCible).WithMany().HasForeignKey(e => e.EntiteCibleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.AffectePar).WithMany().HasForeignKey(e => e.AffecteParId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.ReponduPar).WithMany().HasForeignKey(e => e.ReponduParId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.ProjetId, "ux_affectation_active").IsUnique().HasFilter("est_active AND NOT est_supprime");
        builder.HasIndex(e => new { e.EntiteCibleId, e.Statut }, "ix_affectation_cible_statut");
        builder.HasIndex(e => e.EntiteSourceId, "ix_affectation_source");
        builder.HasIndex(e => e.PrecedenteId, "ix_affectation_precedente");
        builder.HasIndex(e => e.AffecteParId, "ix_affectation_affecte_par");
        builder.HasIndex(e => e.ReponduParId, "ix_affectation_repondu_par");
    }
}
