// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Projets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Projets;

public sealed class ProjetConfiguration : ConfigurationEntiteAuditable<Projet>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Projet> builder)
    {
        builder.ToTable("projet", "projet", t =>
        {
            t.HasCheckConstraint("projet_priorite_check", "priorite IN ('BASSE','NORMALE','HAUTE','CRITIQUE')");
            t.HasCheckConstraint("projet_statut_check", "statut IN ('BROUILLON','EN_AFFECTATION','EN_COURS','SUSPENDU','CLOTURE','ARCHIVE')");
            t.HasCheckConstraint("projet_meteo_check", "meteo IN ('VERT','ORANGE','ROUGE')");
            t.HasCheckConstraint("projet_classification_check", "classification IN ('PUBLIC','INTERNE','CONFIDENTIEL','DIFFUSION_RESTREINTE')");
            t.HasCheckConstraint("projet_mode_ordonnancement_check", "mode_ordonnancement IN ('BLOQUANT','SOUPLE')");
            t.HasCheckConstraint("projet_avancement_pct_check", "avancement_pct BETWEEN 0 AND 100");
            t.HasCheckConstraint("ck_projet_dates_prevues", "date_fin_prevue IS NULL OR date_debut_prevue IS NULL OR date_fin_prevue >= date_debut_prevue");
            t.HasCheckConstraint("ck_projet_dates_reelles", "date_fin_reelle IS NULL OR date_debut_reelle IS NULL OR date_fin_reelle >= date_debut_reelle");
        });

        builder.Property(e => e.Code).HasMaxLength(30);
        builder.Property(e => e.Intitule).HasMaxLength(250);
        builder.Property(e => e.Priorite).HasMaxLength(20).HasDefaultValueSql("'NORMALE'");
        builder.Property(e => e.Statut).HasMaxLength(20).HasDefaultValueSql("'BROUILLON'");
        builder.Property(e => e.Meteo).HasMaxLength(10).HasDefaultValueSql("'VERT'");
        builder.Property(e => e.Classification).HasMaxLength(30).HasDefaultValueSql("'INTERNE'");
        builder.Property(e => e.ModeOrdonnancement).HasMaxLength(10).HasDefaultValueSql("'BLOQUANT'");
        builder.Property(e => e.CheminEntite).HasColumnType("ltree");
        builder.Property(e => e.ChargeEstimeeJh).HasPrecision(10, 2).HasDefaultValueSql("0");
        builder.Property(e => e.ChargeConsommeeJh).HasPrecision(10, 2).HasDefaultValueSql("0");
        builder.Property(e => e.AvancementPct).HasPrecision(5, 2).HasDefaultValueSql("0");
        builder.Property(e => e.ProchainNumeroTicket).HasDefaultValueSql("1");

        builder.HasOne(e => e.TypeProjet).WithMany().HasForeignKey(e => e.TypeProjetId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Createur).WithMany().HasForeignKey(e => e.CreateurId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.EntiteCreatrice).WithMany().HasForeignKey(e => e.EntiteCreatriceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.EntitePorteuse).WithMany().HasForeignKey(e => e.EntitePorteuseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.ChefProjet).WithMany().HasForeignKey(e => e.ChefProjetId).OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.Code, "ux_projet_code").IsUnique().HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.CheminEntite, "ix_projet_chemin").HasMethod("gist");
        builder.HasIndex(e => e.Statut, "ix_projet_statut").HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.TypeProjetId, "ix_projet_type");
        builder.HasIndex(e => e.CreateurId, "ix_projet_createur");
        builder.HasIndex(e => e.EntiteCreatriceId, "ix_projet_entite_creatrice");
        builder.HasIndex(e => e.EntitePorteuseId, "ix_projet_entite_porteuse");
        builder.HasIndex(e => e.ChefProjetId, "ix_projet_chef");
    }
}
