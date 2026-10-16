# Design : charte HCP

La charte fait foi : `design/charte-hcp/README.md` (palette, police, composants, accessibilité) et `design/charte-hcp/app.css` (ne jamais modifier). Ce document n'en est que l'inventaire côté front.

## Fichiers CSS (`src/Gprojet-Front/wwwroot/css/`)

| Fichier | Rôle |
|---|---|
| `app.css` | Copie de la charte. Seule différence : la ligne `@import` Google Fonts est remplacée par 4 blocs `@font-face` locaux (Geist 400/500/600/700, `font-display: swap`, `../fonts/`). Contrôle : `python scripts/verifier-charte.py`. |
| `app.gp.css` | Ajouts GestionProjet, chargé après `app.css`, uniquement avec les variables de la charte. |

Ordre de chargement dans `index.html` : `app.css`, `app.gp.css`, Font Awesome. Chart.js n'est pas chargé globalement : une page qui trace un graphique ajoute `<script src="lib/chartjs-4.5.1/chart.umd.min.js">`, avec les couleurs lues dans les variables CSS.

## Classes de la charte utilisées

Layout : `.page`, `.sidebar`, `.sidebar-brand`, `.nav-section-label`, `.nav-item`, `.nav-badge`, `.sidebar-footer`, `.user-row`, `.main-area`, `.topbar`, `.breadcrumb-nav`, `.search-box`, `.content-area`, `.page-header`. Indicateurs : `.stats-grid`, `.stat-card`, `.stat-label`, `.stat-value`, `.stat-trend`, `.stat-bar`. Listes : `.explorer-toolbar`, `.toolbar-btn`, `.explorer-table`, `.explorer-header`, `.explorer-row`, `.explorer-breadcrumb`. Blocs : `.card-container`, `.card-header`, `.data-row`. Statuts : `.status-badge` (+ `.en-cours`, `.termine`, `.urgent`, `.en-attente`), `.inbox-priority`. Files : `.inbox-tabs`, `.inbox-item`, `.inbox-actions`, `.inbox-action-btn`. Formulaires : `.form-group`, `.form-input`, `.checkbox-label`, `.login-btn`. États vides : `.empty-state`. Mouvement : `.animate-in` (0.25s).

## Ajouts dans `app.gp.css`

- Variables de texte (H-014), seules couleurs hexadécimales autorisées : `--success-text #047857`, `--warning-text #B45309`, `--danger-text #B91C1C`, `--info-text #1D4ED8`, `--purple-text #6D28D9`. Contraste ≥ 4,5:1 sur le fond du badge (couleur d'origine à 8 % sur blanc), vérifié par `CharteTests`.
- Badges de statut (`.status-badge.<statut>`, kebab-case), couleur du texte accessible :
  - bleu : `en-cours`, `gele` ; vert : `termine`, `valide`, `validee`, `accepte`, `atteint`, `livre`, `clos` ;
  - rouge : `urgent`, `bloque`, `rejete`, `en-retard` ; orange : `en-attente`, `suspendu`, `renvoye` ;
  - violet : `a-valider`, `soumise`, `en-validation` ;
  - neutre (texte principal sur `--bg-muted`) : `a-faire`, `a-venir`, `a-produire`, `brouillon`, `ouvert`, `cloture`, `archive`, `annule`, `verrouillee`.
- Textes : `.text-success`, `.text-warning`, `.text-danger`, `.text-info`, `.text-purple`. Messages : `.message-erreur`, `.message-succes`.
- Accessibilité : `.sr-only`, `:focus-visible` (anneau `--accent-soft` de la charte), neutralisation des animations sous `prefers-reduced-motion: reduce`.

## Bibliothèques auto-hébergées (aucune ressource Internet)

| Bibliothèque | Version | Emplacement | Licence | Source |
|---|---|---|---|---|
| Geist (latin 400/500/600/700, woff2) | 5.3.0 (`@fontsource/geist`) | `wwwroot/fonts/` | SIL OFL 1.1 (`LICENSE-Geist-OFL.txt`) | npm |
| Font Awesome Free | 6.7.2 | `wwwroot/lib/fontawesome-free-6.7.2/` (`css/`, `webfonts/` woff2 seuls) | icônes CC BY 4.0, polices OFL 1.1, code MIT (`LICENSE.txt`) | npm `@fortawesome/fontawesome-free` |
| Chart.js | 4.5.1 (`chart.umd.min.js`) | `wwwroot/lib/chartjs-4.5.1/` | MIT (`LICENSE.md`) | npm `chart.js` |

Logo : `wwwroot/images/logo_hcp.png` (copie de `design/charte-hcp/images/`).

## Vérifications

`python scripts/verifier-charte.py` (app.css identique à la charte hors `@import`) ; `dotnet test --filter Category=Charte` (contrastes, pas d'hexadécimal hors des 5 variables, pas d'URL externe dans `index.html` ni `css/`).
