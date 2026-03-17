using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace Inscriptions.Tests.Integration.Fixtures;

/// <summary>
/// Fixture partagee pour les tests d'integration.
/// Demarre un conteneur PostgreSQL ephemere et initialise le schema.
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

        // Creer le schema et les tables
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            CREATE SCHEMA IF NOT EXISTS inscriptions;
            CREATE SCHEMA IF NOT EXISTS auth;

            -- Table des types
            CREATE TABLE IF NOT EXISTS inscriptions.types_inscription (
                id SERIAL PRIMARY KEY,
                name VARCHAR(100) NOT NULL,
                description TEXT,
                is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at TIMESTAMP WITH TIME ZONE,
                created_by VARCHAR(100) NOT NULL DEFAULT 'system',
                updated_by VARCHAR(100),
                deleted_at TIMESTAMP WITH TIME ZONE,
                deleted_by VARCHAR(100)
            );

            -- Table des eleves (simplifiee pour les tests)
            CREATE TABLE IF NOT EXISTS inscriptions.eleves (
                id SERIAL PRIMARY KEY,
                type_id INT NOT NULL DEFAULT 1,
                name VARCHAR(100) NOT NULL DEFAULT '',
                description TEXT,
                first_name VARCHAR(100) NOT NULL,
                last_name VARCHAR(100) NOT NULL,
                date_naissance DATE NOT NULL,
                lieu_naissance VARCHAR(200) NOT NULL,
                genre VARCHAR(10) NOT NULL,
                nationalite VARCHAR(50) NOT NULL DEFAULT 'Guineenne',
                adresse VARCHAR(500),
                photo_url VARCHAR(500),
                numero_matricule VARCHAR(50) NOT NULL,
                niveau_classe_id INT NOT NULL DEFAULT 1,
                classe_id INT,
                parent_id INT,
                statut_inscription VARCHAR(30) NOT NULL DEFAULT 'Prospect',
                user_id INT NOT NULL DEFAULT 1,
                is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at TIMESTAMP WITH TIME ZONE,
                created_by VARCHAR(100) NOT NULL DEFAULT 'system',
                updated_by VARCHAR(100),
                deleted_at TIMESTAMP WITH TIME ZONE,
                deleted_by VARCHAR(100)
            );

            -- Table des annees scolaires
            CREATE TABLE IF NOT EXISTS inscriptions.annees_scolaires (
                id SERIAL PRIMARY KEY,
                name VARCHAR(100) NOT NULL DEFAULT '',
                description TEXT,
                libelle VARCHAR(20) NOT NULL,
                date_debut DATE NOT NULL,
                date_fin DATE NOT NULL,
                est_active BOOLEAN NOT NULL DEFAULT FALSE,
                user_id INT NOT NULL DEFAULT 1,
                is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at TIMESTAMP WITH TIME ZONE,
                created_by VARCHAR(100) NOT NULL DEFAULT 'system',
                updated_by VARCHAR(100),
                deleted_at TIMESTAMP WITH TIME ZONE,
                deleted_by VARCHAR(100)
            );

            -- Table des inscriptions
            CREATE TABLE IF NOT EXISTS inscriptions.inscriptions (
                id SERIAL PRIMARY KEY,
                type_id INT NOT NULL DEFAULT 1,
                name VARCHAR(100) NOT NULL DEFAULT '',
                description TEXT,
                eleve_id INT NOT NULL,
                classe_id INT NOT NULL,
                annee_scolaire_id INT NOT NULL,
                date_inscription DATE NOT NULL,
                montant_inscription NUMERIC(15,2) NOT NULL DEFAULT 0,
                est_paye BOOLEAN NOT NULL DEFAULT FALSE,
                est_redoublant BOOLEAN NOT NULL DEFAULT FALSE,
                type_etablissement VARCHAR(30),
                serie_bac VARCHAR(10),
                statut_inscription VARCHAR(30) NOT NULL DEFAULT 'EnAttente',
                user_id INT NOT NULL DEFAULT 1,
                is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at TIMESTAMP WITH TIME ZONE,
                created_by VARCHAR(100) NOT NULL DEFAULT 'system',
                updated_by VARCHAR(100),
                deleted_at TIMESTAMP WITH TIME ZONE,
                deleted_by VARCHAR(100)
            );

            -- Donnees de reference
            INSERT INTO inscriptions.types_inscription (name) VALUES
                ('Nouvelle inscription'),
                ('Reinscription'),
                ('Transfert entrant'),
                ('Redoublement');

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
