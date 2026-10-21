using Gprojet.Front.Api;
using Gprojet.Front.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Gprojet.Front.Services;

/// <summary>Traduit une exception d'appel API en message utilisateur (snackbar) et la journalise sans donnée sensible.</summary>
public sealed partial class GestionnaireErreursUi(NotificationUiService notifications, ILogger<GestionnaireErreursUi> journal, IStringLocalizer<Textes> t)
{
    public void Gerer(Exception e)
    {
        // Journal : statut et type seulement, jamais le détail ni le message serveur.
        if (e is ApiException api)
        {
            JournaliserApi(api.Statut, api.Type);
        }
        else
        {
            JournaliserInattendue(e);
        }

        notifications.Erreur(Message(e));
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Erreur API {Statut} ({Type})")]
    private partial void JournaliserApi(int statut, string? type);

    [LoggerMessage(Level = LogLevel.Error, Message = "Erreur inattendue")]
    private partial void JournaliserInattendue(Exception e);

    public string Message(Exception e) => e switch
    {
        ApiException { Statut: 400 } a => a.Erreurs.Count > 0
            ? string.Join(" ", a.Erreurs.Values.SelectMany(v => v).Distinct())
            : t["ErrDonnees"],
        ApiException { Statut: 401 } => t["Err401"],
        ApiException { Statut: 403 } => t["Err403"],
        ApiException { Statut: 404 } => t["Err404"],
        ApiException { Statut: 409 } => t["Err409"],
        ApiException { Statut: 422 } a => a.Detail ?? a.Titre ?? t["Err422"],
        _ => t["ErrGenerique"]
    };
}
