# Revue de phase Pn : <nom de la phase>

## Résumé

<!-- Objectif de la phase et ce qui change, en 3 lignes maximum. -->

## Tâches

| ID | Titre | Statut | Commit |
|---|---|---|---|
| Pn-T01 | | A_VALIDER | |

## Preuves

<!-- Pour chaque tâche : commande de test lancée et résultat exact (nombre de tests, sortie clé). -->

- Pn-T01 : `...` → ...

## Hypothèses

<!-- Hypothèses consignées dans backlog/HYPOTHESES.md : question, hypothèse retenue, bloquante oui/non. -->

- Ouvertes :
- Levées :

## Checklist de revue

- [ ] Branche `phase/Pn-<slug>`, un commit par tâche au format `Pn-Tnn: <titre>`
- [ ] Aucun fichier de `backlog/` modifié à la main (BACKLOG.md, HYPOTHESES.md, BACKLOG.xlsx)
- [ ] Respect des couches : les DTO ne descendent pas sous l'Orchestration, les entités ne remontent pas
- [ ] Toute requête sur les projets passe par le périmètre (`PerimetreService`)
- [ ] Code généré uniquement dans des fichiers `*.g.cs` ; code manuel hors `*.g.cs`
- [ ] Tests exécutés sur l'instance jetable (`python scripts/pg-test.py demarrer`), jamais sur `gprojet`
- [ ] `python scripts/tester-schema.py` vert si le schéma ou les migrations ont changé
- [ ] `dotnet build` sans avertissement nouveau
- [ ] Aucun secret, chaîne de connexion ou donnée réelle dans le diff
- [ ] Charte HCP respectée pour toute page ou composant front
