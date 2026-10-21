namespace Gprojet.Backend.Entities.Organisation;

/// <summary>Table org.competence.</summary>
public class Competence : EntiteAuditable
{
    public string Code { get; set; } = null!;

    public string Libelle { get; set; } = null!;

    public string? Categorie { get; set; }

    public ICollection<AgentCompetence> AgentCompetences { get; } = [];
}
