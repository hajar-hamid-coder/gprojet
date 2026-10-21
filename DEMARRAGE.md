# Démarrer avec Claude Code

## 1. Installation (une fois)

```powershell
powershell -ExecutionPolicy Bypass -File scripts\installer-outils.ps1
```

Le script propose, étape par étape : Node.js (Ponytail), uv + graphify, le skill motion-design, Docker Desktop (nécessaire à partir de P3).
Ponytail et UI/UX Pro Max sont déclarés dans `.claude/settings.json` : Claude Code propose de les installer après que vous avez accordé votre confiance au dossier.

## 2. Lancer une session autonome

```powershell
cd "C:\Users\Lenovo\Desktop\HCP\Gestion des projets\Projet"
claude --permission-mode auto --model sonnet
```

- `--permission-mode auto` : Claude enchaîne sans demander d'autorisation ; un classifieur et les hooks du projet bloquent les actions risquées. Ce mode ne peut pas être fixé dans les réglages du projet, il se passe en ligne de commande (ou dans vos réglages utilisateur).
- `--model` : prenez le **modèle conseillé de la phase** (colonne « Modèle » du tableau de bord du backlog). Chaque tâche est ensuite déléguée au sous-agent de son propre modèle (opus / sonnet / haiku).

## 3. Cycle de travail

| Vous | Claude |
|---|---|
| `/lancer-phase P0` | Exécute toutes les tâches de P0 dans l'ordre, délègue chacune au bon modèle, fait vérifier, commit, passe la tâche `A_VALIDER`. Le hook Stop l'empêche de s'arrêter tant qu'il reste du travail exécutable. |
| — | Phase terminée : présente le bilan et demande `/valider-phase P0`. |
| `/refuser-tache P0-T04 <motif>` *(si besoin)* | Reprend la tâche en tenant compte du motif, puis redemande la validation. |
| `/repondre-hypothese H-003 <réponse>` *(si besoin)* | Débloque la tâche concernée et continue. |
| `/valider-phase P0` | Clôt la phase, annonce la suivante et son modèle. Vous poussez la branche vers GitHub et ouvrez la pull request. |
| `/model opus` puis `/lancer-phase P1` | … et ainsi de suite jusqu'à P19. |

`/etat-projet` affiche l'avancement à tout moment ; `/reprendre` relance proprement le travail après une interruption. Au démarrage de chaque session, un hook injecte automatiquement l'état du backlog dans le contexte de Claude.

**Si une commande `/…` réservée échoue** (par exemple pour une question de permission), lancez-la vous-même dans un terminal :

```powershell
python scripts\backlog.py valider-phase P0
```

## 4. Reprendre après une interruption

Fermeture de la session, plantage, coupure, limite d'usage atteinte, ou simplement le lendemain :

1. Rouvrez une session Claude Code dans le dossier `Projet` (même commande qu'au point 2).
2. Tapez `/reprendre` (le hook de démarrage affiche déjà l'état, la commande force la procédure complète).
3. Claude constate l'état (phase, tâche interrompue, fichiers non commités, dernières notes du journal), remet l'environnement d'aplomb, **termine la tâche interrompue sans repartir de zéro** et continue la phase en autonomie. Rien n'est jamais effacé : au pire, des modifications sans rapport sont mises de côté avec `git stash` et signalées.

Si la phase était déjà terminée, Claude re-présente le bilan et attend votre `/valider-phase`.

## 5. Garde-fous en place

| Garde-fou | Mécanisme |
|---|---|
| Claude ne peut ni lancer, ni valider une phase, ni refuser une tâche, ni répondre à une hypothèse | hook `PreToolUse` (`.claude/hooks/garde.py`) |
| Statuts modifiables uniquement via le script (ordre et dépendances contrôlés) | `scripts/backlog.py` + hook qui interdit l'édition directe de `BACKLOG.md` / `HYPOTHESES.md` |
| Pas de `A_VALIDER` sans preuve de test | `backlog.py tache … A_VALIDER --preuve` obligatoire, test rejoué par le sous-agent `verificateur` |
| Autonomie jusqu'à la fin de la phase | hook `Stop` (`backlog.py stop-check`), avec arrêt automatique après 3 relances sans progrès |
| Aucun `git push`, `reset --hard`, `--no-verify`, `DROP DATABASE`, ni écriture hors du projet (sauf `../Documentation`) | hook `PreToolUse` + règles `deny` |
| Secrets hors dépôt | Base de dev `gprojet` via `.claude/settings.local.json` (ignoré par Git) ; tests sur une instance PostgreSQL jetable (`scripts/pg-test.py`), jamais sur `gprojet` |

## 6. Fichiers

| Fichier | Rôle |
|---|---|
| `CLAUDE.md` | Mémoire du projet : contexte, règles d'autonomie, architecture, conventions, outils |
| `backlog/BACKLOG.md` | 20 phases, 148 tâches : code, modèle, ordre, dépendances, explication, test de validation |
| `backlog/BACKLOG.xlsx` | Vue Excel du backlog, régénérée à chaque changement de statut : feuille « Ordre d'exécution » (synthèse des phases + séquence des 148 tâches), une feuille par phase (P0 … P19), « Hypothèses », « Légende ». Lecture seule : toute modification manuelle est écrasée. Si le fichier est ouvert dans Excel pendant que Claude travaille, il est mis à jour à la transition suivante après fermeture (ou `python scriptsacklog_excel.py`). |
| `backlog/HYPOTHESES.md` | Feuille des hypothèses (13 ouvertes au départ, dont 2 bloquantes) |
| `backlog/JOURNAL.md` | Journal automatique des transitions et des preuves |
| `.claude/settings.json` | Permissions, hooks, plugins (Ponytail, UI/UX Pro Max) |
| `.claude/agents/` | `executeur-opus`, `executeur-sonnet`, `executeur-haiku`, `verificateur` |
| `.claude/skills/` | Commandes utilisateur (`lancer-phase`, `valider-phase`, `refuser-tache`, `repondre-hypothese`, `etat-projet`, `reprendre`) et skills de travail (`executer-tache`, `conventions-backend`, `conventions-frontend`) |
| `scripts/backlog.py` | Moteur du backlog |
| `scripts/backlog_excel.py` | Export Excel du backlog (openpyxl) |
| `scripts/pg-test.py` | Instance PostgreSQL 18 jetable pour les tests |
| `scripts/installer-outils.ps1` | Installation guidée des outils |
| `database/` | Schéma SQL de référence, données de référence, tests d'intégrité |
