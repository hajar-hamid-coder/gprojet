-- =============================================================================
-- Données de référence (à exécuter après 01-schema.sql)
-- Idempotence : à exécuter une seule fois sur une base vide.
-- =============================================================================

BEGIN;

-- ----- Paramètres globaux -----
INSERT INTO commun.parametre (cle, valeur, type_valeur, description) VALUES
 ('temps.heures_par_jour',              '8',         'DECIMAL', 'Conversion heures → jours-homme'),
 ('temps.jours_ouvres',                 '[1,2,3,4,5]','JSON',   'Jours ouvrés ISO (1 = lundi)'),
 ('temps.capacite_hebdo_defaut_h',      '40',        'DECIMAL', 'Capacité hebdomadaire par défaut d''un agent'),
 ('alerte.seuil_charge_consommee_pct',  '80',        'ENTIER',  'Alerte quand la charge consommée dépasse ce % de l''estimé'),
 ('alerte.delai_affectation_jours',     '5',         'ENTIER',  'Alerte si un projet reste non affecté au-delà de ce délai'),
 ('alerte.delai_ticket_bloque_jours',   '3',         'ENTIER',  'Escalade d''un ticket bloqué depuis ce nombre de jours'),
 ('projet.prefixe_code',                'PRJ',       'TEXTE',   'Préfixe des codes projet (PRJ-AAAA-NNNN)');

-- ----- Types de projet -----
INSERT INTO projet.type_projet (code, libelle, description) VALUES
 ('DEV',     'Développement',   'Conception et réalisation d''une application'),
 ('ANALYSE', 'Analyse',         'Étude, analyse de l''existant, cahier des charges'),
 ('SECU',    'Sécurité',        'Audit, test d''intrusion, homologation'),
 ('INFRA',   'Infrastructure',  'Mise en place ou évolution d''infrastructure'),
 ('DATA',    'Data',            'Données, migration, reporting, BI'),
 ('AUDIT',   'Audit',           'Audit de conformité ou de qualité'),
 ('MAINT',   'Maintenance',     'Maintenance corrective et évolutive');

-- ----- Modèles de phases -----
INSERT INTO projet.modele_phase (type_projet_id, ordre, nom, objectif, mode_enchainement)
SELECT tp.id, v.ordre, v.nom, v.objectif, v.mode
FROM projet.type_projet tp
JOIN (VALUES
 ('DEV', 1, 'Cadrage',        'Note de cadrage validée',                         'SEQUENTIEL'),
 ('DEV', 2, 'Analyse',        'Cahier des charges et exigences validés',         'SEQUENTIEL'),
 ('DEV', 3, 'Conception',     'Diagrammes, MCD/MLD, maquettes, DAT validés',     'SEQUENTIEL'),
 ('DEV', 4, 'Développement',  'Fonctionnalités réalisées et revues',             'SEQUENTIEL'),
 ('DEV', 5, 'Recette',        'PV de recette signé',                             'SEQUENTIEL'),
 ('DEV', 6, 'Déploiement',    'Mise en production validée',                      'SEQUENTIEL'),
 ('DEV', 7, 'Maintenance',    'Garantie et support',                             'SEQUENTIEL'),
 ('ANALYSE', 1, 'Cadrage',             'Périmètre et objectifs validés',         'SEQUENTIEL'),
 ('ANALYSE', 2, 'Collecte',            'Entretiens et analyse de l''existant',   'SEQUENTIEL'),
 ('ANALYSE', 3, 'Analyse',             'Constats et recommandations',            'SEQUENTIEL'),
 ('ANALYSE', 4, 'Restitution',         'Rapport validé',                         'SEQUENTIEL'),
 ('SECU', 1, 'Cadrage',                'Périmètre et règles d''engagement',      'SEQUENTIEL'),
 ('SECU', 2, 'Analyse de risques',     'Analyse de risques validée',             'SEQUENTIEL'),
 ('SECU', 3, 'Tests',                  'Tests de sécurité réalisés',             'SEQUENTIEL'),
 ('SECU', 4, 'Rapport',                'Rapport de constats validé',             'SEQUENTIEL'),
 ('SECU', 5, 'Suivi des corrections',  'Vulnérabilités corrigées ou acceptées',  'SEQUENTIEL')
) AS v(type_code, ordre, nom, objectif, mode) ON v.type_code = tp.code;

-- ----- Workflows -----
INSERT INTO planif.workflow (code, libelle, type_element, est_defaut) VALUES
 ('WF_TACHE',         'Workflow tâche standard',     'TACHE',  true),
 ('WF_TICKET',        'Workflow ticket standard',    'TICKET', true),
 ('WF_VULNERABILITE', 'Workflow vulnérabilité',      'TICKET', false);

INSERT INTO planif.statut_workflow (workflow_id, code, libelle, categorie, couleur, ordre, est_initial)
SELECT w.id, v.code, v.libelle, v.categorie, v.couleur, v.ordre, v.initial
FROM planif.workflow w
JOIN (VALUES
 ('WF_TACHE', 'A_FAIRE',   'À faire',   'A_FAIRE',  '#9E9E9E', 1, true),
 ('WF_TACHE', 'EN_COURS',  'En cours',  'EN_COURS', '#1E88E5', 2, false),
 ('WF_TACHE', 'EN_REVUE',  'En revue',  'EN_COURS', '#FB8C00', 3, false),
 ('WF_TACHE', 'TERMINEE',  'Terminée',  'TERMINE',  '#43A047', 4, false),
 ('WF_TICKET', 'OUVERT',   'Ouvert',    'A_FAIRE',  '#9E9E9E', 1, true),
 ('WF_TICKET', 'EN_COURS', 'En cours',  'EN_COURS', '#1E88E5', 2, false),
 ('WF_TICKET', 'EN_REVUE', 'En revue',  'EN_COURS', '#FB8C00', 3, false),
 ('WF_TICKET', 'A_TESTER', 'À tester',  'EN_COURS', '#8E24AA', 4, false),
 ('WF_TICKET', 'TERMINE',  'Terminé',   'TERMINE',  '#43A047', 5, false),
 ('WF_TICKET', 'REJETE',   'Rejeté',    'TERMINE',  '#E53935', 6, false),
 ('WF_VULNERABILITE', 'IDENTIFIEE',     'Identifiée',     'A_FAIRE',  '#E53935', 1, true),
 ('WF_VULNERABILITE', 'QUALIFIEE',      'Qualifiée',      'A_FAIRE',  '#FB8C00', 2, false),
 ('WF_VULNERABILITE', 'EN_CORRECTION',  'En correction',  'EN_COURS', '#1E88E5', 3, false),
 ('WF_VULNERABILITE', 'A_VERIFIER',     'À vérifier',     'EN_COURS', '#8E24AA', 4, false),
 ('WF_VULNERABILITE', 'CORRIGEE',       'Corrigée',       'TERMINE',  '#43A047', 5, false),
 ('WF_VULNERABILITE', 'RISQUE_ACCEPTE', 'Risque accepté', 'TERMINE',  '#6D4C41', 6, false)
) AS v(wf, code, libelle, categorie, couleur, ordre, initial) ON v.wf = w.code;

INSERT INTO planif.transition (workflow_id, statut_depuis_id, statut_vers_id, libelle, roles_autorises)
SELECT w.id, d.id, t.id, v.libelle, v.roles
FROM (VALUES
 ('WF_TACHE', 'A_FAIRE',  'EN_COURS', 'Démarrer',        '{}'::text[]),
 ('WF_TACHE', 'EN_COURS', 'EN_REVUE', 'Soumettre',       '{}'),
 ('WF_TACHE', 'EN_REVUE', 'EN_COURS', 'Reprendre',       '{}'),
 ('WF_TACHE', 'EN_REVUE', 'TERMINEE', 'Terminer',        '{CHEF_SERVICE,CHEF_PROJET}'),
 ('WF_TACHE', 'TERMINEE', 'EN_COURS', 'Rouvrir',         '{CHEF_SERVICE,CHEF_PROJET}'),
 ('WF_TICKET', 'OUVERT',   'EN_COURS', 'Prendre en charge', '{}'),
 ('WF_TICKET', 'OUVERT',   'REJETE',   'Rejeter',           '{CHEF_SERVICE,CHEF_PROJET}'),
 ('WF_TICKET', 'EN_COURS', 'EN_REVUE', 'Demander revue',    '{}'),
 ('WF_TICKET', 'EN_REVUE', 'EN_COURS', 'Corriger',          '{}'),
 ('WF_TICKET', 'EN_REVUE', 'A_TESTER', 'Valider la revue',  '{}'),
 ('WF_TICKET', 'A_TESTER', 'EN_COURS', 'Test en échec',     '{}'),
 ('WF_TICKET', 'A_TESTER', 'TERMINE',  'Test réussi',       '{}'),
 ('WF_TICKET', 'TERMINE',  'OUVERT',   'Rouvrir',           '{CHEF_SERVICE,CHEF_PROJET}'),
 ('WF_VULNERABILITE', 'IDENTIFIEE',    'QUALIFIEE',      'Qualifier',          '{}'),
 ('WF_VULNERABILITE', 'QUALIFIEE',     'EN_CORRECTION',  'Corriger',           '{}'),
 ('WF_VULNERABILITE', 'QUALIFIEE',     'RISQUE_ACCEPTE', 'Accepter le risque', '{CHEF_DIVISION,DIRECTEUR}'),
 ('WF_VULNERABILITE', 'EN_CORRECTION', 'A_VERIFIER',     'Soumettre',          '{}'),
 ('WF_VULNERABILITE', 'A_VERIFIER',    'EN_CORRECTION',  'Correction KO',      '{}'),
 ('WF_VULNERABILITE', 'A_VERIFIER',    'CORRIGEE',       'Correction OK',      '{}')
) AS v(wf, depuis, vers, libelle, roles)
JOIN planif.workflow w        ON w.code = v.wf
JOIN planif.statut_workflow d ON d.workflow_id = w.id AND d.code = v.depuis
JOIN planif.statut_workflow t ON t.workflow_id = w.id AND t.code = v.vers;

-- ----- Types de ticket -----
INSERT INTO planif.type_ticket (code, libelle, icone, workflow_id)
SELECT v.code, v.libelle, v.icone, w.id
FROM (VALUES
 ('STORY',              'User story',           'book',        'WF_TICKET'),
 ('BUG',                'Bug',                  'bug',         'WF_TICKET'),
 ('AMELIORATION',       'Amélioration',         'trending-up', 'WF_TICKET'),
 ('SOUS_TACHE',         'Sous-tâche',           'checklist',   'WF_TICKET'),
 ('ANOMALIE_RECETTE',   'Anomalie de recette',  'flask',       'WF_TICKET'),
 ('INCIDENT',           'Incident',             'alert',       'WF_TICKET'),
 ('QUESTION',           'Question',             'help',        'WF_TICKET'),
 ('DEMANDE_CHANGEMENT', 'Demande de changement','swap',        'WF_TICKET'),
 ('VULNERABILITE',      'Vulnérabilité',        'shield',      'WF_VULNERABILITE')
) AS v(code, libelle, icone, wf)
JOIN planif.workflow w ON w.code = v.wf;

-- ----- Catégories d'activité (temps) -----
INSERT INTO temps.categorie_activite (code, libelle, est_projet) VALUES
 ('DEVELOPPEMENT',   'Développement',             true),
 ('ANALYSE',         'Analyse',                   true),
 ('CONCEPTION',      'Conception',                true),
 ('TEST',            'Test / recette',            true),
 ('CORRECTION',      'Correction',                true),
 ('DOCUMENTATION',   'Documentation',             true),
 ('REUNION_PROJET',  'Réunion projet',            true),
 ('DEPLOIEMENT',     'Déploiement',               true),
 ('SUPPORT',         'Support',                   true),
 ('REUNION_SERVICE', 'Réunion de service',        false),
 ('FORMATION',       'Formation',                 false),
 ('ADMINISTRATIF',   'Tâches administratives',    false);

-- ----- Fonctions et compétences -----
INSERT INTO org.fonction (code, libelle) VALUES
 ('DEV_BACK',    'Développeur back-end'),
 ('DEV_FRONT',   'Développeur front-end'),
 ('DEV_FULL',    'Développeur full-stack'),
 ('ANALYSTE',    'Analyste fonctionnel'),
 ('ARCHITECTE',  'Architecte'),
 ('TESTEUR',     'Testeur / QA'),
 ('SECURITE',    'Expert sécurité'),
 ('DEVOPS',      'Ingénieur DevOps'),
 ('DBA',         'Administrateur de bases de données'),
 ('DATA',        'Ingénieur data');

INSERT INTO org.competence (code, libelle, categorie) VALUES
 ('CSHARP',      'C# / .NET',           'Back-end'),
 ('BLAZOR',      'Blazor',              'Front-end'),
 ('POSTGRESQL',  'PostgreSQL',          'Base de données'),
 ('EFCORE',      'Entity Framework Core','Back-end'),
 ('DOCKER',      'Docker',              'DevOps'),
 ('GITLAB_CI',   'GitLab CI/CD',        'DevOps'),
 ('UML',         'UML',                 'Conception'),
 ('MERISE',      'MERISE',              'Conception'),
 ('BPMN',        'BPMN',                'Conception'),
 ('OWASP',       'OWASP / sécurité applicative', 'Sécurité'),
 ('PENTEST',     'Tests d''intrusion',  'Sécurité'),
 ('KEYCLOAK',    'Keycloak / IAM',      'Sécurité');

-- ----- Correspondance des types (générateur) -----
INSERT INTO modelisation.correspondance_type (type_logique, cible, type_physique) VALUES
 ('TEXTE',      'POSTGRESQL', 'varchar({longueur})'),
 ('TEXTE_LONG', 'POSTGRESQL', 'text'),
 ('ENTIER',     'POSTGRESQL', 'integer'),
 ('DECIMAL',    'POSTGRESQL', 'numeric({precision},{echelle})'),
 ('BOOLEEN',    'POSTGRESQL', 'boolean'),
 ('DATE',       'POSTGRESQL', 'date'),
 ('DATE_HEURE', 'POSTGRESQL', 'timestamptz'),
 ('UUID',       'POSTGRESQL', 'uuid'),
 ('ENUM',       'POSTGRESQL', 'varchar(30)'),
 ('JSON',       'POSTGRESQL', 'jsonb'),
 ('FICHIER',    'POSTGRESQL', 'uuid'),
 ('TEXTE',      'CSHARP', 'string'),
 ('TEXTE_LONG', 'CSHARP', 'string'),
 ('ENTIER',     'CSHARP', 'int'),
 ('DECIMAL',    'CSHARP', 'decimal'),
 ('BOOLEEN',    'CSHARP', 'bool'),
 ('DATE',       'CSHARP', 'DateOnly'),
 ('DATE_HEURE', 'CSHARP', 'DateTimeOffset'),
 ('UUID',       'CSHARP', 'Guid'),
 ('ENUM',       'CSHARP', '{enumeration}'),
 ('JSON',       'CSHARP', 'JsonDocument'),
 ('FICHIER',    'CSHARP', 'Guid');

COMMIT;
