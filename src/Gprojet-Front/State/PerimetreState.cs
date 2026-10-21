namespace Gprojet.Front.State;

/// <summary>Périmètre actif (entité, niveau, chemin ltree). Alimenté par le module Organisation (P4) via <see cref="Definir"/>.</summary>
public sealed class PerimetreState
{
    public Guid? EntiteId { get; private set; }
    public string? Niveau { get; private set; }
    public string? Chemin { get; private set; }

    public event Action? Changed;

    public void Definir(Guid? entiteId, string? niveau, string? chemin)
    {
        if (EntiteId == entiteId && Niveau == niveau && Chemin == chemin)
        {
            return;
        }

        (EntiteId, Niveau, Chemin) = (entiteId, niveau, chemin);
        Changed?.Invoke();
    }
}
