-- =============================================================================
-- Données de démonstration (à exécuter APRÈS 01-schema.sql et 02-donnees-reference.sql)
-- Uniquement des données FICTIVES : noms inventés, emails sur le domaine réservé
-- exemple.test, aucun mot de passe. Matricules A0001..A0025 (en-tête X-Dev-Matricule).
--
-- Contenu : 1 direction, 3 divisions, 6 services (10 entités, chemin ltree
-- direction.division.service), 25 agents, 10 responsables (un par entité),
-- compétences, 2 délégations, 5 absences, jours fériés 2026.
-- Utilisée par : python scripts/pg-test.py demarrer --demo
-- =============================================================================

BEGIN;

-- ----- Organigramme (ids fixes pour les chemins ltree ; chemin = ids sans tirets) -----
INSERT INTO org.entite_organisationnelle (id, code, libelle, sigle, niveau, parent_id, chemin)
SELECT v.id::uuid, v.code, v.libelle, v.sigle, v.niveau, NULL, replace(v.id, '-', '')::ltree
FROM (VALUES
 ('00000000-0000-7000-8000-000000000001', 'DND', 'Direction numérique', 'DN', 'DIRECTION')
) AS v(id, code, libelle, sigle, niveau);

INSERT INTO org.entite_organisationnelle (id, code, libelle, sigle, niveau, parent_id, chemin)
SELECT v.id::uuid, v.code, v.libelle, v.sigle, 'DIVISION', p.id, p.chemin || replace(v.id, '-', '')::ltree
FROM (VALUES
 ('00000000-0000-7000-8000-000000000002', 'DIV-APP', 'Division des applications', 'DAP', 'DND'),
 ('00000000-0000-7000-8000-000000000003', 'DIV-INF', 'Division de l''infrastructure', 'DIN', 'DND'),
 ('00000000-0000-7000-8000-000000000004', 'DIV-DON', 'Division des données', 'DDO', 'DND')
) AS v(id, code, libelle, sigle, parent)
JOIN org.entite_organisationnelle p ON p.code = v.parent;

INSERT INTO org.entite_organisationnelle (id, code, libelle, sigle, niveau, parent_id, chemin)
SELECT v.id::uuid, v.code, v.libelle, NULL, 'SERVICE', p.id, p.chemin || replace(v.id, '-', '')::ltree
FROM (VALUES
 ('00000000-0000-7000-8000-000000000005', 'SRV-APP-WEB', 'Service développement web', 'DIV-APP'),
 ('00000000-0000-7000-8000-000000000006', 'SRV-APP-MET', 'Service développement métier', 'DIV-APP'),
 ('00000000-0000-7000-8000-000000000007', 'SRV-INF-RES', 'Service réseaux', 'DIV-INF'),
 ('00000000-0000-7000-8000-000000000008', 'SRV-INF-SYS', 'Service systèmes et exploitation', 'DIV-INF'),
 ('00000000-0000-7000-8000-000000000009', 'SRV-DON-BI', 'Service décisionnel', 'DIV-DON'),
 ('00000000-0000-7000-8000-000000000010', 'SRV-DON-QUA', 'Service qualité des données', 'DIV-DON')
) AS v(id, code, libelle, parent)
JOIN org.entite_organisationnelle p ON p.code = v.parent;

-- ----- Agents (entité de rattachement et fonction par code de référence) -----
INSERT INTO org.agent (matricule, nom, prenom, email, entite_id, fonction_id)
SELECT v.matricule, v.nom, v.prenom, lower(v.prenom) || '.' || lower(v.nom) || '@exemple.test', e.id, f.id
FROM (VALUES
 ('A0001', 'Benslimane', 'Karim',    'DND',         'ARCHITECTE'),
 ('A0002', 'Lahlou',     'Nadia',    'DIV-APP',     'ARCHITECTE'),
 ('A0003', 'Ouazzani',   'Hicham',   'DIV-INF',     'ARCHITECTE'),
 ('A0004', 'Chraibi',    'Leila',    'DIV-DON',     'DATA'),
 ('A0005', 'Tahiri',     'Youssef',  'DIV-APP',     'DEV_BACK'),
 ('A0006', 'Idrissi',    'Soukaina', 'DIV-INF',     'DEVOPS'),
 ('A0007', 'Berrada',    'Amine',    'DIV-DON',     'ANALYSTE'),
 ('A0008', 'Benjelloun', 'Salma',    'SRV-APP-WEB', 'DEV_FRONT'),
 ('A0009', 'Sefrioui',   'Rachid',   'SRV-APP-WEB', 'DEV_BACK'),
 ('A0010', 'Fassi',      'Ghita',    'SRV-APP-WEB', 'DEV_FRONT'),
 ('A0011', 'Bennani',    'Mehdi',    'SRV-APP-MET', 'ANALYSTE'),
 ('A0012', 'Alami',      'Imane',    'SRV-APP-MET', 'ANALYSTE'),
 ('A0013', 'Kabbaj',     'Othmane',  'SRV-APP-MET', 'ANALYSTE'),
 ('A0014', 'Naciri',     'Zineb',    'SRV-INF-RES', 'DEVOPS'),
 ('A0015', 'Mouline',    'Adil',     'SRV-INF-RES', 'DEVOPS'),
 ('A0016', 'Amrani',     'Meryem',   'SRV-INF-RES', 'DEVOPS'),
 ('A0017', 'Belkadi',    'Anas',     'SRV-INF-SYS', 'SECURITE'),
 ('A0018', 'Sbai',       'Dounia',   'SRV-INF-SYS', 'SECURITE'),
 ('A0019', 'Lamrani',    'Reda',     'SRV-INF-SYS', 'TESTEUR'),
 ('A0020', 'Zerouali',   'Ilham',    'SRV-DON-BI',  'DATA'),
 ('A0021', 'Gharbi',     'Tarik',    'SRV-DON-BI',  'DBA'),
 ('A0022', 'Jebli',      'Naima',    'SRV-DON-BI',  'DATA'),
 ('A0023', 'Berrechid',  'Ayoub',    'SRV-DON-QUA', 'SECURITE'),
 ('A0024', 'Ouahbi',     'Hanane',   'SRV-DON-QUA', 'DBA'),
 ('A0025', 'Hadri',      'Soufiane', 'SRV-DON-QUA', 'DEV_FULL')
) AS v(matricule, nom, prenom, entite, fonction)
JOIN org.entite_organisationnelle e ON e.code = v.entite
JOIN org.fonction f                 ON f.code = v.fonction;

-- Rattachement initial de chaque agent (historique : un seul rattachement, en cours)
INSERT INTO org.rattachement (agent_id, entite_id, date_debut, motif)
SELECT a.id, a.entite_id, DATE '2024-01-01', 'Affectation initiale (démo)'
FROM org.agent a;

-- ----- Responsables : un agent distinct par entité (ux_entite_responsable) -----
UPDATE org.entite_organisationnelle e
SET responsable_id = a.id
FROM (VALUES
 ('DND', 'A0001'), ('DIV-APP', 'A0002'), ('DIV-INF', 'A0003'), ('DIV-DON', 'A0004'),
 ('SRV-APP-WEB', 'A0008'), ('SRV-APP-MET', 'A0011'), ('SRV-INF-RES', 'A0014'),
 ('SRV-INF-SYS', 'A0017'), ('SRV-DON-BI', 'A0020'), ('SRV-DON-QUA', 'A0023')
) AS v(code, matricule)
JOIN org.agent a ON a.matricule = v.matricule
WHERE e.code = v.code;

-- ----- Compétences (2 par agent) -----
INSERT INTO org.agent_competence (agent_id, competence_id, niveau)
SELECT a.id, c.id, v.niveau
FROM (VALUES
 ('A0001', 'UML', 3),        ('A0001', 'OWASP', 2),
 ('A0002', 'UML', 4),        ('A0002', 'BPMN', 3),
 ('A0003', 'POSTGRESQL', 4), ('A0003', 'DOCKER', 3),
 ('A0004', 'POSTGRESQL', 4), ('A0004', 'MERISE', 4),
 ('A0005', 'CSHARP', 4),     ('A0005', 'EFCORE', 4),
 ('A0006', 'GITLAB_CI', 4),  ('A0006', 'DOCKER', 4),
 ('A0007', 'POSTGRESQL', 3), ('A0007', 'MERISE', 3),
 ('A0008', 'BLAZOR', 5),     ('A0008', 'CSHARP', 3),
 ('A0009', 'CSHARP', 4),     ('A0009', 'EFCORE', 3),
 ('A0010', 'BLAZOR', 4),     ('A0010', 'UML', 2),
 ('A0011', 'BPMN', 4),       ('A0011', 'UML', 3),
 ('A0012', 'MERISE', 3),     ('A0012', 'BPMN', 3),
 ('A0013', 'UML', 3),        ('A0013', 'BPMN', 2),
 ('A0014', 'DOCKER', 4),     ('A0014', 'GITLAB_CI', 3),
 ('A0015', 'DOCKER', 3),     ('A0015', 'OWASP', 2),
 ('A0016', 'GITLAB_CI', 3),  ('A0016', 'KEYCLOAK', 2),
 ('A0017', 'DOCKER', 4),     ('A0017', 'KEYCLOAK', 3),
 ('A0018', 'OWASP', 3),      ('A0018', 'PENTEST', 2),
 ('A0019', 'PENTEST', 3),    ('A0019', 'OWASP', 3),
 ('A0020', 'POSTGRESQL', 5), ('A0020', 'MERISE', 4),
 ('A0021', 'POSTGRESQL', 4), ('A0021', 'EFCORE', 2),
 ('A0022', 'MERISE', 3),     ('A0022', 'POSTGRESQL', 3),
 ('A0023', 'OWASP', 2),      ('A0023', 'KEYCLOAK', 2),
 ('A0024', 'POSTGRESQL', 2), ('A0024', 'GITLAB_CI', 2),
 ('A0025', 'CSHARP', 3),     ('A0025', 'BLAZOR', 2)
) AS v(matricule, competence, niveau)
JOIN org.agent a         ON a.matricule = v.matricule
JOIN org.competence c    ON c.code = v.competence;

-- ----- Délégations (2) -----
INSERT INTO org.delegation (delegant_id, delegataire_id, entite_id, date_debut, date_fin, motif,
                            peut_creer_projet, peut_affecter, peut_valider)
SELECT d.id, t.id, e.id, v.debut, v.fin, v.motif, v.creer, v.affecter, v.valider
FROM (VALUES
 ('A0002', 'A0008', 'DIV-APP', DATE '2026-03-02', DATE '2026-03-31',
  'Intérim du chef de division pendant ses congés', false, false, true),
 ('A0001', 'A0003', 'DND',     DATE '2026-07-01', DATE '2026-07-15',
  'Déplacement hors site du directeur', true, true, false)
) AS v(delegant, delegataire, entite, debut, fin, motif, creer, affecter, valider)
JOIN org.agent d                    ON d.matricule = v.delegant
JOIN org.agent t                    ON t.matricule = v.delegataire
JOIN org.entite_organisationnelle e ON e.code = v.entite;

-- ----- Absences (5) -----
INSERT INTO org.absence (agent_id, date_debut, date_fin, type, demi_journee, commentaire)
SELECT a.id, v.debut, v.fin, v.type, v.demi, v.commentaire
FROM (VALUES
 ('A0009', DATE '2026-04-06', DATE '2026-04-10', 'CONGE',     false, NULL),
 ('A0015', DATE '2026-02-16', DATE '2026-02-17', 'MALADIE',   false, NULL),
 ('A0021', DATE '2026-05-18', DATE '2026-05-22', 'FORMATION', false, 'Formation PostgreSQL 18'),
 ('A0005', DATE '2026-09-07', DATE '2026-09-09', 'MISSION',   false, 'Mission de recette sur site'),
 ('A0019', DATE '2026-06-12', DATE '2026-06-12', 'CONGE',     true,  'Demi-journée')
) AS v(matricule, debut, fin, type, demi, commentaire)
JOIN org.agent a ON a.matricule = v.matricule;

-- ----- Jours fériés 2026 -----
-- Fixes : calendrier civil marocain (1er janvier, 1er mai, 30 juillet, 14/20/21 août, 6 et 18 novembre).
-- Mobiles (calendrier hégirien) : dates INDICATIVES pour 2026, à confirmer par l'annonce officielle
-- avant chaque année ; à ajuster dans l'écran de paramétrage des jours fériés.
INSERT INTO org.jour_ferie (date_ferie, libelle) VALUES
 (DATE '2026-01-01', 'Nouvel an'),
 (DATE '2026-03-20', 'Aïd al-Fitr (1er jour, indicatif)'),
 (DATE '2026-05-01', 'Fête du Travail'),
 (DATE '2026-05-27', 'Aïd al-Adha (1er jour, indicatif)'),
 (DATE '2026-06-16', 'Nouvel an hégirien (indicatif)'),
 (DATE '2026-07-30', 'Fête du Trône'),
 (DATE '2026-08-14', 'Récupération d''Oued Eddahab'),
 (DATE '2026-08-20', 'Révolution du Roi et du Peuple'),
 (DATE '2026-08-21', 'Fête de la Jeunesse'),
 (DATE '2026-08-25', 'Aïd al-Mawlid (indicatif)'),
 (DATE '2026-11-06', 'Marche verte'),
 (DATE '2026-11-18', 'Fête de l''Indépendance');

COMMIT;
