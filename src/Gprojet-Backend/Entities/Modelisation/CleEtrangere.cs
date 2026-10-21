namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>Table modelisation.cle_etrangere.</summary>
public class CleEtrangere : EntiteAuditable
{
    public Guid TableId { get; set; }

    public Guid ColonneId { get; set; }

    public Guid TableReferenceeId { get; set; }

    public Guid ColonneReferenceeId { get; set; }

    public Guid? PatteSourceId { get; set; }

    public string Nom { get; set; } = null!;

    public RegleSuppression OnDelete { get; set; } = RegleSuppression.Restrict;

    public TableBd Table { get; set; } = null!;

    public Colonne Colonne { get; set; } = null!;

    public TableBd TableReferencee { get; set; } = null!;

    public Colonne ColonneReferencee { get; set; } = null!;

    public Patte? PatteSource { get; set; }
}
