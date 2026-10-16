-- =============================================================================
-- Plateforme de gestion des projets IT — Schéma physique (MPD) PostgreSQL 18
-- Source : Documentation/03-mcd.md + Documentation/04-mld.md
--
-- Conventions
--   * PK `id uuid` par défaut uuidv7() (natif PostgreSQL 18)
--   * Colonnes d'audit : cree_le, cree_par, modifie_le, modifie_par
--   * Suppression logique : est_supprime, supprime_le, supprime_par
--   * Concurrence optimiste : colonne système xmin (aucune colonne à créer)
--   * Énumérations : varchar + CHECK (règle R9, stockage TEXTE)
--   * Unicité « métier » : index uniques partiels WHERE NOT est_supprime
--   * cree_par / modifie_par / supprime_par : uuid d'agent SANS clé étrangère
--     (compte système, import, et pour ne pas bloquer l'archivage)
--
-- Ce script est la référence de conception. En développement, la base sera
-- créée par les migrations EF Core, qui doivent produire un schéma identique.
-- =============================================================================

CREATE EXTENSION IF NOT EXISTS ltree;

CREATE SCHEMA IF NOT EXISTS org;
CREATE SCHEMA IF NOT EXISTS projet;
CREATE SCHEMA IF NOT EXISTS planif;
CREATE SCHEMA IF NOT EXISTS temps;
CREATE SCHEMA IF NOT EXISTS commun;
CREATE SCHEMA IF NOT EXISTS modelisation;

-- =============================================================================
-- SCHÉMA org — Organisation hiérarchique
-- =============================================================================

CREATE TABLE org.entite_organisationnelle (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    code            varchar(30)  NOT NULL,
    libelle         varchar(200) NOT NULL,
    sigle           varchar(30),
    niveau          varchar(20)  NOT NULL CHECK (niveau IN ('DIRECTION','DIVISION','SERVICE')),
    parent_id       uuid REFERENCES org.entite_organisationnelle(id) ON DELETE RESTRICT,
    responsable_id  uuid,                       -- FK vers org.agent ajoutée plus bas (référence circulaire)
    chemin          ltree NOT NULL,             -- ids sans tirets, ex: 0199a1...b2.0199a1...c3
    actif           boolean NOT NULL DEFAULT true,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid,
    CONSTRAINT ck_entite_parent_different CHECK (parent_id IS NULL OR parent_id <> id)
);
CREATE UNIQUE INDEX ux_entite_code        ON org.entite_organisationnelle (code) WHERE NOT est_supprime;
CREATE UNIQUE INDEX ux_entite_responsable ON org.entite_organisationnelle (responsable_id) WHERE responsable_id IS NOT NULL AND NOT est_supprime;
CREATE INDEX ix_entite_parent             ON org.entite_organisationnelle (parent_id);
CREATE INDEX ix_entite_chemin             ON org.entite_organisationnelle USING gist (chemin);

CREATE TABLE org.fonction (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    code            varchar(30)  NOT NULL,
    libelle         varchar(150) NOT NULL,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE UNIQUE INDEX ux_fonction_code ON org.fonction (code) WHERE NOT est_supprime;

CREATE TABLE org.competence (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    code            varchar(30)  NOT NULL,
    libelle         varchar(150) NOT NULL,
    categorie       varchar(100),
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE UNIQUE INDEX ux_competence_code ON org.competence (code) WHERE NOT est_supprime;

CREATE TABLE org.agent (
    id                uuid PRIMARY KEY DEFAULT uuidv7(),
    matricule         varchar(30)  NOT NULL,
    nom               varchar(100) NOT NULL,
    prenom            varchar(100) NOT NULL,
    email             varchar(254) NOT NULL,
    telephone         varchar(30),
    entite_id         uuid NOT NULL REFERENCES org.entite_organisationnelle(id) ON DELETE RESTRICT, -- rattachement actuel
    fonction_id       uuid REFERENCES org.fonction(id) ON DELETE SET NULL,
    capacite_hebdo_h  numeric(5,2) NOT NULL DEFAULT 40 CHECK (capacite_hebdo_h BETWEEN 0 AND 80),
    keycloak_sub      varchar(100),            -- renseigné à l'intégration Keycloak
    actif             boolean NOT NULL DEFAULT true,
    cree_le           timestamptz NOT NULL DEFAULT now(),
    cree_par          uuid,
    modifie_le        timestamptz,
    modifie_par       uuid,
    est_supprime      boolean NOT NULL DEFAULT false,
    supprime_le       timestamptz,
    supprime_par      uuid
);
CREATE UNIQUE INDEX ux_agent_matricule ON org.agent (matricule)     WHERE NOT est_supprime;
CREATE UNIQUE INDEX ux_agent_email     ON org.agent (lower(email))  WHERE NOT est_supprime;
CREATE UNIQUE INDEX ux_agent_keycloak  ON org.agent (keycloak_sub)  WHERE keycloak_sub IS NOT NULL;
CREATE INDEX ix_agent_entite           ON org.agent (entite_id);
CREATE INDEX ix_agent_fonction         ON org.agent (fonction_id);

ALTER TABLE org.entite_organisationnelle
    ADD CONSTRAINT fk_entite_responsable FOREIGN KEY (responsable_id) REFERENCES org.agent(id) ON DELETE SET NULL;

CREATE TABLE org.rattachement (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    agent_id        uuid NOT NULL REFERENCES org.agent(id) ON DELETE CASCADE,
    entite_id       uuid NOT NULL REFERENCES org.entite_organisationnelle(id) ON DELETE RESTRICT,
    date_debut      date NOT NULL,
    date_fin        date,                       -- NULL = rattachement en cours
    motif           varchar(200),
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid,
    CONSTRAINT ck_rattachement_dates CHECK (date_fin IS NULL OR date_fin >= date_debut)
);
CREATE UNIQUE INDEX ux_rattachement_en_cours ON org.rattachement (agent_id) WHERE date_fin IS NULL AND NOT est_supprime;
CREATE INDEX ix_rattachement_entite          ON org.rattachement (entite_id);

CREATE TABLE org.agent_competence (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    agent_id        uuid NOT NULL REFERENCES org.agent(id) ON DELETE CASCADE,
    competence_id   uuid NOT NULL REFERENCES org.competence(id) ON DELETE CASCADE,
    niveau          smallint NOT NULL CHECK (niveau BETWEEN 1 AND 5),
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE UNIQUE INDEX ux_agent_competence ON org.agent_competence (agent_id, competence_id) WHERE NOT est_supprime;
CREATE INDEX ix_agent_competence_comp   ON org.agent_competence (competence_id);

CREATE TABLE org.delegation (
    id                 uuid PRIMARY KEY DEFAULT uuidv7(),
    delegant_id        uuid NOT NULL REFERENCES org.agent(id) ON DELETE RESTRICT,
    delegataire_id     uuid NOT NULL REFERENCES org.agent(id) ON DELETE RESTRICT,
    entite_id          uuid NOT NULL REFERENCES org.entite_organisationnelle(id) ON DELETE RESTRICT,
    date_debut         date NOT NULL,
    date_fin           date NOT NULL,
    motif              varchar(200),
    peut_creer_projet  boolean NOT NULL DEFAULT false,
    peut_affecter      boolean NOT NULL DEFAULT false,
    peut_valider       boolean NOT NULL DEFAULT false,
    cree_le            timestamptz NOT NULL DEFAULT now(),
    cree_par           uuid,
    modifie_le         timestamptz,
    modifie_par        uuid,
    est_supprime       boolean NOT NULL DEFAULT false,
    supprime_le        timestamptz,
    supprime_par       uuid,
    CONSTRAINT ck_delegation_agents CHECK (delegant_id <> delegataire_id),
    CONSTRAINT ck_delegation_dates  CHECK (date_fin >= date_debut)
);
CREATE INDEX ix_delegation_delegataire ON org.delegation (delegataire_id, date_debut, date_fin);
CREATE INDEX ix_delegation_delegant    ON org.delegation (delegant_id);
CREATE INDEX ix_delegation_entite      ON org.delegation (entite_id);

CREATE TABLE org.absence (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    agent_id        uuid NOT NULL REFERENCES org.agent(id) ON DELETE CASCADE,
    date_debut      date NOT NULL,
    date_fin        date NOT NULL,
    type            varchar(20) NOT NULL CHECK (type IN ('CONGE','MALADIE','FORMATION','MISSION','AUTRE')),
    demi_journee    boolean NOT NULL DEFAULT false,
    commentaire     varchar(300),
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid,
    CONSTRAINT ck_absence_dates CHECK (date_fin >= date_debut)
);
CREATE INDEX ix_absence_agent_dates ON org.absence (agent_id, date_debut, date_fin);

CREATE TABLE org.jour_ferie (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    date_ferie      date NOT NULL,
    libelle         varchar(150) NOT NULL,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE UNIQUE INDEX ux_jour_ferie_date ON org.jour_ferie (date_ferie) WHERE NOT est_supprime;

-- =============================================================================
-- SCHÉMA projet — Projets, affectation en cascade, portefeuille
-- =============================================================================

CREATE TABLE projet.type_projet (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    code            varchar(30)  NOT NULL,
    libelle         varchar(150) NOT NULL,
    description     text,
    actif           boolean NOT NULL DEFAULT true,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE UNIQUE INDEX ux_type_projet_code ON projet.type_projet (code) WHERE NOT est_supprime;

CREATE TABLE projet.modele_phase (
    id                     uuid PRIMARY KEY DEFAULT uuidv7(),
    type_projet_id         uuid NOT NULL REFERENCES projet.type_projet(id) ON DELETE CASCADE,
    ordre                  smallint NOT NULL CHECK (ordre > 0),
    nom                    varchar(150) NOT NULL,
    objectif               text,
    criteres_entree        text,
    criteres_sortie        text,
    mode_enchainement      varchar(20) NOT NULL DEFAULT 'SEQUENTIEL' CHECK (mode_enchainement IN ('SEQUENTIEL','CHEVAUCHEMENT','PARALLELE')),
    pct_min_chevauchement  numeric(5,2) CHECK (pct_min_chevauchement BETWEEN 0 AND 100),
    cree_le                timestamptz NOT NULL DEFAULT now(),
    cree_par               uuid,
    modifie_le             timestamptz,
    modifie_par            uuid,
    est_supprime           boolean NOT NULL DEFAULT false,
    supprime_le            timestamptz,
    supprime_par           uuid
);
CREATE UNIQUE INDEX ux_modele_phase_ordre ON projet.modele_phase (type_projet_id, ordre) WHERE NOT est_supprime;

CREATE TABLE projet.projet (
    id                      uuid PRIMARY KEY DEFAULT uuidv7(),
    code                    varchar(30)  NOT NULL,              -- PRJ-2026-0042
    intitule                varchar(250) NOT NULL,
    description             text,
    objectifs               text,
    perimetre               text,
    type_projet_id          uuid NOT NULL REFERENCES projet.type_projet(id) ON DELETE RESTRICT,
    priorite                varchar(20) NOT NULL DEFAULT 'NORMALE' CHECK (priorite IN ('BASSE','NORMALE','HAUTE','CRITIQUE')),
    statut                  varchar(20) NOT NULL DEFAULT 'BROUILLON' CHECK (statut IN ('BROUILLON','EN_AFFECTATION','EN_COURS','SUSPENDU','CLOTURE','ARCHIVE')),
    meteo                   varchar(10) NOT NULL DEFAULT 'VERT' CHECK (meteo IN ('VERT','ORANGE','ROUGE')),
    classification          varchar(30) NOT NULL DEFAULT 'INTERNE' CHECK (classification IN ('PUBLIC','INTERNE','CONFIDENTIEL','DIFFUSION_RESTREINTE')),
    mode_ordonnancement     varchar(10) NOT NULL DEFAULT 'BLOQUANT' CHECK (mode_ordonnancement IN ('BLOQUANT','SOUPLE')),
    createur_id             uuid NOT NULL REFERENCES org.agent(id) ON DELETE RESTRICT,
    entite_creatrice_id     uuid NOT NULL REFERENCES org.entite_organisationnelle(id) ON DELETE RESTRICT,
    entite_porteuse_id      uuid REFERENCES org.entite_organisationnelle(id) ON DELETE RESTRICT,  -- dernière entité ayant accepté
    chef_projet_id          uuid REFERENCES org.agent(id) ON DELETE SET NULL,
    chemin_entite           ltree NOT NULL,                     -- chemin de l'entité porteuse (ou créatrice) : filtre de périmètre
    date_debut_prevue       date,
    date_fin_prevue         date,
    date_debut_reelle       date,
    date_fin_reelle         date,
    charge_estimee_jh       numeric(10,2) NOT NULL DEFAULT 0,   -- caches recalculés par les services
    charge_consommee_jh     numeric(10,2) NOT NULL DEFAULT 0,
    avancement_pct          numeric(5,2)  NOT NULL DEFAULT 0 CHECK (avancement_pct BETWEEN 0 AND 100),
    prochain_numero_ticket  integer NOT NULL DEFAULT 1,         -- compteur atomique : UPDATE ... RETURNING
    cree_le                 timestamptz NOT NULL DEFAULT now(),
    cree_par                uuid,
    modifie_le              timestamptz,
    modifie_par             uuid,
    est_supprime            boolean NOT NULL DEFAULT false,
    supprime_le             timestamptz,
    supprime_par            uuid,
    CONSTRAINT ck_projet_dates_prevues CHECK (date_fin_prevue IS NULL OR date_debut_prevue IS NULL OR date_fin_prevue >= date_debut_prevue),
    CONSTRAINT ck_projet_dates_reelles CHECK (date_fin_reelle IS NULL OR date_debut_reelle IS NULL OR date_fin_reelle >= date_debut_reelle)
);
CREATE UNIQUE INDEX ux_projet_code         ON projet.projet (code) WHERE NOT est_supprime;
CREATE INDEX ix_projet_chemin              ON projet.projet USING gist (chemin_entite);
CREATE INDEX ix_projet_statut              ON projet.projet (statut) WHERE NOT est_supprime;
CREATE INDEX ix_projet_type                ON projet.projet (type_projet_id);
CREATE INDEX ix_projet_createur            ON projet.projet (createur_id);
CREATE INDEX ix_projet_entite_creatrice    ON projet.projet (entite_creatrice_id);
CREATE INDEX ix_projet_entite_porteuse     ON projet.projet (entite_porteuse_id);
CREATE INDEX ix_projet_chef                ON projet.projet (chef_projet_id);

CREATE TABLE projet.affectation_projet (
    id                  uuid PRIMARY KEY DEFAULT uuidv7(),
    projet_id           uuid NOT NULL REFERENCES projet.projet(id) ON DELETE CASCADE,
    precedente_id       uuid REFERENCES projet.affectation_projet(id) ON DELETE SET NULL,
    entite_source_id    uuid NOT NULL REFERENCES org.entite_organisationnelle(id) ON DELETE RESTRICT,
    entite_cible_id     uuid NOT NULL REFERENCES org.entite_organisationnelle(id) ON DELETE RESTRICT,
    niveau_cible        varchar(20) NOT NULL CHECK (niveau_cible IN ('DIVISION','SERVICE')),
    statut              varchar(20) NOT NULL DEFAULT 'EN_ATTENTE' CHECK (statut IN ('EN_ATTENTE','ACCEPTEE','RENVOYEE','ANNULEE')),
    instructions        text,
    echeance_souhaitee  date,
    charge_prevue_jh    numeric(10,2),
    affecte_par_id      uuid NOT NULL REFERENCES org.agent(id) ON DELETE RESTRICT,
    date_affectation    timestamptz NOT NULL DEFAULT now(),
    repondu_par_id      uuid REFERENCES org.agent(id) ON DELETE RESTRICT,
    date_reponse        timestamptz,
    motif_reponse       text,
    est_active          boolean NOT NULL DEFAULT true,
    cree_le             timestamptz NOT NULL DEFAULT now(),
    cree_par            uuid,
    modifie_le          timestamptz,
    modifie_par         uuid,
    est_supprime        boolean NOT NULL DEFAULT false,
    supprime_le         timestamptz,
    supprime_par        uuid,
    CONSTRAINT ck_affectation_entites CHECK (entite_source_id <> entite_cible_id),
    CONSTRAINT ck_affectation_motif   CHECK (statut <> 'RENVOYEE' OR motif_reponse IS NOT NULL)
);
CREATE UNIQUE INDEX ux_affectation_active ON projet.affectation_projet (projet_id) WHERE est_active AND NOT est_supprime;
CREATE INDEX ix_affectation_cible_statut  ON projet.affectation_projet (entite_cible_id, statut);   -- file « Projets à affecter »
CREATE INDEX ix_affectation_source        ON projet.affectation_projet (entite_source_id);
CREATE INDEX ix_affectation_precedente    ON projet.affectation_projet (precedente_id);
CREATE INDEX ix_affectation_affecte_par   ON projet.affectation_projet (affecte_par_id);
CREATE INDEX ix_affectation_repondu_par   ON projet.affectation_projet (repondu_par_id);

CREATE TABLE projet.membre_projet (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    projet_id       uuid NOT NULL REFERENCES projet.projet(id) ON DELETE CASCADE,
    agent_id        uuid NOT NULL REFERENCES org.agent(id) ON DELETE RESTRICT,
    role_projet     varchar(20) NOT NULL DEFAULT 'MEMBRE' CHECK (role_projet IN ('CHEF_PROJET','MEMBRE','OBSERVATEUR')),
    quota_pct       numeric(5,2) NOT NULL DEFAULT 100 CHECK (quota_pct BETWEEN 0 AND 100),
    date_debut      date,
    date_fin        date,
    affecte_par_id  uuid REFERENCES org.agent(id) ON DELETE RESTRICT,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid,
    CONSTRAINT ck_membre_dates CHECK (date_fin IS NULL OR date_debut IS NULL OR date_fin >= date_debut)
);
CREATE UNIQUE INDEX ux_membre_projet_agent ON projet.membre_projet (projet_id, agent_id) WHERE NOT est_supprime;
CREATE INDEX ix_membre_agent               ON projet.membre_projet (agent_id);
CREATE INDEX ix_membre_affecte_par         ON projet.membre_projet (affecte_par_id);

CREATE TABLE projet.jalon (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    projet_id       uuid NOT NULL REFERENCES projet.projet(id) ON DELETE CASCADE,
    phase_id        uuid,                       -- FK vers planif.phase ajoutée plus bas
    libelle         varchar(200) NOT NULL,
    date_prevue     date NOT NULL,
    date_reelle     date,
    statut          varchar(20) NOT NULL DEFAULT 'A_VENIR' CHECK (statut IN ('A_VENIR','ATTEINT','EN_RETARD','ANNULE')),
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE INDEX ix_jalon_projet_date ON projet.jalon (projet_id, date_prevue);
CREATE INDEX ix_jalon_phase       ON projet.jalon (phase_id);

CREATE TABLE projet.element_raid (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    projet_id       uuid NOT NULL REFERENCES projet.projet(id) ON DELETE CASCADE,
    type            varchar(20) NOT NULL CHECK (type IN ('RISQUE','ACTION','PROBLEME','DECISION')),
    titre           varchar(250) NOT NULL,
    description     text,
    probabilite     smallint CHECK (probabilite BETWEEN 1 AND 5),
    impact          smallint CHECK (impact BETWEEN 1 AND 5),
    criticite       smallint GENERATED ALWAYS AS (probabilite * impact) STORED,
    statut          varchar(20) NOT NULL DEFAULT 'OUVERT' CHECK (statut IN ('OUVERT','EN_COURS','CLOS','ANNULE')),
    plan_action     text,
    echeance        date,
    responsable_id  uuid REFERENCES org.agent(id) ON DELETE SET NULL,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid,
    CONSTRAINT ck_raid_risque CHECK (type <> 'RISQUE' OR (probabilite IS NOT NULL AND impact IS NOT NULL))
);
CREATE INDEX ix_raid_projet_type ON projet.element_raid (projet_id, type, statut);
CREATE INDEX ix_raid_responsable ON projet.element_raid (responsable_id);

-- =============================================================================
-- SCHÉMA planif — Phases, tâches, tickets, workflows, ordonnancement
-- =============================================================================

CREATE TABLE planif.workflow (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    code            varchar(30)  NOT NULL,
    libelle         varchar(150) NOT NULL,
    type_element    varchar(10)  NOT NULL CHECK (type_element IN ('TACHE','TICKET')),
    est_defaut      boolean NOT NULL DEFAULT false,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE UNIQUE INDEX ux_workflow_code   ON planif.workflow (code) WHERE NOT est_supprime;
CREATE UNIQUE INDEX ux_workflow_defaut ON planif.workflow (type_element) WHERE est_defaut AND NOT est_supprime;

CREATE TABLE planif.statut_workflow (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    workflow_id     uuid NOT NULL REFERENCES planif.workflow(id) ON DELETE CASCADE,
    code            varchar(30)  NOT NULL,
    libelle         varchar(100) NOT NULL,
    categorie       varchar(10)  NOT NULL CHECK (categorie IN ('A_FAIRE','EN_COURS','TERMINE')),
    couleur         varchar(7)   CHECK (couleur ~ '^#[0-9A-Fa-f]{6}$'),
    ordre           smallint NOT NULL,
    est_initial     boolean NOT NULL DEFAULT false,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE UNIQUE INDEX ux_statut_workflow_code    ON planif.statut_workflow (workflow_id, code) WHERE NOT est_supprime;
CREATE UNIQUE INDEX ux_statut_workflow_initial ON planif.statut_workflow (workflow_id) WHERE est_initial AND NOT est_supprime;

CREATE TABLE planif.transition (
    id                uuid PRIMARY KEY DEFAULT uuidv7(),
    workflow_id       uuid NOT NULL REFERENCES planif.workflow(id) ON DELETE CASCADE,
    statut_depuis_id  uuid NOT NULL REFERENCES planif.statut_workflow(id) ON DELETE CASCADE,
    statut_vers_id    uuid NOT NULL REFERENCES planif.statut_workflow(id) ON DELETE CASCADE,
    libelle           varchar(100),
    roles_autorises   text[] NOT NULL DEFAULT '{}',   -- vide = tous les rôles ayant accès au projet
    cree_le           timestamptz NOT NULL DEFAULT now(),
    cree_par          uuid,
    modifie_le        timestamptz,
    modifie_par       uuid,
    est_supprime      boolean NOT NULL DEFAULT false,
    supprime_le       timestamptz,
    supprime_par      uuid,
    CONSTRAINT ck_transition_statuts CHECK (statut_depuis_id <> statut_vers_id)
);
CREATE UNIQUE INDEX ux_transition ON planif.transition (statut_depuis_id, statut_vers_id) WHERE NOT est_supprime;
CREATE INDEX ix_transition_workflow ON planif.transition (workflow_id);
CREATE INDEX ix_transition_vers     ON planif.transition (statut_vers_id);

CREATE TABLE planif.type_ticket (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    code            varchar(30)  NOT NULL,
    libelle         varchar(100) NOT NULL,
    icone           varchar(50),
    workflow_id     uuid NOT NULL REFERENCES planif.workflow(id) ON DELETE RESTRICT,
    actif           boolean NOT NULL DEFAULT true,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE UNIQUE INDEX ux_type_ticket_code ON planif.type_ticket (code) WHERE NOT est_supprime;
CREATE INDEX ix_type_ticket_workflow    ON planif.type_ticket (workflow_id);

CREATE TABLE planif.phase (
    id                     uuid PRIMARY KEY DEFAULT uuidv7(),
    projet_id              uuid NOT NULL REFERENCES projet.projet(id) ON DELETE CASCADE,
    code                   varchar(30),
    nom                    varchar(150) NOT NULL,
    objectif               text,
    ordre                  smallint NOT NULL CHECK (ordre > 0),   -- unicité garantie par le service (renumérotation)
    statut                 varchar(20) NOT NULL DEFAULT 'A_VENIR' CHECK (statut IN ('A_VENIR','EN_COURS','EN_VALIDATION','VALIDEE','GELEE')),
    mode_enchainement      varchar(20) NOT NULL DEFAULT 'SEQUENTIEL' CHECK (mode_enchainement IN ('SEQUENTIEL','CHEVAUCHEMENT','PARALLELE')),
    pct_min_chevauchement  numeric(5,2) CHECK (pct_min_chevauchement BETWEEN 0 AND 100),
    criteres_entree        text,
    criteres_sortie        text,
    date_debut_prevue      date,
    date_fin_prevue        date,
    date_debut_reelle      date,
    date_fin_reelle        date,
    charge_estimee_h       numeric(10,2) NOT NULL DEFAULT 0,
    avancement_pct         numeric(5,2)  NOT NULL DEFAULT 0 CHECK (avancement_pct BETWEEN 0 AND 100),
    cree_le                timestamptz NOT NULL DEFAULT now(),
    cree_par               uuid,
    modifie_le             timestamptz,
    modifie_par            uuid,
    est_supprime           boolean NOT NULL DEFAULT false,
    supprime_le            timestamptz,
    supprime_par           uuid,
    CONSTRAINT ck_phase_chevauchement CHECK (mode_enchainement <> 'CHEVAUCHEMENT' OR pct_min_chevauchement IS NOT NULL),
    CONSTRAINT ck_phase_dates CHECK (date_fin_prevue IS NULL OR date_debut_prevue IS NULL OR date_fin_prevue >= date_debut_prevue)
);
CREATE INDEX ix_phase_projet_ordre ON planif.phase (projet_id, ordre) WHERE NOT est_supprime;

ALTER TABLE projet.jalon
    ADD CONSTRAINT fk_jalon_phase FOREIGN KEY (phase_id) REFERENCES planif.phase(id) ON DELETE SET NULL;

CREATE TABLE planif.validation_phase (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    phase_id        uuid NOT NULL REFERENCES planif.phase(id) ON DELETE CASCADE,
    valideur_id     uuid NOT NULL REFERENCES org.agent(id) ON DELETE RESTRICT,
    tour            smallint NOT NULL DEFAULT 1,     -- nouvelle soumission après rejet = tour suivant
    ordre_niveau    smallint NOT NULL,
    decision        varchar(15) NOT NULL DEFAULT 'EN_ATTENTE' CHECK (decision IN ('EN_ATTENTE','APPROUVEE','REJETEE')),
    commentaire     text,
    date_decision   timestamptz,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid,
    CONSTRAINT ck_validation_rejet CHECK (decision <> 'REJETEE' OR commentaire IS NOT NULL)
);
CREATE UNIQUE INDEX ux_validation_phase ON planif.validation_phase (phase_id, tour, ordre_niveau) WHERE NOT est_supprime;
CREATE INDEX ix_validation_valideur     ON planif.validation_phase (valideur_id, decision);

CREATE TABLE planif.livrable (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    phase_id        uuid NOT NULL REFERENCES planif.phase(id) ON DELETE CASCADE,
    libelle         varchar(200) NOT NULL,
    description     text,
    statut          varchar(20) NOT NULL DEFAULT 'A_PRODUIRE' CHECK (statut IN ('A_PRODUIRE','EN_COURS','LIVRE','VALIDE','REJETE')),
    date_prevue     date,
    date_livraison  date,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE INDEX ix_livrable_phase ON planif.livrable (phase_id);

CREATE TABLE planif.tache (
    id                   uuid PRIMARY KEY DEFAULT uuidv7(),
    phase_id             uuid NOT NULL REFERENCES planif.phase(id) ON DELETE CASCADE,
    projet_id            uuid NOT NULL REFERENCES projet.projet(id) ON DELETE CASCADE,  -- dénormalisé (périmètre, reporting)
    code_wbs             varchar(20) NOT NULL,          -- 1.3
    titre                varchar(250) NOT NULL,
    description          text,
    ordre                smallint NOT NULL CHECK (ordre > 0),
    mode_execution       varchar(15) NOT NULL DEFAULT 'SEQUENTIELLE' CHECK (mode_execution IN ('SEQUENTIELLE','PARALLELE')),
    priorite             varchar(20) NOT NULL DEFAULT 'NORMALE' CHECK (priorite IN ('BASSE','NORMALE','HAUTE','CRITIQUE')),
    statut_id            uuid NOT NULL REFERENCES planif.statut_workflow(id) ON DELETE RESTRICT,
    responsable_id       uuid REFERENCES org.agent(id) ON DELETE SET NULL,
    date_debut_prevue    date,
    date_fin_prevue      date,
    date_debut_reelle    date,
    date_fin_reelle      date,
    charge_estimee_h     numeric(8,2),
    charge_optimiste_h   numeric(8,2),
    charge_pessimiste_h  numeric(8,2),
    reste_a_faire_h      numeric(8,2),
    avancement_pct       numeric(5,2) NOT NULL DEFAULT 0 CHECK (avancement_pct BETWEEN 0 AND 100),
    marge_totale_j       numeric(6,2),                  -- calculées par le CPM
    marge_libre_j        numeric(6,2),
    est_critique         boolean NOT NULL DEFAULT false,
    est_recurrente       boolean NOT NULL DEFAULT false,
    regle_recurrence     varchar(100),                  -- format RRULE (RFC 5545)
    cree_le              timestamptz NOT NULL DEFAULT now(),
    cree_par             uuid,
    modifie_le           timestamptz,
    modifie_par          uuid,
    est_supprime         boolean NOT NULL DEFAULT false,
    supprime_le          timestamptz,
    supprime_par         uuid,
    CONSTRAINT ck_tache_charges CHECK (
        (charge_optimiste_h  IS NULL OR charge_estimee_h IS NULL OR charge_optimiste_h <= charge_estimee_h) AND
        (charge_pessimiste_h IS NULL OR charge_estimee_h IS NULL OR charge_pessimiste_h >= charge_estimee_h)),
    CONSTRAINT ck_tache_recurrence CHECK (NOT est_recurrente OR regle_recurrence IS NOT NULL),
    CONSTRAINT ck_tache_dates CHECK (date_fin_prevue IS NULL OR date_debut_prevue IS NULL OR date_fin_prevue >= date_debut_prevue)
);
CREATE INDEX ix_tache_phase_ordre  ON planif.tache (phase_id, ordre) WHERE NOT est_supprime;
CREATE INDEX ix_tache_projet       ON planif.tache (projet_id);
CREATE INDEX ix_tache_responsable  ON planif.tache (responsable_id) WHERE NOT est_supprime;
CREATE INDEX ix_tache_statut       ON planif.tache (statut_id);

CREATE TABLE planif.dependance_tache (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    predecesseur_id uuid NOT NULL REFERENCES planif.tache(id) ON DELETE CASCADE,
    successeur_id   uuid NOT NULL REFERENCES planif.tache(id) ON DELETE CASCADE,
    type_lien       varchar(15) NOT NULL DEFAULT 'FIN_DEBUT' CHECK (type_lien IN ('FIN_DEBUT','DEBUT_DEBUT','FIN_FIN','DEBUT_FIN')),
    delai_j         numeric(6,2) NOT NULL DEFAULT 0,        -- négatif = avance
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid,
    CONSTRAINT ck_dependance_differente CHECK (predecesseur_id <> successeur_id)
);
-- Les cycles (A→B→C→A) sont détectés par Services/Planification/OrdonnancementService avant insertion.
CREATE UNIQUE INDEX ux_dependance ON planif.dependance_tache (predecesseur_id, successeur_id) WHERE NOT est_supprime;
CREATE INDEX ix_dependance_successeur ON planif.dependance_tache (successeur_id);

CREATE TABLE planif.derogation_ordre (
    id               uuid PRIMARY KEY DEFAULT uuidv7(),
    tache_id         uuid NOT NULL REFERENCES planif.tache(id) ON DELETE CASCADE,
    predecesseur_id  uuid NOT NULL REFERENCES planif.tache(id) ON DELETE CASCADE,
    agent_id         uuid NOT NULL REFERENCES org.agent(id) ON DELETE RESTRICT,
    justification    text NOT NULL,
    date_derogation  timestamptz NOT NULL DEFAULT now(),
    cree_le          timestamptz NOT NULL DEFAULT now(),
    cree_par         uuid,
    modifie_le       timestamptz,
    modifie_par      uuid,
    est_supprime     boolean NOT NULL DEFAULT false,
    supprime_le      timestamptz,
    supprime_par     uuid
);
CREATE INDEX ix_derogation_tache        ON planif.derogation_ordre (tache_id);
CREATE INDEX ix_derogation_predecesseur ON planif.derogation_ordre (predecesseur_id);
CREATE INDEX ix_derogation_agent        ON planif.derogation_ordre (agent_id);

CREATE TABLE planif.ticket (
    id                    uuid PRIMARY KEY DEFAULT uuidv7(),
    tache_id              uuid NOT NULL REFERENCES planif.tache(id) ON DELETE CASCADE,
    projet_id             uuid NOT NULL REFERENCES projet.projet(id) ON DELETE CASCADE,  -- dénormalisé
    numero                varchar(40) NOT NULL,          -- PRJ-2026-0042-T153
    type_ticket_id        uuid NOT NULL REFERENCES planif.type_ticket(id) ON DELETE RESTRICT,
    statut_id             uuid NOT NULL REFERENCES planif.statut_workflow(id) ON DELETE RESTRICT,
    titre                 varchar(250) NOT NULL,
    description           text,
    priorite              varchar(20) NOT NULL DEFAULT 'NORMALE' CHECK (priorite IN ('BASSE','NORMALE','HAUTE','CRITIQUE')),
    severite              varchar(20) CHECK (severite IN ('MINEURE','MAJEURE','CRITIQUE','BLOQUANTE')),
    rang                  integer NOT NULL DEFAULT 0,
    est_bloquant          boolean NOT NULL DEFAULT false,
    environnement         varchar(50),
    etapes_reproduction   text,
    criteres_acceptation  text,
    estimation_h          numeric(8,2),
    reste_a_faire_h       numeric(8,2),
    date_echeance         date,
    assigne_id            uuid REFERENCES org.agent(id) ON DELETE SET NULL,
    rapporteur_id         uuid NOT NULL REFERENCES org.agent(id) ON DELETE RESTRICT,
    cree_le               timestamptz NOT NULL DEFAULT now(),
    cree_par              uuid,
    modifie_le            timestamptz,
    modifie_par           uuid,
    est_supprime          boolean NOT NULL DEFAULT false,
    supprime_le           timestamptz,
    supprime_par          uuid
);
CREATE UNIQUE INDEX ux_ticket_numero ON planif.ticket (numero);   -- jamais réutilisé, même après suppression
CREATE INDEX ix_ticket_tache_rang    ON planif.ticket (tache_id, rang) WHERE NOT est_supprime;
CREATE INDEX ix_ticket_projet        ON planif.ticket (projet_id);
CREATE INDEX ix_ticket_assigne       ON planif.ticket (assigne_id) WHERE NOT est_supprime;
CREATE INDEX ix_ticket_rapporteur    ON planif.ticket (rapporteur_id);
CREATE INDEX ix_ticket_statut        ON planif.ticket (statut_id);
CREATE INDEX ix_ticket_type          ON planif.ticket (type_ticket_id);

CREATE TABLE planif.lien_ticket (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    source_id       uuid NOT NULL REFERENCES planif.ticket(id) ON DELETE CASCADE,
    cible_id        uuid NOT NULL REFERENCES planif.ticket(id) ON DELETE CASCADE,
    type_lien       varchar(15) NOT NULL CHECK (type_lien IN ('BLOQUE','DUPLIQUE','LIE_A','CAUSE')),
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid,
    CONSTRAINT ck_lien_ticket_different CHECK (source_id <> cible_id)
);
CREATE UNIQUE INDEX ux_lien_ticket ON planif.lien_ticket (source_id, cible_id, type_lien) WHERE NOT est_supprime;
CREATE INDEX ix_lien_ticket_cible  ON planif.lien_ticket (cible_id);

CREATE TABLE planif.baseline (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    projet_id       uuid NOT NULL REFERENCES projet.projet(id) ON DELETE CASCADE,
    libelle         varchar(150) NOT NULL,
    date_creation   timestamptz NOT NULL DEFAULT now(),
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE INDEX ix_baseline_projet ON planif.baseline (projet_id);

CREATE TABLE planif.baseline_tache (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    baseline_id     uuid NOT NULL REFERENCES planif.baseline(id) ON DELETE CASCADE,
    tache_id        uuid NOT NULL REFERENCES planif.tache(id) ON DELETE CASCADE,
    date_debut      date,
    date_fin        date,
    charge_h        numeric(8,2),
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE UNIQUE INDEX ux_baseline_tache ON planif.baseline_tache (baseline_id, tache_id) WHERE NOT est_supprime;
CREATE INDEX ix_baseline_tache_tache  ON planif.baseline_tache (tache_id);

-- =============================================================================
-- SCHÉMA temps — Saisie et validation des temps
-- =============================================================================

CREATE TABLE temps.categorie_activite (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    code            varchar(30)  NOT NULL,
    libelle         varchar(100) NOT NULL,
    est_projet      boolean NOT NULL DEFAULT true,    -- false = temps hors projet (réunion de service, formation…)
    actif           boolean NOT NULL DEFAULT true,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE UNIQUE INDEX ux_categorie_activite_code ON temps.categorie_activite (code) WHERE NOT est_supprime;

CREATE TABLE temps.feuille_temps (
    id               uuid PRIMARY KEY DEFAULT uuidv7(),
    agent_id         uuid NOT NULL REFERENCES org.agent(id) ON DELETE RESTRICT,
    semaine_debut    date NOT NULL CHECK (extract(isodow FROM semaine_debut) = 1),   -- un lundi
    statut           varchar(15) NOT NULL DEFAULT 'BROUILLON' CHECK (statut IN ('BROUILLON','SOUMISE','VALIDEE','REJETEE','VERROUILLEE')),
    date_soumission  timestamptz,
    valideur_id      uuid REFERENCES org.agent(id) ON DELETE RESTRICT,
    date_validation  timestamptz,
    motif_rejet      text,
    cree_le          timestamptz NOT NULL DEFAULT now(),
    cree_par         uuid,
    modifie_le       timestamptz,
    modifie_par      uuid,
    est_supprime     boolean NOT NULL DEFAULT false,
    supprime_le      timestamptz,
    supprime_par     uuid,
    CONSTRAINT ck_feuille_rejet CHECK (statut <> 'REJETEE' OR motif_rejet IS NOT NULL)
);
CREATE UNIQUE INDEX ux_feuille_agent_semaine ON temps.feuille_temps (agent_id, semaine_debut) WHERE NOT est_supprime;
CREATE INDEX ix_feuille_valideur_statut      ON temps.feuille_temps (valideur_id, statut);

CREATE TABLE temps.saisie_temps (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    feuille_id      uuid NOT NULL REFERENCES temps.feuille_temps(id) ON DELETE CASCADE,
    agent_id        uuid NOT NULL REFERENCES org.agent(id) ON DELETE RESTRICT,      -- dénormalisé
    date_saisie     date NOT NULL,
    duree_h         numeric(5,2) NOT NULL CHECK (duree_h > 0 AND duree_h <= 24),
    categorie_id    uuid NOT NULL REFERENCES temps.categorie_activite(id) ON DELETE RESTRICT,
    projet_id       uuid REFERENCES projet.projet(id) ON DELETE RESTRICT,           -- dénormalisé
    tache_id        uuid REFERENCES planif.tache(id) ON DELETE RESTRICT,
    ticket_id       uuid REFERENCES planif.ticket(id) ON DELETE RESTRICT,
    commentaire     varchar(500),
    source          varchar(10) NOT NULL DEFAULT 'MANUEL' CHECK (source IN ('MANUEL','CHRONO','GITLAB')),
    chrono_debut    timestamptz,
    chrono_fin      timestamptz,
    reference_gitlab varchar(100),               -- SHA de commit ou IID de merge request
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid,
    -- La règle « categorie.est_projet => tâche obligatoire » est vérifiée par le service (inter-tables).
    CONSTRAINT ck_saisie_ticket_tache CHECK (ticket_id IS NULL OR tache_id IS NOT NULL),
    CONSTRAINT ck_saisie_tache_projet CHECK (tache_id IS NULL OR projet_id IS NOT NULL),
    CONSTRAINT ck_saisie_chrono       CHECK (chrono_fin IS NULL OR chrono_debut IS NULL OR chrono_fin >= chrono_debut)
);
CREATE INDEX ix_saisie_feuille       ON temps.saisie_temps (feuille_id);
CREATE INDEX ix_saisie_agent_date    ON temps.saisie_temps (agent_id, date_saisie);
CREATE INDEX ix_saisie_projet        ON temps.saisie_temps (projet_id) WHERE projet_id IS NOT NULL;
CREATE INDEX ix_saisie_tache         ON temps.saisie_temps (tache_id)  WHERE tache_id  IS NOT NULL;
CREATE INDEX ix_saisie_ticket        ON temps.saisie_temps (ticket_id) WHERE ticket_id IS NOT NULL;
CREATE INDEX ix_saisie_categorie     ON temps.saisie_temps (categorie_id);

-- =============================================================================
-- SCHÉMA commun — Transverse (polymorphe), paramètres, journal d'audit
-- =============================================================================

CREATE TABLE commun.parametre (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    cle             varchar(100) NOT NULL,
    valeur          text NOT NULL,
    type_valeur     varchar(10) NOT NULL CHECK (type_valeur IN ('TEXTE','ENTIER','DECIMAL','BOOLEEN','JSON')),
    description     varchar(300),
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE UNIQUE INDEX ux_parametre_cle ON commun.parametre (cle) WHERE NOT est_supprime;

CREATE TABLE commun.commentaire (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    type_cible      varchar(30) NOT NULL,        -- PROJET, PHASE, TACHE, TICKET, DIAGRAMME…
    cible_id        uuid NOT NULL,
    parent_id       uuid REFERENCES commun.commentaire(id) ON DELETE CASCADE,   -- fil de discussion
    auteur_id       uuid NOT NULL REFERENCES org.agent(id) ON DELETE RESTRICT,
    contenu         text NOT NULL,
    est_modifie     boolean NOT NULL DEFAULT false,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE INDEX ix_commentaire_cible  ON commun.commentaire (type_cible, cible_id, cree_le);
CREATE INDEX ix_commentaire_parent ON commun.commentaire (parent_id);
CREATE INDEX ix_commentaire_auteur ON commun.commentaire (auteur_id);

CREATE TABLE commun.piece_jointe (
    id               uuid PRIMARY KEY DEFAULT uuidv7(),
    type_cible       varchar(30)  NOT NULL,
    cible_id         uuid NOT NULL,
    nom_fichier      varchar(255) NOT NULL,
    type_mime        varchar(100) NOT NULL,
    taille_octets    bigint NOT NULL CHECK (taille_octets >= 0),
    chemin_stockage  varchar(500) NOT NULL,     -- fichier hors base
    empreinte_sha256 char(64) NOT NULL,
    televerse_par_id uuid NOT NULL REFERENCES org.agent(id) ON DELETE RESTRICT,
    cree_le          timestamptz NOT NULL DEFAULT now(),
    cree_par         uuid,
    modifie_le       timestamptz,
    modifie_par      uuid,
    est_supprime     boolean NOT NULL DEFAULT false,
    supprime_le      timestamptz,
    supprime_par     uuid
);
CREATE INDEX ix_piece_jointe_cible ON commun.piece_jointe (type_cible, cible_id);
CREATE INDEX ix_piece_jointe_auteur ON commun.piece_jointe (televerse_par_id);

CREATE TABLE commun.notification (
    id               uuid PRIMARY KEY DEFAULT uuidv7(),
    destinataire_id  uuid NOT NULL REFERENCES org.agent(id) ON DELETE CASCADE,
    type             varchar(50)  NOT NULL,     -- AFFECTATION_RECUE, VALIDATION_DEMANDEE, ECHEANCE_PROCHE…
    titre            varchar(200) NOT NULL,
    message          text,
    lien             varchar(500),
    est_lue          boolean NOT NULL DEFAULT false,
    date_lecture     timestamptz,
    cree_le          timestamptz NOT NULL DEFAULT now(),
    cree_par         uuid,
    modifie_le       timestamptz,
    modifie_par      uuid,
    est_supprime     boolean NOT NULL DEFAULT false,
    supprime_le      timestamptz,
    supprime_par     uuid
);
CREATE INDEX ix_notification_non_lues ON commun.notification (destinataire_id, cree_le DESC) WHERE NOT est_lue AND NOT est_supprime;

-- Journal d'audit : append-only, chaîné par empreinte (infalsifiable).
CREATE TABLE commun.historique (
    id                    bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_cible            varchar(30) NOT NULL,
    cible_id              uuid NOT NULL,
    action                varchar(15) NOT NULL CHECK (action IN ('CREATION','MODIFICATION','SUPPRESSION','RESTAURATION','CONSULTATION')),
    champ                 varchar(100),
    ancienne_valeur       text,
    nouvelle_valeur       text,
    agent_id              uuid,                -- sans FK : le journal ne doit jamais bloquer
    date_action           timestamptz NOT NULL DEFAULT now(),
    adresse_ip            inet,
    correlation_id        uuid,                -- regroupe les lignes d'une même requête
    empreinte_precedente  char(64),
    empreinte             char(64) NOT NULL    -- sha256(empreinte_precedente || contenu de la ligne), calculée par l'application
);
CREATE INDEX ix_historique_cible ON commun.historique (type_cible, cible_id, date_action);
CREATE INDEX ix_historique_agent ON commun.historique (agent_id, date_action);

CREATE FUNCTION commun.fn_historique_lecture_seule() RETURNS trigger
LANGUAGE plpgsql AS $$
BEGIN
    RAISE EXCEPTION 'commun.historique est en ajout seul : % interdit', TG_OP;
END;
$$;

CREATE TRIGGER tg_historique_lecture_seule
    BEFORE UPDATE OR DELETE ON commun.historique
    FOR EACH ROW EXECUTE FUNCTION commun.fn_historique_lecture_seule();

-- =============================================================================
-- SCHÉMA modelisation — Métamodèle du générateur de code
-- =============================================================================

CREATE TABLE modelisation.modele_domaine (
    id                uuid PRIMARY KEY DEFAULT uuidv7(),
    projet_id         uuid NOT NULL REFERENCES projet.projet(id) ON DELETE CASCADE,
    nom               varchar(150) NOT NULL,
    version           integer NOT NULL DEFAULT 1 CHECK (version > 0),
    statut            varchar(15) NOT NULL DEFAULT 'BROUILLON' CHECK (statut IN ('BROUILLON','EN_VALIDATION','VALIDE','FIGE')),
    namespace_racine  varchar(100) NOT NULL,
    sgbd_cible        varchar(20)  NOT NULL DEFAULT 'POSTGRESQL' CHECK (sgbd_cible IN ('POSTGRESQL')),
    description       text,
    cree_le           timestamptz NOT NULL DEFAULT now(),
    cree_par          uuid,
    modifie_le        timestamptz,
    modifie_par       uuid,
    est_supprime      boolean NOT NULL DEFAULT false,
    supprime_le       timestamptz,
    supprime_par      uuid
);
CREATE UNIQUE INDEX ux_modele_projet_version ON modelisation.modele_domaine (projet_id, version) WHERE NOT est_supprime;

CREATE TABLE modelisation.module (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    modele_id       uuid NOT NULL REFERENCES modelisation.modele_domaine(id) ON DELETE CASCADE,
    code            varchar(50)  NOT NULL,
    nom             varchar(150) NOT NULL,
    schema_bd       varchar(63)  NOT NULL CHECK (schema_bd ~ '^[a-z][a-z0-9_]*$'),
    namespace       varchar(100) NOT NULL,
    ordre           smallint NOT NULL DEFAULT 1,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE UNIQUE INDEX ux_module_code ON modelisation.module (modele_id, code) WHERE NOT est_supprime;

CREATE TABLE modelisation.enumeration (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    module_id       uuid NOT NULL REFERENCES modelisation.module(id) ON DELETE CASCADE,
    nom             varchar(100) NOT NULL,
    nom_technique   varchar(63)  NOT NULL,
    stockage        varchar(10)  NOT NULL DEFAULT 'TEXTE' CHECK (stockage IN ('ENTIER','TEXTE')),
    description     text,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE UNIQUE INDEX ux_enumeration_nom ON modelisation.enumeration (module_id, nom_technique) WHERE NOT est_supprime;

CREATE TABLE modelisation.valeur_enumeration (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    enumeration_id  uuid NOT NULL REFERENCES modelisation.enumeration(id) ON DELETE CASCADE,
    code            varchar(50)  NOT NULL,
    libelle         varchar(150) NOT NULL,
    valeur          integer,
    ordre           smallint NOT NULL DEFAULT 1,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE UNIQUE INDEX ux_valeur_enumeration ON modelisation.valeur_enumeration (enumeration_id, code) WHERE NOT est_supprime;

CREATE TABLE modelisation.entite (
    id                        uuid PRIMARY KEY DEFAULT uuidv7(),
    module_id                 uuid NOT NULL REFERENCES modelisation.module(id) ON DELETE CASCADE,
    entite_parent_id          uuid REFERENCES modelisation.entite(id) ON DELETE RESTRICT,   -- héritage
    nom                       varchar(100) NOT NULL,
    nom_technique             varchar(63)  NOT NULL CHECK (nom_technique ~ '^[a-z][a-z0-9_]*$'),
    nom_pluriel               varchar(100) NOT NULL,
    libelle_ui                varchar(150),
    description               text,
    stereotype                varchar(15) NOT NULL DEFAULT 'ENTITE' CHECK (stereotype IN ('ENTITE','REFERENTIEL','VALEUR','INTERFACE')),
    est_abstraite             boolean NOT NULL DEFAULT false,
    strategie_heritage        varchar(5) CHECK (strategie_heritage IN ('TPH','TPT','TPC')),
    est_racine_agregat        boolean NOT NULL DEFAULT true,
    avec_audit                boolean NOT NULL DEFAULT true,
    avec_suppression_logique  boolean NOT NULL DEFAULT true,
    avec_concurrence          boolean NOT NULL DEFAULT true,
    generer_api               boolean NOT NULL DEFAULT true,
    generer_pages             boolean NOT NULL DEFAULT true,
    attribut_libelle_id       uuid,                -- FK vers modelisation.attribut ajoutée plus bas
    cree_le                   timestamptz NOT NULL DEFAULT now(),
    cree_par                  uuid,
    modifie_le                timestamptz,
    modifie_par               uuid,
    est_supprime              boolean NOT NULL DEFAULT false,
    supprime_le               timestamptz,
    supprime_par              uuid,
    CONSTRAINT ck_entite_heritage CHECK (entite_parent_id IS NULL OR entite_parent_id <> id)
);
CREATE UNIQUE INDEX ux_entite_nom_technique ON modelisation.entite (module_id, nom_technique) WHERE NOT est_supprime;
CREATE INDEX ix_entite_parent               ON modelisation.entite (entite_parent_id);

CREATE TABLE modelisation.entite_interface (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    entite_id       uuid NOT NULL REFERENCES modelisation.entite(id) ON DELETE CASCADE,
    interface_id    uuid NOT NULL REFERENCES modelisation.entite(id) ON DELETE CASCADE,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid,
    CONSTRAINT ck_entite_interface CHECK (entite_id <> interface_id)
);
CREATE UNIQUE INDEX ux_entite_interface ON modelisation.entite_interface (entite_id, interface_id) WHERE NOT est_supprime;
CREATE INDEX ix_entite_interface_itf    ON modelisation.entite_interface (interface_id);

CREATE TABLE modelisation.association (
    id                 uuid PRIMARY KEY DEFAULT uuidv7(),
    modele_id          uuid NOT NULL REFERENCES modelisation.modele_domaine(id) ON DELETE CASCADE,
    nom                varchar(100) NOT NULL,          -- verbe MERISE : contient, affecte…
    nom_technique      varchar(63)  NOT NULL,
    nature             varchar(15)  NOT NULL DEFAULT 'ASSOCIATION' CHECK (nature IN ('ASSOCIATION','COMPOSITION','AGREGATION','POLYMORPHE')),
    regle_suppression  varchar(10)  NOT NULL DEFAULT 'RESTRICT' CHECK (regle_suppression IN ('CASCADE','RESTRICT','SET_NULL')),
    description        text,
    cree_le            timestamptz NOT NULL DEFAULT now(),
    cree_par           uuid,
    modifie_le         timestamptz,
    modifie_par        uuid,
    est_supprime       boolean NOT NULL DEFAULT false,
    supprime_le        timestamptz,
    supprime_par       uuid
);
CREATE INDEX ix_association_modele ON modelisation.association (modele_id);

CREATE TABLE modelisation.patte (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    association_id  uuid NOT NULL REFERENCES modelisation.association(id) ON DELETE CASCADE,
    entite_id       uuid NOT NULL REFERENCES modelisation.entite(id) ON DELETE CASCADE,
    card_min        smallint   NOT NULL CHECK (card_min IN (0,1)),
    card_max        varchar(1) NOT NULL CHECK (card_max IN ('1','N')),
    role            varchar(63),                      -- obligatoire pour une association réflexive (R7)
    nom_navigation  varchar(100),
    est_navigable   boolean NOT NULL DEFAULT true,
    ordre           smallint NOT NULL DEFAULT 1,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE INDEX ix_patte_association ON modelisation.patte (association_id);
CREATE INDEX ix_patte_entite      ON modelisation.patte (entite_id);

CREATE TABLE modelisation.attribut (
    id                   uuid PRIMARY KEY DEFAULT uuidv7(),
    entite_id            uuid REFERENCES modelisation.entite(id) ON DELETE CASCADE,
    association_id       uuid REFERENCES modelisation.association(id) ON DELETE CASCADE,   -- association porteuse
    enumeration_id       uuid REFERENCES modelisation.enumeration(id) ON DELETE RESTRICT,
    nom                  varchar(100) NOT NULL,
    nom_technique        varchar(63)  NOT NULL CHECK (nom_technique ~ '^[a-z][a-z0-9_]*$'),
    libelle_ui           varchar(150),
    type_logique         varchar(15)  NOT NULL CHECK (type_logique IN ('TEXTE','TEXTE_LONG','ENTIER','DECIMAL','BOOLEEN','DATE','DATE_HEURE','UUID','ENUM','JSON','FICHIER')),
    longueur             integer CHECK (longueur > 0),
    precision_num        smallint CHECK (precision_num BETWEEN 1 AND 38),
    echelle              smallint CHECK (echelle >= 0),
    est_identifiant      boolean NOT NULL DEFAULT false,
    est_obligatoire      boolean NOT NULL DEFAULT false,
    est_unique           boolean NOT NULL DEFAULT false,
    est_calcule          boolean NOT NULL DEFAULT false,
    valeur_defaut        varchar(200),
    visibilite           varchar(10) NOT NULL DEFAULT 'PUBLIC' CHECK (visibilite IN ('PUBLIC','PROTEGE','PRIVE')),
    regex_validation     varchar(300),
    valeur_min           varchar(50),
    valeur_max           varchar(50),
    controle_ui          varchar(10) CHECK (controle_ui IN ('TEXTE','ZONE','NOMBRE','DATE','LISTE','CASE','FICHIER')),
    afficher_dans_liste  boolean NOT NULL DEFAULT true,
    est_filtrable        boolean NOT NULL DEFAULT false,
    est_triable          boolean NOT NULL DEFAULT true,
    ordre                smallint NOT NULL DEFAULT 1,
    cree_le              timestamptz NOT NULL DEFAULT now(),
    cree_par             uuid,
    modifie_le           timestamptz,
    modifie_par          uuid,
    est_supprime         boolean NOT NULL DEFAULT false,
    supprime_le          timestamptz,
    supprime_par         uuid,
    CONSTRAINT ck_attribut_proprietaire CHECK (num_nonnulls(entite_id, association_id) = 1),
    CONSTRAINT ck_attribut_enum         CHECK ((type_logique = 'ENUM') = (enumeration_id IS NOT NULL)),
    CONSTRAINT ck_attribut_decimal      CHECK (type_logique <> 'DECIMAL' OR precision_num IS NOT NULL),
    CONSTRAINT ck_attribut_echelle      CHECK (echelle IS NULL OR precision_num IS NULL OR echelle <= precision_num)
);
CREATE UNIQUE INDEX ux_attribut_entite      ON modelisation.attribut (entite_id, nom_technique)      WHERE entite_id IS NOT NULL AND NOT est_supprime;
CREATE UNIQUE INDEX ux_attribut_association ON modelisation.attribut (association_id, nom_technique) WHERE association_id IS NOT NULL AND NOT est_supprime;
CREATE INDEX ix_attribut_enumeration        ON modelisation.attribut (enumeration_id);

ALTER TABLE modelisation.entite
    ADD CONSTRAINT fk_entite_attribut_libelle FOREIGN KEY (attribut_libelle_id) REFERENCES modelisation.attribut(id) ON DELETE SET NULL;

CREATE TABLE modelisation.index_entite (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    entite_id       uuid NOT NULL REFERENCES modelisation.entite(id) ON DELETE CASCADE,
    nom             varchar(63) NOT NULL,
    est_unique      boolean NOT NULL DEFAULT false,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE INDEX ix_index_entite ON modelisation.index_entite (entite_id);

CREATE TABLE modelisation.index_attribut (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    index_id        uuid NOT NULL REFERENCES modelisation.index_entite(id) ON DELETE CASCADE,
    attribut_id     uuid NOT NULL REFERENCES modelisation.attribut(id) ON DELETE CASCADE,
    ordre           smallint NOT NULL DEFAULT 1,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE UNIQUE INDEX ux_index_attribut ON modelisation.index_attribut (index_id, attribut_id) WHERE NOT est_supprime;
CREATE INDEX ix_index_attribut_attr   ON modelisation.index_attribut (attribut_id);

CREATE TABLE modelisation.methode (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    entite_id       uuid NOT NULL REFERENCES modelisation.entite(id) ON DELETE CASCADE,
    nom             varchar(100) NOT NULL,
    type_retour     varchar(150) NOT NULL DEFAULT 'void',
    visibilite      varchar(10)  NOT NULL DEFAULT 'PUBLIC' CHECK (visibilite IN ('PUBLIC','PROTEGE','PRIVE')),
    couche_cible    varchar(15)  NOT NULL DEFAULT 'SERVICE' CHECK (couche_cible IN ('ENTITE','SERVICE','ORCHESTRATION')),
    est_async       boolean NOT NULL DEFAULT true,
    est_statique    boolean NOT NULL DEFAULT false,
    description     text,
    ordre           smallint NOT NULL DEFAULT 1,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE INDEX ix_methode_entite ON modelisation.methode (entite_id);

CREATE TABLE modelisation.parametre_methode (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    methode_id      uuid NOT NULL REFERENCES modelisation.methode(id) ON DELETE CASCADE,
    nom             varchar(100) NOT NULL,
    type            varchar(150) NOT NULL,
    ordre           smallint NOT NULL DEFAULT 1,
    est_optionnel   boolean NOT NULL DEFAULT false,
    valeur_defaut   varchar(200),
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE UNIQUE INDEX ux_parametre_methode ON modelisation.parametre_methode (methode_id, nom) WHERE NOT est_supprime;

-- ----- MLD dérivé (résultat de la transformation R1..R10) -----

CREATE TABLE modelisation.table_bd (
    id                     uuid PRIMARY KEY DEFAULT uuidv7(),
    modele_id              uuid NOT NULL REFERENCES modelisation.modele_domaine(id) ON DELETE CASCADE,
    entite_source_id       uuid REFERENCES modelisation.entite(id) ON DELETE SET NULL,
    association_source_id  uuid REFERENCES modelisation.association(id) ON DELETE SET NULL,
    nom                    varchar(63) NOT NULL,
    schema_bd              varchar(63) NOT NULL,
    est_table_association  boolean NOT NULL DEFAULT false,
    est_surchargee         boolean NOT NULL DEFAULT false,   -- ajustée à la main : protégée à la régénération
    cree_le                timestamptz NOT NULL DEFAULT now(),
    cree_par               uuid,
    modifie_le             timestamptz,
    modifie_par            uuid,
    est_supprime           boolean NOT NULL DEFAULT false,
    supprime_le            timestamptz,
    supprime_par           uuid
);
CREATE UNIQUE INDEX ux_table_bd           ON modelisation.table_bd (modele_id, schema_bd, nom) WHERE NOT est_supprime;
CREATE INDEX ix_table_bd_entite           ON modelisation.table_bd (entite_source_id);
CREATE INDEX ix_table_bd_association      ON modelisation.table_bd (association_source_id);

CREATE TABLE modelisation.colonne (
    id                  uuid PRIMARY KEY DEFAULT uuidv7(),
    table_id            uuid NOT NULL REFERENCES modelisation.table_bd(id) ON DELETE CASCADE,
    attribut_source_id  uuid REFERENCES modelisation.attribut(id) ON DELETE SET NULL,
    nom                 varchar(63)  NOT NULL,
    type_physique       varchar(100) NOT NULL,
    est_nullable        boolean NOT NULL DEFAULT true,
    est_cle_primaire    boolean NOT NULL DEFAULT false,
    valeur_defaut       varchar(200),
    ordre               smallint NOT NULL DEFAULT 1,
    est_surchargee      boolean NOT NULL DEFAULT false,
    cree_le             timestamptz NOT NULL DEFAULT now(),
    cree_par            uuid,
    modifie_le          timestamptz,
    modifie_par         uuid,
    est_supprime        boolean NOT NULL DEFAULT false,
    supprime_le         timestamptz,
    supprime_par        uuid
);
CREATE UNIQUE INDEX ux_colonne        ON modelisation.colonne (table_id, nom) WHERE NOT est_supprime;
CREATE INDEX ix_colonne_attribut      ON modelisation.colonne (attribut_source_id);

CREATE TABLE modelisation.cle_etrangere (
    id                     uuid PRIMARY KEY DEFAULT uuidv7(),
    table_id               uuid NOT NULL REFERENCES modelisation.table_bd(id) ON DELETE CASCADE,
    colonne_id             uuid NOT NULL REFERENCES modelisation.colonne(id) ON DELETE CASCADE,
    table_referencee_id    uuid NOT NULL REFERENCES modelisation.table_bd(id) ON DELETE CASCADE,
    colonne_referencee_id  uuid NOT NULL REFERENCES modelisation.colonne(id) ON DELETE CASCADE,
    patte_source_id        uuid REFERENCES modelisation.patte(id) ON DELETE SET NULL,
    nom                    varchar(63) NOT NULL,
    on_delete              varchar(10) NOT NULL DEFAULT 'RESTRICT' CHECK (on_delete IN ('CASCADE','RESTRICT','SET_NULL')),
    cree_le                timestamptz NOT NULL DEFAULT now(),
    cree_par               uuid,
    modifie_le             timestamptz,
    modifie_par            uuid,
    est_supprime           boolean NOT NULL DEFAULT false,
    supprime_le            timestamptz,
    supprime_par           uuid
);
CREATE INDEX ix_cle_etrangere_table      ON modelisation.cle_etrangere (table_id);
CREATE INDEX ix_cle_etrangere_colonne    ON modelisation.cle_etrangere (colonne_id);
CREATE INDEX ix_cle_etrangere_table_ref  ON modelisation.cle_etrangere (table_referencee_id);
CREATE INDEX ix_cle_etrangere_col_ref    ON modelisation.cle_etrangere (colonne_referencee_id);
CREATE INDEX ix_cle_etrangere_patte      ON modelisation.cle_etrangere (patte_source_id);

CREATE TABLE modelisation.correspondance_type (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    type_logique    varchar(15)  NOT NULL,
    cible           varchar(15)  NOT NULL CHECK (cible IN ('POSTGRESQL','CSHARP')),
    type_physique   varchar(100) NOT NULL,           -- {longueur}, {precision}, {echelle} remplacés par le générateur
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE UNIQUE INDEX ux_correspondance_type ON modelisation.correspondance_type (type_logique, cible) WHERE NOT est_supprime;

-- ----- Diagrammes (vues) -----

CREATE TABLE modelisation.diagramme (
    id                 uuid PRIMARY KEY DEFAULT uuidv7(),
    modele_id          uuid NOT NULL REFERENCES modelisation.modele_domaine(id) ON DELETE CASCADE,
    type               varchar(20) NOT NULL CHECK (type IN ('MCD','CLASSES','MLD','SEQUENCE','CAS_UTILISATION','ACTIVITE','ETATS','COMPOSANTS','DEPLOIEMENT','BPMN','C4')),
    nom                varchar(150) NOT NULL,
    contenu            jsonb,            -- sémantique des diagrammes documentaires
    source_texte       text,             -- PlantUML / Mermaid
    statut_validation  varchar(15) NOT NULL DEFAULT 'BROUILLON' CHECK (statut_validation IN ('BROUILLON','EN_VALIDATION','VALIDE','REJETE')),
    version            integer NOT NULL DEFAULT 1,
    cree_le            timestamptz NOT NULL DEFAULT now(),
    cree_par           uuid,
    modifie_le         timestamptz,
    modifie_par        uuid,
    est_supprime       boolean NOT NULL DEFAULT false,
    supprime_le        timestamptz,
    supprime_par       uuid
);
CREATE INDEX ix_diagramme_modele ON modelisation.diagramme (modele_id, type);

CREATE TABLE modelisation.element_diagramme (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    diagramme_id    uuid NOT NULL REFERENCES modelisation.diagramme(id) ON DELETE CASCADE,
    type_element    varchar(20) NOT NULL CHECK (type_element IN ('ENTITE','ASSOCIATION','ENUMERATION','TABLE','NOTE')),
    element_id      uuid NOT NULL,       -- polymorphe : id de l'entité, association, enum ou table
    x               numeric(10,2) NOT NULL DEFAULT 0,
    y               numeric(10,2) NOT NULL DEFAULT 0,
    largeur         numeric(10,2),
    hauteur         numeric(10,2),
    style           jsonb,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE UNIQUE INDEX ux_element_diagramme ON modelisation.element_diagramme (diagramme_id, type_element, element_id) WHERE NOT est_supprime;

-- ----- Génération -----

CREATE TABLE modelisation.config_generation (
    id               uuid PRIMARY KEY DEFAULT uuidv7(),
    projet_id        uuid NOT NULL REFERENCES projet.projet(id) ON DELETE CASCADE,
    stack_back       varchar(20) NOT NULL DEFAULT 'DOTNET_10'   CHECK (stack_back  IN ('DOTNET_10')),
    stack_front      varchar(20) NOT NULL DEFAULT 'BLAZOR_WASM' CHECK (stack_front IN ('BLAZOR_WASM')),
    architecture     varchar(20) NOT NULL DEFAULT 'COUCHES_GP'  CHECK (architecture IN ('COUCHES_GP')),
    depot_gitlab_id  varchar(100),
    branche_cible    varchar(100) NOT NULL DEFAULT 'generation/socle',
    cree_le          timestamptz NOT NULL DEFAULT now(),
    cree_par         uuid,
    modifie_le       timestamptz,
    modifie_par      uuid,
    est_supprime     boolean NOT NULL DEFAULT false,
    supprime_le      timestamptz,
    supprime_par     uuid
);
CREATE UNIQUE INDEX ux_config_generation_projet ON modelisation.config_generation (projet_id) WHERE NOT est_supprime;

CREATE TABLE modelisation.template_generation (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    config_id       uuid REFERENCES modelisation.config_generation(id) ON DELETE CASCADE,   -- NULL = template global
    couche          varchar(15) NOT NULL CHECK (couche IN ('ENTITE','DTO','MAPPING','REPOSITORY','SERVICE','ORCHESTRATION','CONTROLLER','API_CLIENT','FRONT_SERVICE','FRONT_MODEL','PAGE','SQL','DOCKER','CI')),
    portee          varchar(10) NOT NULL DEFAULT 'ENTITE' CHECK (portee IN ('ENTITE','MODULE','MODELE')),   -- exécuté une fois par…
    nom             varchar(100) NOT NULL,
    chemin_sortie   varchar(300) NOT NULL,        -- src/{Nom}-Backend/Services/{Module}/{Entite}Service.g.cs
    contenu         text NOT NULL,                -- template Scriban
    version         integer NOT NULL DEFAULT 1,
    est_defaut      boolean NOT NULL DEFAULT false,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE INDEX ix_template_config_couche ON modelisation.template_generation (config_id, couche);

CREATE TABLE modelisation.execution_generation (
    id              uuid PRIMARY KEY DEFAULT uuidv7(),
    modele_id       uuid NOT NULL REFERENCES modelisation.modele_domaine(id) ON DELETE CASCADE,
    version_modele  integer NOT NULL,
    lanceur_id      uuid NOT NULL REFERENCES org.agent(id) ON DELETE RESTRICT,
    date_execution  timestamptz NOT NULL DEFAULT now(),
    statut          varchar(10) NOT NULL DEFAULT 'EN_COURS' CHECK (statut IN ('EN_COURS','SUCCES','ECHEC')),
    nb_fichiers     integer,
    branche         varchar(100),
    commit_sha      char(40),
    rapport         jsonb,
    cree_le         timestamptz NOT NULL DEFAULT now(),
    cree_par        uuid,
    modifie_le      timestamptz,
    modifie_par     uuid,
    est_supprime    boolean NOT NULL DEFAULT false,
    supprime_le     timestamptz,
    supprime_par    uuid
);
CREATE INDEX ix_execution_modele ON modelisation.execution_generation (modele_id, date_execution DESC);
CREATE INDEX ix_execution_lanceur ON modelisation.execution_generation (lanceur_id);
