// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Projets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Projets;

public sealed class MembreProjetConfiguration : ConfigurationEntiteAuditable<MembreProjet>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<MembreProjet> builder)
    {
        builder.ToTable("membre_projet", "projet", t =>
        {
            t.HasCheckConstraint("membre_projet_role_projet_check", "role_projet IN ('CHEF_PROJET','MEMBRE','OBSERVATEUR')");
            t.HasCheckConstraint("membre_projet_quota_pct_check", "quota_pct BETWEEN 0 AND 100");
            t.HasCheckConstraint("ck_membre_dates", "date_fin IS NULL OR date_debut IS NULL OR date_fin >= date_debut");
        });

        builder.Property(e => e.RoleProjet).HasMaxLength(20).HasDefaultValueSql("'MEMBRE'");
        builder.Property(e => e.QuotaPct).HasPrecision(5, 2).HasDefaultValueSql("100");

        builder.HasOne(e => e.Projet).WithMany(p => p.MembreProjets).HasForeignKey(e => e.ProjetId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Agent).WithMany().HasForeignKey(e => e.AgentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.AffectePar).WithMany().HasForeignKey(e => e.AffecteParId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.ProjetId, e.AgentId }, "ux_membre_projet_agent").IsUnique().HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.AgentId, "ix_membre_agent");
        builder.HasIndex(e => e.AffecteParId, "ix_membre_affecte_par");
    }
}
