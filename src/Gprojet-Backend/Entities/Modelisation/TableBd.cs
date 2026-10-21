namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.table_bd.</summary>
public class TableBd : EntiteAuditable
{
    public Guid ModeleId { get; set; }

    public Guid? EntiteSourceId { get; set; }

    public Guid? AssociationSourceId { get; set; }

    public string Nom { get; set; } = null!;

    public string SchemaBd { get; set; } = null!;

    public bool EstTableAssociation { get; set; }

    public bool EstSurchargee { get; set; }

    public ModeleDomaine Modele { get; set; } = null!;

    public Entite? EntiteSource { get; set; }

    public Association? AssociationSource { get; set; }

    public ICollection<Colonne> Colonnes { get; } = [];
}
