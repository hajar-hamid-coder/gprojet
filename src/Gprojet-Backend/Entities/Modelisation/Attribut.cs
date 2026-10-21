namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.attribut.</summary>
public class Attribut : EntiteAuditable
{
    public Guid? EntiteId { get; set; }

    public Guid? AssociationId { get; set; }

    public Guid? EnumerationId { get; set; }

    public string Nom { get; set; } = null!;

    public string NomTechnique { get; set; } = null!;

    public string? LibelleUi { get; set; }

    public TypeLogique TypeLogique { get; set; }

    public int? Longueur { get; set; }

    public short? PrecisionNum { get; set; }

    public short? Echelle { get; set; }

    public bool EstIdentifiant { get; set; }

    public bool EstObligatoire { get; set; }

    public bool EstUnique { get; set; }

    public bool EstCalcule { get; set; }

    public string? ValeurDefaut { get; set; }

    public Visibilite Visibilite { get; set; } = Visibilite.Public;

    public string? RegexValidation { get; set; }

    public string? ValeurMin { get; set; }

    public string? ValeurMax { get; set; }

    public ControleUi? ControleUi { get; set; }

    public bool AfficherDansListe { get; set; } = true;

    public bool EstFiltrable { get; set; }

    public bool EstTriable { get; set; } = true;

    public short Ordre { get; set; } = 1;

    public Entite? Entite { get; set; }

    public Association? Association { get; set; }

    public Enumeration? Enumeration { get; set; }

    public ICollection<IndexAttribut> IndexAttributs { get; } = [];
}
