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

// Langue : le cookie gp.culture (posé par POST /bff/langue) donne lang/dir de <html> avant le premier rendu ;
// Program.cs lit la même valeur pour régler la culture de .NET.
window.gpLangue = {
    lire: () => {
        const m = document.cookie.match(/(?:^|; )gp\.culture=([^;]*)/);
        const c = m ? decodeURIComponent(m[1]) : '';
        return ['fr-MA', 'en', 'ar-MA'].includes(c) ? c : 'fr-MA';
    }
};
{
    const c = window.gpLangue.lire();
    document.documentElement.lang = c;
    document.documentElement.dir = c.startsWith('ar') ? 'rtl' : 'ltr';
}
