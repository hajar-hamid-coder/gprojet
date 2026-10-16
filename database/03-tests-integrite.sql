-- =============================================================================
-- Tests d'intégrité du schéma (à exécuter sur une base de TEST après 01 et 02)
-- Chaque bloc « attendu ERREUR » doit échouer ; les autres doivent réussir.
-- psql -d <base_test> -f 03-tests-integrite.sql
-- =============================================================================
\set ON_ERROR_STOP 0
\set VERBOSITY terse
-- Organigramme : 1 direction, 2 divisions, 1 service par division
INSERT INTO org.entite_organisationnelle (id, code, libelle, niveau, parent_id, chemin) VALUES
 ('00000000-0000-7000-8000-000000000001','DSI','Direction SI','DIRECTION',NULL,'d1'),
 ('00000000-0000-7000-8000-000000000002','DEV','Division Dev','DIVISION','00000000-0000-7000-8000-000000000001','d1.dv1'),
 ('00000000-0000-7000-8000-000000000003','INF','Division Infra','DIVISION','00000000-0000-7000-8000-000000000001','d1.dv2'),
 ('00000000-0000-7000-8000-000000000004','SAPP','Service Applis','SERVICE','00000000-0000-7000-8000-000000000002','d1.dv1.s1'),
 ('00000000-0000-7000-8000-000000000005','SRES','Service Reseau','SERVICE','00000000-0000-7000-8000-000000000003','d1.dv2.s2');

INSERT INTO org.agent (id, matricule, nom, prenom, email, entite_id) VALUES
 ('00000000-0000-7000-8000-0000000000a1','M1','Dir','A','dir@x.ma','00000000-0000-7000-8000-000000000001'),
 ('00000000-0000-7000-8000-0000000000a2','M2','ChefDiv','B','div@x.ma','00000000-0000-7000-8000-000000000002'),
 ('00000000-0000-7000-8000-0000000000a3','M3','ChefSrv','C','srv@x.ma','00000000-0000-7000-8000-000000000004'),
 ('00000000-0000-7000-8000-0000000000a4','M4','Agent','D','agt@x.ma','00000000-0000-7000-8000-000000000004');
UPDATE org.entite_organisationnelle SET responsable_id='00000000-0000-7000-8000-0000000000a1' WHERE code='DSI';

\echo '--- T1 attendu ERREUR : email en double (casse differente)'
INSERT INTO org.agent (matricule, nom, prenom, email, entite_id) VALUES ('M9','X','Y','DIR@x.ma','00000000-0000-7000-8000-000000000001');

-- Deux projets : un dans le service Applis (division Dev), un dans le service Reseau (division Infra)
INSERT INTO projet.projet (id, code, intitule, type_projet_id, createur_id, entite_creatrice_id, entite_porteuse_id, chemin_entite)
SELECT v.id::uuid, v.code, v.intitule, tp.id, '00000000-0000-7000-8000-0000000000a1', '00000000-0000-7000-8000-000000000001', v.porteuse::uuid, v.chemin::ltree
FROM projet.type_projet tp,
 (VALUES ('00000000-0000-7000-8000-0000000000b1','PRJ-2026-0001','Portail citoyen','00000000-0000-7000-8000-000000000004','d1.dv1.s1'),
         ('00000000-0000-7000-8000-0000000000b2','PRJ-2026-0002','Refonte reseau','00000000-0000-7000-8000-000000000005','d1.dv2.s2')) v(id,code,intitule,porteuse,chemin)
WHERE tp.code='DEV';

\echo '--- T2 perimetre : directeur (d1) attend 2, chef division Dev (d1.dv1) attend 1, chef service Reseau attend 1'
SELECT 'directeur' qui, count(*) FROM projet.projet WHERE chemin_entite <@ 'd1'
UNION ALL SELECT 'chef_div_dev', count(*) FROM projet.projet WHERE chemin_entite <@ 'd1.dv1'
UNION ALL SELECT 'chef_srv_reseau', count(*) FROM projet.projet WHERE chemin_entite <@ 'd1.dv2.s2';

-- Affectation en cascade
INSERT INTO projet.affectation_projet (projet_id, entite_source_id, entite_cible_id, niveau_cible, affecte_par_id)
VALUES ('00000000-0000-7000-8000-0000000000b1','00000000-0000-7000-8000-000000000001','00000000-0000-7000-8000-000000000002','DIVISION','00000000-0000-7000-8000-0000000000a1');
\echo '--- T3 attendu ERREUR : deux affectations actives pour le meme projet'
INSERT INTO projet.affectation_projet (projet_id, entite_source_id, entite_cible_id, niveau_cible, affecte_par_id)
VALUES ('00000000-0000-7000-8000-0000000000b1','00000000-0000-7000-8000-000000000002','00000000-0000-7000-8000-000000000004','SERVICE','00000000-0000-7000-8000-0000000000a2');
\echo '--- T4 attendu ERREUR : renvoi sans motif'
UPDATE projet.affectation_projet SET statut='RENVOYEE' WHERE projet_id='00000000-0000-7000-8000-0000000000b1';

-- Phase, taches, dependances
INSERT INTO planif.phase (id, projet_id, nom, ordre) VALUES ('00000000-0000-7000-8000-0000000000c1','00000000-0000-7000-8000-0000000000b1','Conception',1);
INSERT INTO planif.tache (id, phase_id, projet_id, code_wbs, titre, ordre, statut_id)
SELECT v.id::uuid, '00000000-0000-7000-8000-0000000000c1', '00000000-0000-7000-8000-0000000000b1', v.wbs, v.titre, v.ordre, s.id
FROM planif.statut_workflow s JOIN planif.workflow w ON w.id=s.workflow_id AND w.code='WF_TACHE' AND s.est_initial,
 (VALUES ('00000000-0000-7000-8000-0000000000d1','1.1','MCD',1),('00000000-0000-7000-8000-0000000000d2','1.2','MLD',2)) v(id,wbs,titre,ordre);
INSERT INTO planif.dependance_tache (predecesseur_id, successeur_id) VALUES ('00000000-0000-7000-8000-0000000000d1','00000000-0000-7000-8000-0000000000d2');
\echo '--- T5 attendu ERREUR : dependance d une tache sur elle-meme'
INSERT INTO planif.dependance_tache (predecesseur_id, successeur_id) VALUES ('00000000-0000-7000-8000-0000000000d1','00000000-0000-7000-8000-0000000000d1');
\echo '--- T6 attendu ERREUR : charge optimiste > estimee'
UPDATE planif.tache SET charge_estimee_h=10, charge_optimiste_h=12 WHERE code_wbs='1.1';

-- Numerotation atomique des tickets
\echo '--- T7 numero de ticket attendu PRJ-2026-0001-T1'
WITH n AS (UPDATE projet.projet SET prochain_numero_ticket = prochain_numero_ticket + 1
           WHERE id='00000000-0000-7000-8000-0000000000b1' RETURNING code, prochain_numero_ticket - 1 AS num)
INSERT INTO planif.ticket (tache_id, projet_id, numero, type_ticket_id, statut_id, titre, rapporteur_id)
SELECT '00000000-0000-7000-8000-0000000000d1','00000000-0000-7000-8000-0000000000b1', n.code || '-T' || n.num, tt.id, s.id, 'Saisir les entites', '00000000-0000-7000-8000-0000000000a3'
FROM n, planif.type_ticket tt JOIN planif.statut_workflow s ON s.workflow_id = tt.workflow_id AND s.est_initial
WHERE tt.code='STORY'
RETURNING numero;

-- Temps
INSERT INTO temps.feuille_temps (id, agent_id, semaine_debut) VALUES ('00000000-0000-7000-8000-0000000000e1','00000000-0000-7000-8000-0000000000a4','2026-10-12');
\echo '--- T8 attendu ERREUR : semaine ne commencant pas un lundi'
INSERT INTO temps.feuille_temps (agent_id, semaine_debut) VALUES ('00000000-0000-7000-8000-0000000000a4','2026-10-14');
\echo '--- T9 attendu ERREUR : ticket sans tache'
INSERT INTO temps.saisie_temps (feuille_id, agent_id, date_saisie, duree_h, categorie_id, ticket_id)
SELECT '00000000-0000-7000-8000-0000000000e1','00000000-0000-7000-8000-0000000000a4','2026-10-12',2,c.id,t.id FROM temps.categorie_activite c, planif.ticket t WHERE c.code='DEVELOPPEMENT';
INSERT INTO temps.saisie_temps (feuille_id, agent_id, date_saisie, duree_h, categorie_id, projet_id, tache_id, ticket_id)
SELECT '00000000-0000-7000-8000-0000000000e1','00000000-0000-7000-8000-0000000000a4','2026-10-12',2,c.id,t.projet_id,t.tache_id,t.id FROM temps.categorie_activite c, planif.ticket t WHERE c.code='DEVELOPPEMENT';
INSERT INTO temps.saisie_temps (feuille_id, agent_id, date_saisie, duree_h, categorie_id)
SELECT '00000000-0000-7000-8000-0000000000e1','00000000-0000-7000-8000-0000000000a4','2026-10-13',1.5,c.id FROM temps.categorie_activite c WHERE c.code='FORMATION';
\echo '--- T10 jours-homme consommes par division (attendu Division Dev 0.25 projet ; hors projet 0.19)'
SELECT e.libelle, round(sum(s.duree_h) FILTER (WHERE s.projet_id IS NOT NULL)/8.0,2) jh_projet,
       round(sum(s.duree_h) FILTER (WHERE s.projet_id IS NULL)/8.0,2) jh_hors_projet
FROM temps.saisie_temps s JOIN org.agent a ON a.id=s.agent_id
JOIN org.entite_organisationnelle sv ON sv.id=a.entite_id
JOIN org.entite_organisationnelle e ON e.niveau='DIVISION' AND sv.chemin <@ e.chemin
GROUP BY e.libelle;

-- Journal d'audit
INSERT INTO commun.historique (type_cible, cible_id, action, empreinte) VALUES ('PROJET','00000000-0000-7000-8000-0000000000b1','CREATION', repeat('a',64));
\echo '--- T11 attendu ERREUR : modification du journal interdite'
UPDATE commun.historique SET action='SUPPRESSION';
\echo '--- T12 attendu ERREUR : suppression du journal interdite'
DELETE FROM commun.historique;

-- Metamodele
INSERT INTO modelisation.modele_domaine (id, projet_id, nom, namespace_racine) VALUES ('00000000-0000-7000-8000-0000000000f1','00000000-0000-7000-8000-0000000000b1','Portail','PC');
INSERT INTO modelisation.module (id, modele_id, code, nom, schema_bd, namespace) VALUES ('00000000-0000-7000-8000-0000000000f2','00000000-0000-7000-8000-0000000000f1','Demandes','Demandes','demande','Demandes');
INSERT INTO modelisation.entite (id, module_id, nom, nom_technique, nom_pluriel) VALUES ('00000000-0000-7000-8000-0000000000f3','00000000-0000-7000-8000-0000000000f2','Demande','demande','Demandes');
\echo '--- T13 attendu ERREUR : attribut rattache a la fois a une entite et une association'
INSERT INTO modelisation.association (id, modele_id, nom, nom_technique) VALUES ('00000000-0000-7000-8000-0000000000f4','00000000-0000-7000-8000-0000000000f1','concerne','concerne');
INSERT INTO modelisation.attribut (entite_id, association_id, nom, nom_technique, type_logique) VALUES ('00000000-0000-7000-8000-0000000000f3','00000000-0000-7000-8000-0000000000f4','X','x','TEXTE');
\echo '--- T14 attendu ERREUR : type ENUM sans enumeration'
INSERT INTO modelisation.attribut (entite_id, nom, nom_technique, type_logique) VALUES ('00000000-0000-7000-8000-0000000000f3','Statut','statut','ENUM');
\echo '--- T15 attendu ERREUR : nom technique invalide'
INSERT INTO modelisation.attribut (entite_id, nom, nom_technique, type_logique) VALUES ('00000000-0000-7000-8000-0000000000f3','Nom','NomCitoyen','TEXTE');
\echo '--- T16 attendu OK : attribut valide'
INSERT INTO modelisation.attribut (entite_id, nom, nom_technique, type_logique, longueur, est_obligatoire) VALUES ('00000000-0000-7000-8000-0000000000f3','Objet','objet','TEXTE',200,true) RETURNING nom_technique;
\echo '--- T17 uuidv7 par defaut (version 7 attendue)'
SELECT uuid_extract_version(id) FROM modelisation.attribut LIMIT 1;
\echo '--- T18 xmin disponible pour la concurrence optimiste'
SELECT xmin IS NOT NULL AS xmin_ok FROM projet.projet LIMIT 1;
