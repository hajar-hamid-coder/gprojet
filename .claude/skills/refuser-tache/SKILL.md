---
name: refuser-tache
description: (Utilisateur) Refuse une tâche A_VALIDER avec un motif ; elle repasse NON_FAIT et Claude la reprend.
argument-hint: "[P1-T03] [motif du refus]"
disable-model-invocation: true
allowed-tools: Bash(python scripts/backlog.py *)
---

## Résultat

!`python scripts/backlog.py refuser-tache "$ARGUMENTS"`

## Consigne

L'utilisateur a refusé cette tâche. Le motif est inscrit sous la tâche (`python scripts/backlog.py voir <id>`).
Reprends-la immédiatement avec le skill `executer-tache` en traitant précisément le motif, puis termine les autres tâches éventuelles de la phase et redemande la validation.
