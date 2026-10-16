# Charte graphique HCP — référence obligatoire du front

Source : `app.css` de l'application GED HCP (copie fidèle, **ne pas modifier ce fichier**). Logos : `images/logo_hcp.png`.
GestionProjet doit avoir **exactement** la même identité visuelle que la GED.

## Règles

1. **`app.css` est copié tel quel** dans `src/Gprojet-Front/wwwroot/css/app.css`. Aucune couleur, police, rayon ou espacement ne s'invente : on utilise les variables (`var(--accent)`, `var(--text-secondary)`, `var(--radius)`…) et les classes existantes.
2. **Ce qui manque** (Kanban, Gantt, dialogues, onglets de fiche, formulaires multi-étapes…) va dans **`wwwroot/css/app.gp.css`**, chargé après `app.css`, construit **uniquement** avec les variables de la charte et dans son style (bordures fines `--border-strong`, rayon 8 px, pas d'ombre portée sauf dialogues, texte 12,5–13 px, en-têtes de tableau en dégradé bordeaux).
3. **Pas de MudBlazor ni de bibliothèque de composants Material** : composants Razor maison dans `Gprojet-Front/Components/` qui produisent le balisage de la charte (même logique que la GED).
4. **Police Geist** : auto-hébergée dans `wwwroot/fonts/` (licence OFL) — l'`@import` Google Fonts de la charte est remplacé dans la copie par un `@font-face` local, seule modification autorisée (intranet sans Internet).
5. **Icônes** : Font Awesome Free 6 (comme la GED), **auto-hébergé** dans `wwwroot/lib/`, pas de CDN.
6. **Graphiques** des tableaux de bord : Chart.js (MIT, comme la GED), auto-hébergé, couleurs = variables de la charte.
7. **Mouvement** : durées de la charte (`0.15s ease` pour les survols, `fadeIn 0.25s` pour les apparitions, `0.6s` pour les barres de progression). Le skill motion-design sert à placer ces mouvements, pas à en inventer d'autres. Tout est neutralisé sous `prefers-reduced-motion: reduce`.
8. **ui-ux-pro-max** sert aux choix d'ergonomie (hiérarchie de l'information, densité, parcours, états vides / chargement / erreur), **jamais** à changer la palette, la police ou le style.

## Correspondance des écrans

| Besoin GestionProjet | Classes de la charte |
|---|---|
| Layout principal | `.page`, `.sidebar` (+ `.sidebar-brand`, `.nav-section-label`, `.nav-item`, `.nav-badge`, `.sidebar-footer`, `.user-row`), `.main-area`, `.topbar`, `.breadcrumb-nav`, `.search-box`, `.content-area` |
| En-tête de page | `.page-header` (h2 + `.subtitle`) |
| Indicateurs de tableau de bord | `.stats-grid`, `.stat-card`, `.stat-label`, `.stat-value`, `.stat-trend`, `.stat-bar` / `.stat-bar-fill` |
| Listes (projets, tâches, tickets, agents) | `.explorer-toolbar`, `.toolbar-btn` (`.primary`), `.toolbar-filter`, `.explorer-table`, `.explorer-header`, `.explorer-row` (adapter `grid-template-columns` dans `app.gp.css`) |
| Blocs / panneaux | `.card-container`, `.card-header`, `.data-row`, `.card-link` |
| Statuts, priorités | `.status-badge` (+ variantes ajoutées dans `app.gp.css` : `.a-faire`, `.a-valider`, `.bloque`, `.valide`…), `.inbox-priority` |
| Files de validation (affectations, temps, phases) | `.inbox-tabs`, `.inbox-tab`, `.inbox-item`, `.inbox-actions`, `.inbox-action-btn.approve / .reject` |
| Fil d'Ariane de drill-down | `.explorer-breadcrumb`, `.crumb`, `.separator` |
| Formulaires | `.form-group`, `.form-input`, `.checkbox-label`, `.login-btn` (bouton principal) |
| États vides | `.empty-state` |
| Connexion | `.login-page`, `.login-card`, `.login-brand`… |

## Accessibilité (RGAA / WCAG 2.2 AA)

Contraste mesuré sur fond blanc : `--text-tertiary` (2,5:1), `--success` (2,5:1), `--warning` (2,2:1), `--danger` (3,8:1), `--blue` (3,7:1) et `--purple` (4,2:1) sont **sous 4,5:1** pour du texte de taille normale.
Sans toucher aux variables de la charte, `app.gp.css` ajoute des variantes **réservées au texte** (badges, messages), toutes ≥ 4,5:1 sur le fond teinté du badge :

| Variable ajoutée | Valeur | Contraste sur fond du badge |
|---|---|---|
| `--success-text` | `#047857` | 5,1:1 |
| `--warning-text` | `#B45309` | 4,7:1 |
| `--danger-text` | `#B91C1C` | 5,8:1 |
| `--info-text` | `#1D4ED8` | 6,1:1 |
| `--purple-text` | `#6D28D9` | 6,5:1 |

`--text-tertiary` reste réservé aux éléments décoratifs ou non essentiels (séparateurs, placeholders, raccourcis clavier) ; toute information utile en petit texte utilise `--text-secondary` (4,8:1). Les couleurs d'origine restent utilisées pour les pastilles, barres, bordures et icônes. (Hypothèse H-014.)
