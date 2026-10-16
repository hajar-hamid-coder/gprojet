using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Gprojet.Backend.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "org");

            migrationBuilder.EnsureSchema(
                name: "projet");

            migrationBuilder.EnsureSchema(
                name: "modelisation");

            migrationBuilder.EnsureSchema(
                name: "planif");

            migrationBuilder.EnsureSchema(
                name: "temps");

            migrationBuilder.EnsureSchema(
                name: "commun");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:ltree", ",,");

            migrationBuilder.CreateTable(
                name: "categorie_activite",
                schema: "temps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    libelle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    est_projet = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    actif = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("categorie_activite_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "competence",
                schema: "org",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    libelle = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    categorie = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("competence_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "correspondance_type",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    type_logique = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    cible = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    type_physique = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("correspondance_type_pkey", x => x.id);
                    table.CheckConstraint("correspondance_type_cible_check", "cible IN ('POSTGRESQL','CSHARP')");
                });

            migrationBuilder.CreateTable(
                name: "fonction",
                schema: "org",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    libelle = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("fonction_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "historique",
                schema: "commun",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    type_cible = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    cible_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    champ = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ancienne_valeur = table.Column<string>(type: "text", nullable: true),
                    nouvelle_valeur = table.Column<string>(type: "text", nullable: true),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    date_action = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    adresse_ip = table.Column<IPAddress>(type: "inet", nullable: true),
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    empreinte_precedente = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: true),
                    empreinte = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("historique_pkey", x => x.id);
                    table.CheckConstraint("historique_action_check", "action IN ('CREATION','MODIFICATION','SUPPRESSION','RESTAURATION','CONSULTATION')");
                });

            migrationBuilder.CreateTable(
                name: "jour_ferie",
                schema: "org",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    date_ferie = table.Column<DateOnly>(type: "date", nullable: false),
                    libelle = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("jour_ferie_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "parametre",
                schema: "commun",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    cle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    valeur = table.Column<string>(type: "text", nullable: false),
                    type_valeur = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("parametre_pkey", x => x.id);
                    table.CheckConstraint("parametre_type_valeur_check", "type_valeur IN ('TEXTE','ENTIER','DECIMAL','BOOLEEN','JSON')");
                });

            migrationBuilder.CreateTable(
                name: "type_projet",
                schema: "projet",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    libelle = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    actif = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("type_projet_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "workflow",
                schema: "planif",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    libelle = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    type_element = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    est_defaut = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("workflow_pkey", x => x.id);
                    table.CheckConstraint("workflow_type_element_check", "type_element IN ('TACHE','TICKET')");
                });

            migrationBuilder.CreateTable(
                name: "modele_phase",
                schema: "projet",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    type_projet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ordre = table.Column<short>(type: "smallint", nullable: false),
                    nom = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    objectif = table.Column<string>(type: "text", nullable: true),
                    criteres_entree = table.Column<string>(type: "text", nullable: true),
                    criteres_sortie = table.Column<string>(type: "text", nullable: true),
                    mode_enchainement = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'SEQUENTIEL'"),
                    pct_min_chevauchement = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("modele_phase_pkey", x => x.id);
                    table.CheckConstraint("modele_phase_mode_enchainement_check", "mode_enchainement IN ('SEQUENTIEL','CHEVAUCHEMENT','PARALLELE')");
                    table.CheckConstraint("modele_phase_ordre_check", "ordre > 0");
                    table.CheckConstraint("modele_phase_pct_min_chevauchement_check", "pct_min_chevauchement BETWEEN 0 AND 100");
                    table.ForeignKey(
                        name: "modele_phase_type_projet_id_fkey",
                        column: x => x.type_projet_id,
                        principalSchema: "projet",
                        principalTable: "type_projet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "statut_workflow",
                schema: "planif",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    workflow_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    libelle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    categorie = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    couleur = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    ordre = table.Column<short>(type: "smallint", nullable: false),
                    est_initial = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("statut_workflow_pkey", x => x.id);
                    table.CheckConstraint("statut_workflow_categorie_check", "categorie IN ('A_FAIRE','EN_COURS','TERMINE')");
                    table.CheckConstraint("statut_workflow_couleur_check", "couleur ~ '^#[0-9A-Fa-f]{6}$'");
                    table.ForeignKey(
                        name: "statut_workflow_workflow_id_fkey",
                        column: x => x.workflow_id,
                        principalSchema: "planif",
                        principalTable: "workflow",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "type_ticket",
                schema: "planif",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    libelle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    icone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    workflow_id = table.Column<Guid>(type: "uuid", nullable: false),
                    actif = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("type_ticket_pkey", x => x.id);
                    table.ForeignKey(
                        name: "type_ticket_workflow_id_fkey",
                        column: x => x.workflow_id,
                        principalSchema: "planif",
                        principalTable: "workflow",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "transition",
                schema: "planif",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    workflow_id = table.Column<Guid>(type: "uuid", nullable: false),
                    statut_depuis_id = table.Column<Guid>(type: "uuid", nullable: false),
                    statut_vers_id = table.Column<Guid>(type: "uuid", nullable: false),
                    libelle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    roles_autorises = table.Column<List<string>>(type: "text[]", nullable: false, defaultValueSql: "'{}'"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("transition_pkey", x => x.id);
                    table.CheckConstraint("ck_transition_statuts", "statut_depuis_id <> statut_vers_id");
                    table.ForeignKey(
                        name: "transition_statut_depuis_id_fkey",
                        column: x => x.statut_depuis_id,
                        principalSchema: "planif",
                        principalTable: "statut_workflow",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "transition_statut_vers_id_fkey",
                        column: x => x.statut_vers_id,
                        principalSchema: "planif",
                        principalTable: "statut_workflow",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "transition_workflow_id_fkey",
                        column: x => x.workflow_id,
                        principalSchema: "planif",
                        principalTable: "workflow",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "absence",
                schema: "org",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_debut = table.Column<DateOnly>(type: "date", nullable: false),
                    date_fin = table.Column<DateOnly>(type: "date", nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    demi_journee = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    commentaire = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("absence_pkey", x => x.id);
                    table.CheckConstraint("absence_type_check", "type IN ('CONGE','MALADIE','FORMATION','MISSION','AUTRE')");
                    table.CheckConstraint("ck_absence_dates", "date_fin >= date_debut");
                });

            migrationBuilder.CreateTable(
                name: "affectation_projet",
                schema: "projet",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    projet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    precedente_id = table.Column<Guid>(type: "uuid", nullable: true),
                    entite_source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entite_cible_id = table.Column<Guid>(type: "uuid", nullable: false),
                    niveau_cible = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    statut = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'EN_ATTENTE'"),
                    instructions = table.Column<string>(type: "text", nullable: true),
                    echeance_souhaitee = table.Column<DateOnly>(type: "date", nullable: true),
                    charge_prevue_jh = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    affecte_par_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_affectation = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    repondu_par_id = table.Column<Guid>(type: "uuid", nullable: true),
                    date_reponse = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    motif_reponse = table.Column<string>(type: "text", nullable: true),
                    est_active = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("affectation_projet_pkey", x => x.id);
                    table.CheckConstraint("affectation_projet_niveau_cible_check", "niveau_cible IN ('DIVISION','SERVICE')");
                    table.CheckConstraint("affectation_projet_statut_check", "statut IN ('EN_ATTENTE','ACCEPTEE','RENVOYEE','ANNULEE')");
                    table.CheckConstraint("ck_affectation_entites", "entite_source_id <> entite_cible_id");
                    table.CheckConstraint("ck_affectation_motif", "statut <> 'RENVOYEE' OR motif_reponse IS NOT NULL");
                    table.ForeignKey(
                        name: "affectation_projet_precedente_id_fkey",
                        column: x => x.precedente_id,
                        principalSchema: "projet",
                        principalTable: "affectation_projet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "agent",
                schema: "org",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    matricule = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    nom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    prenom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    telephone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    entite_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fonction_id = table.Column<Guid>(type: "uuid", nullable: true),
                    capacite_hebdo_h = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false, defaultValueSql: "40"),
                    keycloak_sub = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    actif = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("agent_pkey", x => x.id);
                    table.CheckConstraint("agent_capacite_hebdo_h_check", "capacite_hebdo_h BETWEEN 0 AND 80");
                    table.ForeignKey(
                        name: "agent_fonction_id_fkey",
                        column: x => x.fonction_id,
                        principalSchema: "org",
                        principalTable: "fonction",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "agent_competence",
                schema: "org",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    competence_id = table.Column<Guid>(type: "uuid", nullable: false),
                    niveau = table.Column<short>(type: "smallint", nullable: false),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("agent_competence_pkey", x => x.id);
                    table.CheckConstraint("agent_competence_niveau_check", "niveau BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "agent_competence_agent_id_fkey",
                        column: x => x.agent_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "agent_competence_competence_id_fkey",
                        column: x => x.competence_id,
                        principalSchema: "org",
                        principalTable: "competence",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "commentaire",
                schema: "commun",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    type_cible = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    cible_id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    auteur_id = table.Column<Guid>(type: "uuid", nullable: false),
                    contenu = table.Column<string>(type: "text", nullable: false),
                    est_modifie = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("commentaire_pkey", x => x.id);
                    table.ForeignKey(
                        name: "commentaire_auteur_id_fkey",
                        column: x => x.auteur_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "commentaire_parent_id_fkey",
                        column: x => x.parent_id,
                        principalSchema: "commun",
                        principalTable: "commentaire",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entite_organisationnelle",
                schema: "org",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    libelle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    sigle = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    niveau = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    responsable_id = table.Column<Guid>(type: "uuid", nullable: true),
                    chemin = table.Column<string>(type: "ltree", nullable: false),
                    actif = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("entite_organisationnelle_pkey", x => x.id);
                    table.CheckConstraint("ck_entite_parent_different", "parent_id IS NULL OR parent_id <> id");
                    table.CheckConstraint("entite_organisationnelle_niveau_check", "niveau IN ('DIRECTION','DIVISION','SERVICE')");
                    table.ForeignKey(
                        name: "entite_organisationnelle_parent_id_fkey",
                        column: x => x.parent_id,
                        principalSchema: "org",
                        principalTable: "entite_organisationnelle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_entite_responsable",
                        column: x => x.responsable_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "feuille_temps",
                schema: "temps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    semaine_debut = table.Column<DateOnly>(type: "date", nullable: false),
                    statut = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false, defaultValueSql: "'BROUILLON'"),
                    date_soumission = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    valideur_id = table.Column<Guid>(type: "uuid", nullable: true),
                    date_validation = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    motif_rejet = table.Column<string>(type: "text", nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("feuille_temps_pkey", x => x.id);
                    table.CheckConstraint("ck_feuille_rejet", "statut <> 'REJETEE' OR motif_rejet IS NOT NULL");
                    table.CheckConstraint("feuille_temps_semaine_debut_check", "extract(isodow FROM semaine_debut) = 1");
                    table.CheckConstraint("feuille_temps_statut_check", "statut IN ('BROUILLON','SOUMISE','VALIDEE','REJETEE','VERROUILLEE')");
                    table.ForeignKey(
                        name: "feuille_temps_agent_id_fkey",
                        column: x => x.agent_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "feuille_temps_valideur_id_fkey",
                        column: x => x.valideur_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notification",
                schema: "commun",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    destinataire_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    titre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    message = table.Column<string>(type: "text", nullable: true),
                    lien = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    est_lue = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    date_lecture = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("notification_pkey", x => x.id);
                    table.ForeignKey(
                        name: "notification_destinataire_id_fkey",
                        column: x => x.destinataire_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "piece_jointe",
                schema: "commun",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    type_cible = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    cible_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nom_fichier = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    type_mime = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    taille_octets = table.Column<long>(type: "bigint", nullable: false),
                    chemin_stockage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    empreinte_sha256 = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    televerse_par_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("piece_jointe_pkey", x => x.id);
                    table.CheckConstraint("piece_jointe_taille_octets_check", "taille_octets >= 0");
                    table.ForeignKey(
                        name: "piece_jointe_televerse_par_id_fkey",
                        column: x => x.televerse_par_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "delegation",
                schema: "org",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    delegant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    delegataire_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entite_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_debut = table.Column<DateOnly>(type: "date", nullable: false),
                    date_fin = table.Column<DateOnly>(type: "date", nullable: false),
                    motif = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    peut_creer_projet = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    peut_affecter = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    peut_valider = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("delegation_pkey", x => x.id);
                    table.CheckConstraint("ck_delegation_agents", "delegant_id <> delegataire_id");
                    table.CheckConstraint("ck_delegation_dates", "date_fin >= date_debut");
                    table.ForeignKey(
                        name: "delegation_delegant_id_fkey",
                        column: x => x.delegant_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "delegation_delegataire_id_fkey",
                        column: x => x.delegataire_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "delegation_entite_id_fkey",
                        column: x => x.entite_id,
                        principalSchema: "org",
                        principalTable: "entite_organisationnelle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "projet",
                schema: "projet",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    intitule = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    objectifs = table.Column<string>(type: "text", nullable: true),
                    perimetre = table.Column<string>(type: "text", nullable: true),
                    type_projet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    priorite = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'NORMALE'"),
                    statut = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'BROUILLON'"),
                    meteo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValueSql: "'VERT'"),
                    classification = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValueSql: "'INTERNE'"),
                    mode_ordonnancement = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValueSql: "'BLOQUANT'"),
                    createur_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entite_creatrice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entite_porteuse_id = table.Column<Guid>(type: "uuid", nullable: true),
                    chef_projet_id = table.Column<Guid>(type: "uuid", nullable: true),
                    chemin_entite = table.Column<string>(type: "ltree", nullable: false),
                    date_debut_prevue = table.Column<DateOnly>(type: "date", nullable: true),
                    date_fin_prevue = table.Column<DateOnly>(type: "date", nullable: true),
                    date_debut_reelle = table.Column<DateOnly>(type: "date", nullable: true),
                    date_fin_reelle = table.Column<DateOnly>(type: "date", nullable: true),
                    charge_estimee_jh = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValueSql: "0"),
                    charge_consommee_jh = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValueSql: "0"),
                    avancement_pct = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false, defaultValueSql: "0"),
                    prochain_numero_ticket = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("projet_pkey", x => x.id);
                    table.CheckConstraint("ck_projet_dates_prevues", "date_fin_prevue IS NULL OR date_debut_prevue IS NULL OR date_fin_prevue >= date_debut_prevue");
                    table.CheckConstraint("ck_projet_dates_reelles", "date_fin_reelle IS NULL OR date_debut_reelle IS NULL OR date_fin_reelle >= date_debut_reelle");
                    table.CheckConstraint("projet_avancement_pct_check", "avancement_pct BETWEEN 0 AND 100");
                    table.CheckConstraint("projet_classification_check", "classification IN ('PUBLIC','INTERNE','CONFIDENTIEL','DIFFUSION_RESTREINTE')");
                    table.CheckConstraint("projet_meteo_check", "meteo IN ('VERT','ORANGE','ROUGE')");
                    table.CheckConstraint("projet_mode_ordonnancement_check", "mode_ordonnancement IN ('BLOQUANT','SOUPLE')");
                    table.CheckConstraint("projet_priorite_check", "priorite IN ('BASSE','NORMALE','HAUTE','CRITIQUE')");
                    table.CheckConstraint("projet_statut_check", "statut IN ('BROUILLON','EN_AFFECTATION','EN_COURS','SUSPENDU','CLOTURE','ARCHIVE')");
                    table.ForeignKey(
                        name: "projet_chef_projet_id_fkey",
                        column: x => x.chef_projet_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "projet_createur_id_fkey",
                        column: x => x.createur_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "projet_entite_creatrice_id_fkey",
                        column: x => x.entite_creatrice_id,
                        principalSchema: "org",
                        principalTable: "entite_organisationnelle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "projet_entite_porteuse_id_fkey",
                        column: x => x.entite_porteuse_id,
                        principalSchema: "org",
                        principalTable: "entite_organisationnelle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "projet_type_projet_id_fkey",
                        column: x => x.type_projet_id,
                        principalSchema: "projet",
                        principalTable: "type_projet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "rattachement",
                schema: "org",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entite_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_debut = table.Column<DateOnly>(type: "date", nullable: false),
                    date_fin = table.Column<DateOnly>(type: "date", nullable: true),
                    motif = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("rattachement_pkey", x => x.id);
                    table.CheckConstraint("ck_rattachement_dates", "date_fin IS NULL OR date_fin >= date_debut");
                    table.ForeignKey(
                        name: "rattachement_agent_id_fkey",
                        column: x => x.agent_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "rattachement_entite_id_fkey",
                        column: x => x.entite_id,
                        principalSchema: "org",
                        principalTable: "entite_organisationnelle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "baseline",
                schema: "planif",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    projet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    libelle = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    date_creation = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("baseline_pkey", x => x.id);
                    table.ForeignKey(
                        name: "baseline_projet_id_fkey",
                        column: x => x.projet_id,
                        principalSchema: "projet",
                        principalTable: "projet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "config_generation",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    projet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    stack_back = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'DOTNET_10'"),
                    stack_front = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'BLAZOR_WASM'"),
                    architecture = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'COUCHES_GP'"),
                    depot_gitlab_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    branche_cible = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValueSql: "'generation/socle'"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("config_generation_pkey", x => x.id);
                    table.CheckConstraint("config_generation_architecture_check", "architecture IN ('COUCHES_GP')");
                    table.CheckConstraint("config_generation_stack_back_check", "stack_back IN ('DOTNET_10')");
                    table.CheckConstraint("config_generation_stack_front_check", "stack_front IN ('BLAZOR_WASM')");
                    table.ForeignKey(
                        name: "config_generation_projet_id_fkey",
                        column: x => x.projet_id,
                        principalSchema: "projet",
                        principalTable: "projet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "element_raid",
                schema: "projet",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    projet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    titre = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    probabilite = table.Column<short>(type: "smallint", nullable: true),
                    impact = table.Column<short>(type: "smallint", nullable: true),
                    criticite = table.Column<short>(type: "smallint", nullable: true, computedColumnSql: "probabilite * impact", stored: true),
                    statut = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'OUVERT'"),
                    plan_action = table.Column<string>(type: "text", nullable: true),
                    echeance = table.Column<DateOnly>(type: "date", nullable: true),
                    responsable_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("element_raid_pkey", x => x.id);
                    table.CheckConstraint("ck_raid_risque", "type <> 'RISQUE' OR (probabilite IS NOT NULL AND impact IS NOT NULL)");
                    table.CheckConstraint("element_raid_impact_check", "impact BETWEEN 1 AND 5");
                    table.CheckConstraint("element_raid_probabilite_check", "probabilite BETWEEN 1 AND 5");
                    table.CheckConstraint("element_raid_statut_check", "statut IN ('OUVERT','EN_COURS','CLOS','ANNULE')");
                    table.CheckConstraint("element_raid_type_check", "type IN ('RISQUE','ACTION','PROBLEME','DECISION')");
                    table.ForeignKey(
                        name: "element_raid_projet_id_fkey",
                        column: x => x.projet_id,
                        principalSchema: "projet",
                        principalTable: "projet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "element_raid_responsable_id_fkey",
                        column: x => x.responsable_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "membre_projet",
                schema: "projet",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    projet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_projet = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'MEMBRE'"),
                    quota_pct = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false, defaultValueSql: "100"),
                    date_debut = table.Column<DateOnly>(type: "date", nullable: true),
                    date_fin = table.Column<DateOnly>(type: "date", nullable: true),
                    affecte_par_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("membre_projet_pkey", x => x.id);
                    table.CheckConstraint("ck_membre_dates", "date_fin IS NULL OR date_debut IS NULL OR date_fin >= date_debut");
                    table.CheckConstraint("membre_projet_quota_pct_check", "quota_pct BETWEEN 0 AND 100");
                    table.CheckConstraint("membre_projet_role_projet_check", "role_projet IN ('CHEF_PROJET','MEMBRE','OBSERVATEUR')");
                    table.ForeignKey(
                        name: "membre_projet_affecte_par_id_fkey",
                        column: x => x.affecte_par_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "membre_projet_agent_id_fkey",
                        column: x => x.agent_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "membre_projet_projet_id_fkey",
                        column: x => x.projet_id,
                        principalSchema: "projet",
                        principalTable: "projet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "modele_domaine",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    projet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nom = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1"),
                    statut = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false, defaultValueSql: "'BROUILLON'"),
                    namespace_racine = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    sgbd_cible = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'POSTGRESQL'"),
                    description = table.Column<string>(type: "text", nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("modele_domaine_pkey", x => x.id);
                    table.CheckConstraint("modele_domaine_sgbd_cible_check", "sgbd_cible IN ('POSTGRESQL')");
                    table.CheckConstraint("modele_domaine_statut_check", "statut IN ('BROUILLON','EN_VALIDATION','VALIDE','FIGE')");
                    table.CheckConstraint("modele_domaine_version_check", "version > 0");
                    table.ForeignKey(
                        name: "modele_domaine_projet_id_fkey",
                        column: x => x.projet_id,
                        principalSchema: "projet",
                        principalTable: "projet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "phase",
                schema: "planif",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    projet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    nom = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    objectif = table.Column<string>(type: "text", nullable: true),
                    ordre = table.Column<short>(type: "smallint", nullable: false),
                    statut = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'A_VENIR'"),
                    mode_enchainement = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'SEQUENTIEL'"),
                    pct_min_chevauchement = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    criteres_entree = table.Column<string>(type: "text", nullable: true),
                    criteres_sortie = table.Column<string>(type: "text", nullable: true),
                    date_debut_prevue = table.Column<DateOnly>(type: "date", nullable: true),
                    date_fin_prevue = table.Column<DateOnly>(type: "date", nullable: true),
                    date_debut_reelle = table.Column<DateOnly>(type: "date", nullable: true),
                    date_fin_reelle = table.Column<DateOnly>(type: "date", nullable: true),
                    charge_estimee_h = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValueSql: "0"),
                    avancement_pct = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false, defaultValueSql: "0"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("phase_pkey", x => x.id);
                    table.CheckConstraint("ck_phase_chevauchement", "mode_enchainement <> 'CHEVAUCHEMENT' OR pct_min_chevauchement IS NOT NULL");
                    table.CheckConstraint("ck_phase_dates", "date_fin_prevue IS NULL OR date_debut_prevue IS NULL OR date_fin_prevue >= date_debut_prevue");
                    table.CheckConstraint("phase_avancement_pct_check", "avancement_pct BETWEEN 0 AND 100");
                    table.CheckConstraint("phase_mode_enchainement_check", "mode_enchainement IN ('SEQUENTIEL','CHEVAUCHEMENT','PARALLELE')");
                    table.CheckConstraint("phase_ordre_check", "ordre > 0");
                    table.CheckConstraint("phase_pct_min_chevauchement_check", "pct_min_chevauchement BETWEEN 0 AND 100");
                    table.CheckConstraint("phase_statut_check", "statut IN ('A_VENIR','EN_COURS','EN_VALIDATION','VALIDEE','GELEE')");
                    table.ForeignKey(
                        name: "phase_projet_id_fkey",
                        column: x => x.projet_id,
                        principalSchema: "projet",
                        principalTable: "projet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "template_generation",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    config_id = table.Column<Guid>(type: "uuid", nullable: true),
                    couche = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    portee = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValueSql: "'ENTITE'"),
                    nom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    chemin_sortie = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    contenu = table.Column<string>(type: "text", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1"),
                    est_defaut = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("template_generation_pkey", x => x.id);
                    table.CheckConstraint("template_generation_couche_check", "couche IN ('ENTITE','DTO','MAPPING','REPOSITORY','SERVICE','ORCHESTRATION','CONTROLLER','API_CLIENT','FRONT_SERVICE','FRONT_MODEL','PAGE','SQL','DOCKER','CI')");
                    table.CheckConstraint("template_generation_portee_check", "portee IN ('ENTITE','MODULE','MODELE')");
                    table.ForeignKey(
                        name: "template_generation_config_id_fkey",
                        column: x => x.config_id,
                        principalSchema: "modelisation",
                        principalTable: "config_generation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "association",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    modele_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nom_technique = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                    nature = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false, defaultValueSql: "'ASSOCIATION'"),
                    regle_suppression = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValueSql: "'RESTRICT'"),
                    description = table.Column<string>(type: "text", nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("association_pkey", x => x.id);
                    table.CheckConstraint("association_nature_check", "nature IN ('ASSOCIATION','COMPOSITION','AGREGATION','POLYMORPHE')");
                    table.CheckConstraint("association_regle_suppression_check", "regle_suppression IN ('CASCADE','RESTRICT','SET_NULL')");
                    table.ForeignKey(
                        name: "association_modele_id_fkey",
                        column: x => x.modele_id,
                        principalSchema: "modelisation",
                        principalTable: "modele_domaine",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "diagramme",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    modele_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nom = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    contenu = table.Column<string>(type: "jsonb", nullable: true),
                    source_texte = table.Column<string>(type: "text", nullable: true),
                    statut_validation = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false, defaultValueSql: "'BROUILLON'"),
                    version = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("diagramme_pkey", x => x.id);
                    table.CheckConstraint("diagramme_statut_validation_check", "statut_validation IN ('BROUILLON','EN_VALIDATION','VALIDE','REJETE')");
                    table.CheckConstraint("diagramme_type_check", "type IN ('MCD','CLASSES','MLD','SEQUENCE','CAS_UTILISATION','ACTIVITE','ETATS','COMPOSANTS','DEPLOIEMENT','BPMN','C4')");
                    table.ForeignKey(
                        name: "diagramme_modele_id_fkey",
                        column: x => x.modele_id,
                        principalSchema: "modelisation",
                        principalTable: "modele_domaine",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "execution_generation",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    modele_id = table.Column<Guid>(type: "uuid", nullable: false),
                    version_modele = table.Column<int>(type: "integer", nullable: false),
                    lanceur_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_execution = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    statut = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValueSql: "'EN_COURS'"),
                    nb_fichiers = table.Column<int>(type: "integer", nullable: true),
                    branche = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    commit_sha = table.Column<string>(type: "character(40)", fixedLength: true, maxLength: 40, nullable: true),
                    rapport = table.Column<string>(type: "jsonb", nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("execution_generation_pkey", x => x.id);
                    table.CheckConstraint("execution_generation_statut_check", "statut IN ('EN_COURS','SUCCES','ECHEC')");
                    table.ForeignKey(
                        name: "execution_generation_lanceur_id_fkey",
                        column: x => x.lanceur_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "execution_generation_modele_id_fkey",
                        column: x => x.modele_id,
                        principalSchema: "modelisation",
                        principalTable: "modele_domaine",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "module",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    modele_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nom = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    schema_bd = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                    @namespace = table.Column<string>(name: "namespace", type: "character varying(100)", maxLength: 100, nullable: false),
                    ordre = table.Column<short>(type: "smallint", nullable: false, defaultValueSql: "1"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("module_pkey", x => x.id);
                    table.CheckConstraint("module_schema_bd_check", "schema_bd ~ '^[a-z][a-z0-9_]*$'");
                    table.ForeignKey(
                        name: "module_modele_id_fkey",
                        column: x => x.modele_id,
                        principalSchema: "modelisation",
                        principalTable: "modele_domaine",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "jalon",
                schema: "projet",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    projet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    phase_id = table.Column<Guid>(type: "uuid", nullable: true),
                    libelle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    date_prevue = table.Column<DateOnly>(type: "date", nullable: false),
                    date_reelle = table.Column<DateOnly>(type: "date", nullable: true),
                    statut = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'A_VENIR'"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("jalon_pkey", x => x.id);
                    table.CheckConstraint("jalon_statut_check", "statut IN ('A_VENIR','ATTEINT','EN_RETARD','ANNULE')");
                    table.ForeignKey(
                        name: "fk_jalon_phase",
                        column: x => x.phase_id,
                        principalSchema: "planif",
                        principalTable: "phase",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "jalon_projet_id_fkey",
                        column: x => x.projet_id,
                        principalSchema: "projet",
                        principalTable: "projet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "livrable",
                schema: "planif",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    phase_id = table.Column<Guid>(type: "uuid", nullable: false),
                    libelle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    statut = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'A_PRODUIRE'"),
                    date_prevue = table.Column<DateOnly>(type: "date", nullable: true),
                    date_livraison = table.Column<DateOnly>(type: "date", nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("livrable_pkey", x => x.id);
                    table.CheckConstraint("livrable_statut_check", "statut IN ('A_PRODUIRE','EN_COURS','LIVRE','VALIDE','REJETE')");
                    table.ForeignKey(
                        name: "livrable_phase_id_fkey",
                        column: x => x.phase_id,
                        principalSchema: "planif",
                        principalTable: "phase",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tache",
                schema: "planif",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    phase_id = table.Column<Guid>(type: "uuid", nullable: false),
                    projet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code_wbs = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    titre = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    ordre = table.Column<short>(type: "smallint", nullable: false),
                    mode_execution = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false, defaultValueSql: "'SEQUENTIELLE'"),
                    priorite = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'NORMALE'"),
                    statut_id = table.Column<Guid>(type: "uuid", nullable: false),
                    responsable_id = table.Column<Guid>(type: "uuid", nullable: true),
                    date_debut_prevue = table.Column<DateOnly>(type: "date", nullable: true),
                    date_fin_prevue = table.Column<DateOnly>(type: "date", nullable: true),
                    date_debut_reelle = table.Column<DateOnly>(type: "date", nullable: true),
                    date_fin_reelle = table.Column<DateOnly>(type: "date", nullable: true),
                    charge_estimee_h = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    charge_optimiste_h = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    charge_pessimiste_h = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    reste_a_faire_h = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    avancement_pct = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false, defaultValueSql: "0"),
                    marge_totale_j = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true),
                    marge_libre_j = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true),
                    est_critique = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    est_recurrente = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    regle_recurrence = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tache_pkey", x => x.id);
                    table.CheckConstraint("ck_tache_charges", "(charge_optimiste_h IS NULL OR charge_estimee_h IS NULL OR charge_optimiste_h <= charge_estimee_h) AND (charge_pessimiste_h IS NULL OR charge_estimee_h IS NULL OR charge_pessimiste_h >= charge_estimee_h)");
                    table.CheckConstraint("ck_tache_dates", "date_fin_prevue IS NULL OR date_debut_prevue IS NULL OR date_fin_prevue >= date_debut_prevue");
                    table.CheckConstraint("ck_tache_recurrence", "NOT est_recurrente OR regle_recurrence IS NOT NULL");
                    table.CheckConstraint("tache_avancement_pct_check", "avancement_pct BETWEEN 0 AND 100");
                    table.CheckConstraint("tache_mode_execution_check", "mode_execution IN ('SEQUENTIELLE','PARALLELE')");
                    table.CheckConstraint("tache_ordre_check", "ordre > 0");
                    table.CheckConstraint("tache_priorite_check", "priorite IN ('BASSE','NORMALE','HAUTE','CRITIQUE')");
                    table.ForeignKey(
                        name: "tache_phase_id_fkey",
                        column: x => x.phase_id,
                        principalSchema: "planif",
                        principalTable: "phase",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "tache_projet_id_fkey",
                        column: x => x.projet_id,
                        principalSchema: "projet",
                        principalTable: "projet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "tache_responsable_id_fkey",
                        column: x => x.responsable_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "tache_statut_id_fkey",
                        column: x => x.statut_id,
                        principalSchema: "planif",
                        principalTable: "statut_workflow",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "validation_phase",
                schema: "planif",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    phase_id = table.Column<Guid>(type: "uuid", nullable: false),
                    valideur_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tour = table.Column<short>(type: "smallint", nullable: false, defaultValueSql: "1"),
                    ordre_niveau = table.Column<short>(type: "smallint", nullable: false),
                    decision = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false, defaultValueSql: "'EN_ATTENTE'"),
                    commentaire = table.Column<string>(type: "text", nullable: true),
                    date_decision = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("validation_phase_pkey", x => x.id);
                    table.CheckConstraint("ck_validation_rejet", "decision <> 'REJETEE' OR commentaire IS NOT NULL");
                    table.CheckConstraint("validation_phase_decision_check", "decision IN ('EN_ATTENTE','APPROUVEE','REJETEE')");
                    table.ForeignKey(
                        name: "validation_phase_phase_id_fkey",
                        column: x => x.phase_id,
                        principalSchema: "planif",
                        principalTable: "phase",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "validation_phase_valideur_id_fkey",
                        column: x => x.valideur_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "element_diagramme",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    diagramme_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type_element = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    element_id = table.Column<Guid>(type: "uuid", nullable: false),
                    x = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValueSql: "0"),
                    y = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValueSql: "0"),
                    largeur = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    hauteur = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    style = table.Column<string>(type: "jsonb", nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("element_diagramme_pkey", x => x.id);
                    table.CheckConstraint("element_diagramme_type_element_check", "type_element IN ('ENTITE','ASSOCIATION','ENUMERATION','TABLE','NOTE')");
                    table.ForeignKey(
                        name: "element_diagramme_diagramme_id_fkey",
                        column: x => x.diagramme_id,
                        principalSchema: "modelisation",
                        principalTable: "diagramme",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "enumeration",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    module_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nom_technique = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                    stockage = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValueSql: "'TEXTE'"),
                    description = table.Column<string>(type: "text", nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("enumeration_pkey", x => x.id);
                    table.CheckConstraint("enumeration_stockage_check", "stockage IN ('ENTIER','TEXTE')");
                    table.ForeignKey(
                        name: "enumeration_module_id_fkey",
                        column: x => x.module_id,
                        principalSchema: "modelisation",
                        principalTable: "module",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "baseline_tache",
                schema: "planif",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    baseline_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tache_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_debut = table.Column<DateOnly>(type: "date", nullable: true),
                    date_fin = table.Column<DateOnly>(type: "date", nullable: true),
                    charge_h = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("baseline_tache_pkey", x => x.id);
                    table.ForeignKey(
                        name: "baseline_tache_baseline_id_fkey",
                        column: x => x.baseline_id,
                        principalSchema: "planif",
                        principalTable: "baseline",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "baseline_tache_tache_id_fkey",
                        column: x => x.tache_id,
                        principalSchema: "planif",
                        principalTable: "tache",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dependance_tache",
                schema: "planif",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    predecesseur_id = table.Column<Guid>(type: "uuid", nullable: false),
                    successeur_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type_lien = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false, defaultValueSql: "'FIN_DEBUT'"),
                    delai_j = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false, defaultValueSql: "0"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("dependance_tache_pkey", x => x.id);
                    table.CheckConstraint("ck_dependance_differente", "predecesseur_id <> successeur_id");
                    table.CheckConstraint("dependance_tache_type_lien_check", "type_lien IN ('FIN_DEBUT','DEBUT_DEBUT','FIN_FIN','DEBUT_FIN')");
                    table.ForeignKey(
                        name: "dependance_tache_predecesseur_id_fkey",
                        column: x => x.predecesseur_id,
                        principalSchema: "planif",
                        principalTable: "tache",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "dependance_tache_successeur_id_fkey",
                        column: x => x.successeur_id,
                        principalSchema: "planif",
                        principalTable: "tache",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "derogation_ordre",
                schema: "planif",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    tache_id = table.Column<Guid>(type: "uuid", nullable: false),
                    predecesseur_id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    justification = table.Column<string>(type: "text", nullable: false),
                    date_derogation = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("derogation_ordre_pkey", x => x.id);
                    table.ForeignKey(
                        name: "derogation_ordre_agent_id_fkey",
                        column: x => x.agent_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "derogation_ordre_predecesseur_id_fkey",
                        column: x => x.predecesseur_id,
                        principalSchema: "planif",
                        principalTable: "tache",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "derogation_ordre_tache_id_fkey",
                        column: x => x.tache_id,
                        principalSchema: "planif",
                        principalTable: "tache",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ticket",
                schema: "planif",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    tache_id = table.Column<Guid>(type: "uuid", nullable: false),
                    projet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    numero = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    type_ticket_id = table.Column<Guid>(type: "uuid", nullable: false),
                    statut_id = table.Column<Guid>(type: "uuid", nullable: false),
                    titre = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    priorite = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'NORMALE'"),
                    severite = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    rang = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "0"),
                    est_bloquant = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    environnement = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    etapes_reproduction = table.Column<string>(type: "text", nullable: true),
                    criteres_acceptation = table.Column<string>(type: "text", nullable: true),
                    estimation_h = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    reste_a_faire_h = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    date_echeance = table.Column<DateOnly>(type: "date", nullable: true),
                    assigne_id = table.Column<Guid>(type: "uuid", nullable: true),
                    rapporteur_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ticket_pkey", x => x.id);
                    table.CheckConstraint("ticket_priorite_check", "priorite IN ('BASSE','NORMALE','HAUTE','CRITIQUE')");
                    table.CheckConstraint("ticket_severite_check", "severite IN ('MINEURE','MAJEURE','CRITIQUE','BLOQUANTE')");
                    table.ForeignKey(
                        name: "ticket_assigne_id_fkey",
                        column: x => x.assigne_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "ticket_projet_id_fkey",
                        column: x => x.projet_id,
                        principalSchema: "projet",
                        principalTable: "projet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ticket_rapporteur_id_fkey",
                        column: x => x.rapporteur_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "ticket_statut_id_fkey",
                        column: x => x.statut_id,
                        principalSchema: "planif",
                        principalTable: "statut_workflow",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "ticket_tache_id_fkey",
                        column: x => x.tache_id,
                        principalSchema: "planif",
                        principalTable: "tache",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ticket_type_ticket_id_fkey",
                        column: x => x.type_ticket_id,
                        principalSchema: "planif",
                        principalTable: "type_ticket",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "valeur_enumeration",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    enumeration_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    libelle = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    valeur = table.Column<int>(type: "integer", nullable: true),
                    ordre = table.Column<short>(type: "smallint", nullable: false, defaultValueSql: "1"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("valeur_enumeration_pkey", x => x.id);
                    table.ForeignKey(
                        name: "valeur_enumeration_enumeration_id_fkey",
                        column: x => x.enumeration_id,
                        principalSchema: "modelisation",
                        principalTable: "enumeration",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lien_ticket",
                schema: "planif",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cible_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type_lien = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("lien_ticket_pkey", x => x.id);
                    table.CheckConstraint("ck_lien_ticket_different", "source_id <> cible_id");
                    table.CheckConstraint("lien_ticket_type_lien_check", "type_lien IN ('BLOQUE','DUPLIQUE','LIE_A','CAUSE')");
                    table.ForeignKey(
                        name: "lien_ticket_cible_id_fkey",
                        column: x => x.cible_id,
                        principalSchema: "planif",
                        principalTable: "ticket",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "lien_ticket_source_id_fkey",
                        column: x => x.source_id,
                        principalSchema: "planif",
                        principalTable: "ticket",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "saisie_temps",
                schema: "temps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    feuille_id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_saisie = table.Column<DateOnly>(type: "date", nullable: false),
                    duree_h = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    categorie_id = table.Column<Guid>(type: "uuid", nullable: false),
                    projet_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tache_id = table.Column<Guid>(type: "uuid", nullable: true),
                    ticket_id = table.Column<Guid>(type: "uuid", nullable: true),
                    commentaire = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    source = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValueSql: "'MANUEL'"),
                    chrono_debut = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    chrono_fin = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    reference_gitlab = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("saisie_temps_pkey", x => x.id);
                    table.CheckConstraint("ck_saisie_chrono", "chrono_fin IS NULL OR chrono_debut IS NULL OR chrono_fin >= chrono_debut");
                    table.CheckConstraint("ck_saisie_tache_projet", "tache_id IS NULL OR projet_id IS NOT NULL");
                    table.CheckConstraint("ck_saisie_ticket_tache", "ticket_id IS NULL OR tache_id IS NOT NULL");
                    table.CheckConstraint("saisie_temps_duree_h_check", "duree_h > 0 AND duree_h <= 24");
                    table.CheckConstraint("saisie_temps_source_check", "source IN ('MANUEL','CHRONO','GITLAB')");
                    table.ForeignKey(
                        name: "saisie_temps_agent_id_fkey",
                        column: x => x.agent_id,
                        principalSchema: "org",
                        principalTable: "agent",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "saisie_temps_categorie_id_fkey",
                        column: x => x.categorie_id,
                        principalSchema: "temps",
                        principalTable: "categorie_activite",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "saisie_temps_feuille_id_fkey",
                        column: x => x.feuille_id,
                        principalSchema: "temps",
                        principalTable: "feuille_temps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "saisie_temps_projet_id_fkey",
                        column: x => x.projet_id,
                        principalSchema: "projet",
                        principalTable: "projet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "saisie_temps_tache_id_fkey",
                        column: x => x.tache_id,
                        principalSchema: "planif",
                        principalTable: "tache",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "saisie_temps_ticket_id_fkey",
                        column: x => x.ticket_id,
                        principalSchema: "planif",
                        principalTable: "ticket",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "attribut",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    entite_id = table.Column<Guid>(type: "uuid", nullable: true),
                    association_id = table.Column<Guid>(type: "uuid", nullable: true),
                    enumeration_id = table.Column<Guid>(type: "uuid", nullable: true),
                    nom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nom_technique = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                    libelle_ui = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    type_logique = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    longueur = table.Column<int>(type: "integer", nullable: true),
                    precision_num = table.Column<short>(type: "smallint", nullable: true),
                    echelle = table.Column<short>(type: "smallint", nullable: true),
                    est_identifiant = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    est_obligatoire = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    est_unique = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    est_calcule = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    valeur_defaut = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    visibilite = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValueSql: "'PUBLIC'"),
                    regex_validation = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    valeur_min = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    valeur_max = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    controle_ui = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    afficher_dans_liste = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    est_filtrable = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    est_triable = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    ordre = table.Column<short>(type: "smallint", nullable: false, defaultValueSql: "1"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("attribut_pkey", x => x.id);
                    table.CheckConstraint("attribut_controle_ui_check", "controle_ui IN ('TEXTE','ZONE','NOMBRE','DATE','LISTE','CASE','FICHIER')");
                    table.CheckConstraint("attribut_echelle_check", "echelle >= 0");
                    table.CheckConstraint("attribut_longueur_check", "longueur > 0");
                    table.CheckConstraint("attribut_nom_technique_check", "nom_technique ~ '^[a-z][a-z0-9_]*$'");
                    table.CheckConstraint("attribut_precision_num_check", "precision_num BETWEEN 1 AND 38");
                    table.CheckConstraint("attribut_type_logique_check", "type_logique IN ('TEXTE','TEXTE_LONG','ENTIER','DECIMAL','BOOLEEN','DATE','DATE_HEURE','UUID','ENUM','JSON','FICHIER')");
                    table.CheckConstraint("attribut_visibilite_check", "visibilite IN ('PUBLIC','PROTEGE','PRIVE')");
                    table.CheckConstraint("ck_attribut_decimal", "type_logique <> 'DECIMAL' OR precision_num IS NOT NULL");
                    table.CheckConstraint("ck_attribut_echelle", "echelle IS NULL OR precision_num IS NULL OR echelle <= precision_num");
                    table.CheckConstraint("ck_attribut_enum", "(type_logique = 'ENUM') = (enumeration_id IS NOT NULL)");
                    table.CheckConstraint("ck_attribut_proprietaire", "num_nonnulls(entite_id, association_id) = 1");
                    table.ForeignKey(
                        name: "attribut_association_id_fkey",
                        column: x => x.association_id,
                        principalSchema: "modelisation",
                        principalTable: "association",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "attribut_enumeration_id_fkey",
                        column: x => x.enumeration_id,
                        principalSchema: "modelisation",
                        principalTable: "enumeration",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "entite",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    module_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entite_parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    nom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nom_technique = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                    nom_pluriel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    libelle_ui = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    stereotype = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false, defaultValueSql: "'ENTITE'"),
                    est_abstraite = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    strategie_heritage = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    est_racine_agregat = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    avec_audit = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    avec_suppression_logique = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    avec_concurrence = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    generer_api = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    generer_pages = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    attribut_libelle_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("entite_pkey", x => x.id);
                    table.CheckConstraint("ck_entite_heritage", "entite_parent_id IS NULL OR entite_parent_id <> id");
                    table.CheckConstraint("entite_nom_technique_check", "nom_technique ~ '^[a-z][a-z0-9_]*$'");
                    table.CheckConstraint("entite_stereotype_check", "stereotype IN ('ENTITE','REFERENTIEL','VALEUR','INTERFACE')");
                    table.CheckConstraint("entite_strategie_heritage_check", "strategie_heritage IN ('TPH','TPT','TPC')");
                    table.ForeignKey(
                        name: "entite_entite_parent_id_fkey",
                        column: x => x.entite_parent_id,
                        principalSchema: "modelisation",
                        principalTable: "entite",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "entite_module_id_fkey",
                        column: x => x.module_id,
                        principalSchema: "modelisation",
                        principalTable: "module",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_entite_attribut_libelle",
                        column: x => x.attribut_libelle_id,
                        principalSchema: "modelisation",
                        principalTable: "attribut",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "entite_interface",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    entite_id = table.Column<Guid>(type: "uuid", nullable: false),
                    interface_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("entite_interface_pkey", x => x.id);
                    table.CheckConstraint("ck_entite_interface", "entite_id <> interface_id");
                    table.ForeignKey(
                        name: "entite_interface_entite_id_fkey",
                        column: x => x.entite_id,
                        principalSchema: "modelisation",
                        principalTable: "entite",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "entite_interface_interface_id_fkey",
                        column: x => x.interface_id,
                        principalSchema: "modelisation",
                        principalTable: "entite",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "index_entite",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    entite_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nom = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                    est_unique = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("index_entite_pkey", x => x.id);
                    table.ForeignKey(
                        name: "index_entite_entite_id_fkey",
                        column: x => x.entite_id,
                        principalSchema: "modelisation",
                        principalTable: "entite",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "methode",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    entite_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    type_retour = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false, defaultValueSql: "'void'"),
                    visibilite = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValueSql: "'PUBLIC'"),
                    couche_cible = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false, defaultValueSql: "'SERVICE'"),
                    est_async = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    est_statique = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    description = table.Column<string>(type: "text", nullable: true),
                    ordre = table.Column<short>(type: "smallint", nullable: false, defaultValueSql: "1"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("methode_pkey", x => x.id);
                    table.CheckConstraint("methode_couche_cible_check", "couche_cible IN ('ENTITE','SERVICE','ORCHESTRATION')");
                    table.CheckConstraint("methode_visibilite_check", "visibilite IN ('PUBLIC','PROTEGE','PRIVE')");
                    table.ForeignKey(
                        name: "methode_entite_id_fkey",
                        column: x => x.entite_id,
                        principalSchema: "modelisation",
                        principalTable: "entite",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "patte",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    association_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entite_id = table.Column<Guid>(type: "uuid", nullable: false),
                    card_min = table.Column<short>(type: "smallint", nullable: false),
                    card_max = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    role = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: true),
                    nom_navigation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    est_navigable = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    ordre = table.Column<short>(type: "smallint", nullable: false, defaultValueSql: "1"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("patte_pkey", x => x.id);
                    table.CheckConstraint("patte_card_max_check", "card_max IN ('1','N')");
                    table.CheckConstraint("patte_card_min_check", "card_min IN (0,1)");
                    table.ForeignKey(
                        name: "patte_association_id_fkey",
                        column: x => x.association_id,
                        principalSchema: "modelisation",
                        principalTable: "association",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "patte_entite_id_fkey",
                        column: x => x.entite_id,
                        principalSchema: "modelisation",
                        principalTable: "entite",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "table_bd",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    modele_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entite_source_id = table.Column<Guid>(type: "uuid", nullable: true),
                    association_source_id = table.Column<Guid>(type: "uuid", nullable: true),
                    nom = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                    schema_bd = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                    est_table_association = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    est_surchargee = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("table_bd_pkey", x => x.id);
                    table.ForeignKey(
                        name: "table_bd_association_source_id_fkey",
                        column: x => x.association_source_id,
                        principalSchema: "modelisation",
                        principalTable: "association",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "table_bd_entite_source_id_fkey",
                        column: x => x.entite_source_id,
                        principalSchema: "modelisation",
                        principalTable: "entite",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "table_bd_modele_id_fkey",
                        column: x => x.modele_id,
                        principalSchema: "modelisation",
                        principalTable: "modele_domaine",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "index_attribut",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    index_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attribut_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ordre = table.Column<short>(type: "smallint", nullable: false, defaultValueSql: "1"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("index_attribut_pkey", x => x.id);
                    table.ForeignKey(
                        name: "index_attribut_attribut_id_fkey",
                        column: x => x.attribut_id,
                        principalSchema: "modelisation",
                        principalTable: "attribut",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "index_attribut_index_id_fkey",
                        column: x => x.index_id,
                        principalSchema: "modelisation",
                        principalTable: "index_entite",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "parametre_methode",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    methode_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    type = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ordre = table.Column<short>(type: "smallint", nullable: false, defaultValueSql: "1"),
                    est_optionnel = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    valeur_defaut = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("parametre_methode_pkey", x => x.id);
                    table.ForeignKey(
                        name: "parametre_methode_methode_id_fkey",
                        column: x => x.methode_id,
                        principalSchema: "modelisation",
                        principalTable: "methode",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "colonne",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    table_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attribut_source_id = table.Column<Guid>(type: "uuid", nullable: true),
                    nom = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                    type_physique = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    est_nullable = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    est_cle_primaire = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    valeur_defaut = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ordre = table.Column<short>(type: "smallint", nullable: false, defaultValueSql: "1"),
                    est_surchargee = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("colonne_pkey", x => x.id);
                    table.ForeignKey(
                        name: "colonne_attribut_source_id_fkey",
                        column: x => x.attribut_source_id,
                        principalSchema: "modelisation",
                        principalTable: "attribut",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "colonne_table_id_fkey",
                        column: x => x.table_id,
                        principalSchema: "modelisation",
                        principalTable: "table_bd",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cle_etrangere",
                schema: "modelisation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    table_id = table.Column<Guid>(type: "uuid", nullable: false),
                    colonne_id = table.Column<Guid>(type: "uuid", nullable: false),
                    table_referencee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    colonne_referencee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patte_source_id = table.Column<Guid>(type: "uuid", nullable: true),
                    nom = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                    on_delete = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValueSql: "'RESTRICT'"),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cree_par = table.Column<Guid>(type: "uuid", nullable: true),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par = table.Column<Guid>(type: "uuid", nullable: true),
                    est_supprime = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    supprime_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    supprime_par = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("cle_etrangere_pkey", x => x.id);
                    table.CheckConstraint("cle_etrangere_on_delete_check", "on_delete IN ('CASCADE','RESTRICT','SET_NULL')");
                    table.ForeignKey(
                        name: "cle_etrangere_colonne_id_fkey",
                        column: x => x.colonne_id,
                        principalSchema: "modelisation",
                        principalTable: "colonne",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "cle_etrangere_colonne_referencee_id_fkey",
                        column: x => x.colonne_referencee_id,
                        principalSchema: "modelisation",
                        principalTable: "colonne",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "cle_etrangere_patte_source_id_fkey",
                        column: x => x.patte_source_id,
                        principalSchema: "modelisation",
                        principalTable: "patte",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "cle_etrangere_table_id_fkey",
                        column: x => x.table_id,
                        principalSchema: "modelisation",
                        principalTable: "table_bd",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "cle_etrangere_table_referencee_id_fkey",
                        column: x => x.table_referencee_id,
                        principalSchema: "modelisation",
                        principalTable: "table_bd",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_absence_agent_dates",
                schema: "org",
                table: "absence",
                columns: new[] { "agent_id", "date_debut", "date_fin" });

            migrationBuilder.CreateIndex(
                name: "ix_affectation_affecte_par",
                schema: "projet",
                table: "affectation_projet",
                column: "affecte_par_id");

            migrationBuilder.CreateIndex(
                name: "ix_affectation_cible_statut",
                schema: "projet",
                table: "affectation_projet",
                columns: new[] { "entite_cible_id", "statut" });

            migrationBuilder.CreateIndex(
                name: "ix_affectation_precedente",
                schema: "projet",
                table: "affectation_projet",
                column: "precedente_id");

            migrationBuilder.CreateIndex(
                name: "ix_affectation_repondu_par",
                schema: "projet",
                table: "affectation_projet",
                column: "repondu_par_id");

            migrationBuilder.CreateIndex(
                name: "ix_affectation_source",
                schema: "projet",
                table: "affectation_projet",
                column: "entite_source_id");

            migrationBuilder.CreateIndex(
                name: "ux_affectation_active",
                schema: "projet",
                table: "affectation_projet",
                column: "projet_id",
                unique: true,
                filter: "est_active AND NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_agent_entite",
                schema: "org",
                table: "agent",
                column: "entite_id");

            migrationBuilder.CreateIndex(
                name: "ix_agent_fonction",
                schema: "org",
                table: "agent",
                column: "fonction_id");

            // Index sur expression (01-schema.sql) : EF ne sait que l'index sur colonne déclaré dans AgentConfiguration.
            migrationBuilder.Sql("CREATE UNIQUE INDEX ux_agent_email ON org.agent (lower(email)) WHERE NOT est_supprime;");

            migrationBuilder.CreateIndex(
                name: "ux_agent_keycloak",
                schema: "org",
                table: "agent",
                column: "keycloak_sub",
                unique: true,
                filter: "keycloak_sub IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ux_agent_matricule",
                schema: "org",
                table: "agent",
                column: "matricule",
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_agent_competence_comp",
                schema: "org",
                table: "agent_competence",
                column: "competence_id");

            migrationBuilder.CreateIndex(
                name: "ux_agent_competence",
                schema: "org",
                table: "agent_competence",
                columns: new[] { "agent_id", "competence_id" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_association_modele",
                schema: "modelisation",
                table: "association",
                column: "modele_id");

            migrationBuilder.CreateIndex(
                name: "ix_attribut_enumeration",
                schema: "modelisation",
                table: "attribut",
                column: "enumeration_id");

            migrationBuilder.CreateIndex(
                name: "ux_attribut_association",
                schema: "modelisation",
                table: "attribut",
                columns: new[] { "association_id", "nom_technique" },
                unique: true,
                filter: "association_id IS NOT NULL AND NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ux_attribut_entite",
                schema: "modelisation",
                table: "attribut",
                columns: new[] { "entite_id", "nom_technique" },
                unique: true,
                filter: "entite_id IS NOT NULL AND NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_baseline_projet",
                schema: "planif",
                table: "baseline",
                column: "projet_id");

            migrationBuilder.CreateIndex(
                name: "ix_baseline_tache_tache",
                schema: "planif",
                table: "baseline_tache",
                column: "tache_id");

            migrationBuilder.CreateIndex(
                name: "ux_baseline_tache",
                schema: "planif",
                table: "baseline_tache",
                columns: new[] { "baseline_id", "tache_id" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ux_categorie_activite_code",
                schema: "temps",
                table: "categorie_activite",
                column: "code",
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_cle_etrangere_col_ref",
                schema: "modelisation",
                table: "cle_etrangere",
                column: "colonne_referencee_id");

            migrationBuilder.CreateIndex(
                name: "ix_cle_etrangere_colonne",
                schema: "modelisation",
                table: "cle_etrangere",
                column: "colonne_id");

            migrationBuilder.CreateIndex(
                name: "ix_cle_etrangere_patte",
                schema: "modelisation",
                table: "cle_etrangere",
                column: "patte_source_id");

            migrationBuilder.CreateIndex(
                name: "ix_cle_etrangere_table",
                schema: "modelisation",
                table: "cle_etrangere",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "ix_cle_etrangere_table_ref",
                schema: "modelisation",
                table: "cle_etrangere",
                column: "table_referencee_id");

            migrationBuilder.CreateIndex(
                name: "ix_colonne_attribut",
                schema: "modelisation",
                table: "colonne",
                column: "attribut_source_id");

            migrationBuilder.CreateIndex(
                name: "ux_colonne",
                schema: "modelisation",
                table: "colonne",
                columns: new[] { "table_id", "nom" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_commentaire_auteur",
                schema: "commun",
                table: "commentaire",
                column: "auteur_id");

            migrationBuilder.CreateIndex(
                name: "ix_commentaire_cible",
                schema: "commun",
                table: "commentaire",
                columns: new[] { "type_cible", "cible_id", "cree_le" });

            migrationBuilder.CreateIndex(
                name: "ix_commentaire_parent",
                schema: "commun",
                table: "commentaire",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ux_competence_code",
                schema: "org",
                table: "competence",
                column: "code",
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ux_config_generation_projet",
                schema: "modelisation",
                table: "config_generation",
                column: "projet_id",
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ux_correspondance_type",
                schema: "modelisation",
                table: "correspondance_type",
                columns: new[] { "type_logique", "cible" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_delegation_delegant",
                schema: "org",
                table: "delegation",
                column: "delegant_id");

            migrationBuilder.CreateIndex(
                name: "ix_delegation_delegataire",
                schema: "org",
                table: "delegation",
                columns: new[] { "delegataire_id", "date_debut", "date_fin" });

            migrationBuilder.CreateIndex(
                name: "ix_delegation_entite",
                schema: "org",
                table: "delegation",
                column: "entite_id");

            migrationBuilder.CreateIndex(
                name: "ix_dependance_successeur",
                schema: "planif",
                table: "dependance_tache",
                column: "successeur_id");

            migrationBuilder.CreateIndex(
                name: "ux_dependance",
                schema: "planif",
                table: "dependance_tache",
                columns: new[] { "predecesseur_id", "successeur_id" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_derogation_agent",
                schema: "planif",
                table: "derogation_ordre",
                column: "agent_id");

            migrationBuilder.CreateIndex(
                name: "ix_derogation_predecesseur",
                schema: "planif",
                table: "derogation_ordre",
                column: "predecesseur_id");

            migrationBuilder.CreateIndex(
                name: "ix_derogation_tache",
                schema: "planif",
                table: "derogation_ordre",
                column: "tache_id");

            migrationBuilder.CreateIndex(
                name: "ix_diagramme_modele",
                schema: "modelisation",
                table: "diagramme",
                columns: new[] { "modele_id", "type" });

            migrationBuilder.CreateIndex(
                name: "ux_element_diagramme",
                schema: "modelisation",
                table: "element_diagramme",
                columns: new[] { "diagramme_id", "type_element", "element_id" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_raid_projet_type",
                schema: "projet",
                table: "element_raid",
                columns: new[] { "projet_id", "type", "statut" });

            migrationBuilder.CreateIndex(
                name: "ix_raid_responsable",
                schema: "projet",
                table: "element_raid",
                column: "responsable_id");

            migrationBuilder.CreateIndex(
                name: "ix_entite_parent",
                schema: "modelisation",
                table: "entite",
                column: "entite_parent_id");

            migrationBuilder.CreateIndex(
                name: "ux_entite_nom_technique",
                schema: "modelisation",
                table: "entite",
                columns: new[] { "module_id", "nom_technique" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_entite_interface_itf",
                schema: "modelisation",
                table: "entite_interface",
                column: "interface_id");

            migrationBuilder.CreateIndex(
                name: "ux_entite_interface",
                schema: "modelisation",
                table: "entite_interface",
                columns: new[] { "entite_id", "interface_id" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_entite_chemin",
                schema: "org",
                table: "entite_organisationnelle",
                column: "chemin")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "ix_entite_parent",
                schema: "org",
                table: "entite_organisationnelle",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ux_entite_code",
                schema: "org",
                table: "entite_organisationnelle",
                column: "code",
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ux_entite_responsable",
                schema: "org",
                table: "entite_organisationnelle",
                column: "responsable_id",
                unique: true,
                filter: "responsable_id IS NOT NULL AND NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ux_enumeration_nom",
                schema: "modelisation",
                table: "enumeration",
                columns: new[] { "module_id", "nom_technique" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_execution_lanceur",
                schema: "modelisation",
                table: "execution_generation",
                column: "lanceur_id");

            migrationBuilder.CreateIndex(
                name: "ix_execution_modele",
                schema: "modelisation",
                table: "execution_generation",
                columns: new[] { "modele_id", "date_execution" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "ix_feuille_valideur_statut",
                schema: "temps",
                table: "feuille_temps",
                columns: new[] { "valideur_id", "statut" });

            migrationBuilder.CreateIndex(
                name: "ux_feuille_agent_semaine",
                schema: "temps",
                table: "feuille_temps",
                columns: new[] { "agent_id", "semaine_debut" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ux_fonction_code",
                schema: "org",
                table: "fonction",
                column: "code",
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_historique_agent",
                schema: "commun",
                table: "historique",
                columns: new[] { "agent_id", "date_action" });

            migrationBuilder.CreateIndex(
                name: "ix_historique_cible",
                schema: "commun",
                table: "historique",
                columns: new[] { "type_cible", "cible_id", "date_action" });

            migrationBuilder.CreateIndex(
                name: "ix_index_attribut_attr",
                schema: "modelisation",
                table: "index_attribut",
                column: "attribut_id");

            migrationBuilder.CreateIndex(
                name: "ux_index_attribut",
                schema: "modelisation",
                table: "index_attribut",
                columns: new[] { "index_id", "attribut_id" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_index_entite",
                schema: "modelisation",
                table: "index_entite",
                column: "entite_id");

            migrationBuilder.CreateIndex(
                name: "ix_jalon_phase",
                schema: "projet",
                table: "jalon",
                column: "phase_id");

            migrationBuilder.CreateIndex(
                name: "ix_jalon_projet_date",
                schema: "projet",
                table: "jalon",
                columns: new[] { "projet_id", "date_prevue" });

            migrationBuilder.CreateIndex(
                name: "ux_jour_ferie_date",
                schema: "org",
                table: "jour_ferie",
                column: "date_ferie",
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_lien_ticket_cible",
                schema: "planif",
                table: "lien_ticket",
                column: "cible_id");

            migrationBuilder.CreateIndex(
                name: "ux_lien_ticket",
                schema: "planif",
                table: "lien_ticket",
                columns: new[] { "source_id", "cible_id", "type_lien" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_livrable_phase",
                schema: "planif",
                table: "livrable",
                column: "phase_id");

            migrationBuilder.CreateIndex(
                name: "ix_membre_affecte_par",
                schema: "projet",
                table: "membre_projet",
                column: "affecte_par_id");

            migrationBuilder.CreateIndex(
                name: "ix_membre_agent",
                schema: "projet",
                table: "membre_projet",
                column: "agent_id");

            migrationBuilder.CreateIndex(
                name: "ux_membre_projet_agent",
                schema: "projet",
                table: "membre_projet",
                columns: new[] { "projet_id", "agent_id" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_methode_entite",
                schema: "modelisation",
                table: "methode",
                column: "entite_id");

            migrationBuilder.CreateIndex(
                name: "ux_modele_projet_version",
                schema: "modelisation",
                table: "modele_domaine",
                columns: new[] { "projet_id", "version" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ux_modele_phase_ordre",
                schema: "projet",
                table: "modele_phase",
                columns: new[] { "type_projet_id", "ordre" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ux_module_code",
                schema: "modelisation",
                table: "module",
                columns: new[] { "modele_id", "code" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_notification_non_lues",
                schema: "commun",
                table: "notification",
                columns: new[] { "destinataire_id", "cree_le" },
                descending: new[] { false, true },
                filter: "NOT est_lue AND NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ux_parametre_cle",
                schema: "commun",
                table: "parametre",
                column: "cle",
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ux_parametre_methode",
                schema: "modelisation",
                table: "parametre_methode",
                columns: new[] { "methode_id", "nom" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_patte_association",
                schema: "modelisation",
                table: "patte",
                column: "association_id");

            migrationBuilder.CreateIndex(
                name: "ix_patte_entite",
                schema: "modelisation",
                table: "patte",
                column: "entite_id");

            migrationBuilder.CreateIndex(
                name: "ix_phase_projet_ordre",
                schema: "planif",
                table: "phase",
                columns: new[] { "projet_id", "ordre" },
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_piece_jointe_auteur",
                schema: "commun",
                table: "piece_jointe",
                column: "televerse_par_id");

            migrationBuilder.CreateIndex(
                name: "ix_piece_jointe_cible",
                schema: "commun",
                table: "piece_jointe",
                columns: new[] { "type_cible", "cible_id" });

            migrationBuilder.CreateIndex(
                name: "ix_projet_chef",
                schema: "projet",
                table: "projet",
                column: "chef_projet_id");

            migrationBuilder.CreateIndex(
                name: "ix_projet_chemin",
                schema: "projet",
                table: "projet",
                column: "chemin_entite")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "ix_projet_createur",
                schema: "projet",
                table: "projet",
                column: "createur_id");

            migrationBuilder.CreateIndex(
                name: "ix_projet_entite_creatrice",
                schema: "projet",
                table: "projet",
                column: "entite_creatrice_id");

            migrationBuilder.CreateIndex(
                name: "ix_projet_entite_porteuse",
                schema: "projet",
                table: "projet",
                column: "entite_porteuse_id");

            migrationBuilder.CreateIndex(
                name: "ix_projet_statut",
                schema: "projet",
                table: "projet",
                column: "statut",
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_projet_type",
                schema: "projet",
                table: "projet",
                column: "type_projet_id");

            migrationBuilder.CreateIndex(
                name: "ux_projet_code",
                schema: "projet",
                table: "projet",
                column: "code",
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_rattachement_entite",
                schema: "org",
                table: "rattachement",
                column: "entite_id");

            migrationBuilder.CreateIndex(
                name: "ux_rattachement_en_cours",
                schema: "org",
                table: "rattachement",
                column: "agent_id",
                unique: true,
                filter: "date_fin IS NULL AND NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_saisie_agent_date",
                schema: "temps",
                table: "saisie_temps",
                columns: new[] { "agent_id", "date_saisie" });

            migrationBuilder.CreateIndex(
                name: "ix_saisie_categorie",
                schema: "temps",
                table: "saisie_temps",
                column: "categorie_id");

            migrationBuilder.CreateIndex(
                name: "ix_saisie_feuille",
                schema: "temps",
                table: "saisie_temps",
                column: "feuille_id");

            migrationBuilder.CreateIndex(
                name: "ix_saisie_projet",
                schema: "temps",
                table: "saisie_temps",
                column: "projet_id",
                filter: "projet_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_saisie_tache",
                schema: "temps",
                table: "saisie_temps",
                column: "tache_id",
                filter: "tache_id  IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_saisie_ticket",
                schema: "temps",
                table: "saisie_temps",
                column: "ticket_id",
                filter: "ticket_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ux_statut_workflow_code",
                schema: "planif",
                table: "statut_workflow",
                columns: new[] { "workflow_id", "code" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ux_statut_workflow_initial",
                schema: "planif",
                table: "statut_workflow",
                column: "workflow_id",
                unique: true,
                filter: "est_initial AND NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_table_bd_association",
                schema: "modelisation",
                table: "table_bd",
                column: "association_source_id");

            migrationBuilder.CreateIndex(
                name: "ix_table_bd_entite",
                schema: "modelisation",
                table: "table_bd",
                column: "entite_source_id");

            migrationBuilder.CreateIndex(
                name: "ux_table_bd",
                schema: "modelisation",
                table: "table_bd",
                columns: new[] { "modele_id", "schema_bd", "nom" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_tache_phase_ordre",
                schema: "planif",
                table: "tache",
                columns: new[] { "phase_id", "ordre" },
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_tache_projet",
                schema: "planif",
                table: "tache",
                column: "projet_id");

            migrationBuilder.CreateIndex(
                name: "ix_tache_responsable",
                schema: "planif",
                table: "tache",
                column: "responsable_id",
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_tache_statut",
                schema: "planif",
                table: "tache",
                column: "statut_id");

            migrationBuilder.CreateIndex(
                name: "ix_template_config_couche",
                schema: "modelisation",
                table: "template_generation",
                columns: new[] { "config_id", "couche" });

            migrationBuilder.CreateIndex(
                name: "ix_ticket_assigne",
                schema: "planif",
                table: "ticket",
                column: "assigne_id",
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_projet",
                schema: "planif",
                table: "ticket",
                column: "projet_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_rapporteur",
                schema: "planif",
                table: "ticket",
                column: "rapporteur_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_statut",
                schema: "planif",
                table: "ticket",
                column: "statut_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_tache_rang",
                schema: "planif",
                table: "ticket",
                columns: new[] { "tache_id", "rang" },
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_type",
                schema: "planif",
                table: "ticket",
                column: "type_ticket_id");

            migrationBuilder.CreateIndex(
                name: "ux_ticket_numero",
                schema: "planif",
                table: "ticket",
                column: "numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_transition_vers",
                schema: "planif",
                table: "transition",
                column: "statut_vers_id");

            migrationBuilder.CreateIndex(
                name: "ix_transition_workflow",
                schema: "planif",
                table: "transition",
                column: "workflow_id");

            migrationBuilder.CreateIndex(
                name: "ux_transition",
                schema: "planif",
                table: "transition",
                columns: new[] { "statut_depuis_id", "statut_vers_id" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ux_type_projet_code",
                schema: "projet",
                table: "type_projet",
                column: "code",
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_type_ticket_workflow",
                schema: "planif",
                table: "type_ticket",
                column: "workflow_id");

            migrationBuilder.CreateIndex(
                name: "ux_type_ticket_code",
                schema: "planif",
                table: "type_ticket",
                column: "code",
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ux_valeur_enumeration",
                schema: "modelisation",
                table: "valeur_enumeration",
                columns: new[] { "enumeration_id", "code" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ix_validation_valideur",
                schema: "planif",
                table: "validation_phase",
                columns: new[] { "valideur_id", "decision" });

            migrationBuilder.CreateIndex(
                name: "ux_validation_phase",
                schema: "planif",
                table: "validation_phase",
                columns: new[] { "phase_id", "tour", "ordre_niveau" },
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ux_workflow_code",
                schema: "planif",
                table: "workflow",
                column: "code",
                unique: true,
                filter: "NOT est_supprime");

            migrationBuilder.CreateIndex(
                name: "ux_workflow_defaut",
                schema: "planif",
                table: "workflow",
                column: "type_element",
                unique: true,
                filter: "est_defaut AND NOT est_supprime");

            migrationBuilder.AddForeignKey(
                name: "absence_agent_id_fkey",
                schema: "org",
                table: "absence",
                column: "agent_id",
                principalSchema: "org",
                principalTable: "agent",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "affectation_projet_affecte_par_id_fkey",
                schema: "projet",
                table: "affectation_projet",
                column: "affecte_par_id",
                principalSchema: "org",
                principalTable: "agent",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "affectation_projet_repondu_par_id_fkey",
                schema: "projet",
                table: "affectation_projet",
                column: "repondu_par_id",
                principalSchema: "org",
                principalTable: "agent",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "affectation_projet_entite_cible_id_fkey",
                schema: "projet",
                table: "affectation_projet",
                column: "entite_cible_id",
                principalSchema: "org",
                principalTable: "entite_organisationnelle",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "affectation_projet_entite_source_id_fkey",
                schema: "projet",
                table: "affectation_projet",
                column: "entite_source_id",
                principalSchema: "org",
                principalTable: "entite_organisationnelle",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "affectation_projet_projet_id_fkey",
                schema: "projet",
                table: "affectation_projet",
                column: "projet_id",
                principalSchema: "projet",
                principalTable: "projet",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "agent_entite_id_fkey",
                schema: "org",
                table: "agent",
                column: "entite_id",
                principalSchema: "org",
                principalTable: "entite_organisationnelle",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "attribut_entite_id_fkey",
                schema: "modelisation",
                table: "attribut",
                column: "entite_id",
                principalSchema: "modelisation",
                principalTable: "entite",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            // Journal d'audit en ajout seul, à l'identique de 01-schema.sql.
            migrationBuilder.Sql("""
                CREATE FUNCTION commun.fn_historique_lecture_seule() RETURNS trigger
                LANGUAGE plpgsql AS $$
                BEGIN
                    RAISE EXCEPTION 'commun.historique est en ajout seul : % interdit', TG_OP;
                END;
                $$;

                CREATE TRIGGER tg_historique_lecture_seule
                    BEFORE UPDATE OR DELETE ON commun.historique
                    FOR EACH ROW EXECUTE FUNCTION commun.fn_historique_lecture_seule();
                """);

            migrationBuilder.Sql(LireRessource("Migrations.Sql.20261016143346_DonneesReference.sql"));
        }

        private static string LireRessource(string nom)
        {
            using var flux = typeof(InitialCreate).Assembly.GetManifestResourceStream(nom)
                             ?? throw new InvalidOperationException($"Ressource embarquée introuvable : {nom}");
            using var lecteur = new System.IO.StreamReader(flux);
            return lecteur.ReadToEnd();
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER tg_historique_lecture_seule ON commun.historique;");
            migrationBuilder.Sql("DROP FUNCTION commun.fn_historique_lecture_seule();");

            migrationBuilder.DropForeignKey(
                name: "fk_entite_responsable",
                schema: "org",
                table: "entite_organisationnelle");

            migrationBuilder.DropForeignKey(
                name: "projet_chef_projet_id_fkey",
                schema: "projet",
                table: "projet");

            migrationBuilder.DropForeignKey(
                name: "projet_createur_id_fkey",
                schema: "projet",
                table: "projet");

            migrationBuilder.DropForeignKey(
                name: "projet_entite_creatrice_id_fkey",
                schema: "projet",
                table: "projet");

            migrationBuilder.DropForeignKey(
                name: "projet_entite_porteuse_id_fkey",
                schema: "projet",
                table: "projet");

            migrationBuilder.DropForeignKey(
                name: "modele_domaine_projet_id_fkey",
                schema: "modelisation",
                table: "modele_domaine");

            migrationBuilder.DropForeignKey(
                name: "association_modele_id_fkey",
                schema: "modelisation",
                table: "association");

            migrationBuilder.DropForeignKey(
                name: "module_modele_id_fkey",
                schema: "modelisation",
                table: "module");

            migrationBuilder.DropForeignKey(
                name: "attribut_association_id_fkey",
                schema: "modelisation",
                table: "attribut");

            migrationBuilder.DropForeignKey(
                name: "attribut_entite_id_fkey",
                schema: "modelisation",
                table: "attribut");

            migrationBuilder.DropTable(
                name: "absence",
                schema: "org");

            migrationBuilder.DropTable(
                name: "affectation_projet",
                schema: "projet");

            migrationBuilder.DropTable(
                name: "agent_competence",
                schema: "org");

            migrationBuilder.DropTable(
                name: "baseline_tache",
                schema: "planif");

            migrationBuilder.DropTable(
                name: "cle_etrangere",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "commentaire",
                schema: "commun");

            migrationBuilder.DropTable(
                name: "correspondance_type",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "delegation",
                schema: "org");

            migrationBuilder.DropTable(
                name: "dependance_tache",
                schema: "planif");

            migrationBuilder.DropTable(
                name: "derogation_ordre",
                schema: "planif");

            migrationBuilder.DropTable(
                name: "element_diagramme",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "element_raid",
                schema: "projet");

            migrationBuilder.DropTable(
                name: "entite_interface",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "execution_generation",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "historique",
                schema: "commun");

            migrationBuilder.DropTable(
                name: "index_attribut",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "jalon",
                schema: "projet");

            migrationBuilder.DropTable(
                name: "jour_ferie",
                schema: "org");

            migrationBuilder.DropTable(
                name: "lien_ticket",
                schema: "planif");

            migrationBuilder.DropTable(
                name: "livrable",
                schema: "planif");

            migrationBuilder.DropTable(
                name: "membre_projet",
                schema: "projet");

            migrationBuilder.DropTable(
                name: "modele_phase",
                schema: "projet");

            migrationBuilder.DropTable(
                name: "notification",
                schema: "commun");

            migrationBuilder.DropTable(
                name: "parametre",
                schema: "commun");

            migrationBuilder.DropTable(
                name: "parametre_methode",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "piece_jointe",
                schema: "commun");

            migrationBuilder.DropTable(
                name: "rattachement",
                schema: "org");

            migrationBuilder.DropTable(
                name: "saisie_temps",
                schema: "temps");

            migrationBuilder.DropTable(
                name: "template_generation",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "transition",
                schema: "planif");

            migrationBuilder.DropTable(
                name: "valeur_enumeration",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "validation_phase",
                schema: "planif");

            migrationBuilder.DropTable(
                name: "competence",
                schema: "org");

            migrationBuilder.DropTable(
                name: "baseline",
                schema: "planif");

            migrationBuilder.DropTable(
                name: "colonne",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "patte",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "diagramme",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "index_entite",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "methode",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "categorie_activite",
                schema: "temps");

            migrationBuilder.DropTable(
                name: "feuille_temps",
                schema: "temps");

            migrationBuilder.DropTable(
                name: "ticket",
                schema: "planif");

            migrationBuilder.DropTable(
                name: "config_generation",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "table_bd",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "tache",
                schema: "planif");

            migrationBuilder.DropTable(
                name: "type_ticket",
                schema: "planif");

            migrationBuilder.DropTable(
                name: "phase",
                schema: "planif");

            migrationBuilder.DropTable(
                name: "statut_workflow",
                schema: "planif");

            migrationBuilder.DropTable(
                name: "workflow",
                schema: "planif");

            migrationBuilder.DropTable(
                name: "agent",
                schema: "org");

            migrationBuilder.DropTable(
                name: "fonction",
                schema: "org");

            migrationBuilder.DropTable(
                name: "entite_organisationnelle",
                schema: "org");

            migrationBuilder.DropTable(
                name: "projet",
                schema: "projet");

            migrationBuilder.DropTable(
                name: "type_projet",
                schema: "projet");

            migrationBuilder.DropTable(
                name: "modele_domaine",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "association",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "entite",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "attribut",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "enumeration",
                schema: "modelisation");

            migrationBuilder.DropTable(
                name: "module",
                schema: "modelisation");
        }
    }
}
