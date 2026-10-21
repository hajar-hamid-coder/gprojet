namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.methode.</summary>
public class Methode : EntiteAuditable
{
    public Guid EntiteId { get; set; }

    public string Nom { get; set; } = null!;

    public string TypeRetour { get; set; } = "void";

    public Visibilite Visibilite { get; set; } = Visibilite.Public;

    public CoucheCible CoucheCible { get; set; } = CoucheCible.Service;

    public bool EstAsync { get; set; } = true;

    public bool EstStatique { get; set; }

    public string? Description { get; set; }

    public short Ordre { get; set; } = 1;

    public Entite Entite { get; set; } = null!;

    public ICollection<ParametreMethode> ParametreMethodes { get; } = [];
}
