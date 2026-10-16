// Ctrl+K : focus sur la recherche. Lien d'évitement : focus sur le contenu (un href="#…" serait résolu contre <base href="/">).
document.addEventListener('keydown', e => {
    if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 'k') {
        e.preventDefault();
        document.getElementById('recherche-globale')?.focus();
    }
});
document.addEventListener('click', e => {
    if (e.target.closest?.('.skip-link')) {
        e.preventDefault();
        document.getElementById('contenu')?.focus();
    }
}, true);
