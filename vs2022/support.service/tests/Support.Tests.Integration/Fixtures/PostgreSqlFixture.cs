using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace Support.Tests.Integration.Fixtures;

/// <summary>
/// Fixture partagee — conteneur PostgreSQL ephemere pour le module support.
/// </summary>
public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("kouroukan_support_test")
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
            CREATE SCHEMA IF NOT EXISTS support;

            -- Types
            CREATE TABLE IF NOT EXISTS support.types_ticket (
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

            -- Tickets
            CREATE TABLE IF NOT EXISTS support.tickets (
                id SERIAL PRIMARY KEY,
                type_id INT NOT NULL DEFAULT 1,
                name VARCHAR(100) NOT NULL DEFAULT '',
                description TEXT,
                auteur_id INT NOT NULL,
                sujet VARCHAR(200) NOT NULL,
                contenu TEXT NOT NULL,
                priorite VARCHAR(20) NOT NULL DEFAULT 'Moyenne',
                statut_ticket VARCHAR(30) NOT NULL DEFAULT 'Ouvert',
                categorie_ticket VARCHAR(50) NOT NULL DEFAULT 'Question',
                module_concerne VARCHAR(50),
                assigne_a_id INT,
                date_resolution TIMESTAMP WITH TIME ZONE,
                note_satisfaction INT,
                piece_jointe_url VARCHAR(500),
                user_id INT NOT NULL DEFAULT 1,
                is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at TIMESTAMP WITH TIME ZONE,
                created_by VARCHAR(100) NOT NULL DEFAULT 'system',
                updated_by VARCHAR(100),
                deleted_at TIMESTAMP WITH TIME ZONE,
                deleted_by VARCHAR(100)
            );

            -- Suggestions
            CREATE TABLE IF NOT EXISTS support.suggestions (
                id SERIAL PRIMARY KEY,
                type_id INT NOT NULL DEFAULT 1,
                name VARCHAR(100) NOT NULL DEFAULT '',
                description TEXT,
                auteur_id INT NOT NULL,
                titre VARCHAR(200) NOT NULL,
                contenu TEXT NOT NULL,
                module_concerne VARCHAR(50),
                statut_suggestion VARCHAR(30) NOT NULL DEFAULT 'Soumise',
                nombre_votes INT NOT NULL DEFAULT 0,
                commentaire_admin TEXT,
                user_id INT NOT NULL DEFAULT 1,
                is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at TIMESTAMP WITH TIME ZONE,
                created_by VARCHAR(100) NOT NULL DEFAULT 'system',
                updated_by VARCHAR(100),
                deleted_at TIMESTAMP WITH TIME ZONE,
                deleted_by VARCHAR(100)
            );

            -- Votes
            CREATE TABLE IF NOT EXISTS support.votes_suggestions (
                id SERIAL PRIMARY KEY,
                name VARCHAR(100) NOT NULL DEFAULT '',
                description TEXT,
                suggestion_id INT NOT NULL REFERENCES support.suggestions(id),
                votant_id INT NOT NULL,
                user_id INT NOT NULL DEFAULT 1,
                is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at TIMESTAMP WITH TIME ZONE,
                created_by VARCHAR(100) NOT NULL DEFAULT 'system',
                updated_by VARCHAR(100),
                deleted_at TIMESTAMP WITH TIME ZONE,
                deleted_by VARCHAR(100),
                UNIQUE(suggestion_id, votant_id)
            );

            -- Articles d'aide
            CREATE TABLE IF NOT EXISTS support.articles_aide (
                id SERIAL PRIMARY KEY,
                type_id INT NOT NULL DEFAULT 1,
                name VARCHAR(100) NOT NULL DEFAULT '',
                description TEXT,
                titre VARCHAR(200) NOT NULL,
                contenu TEXT NOT NULL,
                slug VARCHAR(200) NOT NULL UNIQUE,
                categorie VARCHAR(100) NOT NULL DEFAULT 'FAQ',
                module_concerne VARCHAR(50),
                ordre INT NOT NULL DEFAULT 0,
                est_publie BOOLEAN NOT NULL DEFAULT TRUE,
                nombre_vues INT NOT NULL DEFAULT 0,
                nombre_utile INT NOT NULL DEFAULT 0,
                user_id INT NOT NULL DEFAULT 1,
                is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                updated_at TIMESTAMP WITH TIME ZONE,
                created_by VARCHAR(100) NOT NULL DEFAULT 'system',
                updated_by VARCHAR(100),
                deleted_at TIMESTAMP WITH TIME ZONE,
                deleted_by VARCHAR(100)
            );

            -- Full-text search index
            CREATE INDEX IF NOT EXISTS idx_articles_aide_fts
                ON support.articles_aide
                USING GIN (to_tsvector('french', titre || ' ' || contenu));

            -- Seed data
            INSERT INTO support.types_ticket (name) VALUES
                ('Bug / Dysfonctionnement'),
                ('Question / Comment faire'),
                ('Demande d''amelioration');

            INSERT INTO support.articles_aide (titre, contenu, slug, categorie) VALUES
                ('Comment inscrire un eleve', 'Pour inscrire un eleve, allez dans le module Inscriptions...', 'comment-inscrire-eleve', 'Tutoriel'),
                ('Guide de paiement Mobile Money', 'Les paiements Orange Money et MTN MoMo sont integres...', 'guide-paiement-mobile-money', 'Guide de demarrage');
        """;

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
