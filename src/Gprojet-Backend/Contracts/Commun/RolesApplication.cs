namespace Gprojet.Backend.Contracts.Commun;

/// <summary>
/// Rôles et noms de politiques : source unique partagée par le serveur (claims) et le front (menu, AuthorizeView).
/// Rôles déduits de l'organigramme (pas de table de rôles) ; les rôles fins viendront avec les modules concernés.
/// </summary>
public static class RolesApplication
{
    /// <summary>Agent désigné responsable (<c>responsable_id</c>) d'au moins une entité organisationnelle active.</summary>
    public const string ResponsableEntite = "RESPONSABLE_ENTITE";

    /// <summary>Agent bénéficiaire d'une délégation dont la période couvre aujourd'hui.</summary>
    public const string Delegataire = "DELEGATAIRE";

    /// <summary>Responsable d'une entité de niveau DIRECTION. Pas encore déduit côté serveur : prévu en P4 (H-027).</summary>
    public const string Directeur = "DIRECTEUR";

    /// <summary>Politique : vue consolidée de la direction.</summary>
    public const string PolitiquePortefeuilleDirection = "PortefeuilleDirection";

    /// <summary>Politique : administration de l'organisation (les chefs).</summary>
    public const string PolitiqueAdministration = "Administration";
}
