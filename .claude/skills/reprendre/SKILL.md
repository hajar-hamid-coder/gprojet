---
name: reprendre
description: Procédure de REPRISE du projet après une interruption (nouvelle session, fermeture, plantage, contexte compacté, limite d'usage atteinte). À appliquer avant toute autre action quand le contexte de démarrage signale une tâche interrompue, des fichiers non commités, ou quand l'utilisateur tape /reprendre.
allowed-tools: Bash(python scripts/backlog.py *) Bash(python scripts/pg-test.py *) Bash(python scripts/backlog_excel.py) Bash(git status*) Bash(git log*) Bash(git diff*) Bash(git branch*) Bash(git stash list*)
---

# Reprise du projet

## État constaté

```!
python scripts/backlog.py contexte
```

```!
git status --short 2>/dev/null | head -40 || echo "(pas encore de dépôt Git)"
git branch --show-current 2>/dev/null || true
git log --oneline -8 2>/dev/null || true
git stash list 2>/dev/null | head -5 || true
```

## Journal récent

!`python scripts/backlog.py journal 15`

## Procédure (dans l'ordre, sans demander l'autorisation à l'utilisateur sauf au cas 1 et au cas 2)

### 0. Remettre l'environnement d'aplomb
- Dossier `.pgtest/` signalé → `python scripts/pg-test.py arreter`.
- `BACKLOG.xlsx` en retard → `python scripts/backlog_excel.py`.
- `ConnectionStrings__Gp` absente → le signaler à l'utilisateur (ne jamais chercher ni écrire le mot de passe), puis continuer avec ce qui ne touche pas la base de dev.
- Si une phase est active et que la branche courante n'est pas `phase/<Pn>-…` → `git switch` vers la branche de la phase (la créer si elle n'existe pas). Jamais de `reset`, jamais de suppression de modifications.

### 1. Aucune phase active
Annonce la dernière phase validée, la prochaine phase et son **modèle conseillé**, puis attends `/lancer-phase`. Ne code rien.

### 2. Phase `A_VALIDER`
Re-présente le bilan de phase (tâches, preuves tirées du journal, hypothèses ouvertes, points à vérifier) et attends `/valider-phase` ou `/refuser-tache`. Ne code rien.

### 3. Phase `EN_COURS` avec une tâche `EN_COURS` (tâche interrompue)
Lis le bloc (`python scripts/backlog.py voir <ID>`) et les lignes `NOTE <ID>` du journal, puis décide selon le **code** non commité (les fichiers `backlog/` modifiés sont normaux et ne comptent pas) :

| Situation constatée | Action |
|---|---|
| **Fichiers modifiés non commités** cohérents avec la tâche (même périmètre que l'explication) | **Ne pas repartir de zéro** : appelle `verificateur` sur l'état actuel. OK → commit `<ID>: <titre>` puis `A_VALIDER` avec preuve. KO → renvoie à `executeur-<modèle>` le bloc + le diff existant (`git diff --stat`) + les écarts, en lui demandant de **compléter** le travail. |
| Fichiers de code modifiés **sans rapport** avec la tâche | `git stash push -u -m "reprise <ID> <date>" -- . ":(exclude)backlog"` (**jamais** les fichiers `backlog/`, qui portent les statuts), note-le (`backlog.py note <ID> "stash …"`), signale-le dans le bilan, puis reprends la tâche depuis l'étape 4 d'`executer-tache`. |
| Arbre propre et **dernier commit = `<ID>: …`** | La tâche était finie mais pas clôturée : `verificateur` → si OK, `tache <ID> A_VALIDER --preuve …` ; si KO, correction par l'exécuteur. |
| Arbre propre, aucun commit de la tâche | Reprends la tâche depuis l'étape 4 d'`executer-tache`. |
| Les notes montrent déjà **3 échecs** du vérificateur | Hypothèse bloquante décrivant le problème, `tache <ID> BLOQUE`, tâche suivante. |

### 4. Phase `EN_COURS` sans tâche en cours
Reprends simplement la boucle : `python scripts/backlog.py suivante` → skill `executer-tache`.

### 5. Tâches toutes bloquées
Liste les hypothèses bloquantes (question + hypothèse par défaut) et demande `/repondre-hypothese`.

## Toujours
- Trace la reprise : `python scripts/backlog.py note <ID> "Reprise de session : <état constaté> → <action>"` (ou sur la première tâche de la phase si aucune tâche n'est en cours).
- Modèle : si le modèle de cette session diffère du **modèle conseillé** de la phase, signale-le une fois à l'utilisateur (`/model …`) **sans t'arrêter** ; les tâches restent déléguées au sous-agent de leur propre modèle.
- Après la reprise, travaille en autonomie jusqu'à la fin de la phase comme d'habitude (le hook Stop te relancera).
