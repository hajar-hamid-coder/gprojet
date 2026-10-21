using System.Diagnostics.CodeAnalysis;

namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.module.</summary>
[SuppressMessage("Naming", "CA1716", Justification = "Nom de la table modelisation.module ; pas de consommateur VB.")]
public class Module : EntiteAuditable
{
    public Guid ModeleId { get; set; }

    public string Code { get; set; } = null!;

    public string Nom { get; set; } = null!;

    public string SchemaBd { get; set; } = null!;

    public string Namespace { get; set; } = null!;

    public short Ordre { get; set; } = 1;

    public ModeleDomaine Modele { get; set; } = null!;

    public ICollection<Enumeration> Enumerations { get; } = [];

    public ICollection<Entite> Entites { get; } = [];
}
