// Fichier initialement généré depuis database/01-schema.sql (P1-T05) ; maintenu à la main ensuite.
using Gprojet.Backend.Entities.Organisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gprojet.Backend.Repositories.Configurations.Organisation;

public sealed class AgentCompetenceConfiguration : ConfigurationEntiteAuditable<AgentCompetence>
{
    protected override void ConfigurerEntite(EntityTypeBuilder<AgentCompetence> builder)
    {
        builder.ToTable("agent_competence", "org", t =>
        {
            t.HasCheckConstraint("agent_competence_niveau_check", "niveau BETWEEN 1 AND 5");
        });

        builder.HasOne(e => e.Agent).WithMany(p => p.AgentCompetences).HasForeignKey(e => e.AgentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Competence).WithMany(p => p.AgentCompetences).HasForeignKey(e => e.CompetenceId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.AgentId, e.CompetenceId }, "ux_agent_competence").IsUnique().HasFilter("NOT est_supprime");
        builder.HasIndex(e => e.CompetenceId, "ix_agent_competence_comp");
    }
}
