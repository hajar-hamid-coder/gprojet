namespace Gprojet.Backend.Entities.Organisation;

/// <summary>Table org.agent_competence.</summary>
public class AgentCompetence : EntiteAuditable
{
    public Guid AgentId { get; set; }

    public Guid CompetenceId { get; set; }

    public short Niveau { get; set; }

    public Agent Agent { get; set; } = null!;

    public Competence Competence { get; set; } = null!;
}
