namespace Gprojet.Front.Services;

public enum TypeNotification { Succes, Erreur, Info }

public sealed record Notification(Guid Id, TypeNotification Type, string Message);

/// <summary>File de messages du snackbar ; disparition automatique après <see cref="Delai"/> (Task.Delay annulable).</summary>
public sealed class NotificationUiService : IDisposable
{
    private readonly List<Notification> _messages = [];
    private readonly Dictionary<Guid, CancellationTokenSource> _minuteries = [];

    public TimeSpan Delai { get; set; } = TimeSpan.FromSeconds(6);
    public IReadOnlyList<Notification> Messages => _messages;
    public event Action? Changed;

    public void Succes(string message) => Ajouter(TypeNotification.Succes, message);
    public void Erreur(string message) => Ajouter(TypeNotification.Erreur, message);
    public void Info(string message) => Ajouter(TypeNotification.Info, message);

    public void Fermer(Guid id)
    {
        if (_minuteries.Remove(id, out var cts)) { cts.Cancel(); cts.Dispose(); }
        if (_messages.RemoveAll(m => m.Id == id) > 0) Changed?.Invoke();
    }

    private void Ajouter(TypeNotification type, string message)
    {
        var n = new Notification(Guid.NewGuid(), type, message);
        _messages.Add(n);
        var cts = new CancellationTokenSource();
        _minuteries[n.Id] = cts;
        Changed?.Invoke();
        _ = DisparaitreAsync(n.Id, cts.Token);
    }

    private async Task DisparaitreAsync(Guid id, CancellationToken jeton)
    {
        try { await Task.Delay(Delai, jeton); }
        catch (OperationCanceledException) { return; }
        Fermer(id);
    }

    public void Dispose()
    {
        foreach (var cts in _minuteries.Values) { cts.Cancel(); cts.Dispose(); }
        _minuteries.Clear();
    }
}
