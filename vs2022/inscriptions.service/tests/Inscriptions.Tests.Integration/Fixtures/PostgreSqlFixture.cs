using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace Inscriptions.Tests.Integration.Fixtures;

/// <summary>
/// Fixture partagee pour les tests d'integration.
/// Demarre un conteneur PostgreSQL ephemere et initialise le schema.
/// Les tables doivent correspondre exactement aux proprietes des DTOs
/// (le Repository genere les colonnes en snake_case depuis les noms C#).
/// </summary>
public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("kouroukan_test")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            CREATE SCHEMA IF NOT EXISTS inscriptions;

            -- ── Types d'inscription ────────────────────────────────────────────
            CREATE TABLE IF NOT EXISTS inscriptions.type_inscriptions (
                id          SERIAL PRIMARY KEY,
                name        VARCHAR(100) NOT NULL,
                description TEXT,
                created_at  TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at  TIMESTAMP WITH TIME ZONE,
                created_by  VARCHAR(100) NOT NULL DEFAULT 'system',
                updated_by  VARCHAR(100),
                is_deleted  BOOLEAN NOT NULL DEFAULT FALSE,
                deleted_at  TIMESTAMP WITH TIME ZONE,
                deleted_by  VARCHAR(100)
            );

            -- ── Types de dossiers d'admission ──────────────────────────────────
            CREATE TABLE IF NOT EXISTS inscriptions.type_dossiers_admission (
                id          SERIAL PRIMARY KEY,
                name        VARCHAR(100) NOT NULL,
                description TEXT,
                created_at  TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at  TIMESTAMP WITH TIME ZONE,
                created_by  VARCHAR(100) NOT NULL DEFAULT 'system',
                updated_by  VARCHAR(100),
                is_deleted  BOOLEAN NOT NULL DEFAULT FALSE,
                deleted_at  TIMESTAMP WITH TIME ZONE,
                deleted_by  VARCHAR(100)
            );

            -- ── Annees scolaires (colonnes = AnneeScolaireDto) ─────────────────
            CREATE TABLE IF NOT EXISTS inscriptions.annees_scolaires (
                id               SERIAL PRIMARY KEY,
                libelle          VARCHAR(20)  NOT NULL,
                date_debut       DATE         NOT NULL,
                date_fin         DATE         NOT NULL,
                est_active       BOOLEAN      NOT NULL DEFAULT FALSE,
                code             VARCHAR(50),
                description      TEXT,
                statut           VARCHAR(30)  NOT NULL DEFAULT 'preparation',
                date_rentree     DATE,
                nombre_periodes  INT          NOT NULL DEFAULT 3,
                type_periode     VARCHAR(30)  NOT NULL DEFAULT 'trimestre',
                created_at       TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at       TIMESTAMP WITH TIME ZONE,
                created_by       VARCHAR(100) NOT NULL DEFAULT 'system',
                updated_by       VARCHAR(100),
                is_deleted       BOOLEAN      NOT NULL DEFAULT FALSE,
                deleted_at       TIMESTAMP WITH TIME ZONE,
                deleted_by       VARCHAR(100)
            );

            -- ── Eleves (colonnes = EleveDto) ───────────────────────────────────
            CREATE TABLE IF NOT EXISTS inscriptions.eleves (
                id                  SERIAL PRIMARY KEY,
                first_name          VARCHAR(100) NOT NULL,
                last_name           VARCHAR(100) NOT NULL,
                date_naissance      DATE         NOT NULL,
                lieu_naissance      VARCHAR(200) NOT NULL,
                genre               VARCHAR(10)  NOT NULL,
                nationalite         VARCHAR(50)  NOT NULL DEFAULT 'Guineenne',
                adresse             VARCHAR(500),
                photo_url           VARCHAR(500),
                numero_matricule    VARCHAR(50)  NOT NULL,
                niveau_classe_id    INT          NOT NULL DEFAULT 1,
                classe_id           INT,
                parent_id           INT,
                statut_inscription  VARCHAR(30)  NOT NULL DEFAULT 'Prospect',
                user_id             INT          NOT NULL DEFAULT 1,
                created_at          TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at          TIMESTAMP WITH TIME ZONE,
                created_by          VARCHAR(100) NOT NULL DEFAULT 'system',
                updated_by          VARCHAR(100),
                is_deleted          BOOLEAN      NOT NULL DEFAULT FALSE,
                deleted_at          TIMESTAMP WITH TIME ZONE,
                deleted_by          VARCHAR(100)
            );

            -- ── Inscriptions (colonnes = InscriptionDto) ───────────────────────
            CREATE TABLE IF NOT EXISTS inscriptions.inscriptions (
                id                      SERIAL PRIMARY KEY,
                type_id                 INT          NOT NULL DEFAULT 1,
                eleve_id                INT          NOT NULL,
                classe_id               INT          NOT NULL,
                annee_scolaire_id       INT          NOT NULL,
                date_inscription        DATE         NOT NULL,
                montant_inscription     NUMERIC(15,2) NOT NULL DEFAULT 0,
                est_paye                BOOLEAN      NOT NULL DEFAULT FALSE,
                est_redoublant          BOOLEAN      NOT NULL DEFAULT FALSE,
                type_etablissement      VARCHAR(30),
                serie_bac               VARCHAR(10),
                statut_inscription      VARCHAR(30)  NOT NULL DEFAULT 'EnAttente',
                user_id                 INT          NOT NULL DEFAULT 1,
                created_at              TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at              TIMESTAMP WITH TIME ZONE,
                created_by              VARCHAR(100) NOT NULL DEFAULT 'system',
                updated_by              VARCHAR(100),
                is_deleted              BOOLEAN      NOT NULL DEFAULT FALSE,
                deleted_at              TIMESTAMP WITH TIME ZONE,
                deleted_by              VARCHAR(100)
            );

            -- ── Dossiers admission (colonnes = DossierAdmissionDto) ────────────
            CREATE TABLE IF NOT EXISTS inscriptions.dossiers_admission (
                id                          SERIAL PRIMARY KEY,
                type_id                     INT          NOT NULL DEFAULT 1,
                eleve_id                    INT          NOT NULL,
                annee_scolaire_id           INT          NOT NULL,
                statut_dossier              VARCHAR(30)  NOT NULL DEFAULT 'EnEtude',
                etape_actuelle              VARCHAR(50)  NOT NULL DEFAULT 'initiale',
                date_demande                DATE         NOT NULL,
                date_decision               DATE,
                motif_refus                 VARCHAR(500),
                scoring_interne             NUMERIC(5,2),
                commentaires                TEXT,
                responsable_admission_id    INT,
                user_id                     INT          NOT NULL DEFAULT 1,
                created_at                  TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at                  TIMESTAMP WITH TIME ZONE,
                created_by                  VARCHAR(100) NOT NULL DEFAULT 'system',
                updated_by                  VARCHAR(100),
                is_deleted                  BOOLEAN      NOT NULL DEFAULT FALSE,
                deleted_at                  TIMESTAMP WITH TIME ZONE,
                deleted_by                  VARCHAR(100)
            );

            -- ── Transferts (colonnes = TransfertDto) ───────────────────────────
            CREATE TABLE IF NOT EXISTS inscriptions.transferts (
                id                  SERIAL PRIMARY KEY,
                eleve_id            INT          NOT NULL,
                company_origine_id  INT          NOT NULL,
                company_cible_id    INT          NOT NULL,
                statut              VARCHAR(30)  NOT NULL DEFAULT 'pending',
                motif               TEXT         NOT NULL DEFAULT '',
                documents           TEXT,
                date_demande        DATE         NOT NULL,
                date_traitement     DATE,
                created_at          TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at          TIMESTAMP WITH TIME ZONE,
                created_by          VARCHAR(100) NOT NULL DEFAULT 'system',
                updated_by          VARCHAR(100),
                is_deleted          BOOLEAN      NOT NULL DEFAULT FALSE,
                deleted_at          TIMESTAMP WITH TIME ZONE,
                deleted_by          VARCHAR(100)
            );

            -- ── Radiations (colonnes = RadiationDto) ───────────────────────────
            CREATE TABLE IF NOT EXISTS inscriptions.radiations (
                id               SERIAL PRIMARY KEY,
                eleve_id         INT          NOT NULL,
                company_id       INT          NOT NULL,
                motif            TEXT         NOT NULL DEFAULT '',
                documents        TEXT,
                date_radiation   DATE         NOT NULL,
                created_at       TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at       TIMESTAMP WITH TIME ZONE,
                created_by       VARCHAR(100) NOT NULL DEFAULT 'system',
                updated_by       VARCHAR(100),
                is_deleted       BOOLEAN      NOT NULL DEFAULT FALSE,
                deleted_at       TIMESTAMP WITH TIME ZONE,
                deleted_by       VARCHAR(100)
            );

            -- ── Liaisons parent (colonnes = LiaisonParentDto) ──────────────────
            CREATE TABLE IF NOT EXISTS inscriptions.liaisons_parent (
                id              SERIAL PRIMARY KEY,
                parent_user_id  INT          NOT NULL,
                eleve_id        INT          NOT NULL,
                company_id      INT          NOT NULL,
                statut          VARCHAR(30)  NOT NULL DEFAULT 'pending',
                created_at      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at      TIMESTAMP WITH TIME ZONE,
                created_by      VARCHAR(100) NOT NULL DEFAULT 'system',
                updated_by      VARCHAR(100),
                is_deleted      BOOLEAN      NOT NULL DEFAULT FALSE,
                deleted_at      TIMESTAMP WITH TIME ZONE,
                deleted_by      VARCHAR(100)
            );

            -- ── Donnees de reference pour les tests ────────────────────────────
            INSERT INTO inscriptions.type_inscriptions (name) VALUES
                ('Nouvelle inscription'),
                ('Reinscription'),
                ('Transfert entrant'),
                ('Redoublement');

            INSERT INTO inscriptions.type_dossiers_admission (name) VALUES
                ('Standard'),
                ('Transfert');

            INSERT INTO inscriptions.annees_scolaires (libelle, date_debut, date_fin, est_active) VALUES
                ('2025-2026', '2025-09-01', '2026-06-30', TRUE);

            INSERT INTO inscriptions.eleves (first_name, last_name, date_naissance, lieu_naissance, genre, numero_matricule) VALUES
                ('Mamadou', 'Diallo', '2010-03-15', 'Conakry', 'M', 'MAT-001'),
                ('Fatoumata', 'Camara', '2011-07-22', 'Kindia', 'F', 'MAT-002');
        """;

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
