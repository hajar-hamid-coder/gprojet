using Gprojet.Front.Api;
using Microsoft.Extensions.Logging;

namespace Gprojet.Front.Services;

/// <summary>Traduit une exception d'appel API en message utilisateur (snackbar) et la journalise sans donnée sensible.</summary>
public sealed partial class GestionnaireErreursUi(NotificationUiService notifications, ILogger<GestionnaireErreursUi> journal)
{
    public void Gerer(Exception e)
    {
        // Journal : statut et type seulement, jamais le détail ni le message serveur.
        if (e is ApiException api) JournaliserApi(api.Statut, api.Type);
        else JournaliserInattendue(e);
        notifications.Erreur(Message(e));
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Erreur API {Statut} ({Type})")]
    private partial void JournaliserApi(int statut, string? type);

    [LoggerMessage(Level = LogLevel.Error, Message = "Erreur inattendue")]
    private partial void JournaliserInattendue(Exception e);

    public static string Message(Exception e) => e switch
    {
        ApiException { Statut: 400 } a => a.Erreurs.Count > 0
            ? string.Join(" ", a.Erreurs.Values.SelectMany(v => v).Distinct())
            : "Les données saisies sont invalides.",
        ApiException { Statut: 401 } => "Session expirée, reconnectez-vous.",
        ApiException { Statut: 403 } => "Accès refusé.",
        ApiException { Statut: 404 } => "Élément introuvable.",
        ApiException { Statut: 409 } => "Les données ont été modifiées par un autre utilisateur.",
        ApiException { Statut: 422 } a => a.Detail ?? a.Titre ?? "Opération impossible.",
        _ => "Une erreur est survenue, veuillez réessayer plus tard."
    };
}
