// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Planification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Planification;

public sealed class TacheConfiguration : ConfigurationEntiteAuditable<Tache>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<Tache> builder)
    {
        builder.ToTable("tache", "planif", t =>
        {
            t.HasCheckConstraint("tache_ordre_check", "ordre > 0");
            t.HasCheckConstraint("tache_mode_execution_check", "mode_execution IN ('SEQUENTIELLE','PARALLELE')");
            t.HasCheckConstraint("tache_priorite_check", "priorite IN ('BASSE','NORMALE','HAUTE','CRITIQUE')");
            t.HasCheckConstraint("tache_avancement_pct_check", "avancement_pct BETWEEN 0 AND 100");
            t.HasCheckConstraint("ck_tache_charges", "(charge_optimiste_h IS NULL OR charge_estimee_h IS NULL OR charge_optimiste_h <= charge_estimee_h) AND (charge_pessimiste_h IS NULL OR charge_estimee_h IS NULL OR charge_pessimiste_h >= charge_estimee_h)");
            t.HasCheckConstraint("ck_tache_recurrence", "NOT est_recurrente OR regle_recurrence IS NOT NULL");
            t.HasCheckConstraint("ck_tache_dates", "date_fin_prevue IS NULL OR date_debut_prevue IS NULL OR date_fin_prevue >= date_debut_prevue");
        });

        builder.Property(e => e.CodeWbs).HasMaxLength(20);
        builder.Property(e => e.Titre).HasMaxLength(250);
        builder.Property(e => e.ModeExecution).HasMaxLength(15).HasDefaultValueSql("'SEQUENTIELLE'");
        builder.Property(e => e.Priorite).HasMaxLength(20).HasDefaultValueSql("'NORMALE'");
        builder.Property(e => e.ChargeEstimeeH).HasPrecision(8, 2);
        builder.Property(e => e.ChargeOptimisteH).HasPrecision(8, 2);
        builder.Property(e => e.ChargePessimisteH).HasPrecision(8, 2);
        builder.Property(e => e.ResteAFaireH).HasPrecision(8, 2);
        builder.Property(e => e.AvancementPct).HasPrecision(5, 2).HasDefaultValueSql("0");
        builder.Property(e => e.MargeTotaleJ).HasPrecision(6, 2);
        builder.Property(e => e.MargeLibreJ).HasPrecision(6, 2);
        builder.Property(e => e.EstCritique).HasDefaultValueSql("false");
        builder.Property(e => e.EstRecurrente).HasDefaultValueSql("false");
        builder.Property(e => e.RegleRecurrence).HasMaxLength(100);

        builder.HasOne(e => e.Phase).WithMany(p => p.Taches).HasForeignKey(e => e.PhaseId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Projet).WithMany(p => p.Taches).HasForeignKey(e => e.ProjetId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Statut).WithMany().HasForeignKey(e => e.StatutId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Responsable).WithMany().HasForeignKey(e => e.ResponsableId).OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => new { e.PhaseId, e.Ordre }, "ix_tache_phase_ordre").HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.ProjetId, "ix_tache_projet");
        builder.HasIndex(e => e.ResponsableId, "ix_tache_responsable").HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.StatutId, "ix_tache_statut");
    }
}
