# Feuille des hypothèses

> Une hypothèse est créée quand une tâche manque de précision. **Bloquante** : la tâche passe `BLOQUE` jusqu'à la réponse. **Non bloquante** : Claude avance avec l'hypothèse par défaut et la signale au bilan de phase.
> Création : `python scripts/backlog.py hypothese-ajouter …` (Claude). Réponse : `/repondre-hypothese H-00x <réponse>` (utilisateur).
> Statuts : `OUVERTE` → `REPONDUE`.


### H-001 · P3-T01
> **Statut** : REPONDUE · **Bloquante** : oui · **Date** : 2026-10-16 10:54
**Question** : Docker n'est pas installé sur le poste. Faut-il installer Docker Desktop (ou Rancher Desktop / Podman) avant la phase P3 ?
**Hypothèse retenue par défaut** : Docker Desktop sera installé par l'utilisateur avant P3 ; P3-T03, P3-T04 et P3-T05 peuvent avancer sans Docker.
**Impact si l'hypothèse est fausse** : Sans moteur de conteneurs, les images, docker compose et les tests Testcontainers ne peuvent pas être validés.
**Réponse utilisateur** (2026-10-16 11:23) : Docker 29 est installé

### H-002 · P3-T04
> **Statut** : REPONDUE · **Bloquante** : non · **Date** : 2026-10-16 10:54
**Question** : Quelle instance GitLab (URL, version, runners Docker disponibles) exécutera le pipeline ?
**Hypothèse retenue par défaut** : GitLab self-managed ≥ 17 avec runners Docker et Container Registry activé ; validation du YAML par script local, CI Lint GitLab dès que l'URL est connue.
**Impact si l'hypothèse est fausse** : Mots-clés ou templates de sécurité indisponibles selon l'édition (SAST/DAST complets = Ultimate).
**Réponse utilisateur** (2026-10-16 12:48) : Choix utilisateur (2026-10-16) : le dépôt du projet est sur GitHub (hajar-hamid-coder/gprojet, public) ; l'intégration continue se fait avec GitHub Actions et l'image est publiée sur ghcr.io. GitLab reste une intégration fonctionnelle de la plateforme (P12-P13).

### H-003 · P0-T06
> **Statut** : REPONDUE · **Bloquante** : non · **Date** : 2026-10-16 10:54
**Question** : Node.js n'est pas installé : faut-il l'installer pour Ponytail (hooks d'activation) et l'installation du skill motion-design ?
**Hypothèse retenue par défaut** : Installer Node.js LTS via scripts/installer-outils.ps1 ; sans Node, les skills Ponytail restent utilisables manuellement et motion-design est copié par git clone.
**Impact si l'hypothèse est fausse** : Ponytail ne s'active pas automatiquement à chaque session.
**Réponse utilisateur** (2026-10-16 11:23) : Node.js v26 est installé

### H-004 · P1-T09
> **Statut** : REPONDUE · **Bloquante** : non · **Date** : 2026-10-16 10:54
**Question** : Pour exécuter l'API en développement (hors tests), quelle base PostgreSQL utiliser et avec quel compte ?
**Hypothèse retenue par défaut** : Tests sur l'instance jetable pg-test ; exécution locale sur le service PostgreSQL 18 du poste avec une chaîne de connexion définie par l'utilisateur dans dotnet user-secrets (Claude ne connaît aucun mot de passe).
**Impact si l'hypothèse est fausse** : Si une autre base est imposée, seule la configuration change.
**Réponse utilisateur** (2026-10-16 11:31) : Base de développement fournie : gprojet (vide) sur localhost:5432, compte postgres. Chaîne de connexion dans .claude/settings.local.json (variable ConnectionStrings__Gp, non versionnée). Les tests restent sur l'instance jetable pg-test, jamais sur gprojet.

### H-005 · P6-T11
> **Statut** : OUVERTE · **Bloquante** : non · **Date** : 2026-10-16 10:54
**Question** : Quelle bibliothèque de Gantt utiliser (licence compatible marché public) ?
**Hypothèse retenue par défaut** : Frappe Gantt (MIT) via JS Interop ; vue PERT avec Cytoscape.js (MIT).
**Impact si l'hypothèse est fausse** : Fonctions avancées (ressources, baseline visuelle) à développer soi-même ; alternative commerciale DHTMLX / Syncfusion.
**Réponse utilisateur** : _en attente_

### H-006 · P8-T07
> **Statut** : OUVERTE · **Bloquante** : non · **Date** : 2026-10-16 10:54
**Question** : Bibliothèques d'export Excel et PDF autorisées ?
**Hypothèse retenue par défaut** : ClosedXML (MIT) pour Excel, PDFsharp / MigraDoc (MIT) pour PDF ; QuestPDF écarté (licence communautaire conditionnelle).
**Impact si l'hypothèse est fausse** : Mise en page PDF plus verbeuse à coder.
**Réponse utilisateur** : _en attente_

### H-007 · P8-T07
> **Statut** : OUVERTE · **Bloquante** : non · **Date** : 2026-10-16 10:54
**Question** : Charte officielle (logo, en-tête, mentions) à appliquer sur les exports et les livrables ?
**Hypothèse retenue par défaut** : Logo et en-tête génériques paramétrables par entité, remplacés par les fichiers officiels fournis plus tard.
**Impact si l'hypothèse est fausse** : Exports à régénérer une fois la charte fournie.
**Réponse utilisateur** : _en attente_

### H-008 · P9-T02
> **Statut** : OUVERTE · **Bloquante** : non · **Date** : 2026-10-16 10:54
**Question** : Les pièces jointes doivent-elles passer par un antivirus ?
**Hypothèse retenue par défaut** : Point d'extension IAnalyseurFichier ; implémentation ClamAV (conteneur) optionnelle, désactivée en développement.
**Impact si l'hypothèse est fausse** : Sans antivirus, risque de dépôt de fichiers malveillants en production.
**Réponse utilisateur** : _en attente_

### H-009 · P10-T07
> **Statut** : OUVERTE · **Bloquante** : non · **Date** : 2026-10-16 10:54
**Question** : Quelle bibliothèque pour l'éditeur visuel MCD / classes ?
**Hypothèse retenue par défaut** : JointJS core (MPL-2.0) via JS Interop.
**Impact si l'hypothèse est fausse** : Si refusée, alternative Drawflow (MIT) avec plus de développement.
**Réponse utilisateur** : _en attente_

### H-010 · P10-T08
> **Statut** : OUVERTE · **Bloquante** : non · **Date** : 2026-10-16 10:54
**Question** : bpmn-js impose de conserver le filigrane bpmn.io : est-ce acceptable ?
**Hypothèse retenue par défaut** : Oui, filigrane conservé conformément à la licence.
**Impact si l'hypothèse est fausse** : Sinon, BPMN limité à un rendu texte / Mermaid.
**Réponse utilisateur** : _en attente_

### H-011 · P12-T01
> **Statut** : OUVERTE · **Bloquante** : non · **Date** : 2026-10-16 10:54
**Question** : URL GitLab et jeton technique (portée api) pour l'intégration réelle ?
**Hypothèse retenue par défaut** : Développement et tests sur serveur GitLab simulé ; l'URL et le jeton seront fournis via la configuration sécurisée avant la recette.
**Impact si l'hypothèse est fausse** : Intégration non testée de bout en bout tant que l'instance n'est pas fournie.
**Réponse utilisateur** : _en attente_

### H-012 · P15-T03
> **Statut** : OUVERTE · **Bloquante** : non · **Date** : 2026-10-16 10:54
**Question** : Une instance SonarQube est-elle disponible ?
**Hypothèse retenue par défaut** : Intégration optionnelle, désactivée si aucune URL n'est configurée.
**Impact si l'hypothèse est fausse** : Indicateurs de dette technique absents.
**Réponse utilisateur** : _en attente_

### H-013 · P18-T01
> **Statut** : OUVERTE · **Bloquante** : oui · **Date** : 2026-10-16 10:54
**Question** : Paramètres de l'annuaire AD / LDAP : URL, base DN, compte de service, groupes à mapper vers les rôles, attribut portant le matricule ?
**Hypothèse retenue par défaut** : Aucune : ces valeurs ne peuvent pas être devinées.
**Impact si l'hypothèse est fausse** : Fédération Keycloak impossible sans elles.
**Réponse utilisateur** : _en attente_

### H-014 · P2-T02
> **Statut** : REPONDUE · **Bloquante** : non · **Date** : 2026-10-16 11:31
**Question** : La charte HCP utilise des couleurs de texte sous le seuil RGAA/WCAG (text-tertiary 2,5:1, success 2,5:1, warning 2,2:1, danger 3,8:1, blue 3,7:1, purple 4,2:1). Accepte-t-on des variantes foncées réservées au texte, ajoutées dans app.gp.css sans toucher à app.css ?
**Hypothèse retenue par défaut** : Oui : --success-text #047857, --warning-text #B45309, --danger-text #B91C1C, --info-text #1D4ED8, --purple-text #6D28D9 (toutes ≥ 4,5:1 sur le fond du badge) ; couleurs d'origine conservées pour pastilles, barres, bordures et icônes ; text-tertiary réservé au décoratif.
**Impact si l'hypothèse est fausse** : Si refusé : badges et petits textes de statut non conformes RGAA, la phase P17 (0 violation axe) ne pourra pas être validée.
**Réponse utilisateur** (2026-10-16 11:34) : Oui, variantes de texte accessibles validées

### H-015 · P0-T02
> **Statut** : OUVERTE · **Bloquante** : non · **Date** : 2026-10-16 11:51
**Question** : Que faire si PG_BIN pointe vers un dossier sans binaires PostgreSQL ?
**Hypothèse retenue par défaut** : PG_BIN est exclusif dans verifier-env.py (pas de repli), sinon le cas test PG_BIN vide ne peut jamais sortir en 1 ici
**Impact si l'hypothèse est fausse** : pg-test.py retombe sur les chemins par défaut : comportement différent entre les deux scripts
**Réponse utilisateur** : _en attente_

### H-016 · P0-T03
> **Statut** : OUVERTE · **Bloquante** : non · **Date** : 2026-10-16 11:52
**Question** : database/03-tests-integrite.sql compte-t-il 13 blocs ERREUR + 5 OK comme l'indique la tâche ?
**Hypothèse retenue par défaut** : Il contient 12 ERREUR + 6 OK (total 18 conforme) ; la répartition 13+5 est une erreur de décompte, seul le total compte
**Impact si l'hypothèse est fausse** : Si faux, un bloc attendu ERREUR manquerait dans le fichier 03
**Réponse utilisateur** : _en attente_

### H-017 · P0-T04
> **Statut** : REPONDUE · **Bloquante** : non · **Date** : 2026-10-16 11:55
**Question** : FluentAssertions 8.x (licence commerciale Xceed) est-il acceptable pour un projet d'État ?
**Hypothèse retenue par défaut** : Version 8.11.0 retenue pour l'instant ; si la licence pose problème, épingler 7.x (Apache 2.0) dans Directory.Packages.props
**Impact si l'hypothèse est fausse** : Aucun avant l'écriture des premiers tests (P0-T05+ ou P1)
**Réponse utilisateur** (2026-10-16 12:51) : Non : épingler FluentAssertions 7.x (Apache 2.0), pas de licence commerciale

### H-018 · P0-T06
> **Statut** : OUVERTE · **Bloquante** : non · **Date** : 2026-10-16 11:58
**Question** : graphify (CLI, paquet PyPI graphifyy) est absent du poste et uv (requis pour l'installer) est absent aussi : faut-il les installer maintenant pour le graphe de connaissances du code ?
**Hypothèse retenue par défaut** : Ne pas installer maintenant : graphify n'est utile qu'à partir de la première phase qui produit du code (P1). Installation prévue via scripts/installer-outils.ps1 (étapes uv puis uv tool install graphifyy, puis graphify install --project) avant P1 ; en attendant, la recherche se fait par Grep/Explore.
**Impact si l'hypothèse est fausse** : Sans graphify, les requêtes 'graphify query' de CLAUDE.md ne fonctionnent pas : le repérage du code se fait par recherche classique (plus lent, plus de tokens). Aucun blocage de P0.
**Réponse utilisateur** : _en attente_

### H-019 · P1-T02
> **Statut** : OUVERTE · **Bloquante** : non · **Date** : 2026-10-16 13:01
**Question** : Comment reconnaître un contrôleur, et la liste blanche System.* de Contracts suffit-elle ?
**Hypothèse retenue par défaut** : Contrôleur = hérite de ControllerBase ou nom en *Controller ; Contracts autorise tout System.* (les API absentes du navigateur ne sont pas détectées, la compilation du front en P2 servira de seconde garde)
**Impact si l'hypothèse est fausse** : Des endpoints minimal API échapperaient à la règle ; une API System non disponible en WASM serait détectée seulement à la compilation du front
**Réponse utilisateur** : _en attente_

### H-020 · P1-T03
> **Statut** : OUVERTE · **Bloquante** : non · **Date** : 2026-10-16 13:05
**Question** : Dans quel fuseau calcule-t-on la date du jour (saisie des temps, échéances) ?
**Hypothèse retenue par défaut** : Fuseau Africa/Casablanca (HCP) ; IHorloge.Aujourdhui est calculé dans ce fuseau, Maintenant reste en UTC
**Impact si l'hypothèse est fausse** : Si un autre fuseau est attendu, changer la valeur passée à HorlogeSysteme ; en UTC pur, les saisies entre minuit et 1 h seraient datées de la veille
**Réponse utilisateur** : _en attente_