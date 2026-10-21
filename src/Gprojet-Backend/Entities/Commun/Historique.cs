namespace Gprojet.Backend.Entities.Commun;

/// <summary>Table commun.historique. Journal d'audit en ajout seul (id bigint, sans colonnes d'audit ni suppression logique).</summary>
public class Historique
{
    public long Id { get; set; }

    public string TypeCible { get; set; } = null!;

    public Guid CibleId { get; set; }

    public ActionHistorique Action { get; set; }

    public string? Champ { get; set; }

    public string? AncienneValeur { get; set; }

    public string? NouvelleValeur { get; set; }

    public Guid? AgentId { get; set; }

    public DateTimeOffset DateAction { get; set; }

    public System.Net.IPAddress? AdresseIp { get; set; }

    public Guid? CorrelationId { get; set; }

    public string? EmpreintePrecedente { get; set; }

    public string Empreinte { get; set; } = null!;
}
