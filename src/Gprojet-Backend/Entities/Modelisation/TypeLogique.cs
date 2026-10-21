using System.Diagnostics.CodeAnalysis;

namespace Gprojet.Backend.Entities.Modelisation;

/// <summary>CHECK IN de modelisation.attribut.type_logique.</summary>
public enum TypeLogique
{
    Texte,
    TexteLong,
    Entier,
    [SuppressMessage("Naming", "CA1720", Justification = "Valeur métier DECIMAL du CHECK SQL.")]
    Decimal,
    Booleen,
    Date,
    DateHeure,
    Uuid,
    Enum,
    Json,
    Fichier,
}
