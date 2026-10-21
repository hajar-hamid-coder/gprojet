---
name: valider-phase
description: (Utilisateur) Valide une phase terminée : ses tâches passent VALIDE et la phase suivante peut être lancée.
argument-hint: "[P1]"
disable-model-invocation: true
allowed-tools: Bash(python scripts/backlog.py *)
---

## Résultat de la validation

!`python scripts/backlog.py valider-phase $0`

## Consigne

Si la validation a réussi :
1. Fais un commit de clôture si des fichiers restent modifiés (`Pn: clôture de phase`), sans pousser.
2. Rappelle à l'utilisateur qu'il peut maintenant pousser la branche vers GitHub (`git push -u origin <branche>`) et ouvrir la pull request vers `main`.
3. Annonce la phase suivante, son modèle conseillé (à sélectionner avec `/model`) et la commande `/lancer-phase Pn`.
4. **Ne lance pas** la phase suivante.

Si la validation a échoué, explique pourquoi en une phrase (statut de la phase) et ce qu'il reste à faire.
