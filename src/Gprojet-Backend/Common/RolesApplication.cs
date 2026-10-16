namespace Gprojet.Backend.Common;

/// <summary>Rôles déduits de l'organigramme (pas de table de rôles). Les rôles fins (chef de projet, etc.) viendront avec les modules concernés.</summary>
public static class RolesApplication
{
    /// <summary>Agent désigné responsable (<c>responsable_id</c>) d'au moins une entité organisationnelle active.</summary>
    public const string ResponsableEntite = "RESPONSABLE_ENTITE";

    /// <summary>Agent bénéficiaire d'une délégation dont la période couvre aujourd'hui.</summary>
    public const string Delegataire = "DELEGATAIRE";
}
