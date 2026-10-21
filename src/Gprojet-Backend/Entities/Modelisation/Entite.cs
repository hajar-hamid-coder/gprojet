namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.entite.</summary>
public class Entite : EntiteAuditable
{
    public Guid ModuleId { get; set; }

    public Guid? EntiteParentId { get; set; }

    public string Nom { get; set; } = null!;

    public string NomTechnique { get; set; } = null!;

    public string NomPluriel { get; set; } = null!;

    public string? LibelleUi { get; set; }

    public string? Description { get; set; }

    public Stereotype Stereotype { get; set; } = Stereotype.Entite;

    public bool EstAbstraite { get; set; }

    public StrategieHeritage? StrategieHeritage { get; set; }

    public bool EstRacineAgregat { get; set; } = true;

    public bool AvecAudit { get; set; } = true;

    public bool AvecSuppressionLogique { get; set; } = true;

    public bool AvecConcurrence { get; set; } = true;

    public bool GenererApi { get; set; } = true;

    public bool GenererPages { get; set; } = true;

    public Guid? AttributLibelleId { get; set; }

    public Module Module { get; set; } = null!;

    public Entite? EntiteParent { get; set; }

    public Attribut? AttributLibelle { get; set; }

    public ICollection<Patte> Pattes { get; } = [];

    public ICollection<Attribut> Attributs { get; } = [];

    public ICollection<IndexEntite> IndexEntites { get; } = [];

    public ICollection<Methode> Methodes { get; } = [];
}
