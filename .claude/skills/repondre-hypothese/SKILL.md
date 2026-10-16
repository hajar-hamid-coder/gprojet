---
name: repondre-hypothese
description: (Utilisateur) Répond à une hypothèse ouverte ; si elle bloquait une tâche, la tâche est débloquée.
argument-hint: "[H-001] [réponse]"
disable-model-invocation: true
allowed-tools: Bash(python scripts/backlog.py *)
---

## Résultat

!`python scripts/backlog.py repondre-hypothese "$ARGUMENTS"`

## Consigne

- Si une tâche a été débloquée et que la phase est `EN_COURS`, reprends l'exécution autonome (skill `executer-tache`) jusqu'à la fin de la phase.
- Si la réponse contredit l'hypothèse par défaut d'une tâche déjà `A_VALIDER`, signale-le à l'utilisateur et propose `/refuser-tache` pour cette tâche (tu ne peux pas la rouvrir toi-même si la phase est `A_VALIDER`).
