namespace Gprojet.Front.Services;

/// <summary>Compteur d'opérations en cours : <c>using var _ = chargement.Demarrer();</c>.</summary>
public sealed class ChargementService
{
    private int _compteur;

    public bool EnCours => Volatile.Read(ref _compteur) > 0;
    public event Action? Changed;

    public IDisposable Demarrer()
    {
        Interlocked.Increment(ref _compteur);
        Changed?.Invoke();
        return new Fin(this);
    }

    private sealed class Fin(ChargementService service) : IDisposable
    {
        private int _termine;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _termine, 1) == 1)
            {
                return;
            }

            Interlocked.Decrement(ref service._compteur);
            service.Changed?.Invoke();
        }
    }
}
