// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Temps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Temps;

public sealed class FeuilleTempsConfiguration : ConfigurationEntiteAuditable<FeuilleTemps>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<FeuilleTemps> builder)
    {
        builder.ToTable("feuille_temps", "temps", t =>
        {
            t.HasCheckConstraint("feuille_temps_semaine_debut_check", "extract(isodow FROM semaine_debut) = 1");
            t.HasCheckConstraint("feuille_temps_statut_check", "statut IN ('BROUILLON','SOUMISE','VALIDEE','REJETEE','VERROUILLEE')");
            t.HasCheckConstraint("ck_feuille_rejet", "statut <> 'REJETEE' OR motif_rejet IS NOT NULL");
        });

        builder.Property(e => e.Statut).HasMaxLength(15).HasDefaultValueSql("'BROUILLON'");

        builder.HasOne(e => e.Agent).WithMany().HasForeignKey(e => e.AgentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Valideur).WithMany().HasForeignKey(e => e.ValideurId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.AgentId, e.SemaineDebut }, "ux_feuille_agent_semaine").IsUnique().HasFilter("NOT est_supprime");
        builder.HasIndex(e => new { e.ValideurId, e.Statut }, "ix_feuille_valideur_statut");
    }
}
