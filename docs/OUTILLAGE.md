# Outillage Claude Code — état constaté

Constat réalisé le 2026-10-16 sur ce poste (Windows 11). Chaque statut est appuyé par la preuve trouvée. Pour installer un élément manquant : `powershell -ExecutionPolicy Bypass -File scripts\installer-outils.ps1` (chaque étape demande confirmation).

## Outils de la CLAUDE.md

| Outil | Type | Statut | Preuve constatée |
|---|---|---|---|
| graphify | Outil CLI (paquet PyPI `graphifyy`) | MANQUANT | `graphify` absent du PATH (`graphify --version` introuvable) ; `uv` absent (prérequis) ; aucun hook graphify dans `.claude/settings.json`. Hypothèse H-018 (non bloquante). |
| ponytail | Plugin | INSTALLÉ | `~/.claude/plugins/installed_plugins.json` : `ponytail@ponytail` v5.3.0, portée projet ; dossier `~/.claude/plugins/cache/ponytail/ponytail/5.3.0` ; activé dans `.claude/settings.json` (`enabledPlugins`). |
| ui-ux-pro-max | Plugin | INSTALLÉ | `~/.claude/plugins/installed_plugins.json` : `ui-ux-pro-max@ui-ux-pro-max-skill` v2.15.0, portée projet ; dossier `~/.claude/plugins/cache/ui-ux-pro-max-skill/ui-ux-pro-max/2.15.0` ; activé dans `.claude/settings.json`. |
| motion-design | Skill | INSTALLÉ | `.claude/skills/motion-design/SKILL.md` présent (avec `director/`, `patterns/`, `reference/`). |

### Commandes d'installation des éléments manquants

- **graphify** (étape 2 de `scripts/installer-outils.ps1`) :
  ```
  winget install --id astral-sh.uv -e --accept-package-agreements
  uv tool install graphifyy
  uv tool update-shell
  graphify install --project
  ```
  `graphify install --project` ajoute skill et hooks dans `.claude/` : vérifier ensuite le diff de `.claude/settings.json`.

### Commandes d'installation des plugins (à taper dans Claude Code si besoin)

```
/plugin marketplace add DietrichGebert/ponytail
/plugin install ponytail@ponytail
/plugin marketplace add nextlevelbuilder/ui-ux-pro-max-skill
/plugin install ui-ux-pro-max@ui-ux-pro-max-skill
```

## Prérequis vérifiés

| Élément | Statut | Preuve |
|---|---|---|
| Node.js (hooks Ponytail, motion-design) | INSTALLÉ (v26.12.0) | `node --version` ; réponse à H-003 |
| openpyxl | INSTALLÉ | `scripts/verifier-env.py` |
| uv | MANQUANT | `uv --version` introuvable ; bloque graphify uniquement |

## Hooks du projet (`.claude/settings.json`)

| Événement | Commande | Rôle |
|---|---|---|
| SessionStart | `scripts/backlog.py contexte` | injecte l'état du backlog et de Git |
| PreToolUse (Edit, Write, MultiEdit, NotebookEdit, Bash, PowerShell) | `.claude/hooks/garde.py` | garde-fou des actions |
| Stop | `scripts/backlog.py stop-check` | relance tant qu'une tâche reste exécutable |

Hooks graphify : absents (à ajouter par `graphify install --project`).

## Sous-agents du projet (`.claude/agents/`)

- `executeur-haiku.md` — tâches simples
- `executeur-sonnet.md` — tâches moyennes
- `executeur-opus.md` — tâches complexes
- `verificateur.md` — vérification indépendante avant `A_VALIDER`

## Skills du projet (`.claude/skills/`)

`conventions-backend`, `conventions-frontend`, `etat-projet`, `executer-tache`, `lancer-phase`, `motion-design`, `refuser-tache`, `repondre-hypothese`, `reprendre`, `valider-phase`.

## Points d'attention

- Sans graphify, les requêtes `graphify query` prévues par `CLAUDE.md` ne sont pas disponibles : la recherche de code se fait par Grep ou par l'agent Explore.
- Les valeurs de configuration locale (`.claude/settings.local.json`, non versionné) ne sont pas reportées ici.
