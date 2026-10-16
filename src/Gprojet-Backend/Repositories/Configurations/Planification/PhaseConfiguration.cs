// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Planification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Planification;

public sealed class PhaseConfiguration : ConfigurationEntiteAuditable<Phase>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Phase> builder)
    {
        builder.ToTable("phase", "planif", t =>
        {
            t.HasCheckConstraint("phase_ordre_check", "ordre > 0");
            t.HasCheckConstraint("phase_statut_check", "statut IN ('A_VENIR','EN_COURS','EN_VALIDATION','VALIDEE','GELEE')");
            t.HasCheckConstraint("phase_mode_enchainement_check", "mode_enchainement IN ('SEQUENTIEL','CHEVAUCHEMENT','PARALLELE')");
            t.HasCheckConstraint("phase_pct_min_chevauchement_check", "pct_min_chevauchement BETWEEN 0 AND 100");
            t.HasCheckConstraint("phase_avancement_pct_check", "avancement_pct BETWEEN 0 AND 100");
            t.HasCheckConstraint("ck_phase_chevauchement", "mode_enchainement <> 'CHEVAUCHEMENT' OR pct_min_chevauchement IS NOT NULL");
            t.HasCheckConstraint("ck_phase_dates", "date_fin_prevue IS NULL OR date_debut_prevue IS NULL OR date_fin_prevue >= date_debut_prevue");
        });

        builder.Property(e => e.Code).HasMaxLength(30);
        builder.Property(e => e.Nom).HasMaxLength(150);
        builder.Property(e => e.Statut).HasMaxLength(20).HasDefaultValueSql("'A_VENIR'");
        builder.Property(e => e.ModeEnchainement).HasMaxLength(20).HasDefaultValueSql("'SEQUENTIEL'");
        builder.Property(e => e.PctMinChevauchement).HasPrecision(5, 2);
        builder.Property(e => e.ChargeEstimeeH).HasPrecision(10, 2).HasDefaultValueSql("0");
        builder.Property(e => e.AvancementPct).HasPrecision(5, 2).HasDefaultValueSql("0");

        builder.HasOne(e => e.Projet).WithMany(p => p.Phases).HasForeignKey(e => e.ProjetId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.ProjetId, e.Ordre }, "ix_phase_projet_ordre").HasFilter("NOT est_supprime");
    }
}
