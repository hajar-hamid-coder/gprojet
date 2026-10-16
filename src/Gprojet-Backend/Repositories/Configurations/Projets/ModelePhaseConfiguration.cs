// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Projets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Projets;

public sealed class ModelePhaseConfiguration : ConfigurationEntiteAuditable<ModelePhase>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<ModelePhase> builder)
    {
        builder.ToTable("modele_phase", "projet", t =>
        {
            t.HasCheckConstraint("modele_phase_ordre_check", "ordre > 0");
            t.HasCheckConstraint("modele_phase_mode_enchainement_check", "mode_enchainement IN ('SEQUENTIEL','CHEVAUCHEMENT','PARALLELE')");
            t.HasCheckConstraint("modele_phase_pct_min_chevauchement_check", "pct_min_chevauchement BETWEEN 0 AND 100");
        });

        builder.Property(e => e.Nom).HasMaxLength(150);
        builder.Property(e => e.ModeEnchainement).HasMaxLength(20).HasDefaultValueSql("'SEQUENTIEL'");
        builder.Property(e => e.PctMinChevauchement).HasPrecision(5, 2);

        builder.HasOne(e => e.TypeProjet).WithMany(p => p.ModelePhases).HasForeignKey(e => e.TypeProjetId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.TypeProjetId, e.Ordre }, "ux_modele_phase_ordre").IsUnique().HasFilter("NOT est_supprime");
    }
}
