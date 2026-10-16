using Gprojet.Backend.Entities.Organisation;

namespace Gprojet.Backend.Entities.Commun;

/// <summary>Table commun.commentaire.</summary>
public class Commentaire : EntiteAuditable
{
    public string TypeCible { get; set; } = null!;

    public Guid CibleId { get; set; }

    public Guid? ParentId { get; set; }

    public Guid AuteurId { get; set; }

    public string Contenu { get; set; } = null!;

    public bool EstModifie { get; set; }

    public Commentaire? Parent { get; set; }

    public Agent Auteur { get; set; } = null!;

    public ICollection<Commentaire> Enfants { get; } = [];
}
