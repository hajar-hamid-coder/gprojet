---
name: executer-tache
description: Procédure d'orchestration d'UNE tâche du backlog (choix, démarrage, délégation au bon modèle, vérification, commit, passage A_VALIDER). À appliquer à chaque tâche pendant une phase EN_COURS.
user-invocable: false
allowed-tools: Bash(python scripts/backlog.py *) Bash(git *) Bash(python scripts/pg-test.py *)
---

# Exécuter une tâche du backlog

## 1. Choisir
`python scripts/backlog.py suivante` → `ID|modèle|titre|NOUVELLE ou REPRENDRE`.
- `AUCUNE` → arrête la boucle et fais le bilan (voir skill `lancer-phase`, étape 4).
- Ne choisis jamais une autre tâche que celle renvoyée : l'ordre d'exécution et les dépendances sont calculés par le script.
- `REPRENDRE` (tâche déjà `EN_COURS`) → applique d'abord le skill **`reprendre`** (état Git, notes) : on termine le travail existant, on ne repart pas de zéro.

## 2. Démarrer
- `NOUVELLE` : `python scripts/backlog.py tache <ID> EN_COURS` (le script refuse si une dépendance n'est pas faite).
- `python scripts/backlog.py voir <ID>` → bloc complet : explication, fichiers, test de validation, critères, retours utilisateur, hypothèses liées.

## 3. Lever les doutes avant de coder
Si l'explication laisse une décision ouverte (règle métier, valeur, choix d'UI, dépendance externe absente) :
`python scripts/backlog.py hypothese-ajouter --tache <ID> --bloquante oui|non --question "…" --hypothese "…" --impact "…"`
- bloquante → `tache <ID> BLOQUE`, retour à l'étape 1 ;
- non bloquante → continue avec l'hypothèse par défaut.

## 4. Déléguer au bon modèle
Appelle l'outil Agent avec `subagent_type` = `executeur-<modèle>` (opus / sonnet / haiku, celui de la tâche), au premier plan (`run_in_background: false`). Prompt :
- le bloc complet de la tâche (copie intégrale de `voir`) ;
- la branche courante et les tâches déjà faites dans la phase (une ligne chacune) ;
- « Exécute uniquement cette tâche, lance le test de validation, renvoie le rapport normalisé. »

Juste avant l'appel : `python scripts/backlog.py note <ID> "délégué à executeur-<modèle> (essai n)"`.
Si l'exécuteur remonte des hypothèses → consigne-les (étape 3).

## 5. Vérifier
Appelle `subagent_type: verificateur` avec le bloc de la tâche et le rapport de l'exécuteur.
- Après chaque verdict : `python scripts/backlog.py note <ID> "vérificateur OK|KO (essai n) : <écart principal>"`.
- `VERDICT: KO` → renvoie la liste des écarts au même exécuteur (étape 4). **3 essais maximum** ; au 3ᵉ échec, crée une hypothèse bloquante décrivant le problème, passe la tâche `BLOQUE` et continue avec la suivante.
- Pour les tâches `sonnet`/`opus` qui ajoutent du code, lance aussi `ponytail-review` sur le diff si le plugin est installé et corrige les remarques pertinentes (sans casser les couches).

## 6. Commit
`git add -A` puis `git commit -m "<ID>: <titre>"` (corps : résumé + résultat du test). Jamais de push.
`python scripts/backlog.py note <ID> "commit <sha court>"`.

## 7. Clore
`python scripts/backlog.py tache <ID> A_VALIDER --preuve "<commande> → <résultat chiffré>; vérificateur OK; commit <sha court>"`
Puis retour à l'étape 1, sans attendre l'utilisateur.

> Les `note` servent à la **reprise** : si la session s'interrompt, la session suivante sait où en était la tâche (dernier essai, dernier verdict, commit).
