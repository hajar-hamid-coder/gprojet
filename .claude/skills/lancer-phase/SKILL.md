---
name: lancer-phase
description: (Utilisateur) Lance une phase du backlog et démarre l'exécution autonome de toutes ses tâches.
argument-hint: "[P1]"
disable-model-invocation: true
allowed-tools: Bash(python scripts/backlog.py *)
---

## Résultat du lancement

!`python scripts/backlog.py lancer-phase $0`

## Consigne

La phase ci-dessus vient d'être lancée par l'utilisateur. Travaille maintenant **de façon autonome** jusqu'à ce que la phase passe `A_VALIDER` :

1. Crée ou bascule sur la branche `phase/$0-<slug court>` (`git switch -c …` ; si le dépôt n'existe pas encore, la première tâche de P0 l'initialise).
2. Boucle : `python scripts/backlog.py suivante` → applique le skill `executer-tache` → recommence.
3. Ne pose pas de question à l'utilisateur pendant la phase : consigne les doutes en hypothèses (bloquantes ou non) et continue.
4. Quand `suivante` répond `AUCUNE` :
   - si la phase est `A_VALIDER` : rafraîchis le graphe (`/graphify .` si graphify est installé), puis présente le **bilan de phase** (tableau des tâches avec preuves, hypothèses ouvertes, points à vérifier manuellement, commits) et demande : « Validez-vous la phase avec `/valider-phase $0` ? »
   - si des tâches sont `BLOQUE` : liste les hypothèses bloquantes et demande `/repondre-hypothese`.
