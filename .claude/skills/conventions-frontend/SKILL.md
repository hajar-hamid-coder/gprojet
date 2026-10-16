---
name: conventions-frontend
description: Conventions du front Blazor WebAssembly GestionProjet (couches Api/Services/Models/Pages, charte graphique HCP obligatoire sans MudBlazor, ergonomie via ui-ux-pro-max, mouvements de la charte, accessibilité, i18n). À charger avant de créer ou modifier une page, un composant ou une feuille de style.
paths: "src/Gprojet-Front/**, tests/Gprojet-Front.Tests/**, design/**"
---

# Conventions frontend

## Couches (projet unique `src/Gprojet-Front`, `RootNamespace Gprojet.Front`)

Un seul projet front (Blazor WebAssembly), servi par `Gprojet-Backend`. Les couches sont des dossiers. Les DTO viennent de `Contracts/` : fichiers **liés** depuis `Gprojet-Backend/Contracts` (ne jamais les copier ni les redéfinir).

- `Api/` : un client typé par ressource (`ProjetsApiClient`), écrit sur le contrat OpenAPI, retourne les DTO de `Contracts`. Appels relatifs (`/api/...`) : même origine que le backend, pas de CORS.
- `Services/` : logique d'écran (cache, combinaison d'appels, erreurs → `NotificationUiService`). Les pages n'injectent **que** des services.
- `Models/` : modèles de formulaire / vue. Validation avec les validateurs FluentValidation partagés.
- `Pages/<Module>/` : composants routés, minces. `Components/` : composants réutilisables maison (tableau, Kanban, Gantt, arbre d'organisation, dialogue, onglets, fil d'Ariane de drill-down, badge de statut).
- `State/` : utilisateur courant, périmètre, projet actif. `Auth/` : `AuthenticationStateProvider` (BFF), `<AuthorizeView Policy=…>` (rôles fournis par `/bff/user`) pour masquer les actions non permises.

## Charte graphique HCP (prime sur tout le reste)
Lire `design/charte-hcp/README.md` avant toute UI.
1. `wwwroot/css/app.css` = copie fidèle de la charte (seul l'`@import` de police est remplacé par un `@font-face` local). **Ne jamais le modifier.**
2. Tout ajout va dans `wwwroot/css/app.gp.css`, avec les seules variables de la charte (`var(--accent)`, `var(--border-strong)`, `var(--radius)`…). Aucune couleur hexadécimale dans les `.razor`.
3. Réutiliser les classes existantes : layout (`.page`, `.sidebar`, `.topbar`, `.content-area`), listes (`.explorer-table`, `.explorer-header`, `.explorer-row`), cartes (`.card-container`, `.stat-card`), files (`.inbox-item`), formulaires (`.form-group`, `.form-input`), badges (`.status-badge`), états vides (`.empty-state`).
4. **Pas de MudBlazor** ni d'autre bibliothèque de composants Material. Icônes Font Awesome Free et graphiques Chart.js, auto-hébergés (`wwwroot/lib/`). Aucune ressource chargée depuis Internet.
5. **ui-ux-pro-max** : uniquement pour l'ergonomie (hiérarchie, densité, parcours, états vide / chargement / erreur). Si une recommandation contredit la charte, la charte gagne.
6. **Mouvement** : seulement les durées de la charte (0.15s survols, `.animate-in` 0.25s apparitions, 0.6s barres). **motion-design** aide à choisir où les appliquer. Neutraliser sous `@media (prefers-reduced-motion: reduce)`.

## Accessibilité (RGAA / WCAG 2.2 AA)
- Petit texte porteur d'information : `--text-primary`, `--text-secondary` ou les variantes `--*-text` d'`app.gp.css`. `--text-tertiary` et les couleurs de statut brutes : pastilles, barres, bordures, icônes uniquement.
- Labels explicites, ordre de tabulation, focus visible (`box-shadow: 0 0 0 2px var(--accent-soft)` comme la charte), `aria-*` sur les composants maison.
- i18n : textes dans `Resources/*.resx` (fr par défaut, en, ar avec `dir="rtl"`). Aucun texte en dur.

## Tests attendus
- Composant ou page avec logique → test **bUnit** (`tests/Gprojet-Front.Tests`) : rendu avec les classes de la charte, états vide / chargement / erreur, action principale.
- Parcours critique (phases P17-P19) → test E2E Playwright .NET.
