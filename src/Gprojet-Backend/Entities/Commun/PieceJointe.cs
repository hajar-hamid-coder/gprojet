using Gprojet.Backend.Entities.Organisation;

namespace Gprojet.Backend.Entities.Commun;

/// <summary>Table commun.piece_jointe.</summary>
public class PieceJointe : EntiteAuditable
{
    public string TypeCible { get; set; } = null!;

    public Guid CibleId { get; set; }

    public string NomFichier { get; set; } = null!;

    public string TypeMime { get; set; } = null!;

    public long TailleOctets { get; set; }

    public string CheminStockage { get; set; } = null!;

    public string EmpreinteSha256 { get; set; } = null!;

    public Guid TeleverseParId { get; set; }

    public Agent TeleversePar { get; set; } = null!;
}
