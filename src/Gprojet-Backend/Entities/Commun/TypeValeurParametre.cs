using System.Diagnostics.CodeAnalysis;

namespace Gprojet.Backend.Entities.Commun;

/// <summary>CHECK IN de commun.parametre.type_valeur.</summary>
public enum TypeValeurParametre
{
    Texte,
    Entier,
    [SuppressMessage("Naming", "CA1720", Justification = "Valeur métier DECIMAL du CHECK SQL.")]
    Decimal,
    Booleen,
    Json,
}
