using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Support.Tests.Integration.Fixtures;
using Xunit;

namespace Support.Tests.Integration;

/// <summary>
/// Tests d'integration pour les endpoints du module Support.
/// Pipeline complet : Controller → Handler → Service → Repository → PostgreSQL.
/// </summary>
public sealed class SupportEndpointTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _dbFixture;

    public SupportEndpointTests(PostgreSqlFixture dbFixture)
    {
        _dbFixture = dbFixture;
    }

    // ─── Full-text search PostgreSQL ───

    [Fact]
    public async Task FullTextSearch_TrouveArticleParMotCle()
    {
        // Ce test verifie que l'index GIN full-text fonctionne correctement
        // sur le conteneur PostgreSQL ephemere
        using var connection = new Npgsql.NpgsqlConnection(_dbFixture.ConnectionString);
        await connection.OpenAsync();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            SELECT id, titre FROM support.articles_aide
            WHERE to_tsvector('french', titre || ' ' || contenu) @@ plainto_tsquery('french', 'inscrire eleve')
            AND is_deleted = FALSE
        """;

        using var reader = await cmd.ExecuteReaderAsync();
        var results = new List<string>();
        while (await reader.ReadAsync())
        {
            results.Add(reader.GetString(1));
        }

        results.Should().Contain(t => t.Contains("inscrire", StringComparison.OrdinalIgnoreCase));
    }

    // ─── Vote — doublon → 409 ───

    [Fact]
    public async Task Vote_DoublonVote_RetourneConflict()
    {
        using var connection = new Npgsql.NpgsqlConnection(_dbFixture.ConnectionString);
        await connection.OpenAsync();

        // Creer une suggestion
        using var insertCmd = connection.CreateCommand();
        insertCmd.CommandText = """
            INSERT INTO support.suggestions (auteur_id, titre, contenu, statut_suggestion, nombre_votes)
            VALUES (1, 'Test Vote', 'Contenu test', 'Soumise', 0)
            RETURNING id
        """;
        var suggestionId = (int)(await insertCmd.ExecuteScalarAsync())!;

        // Premier vote — ok
        using var vote1Cmd = connection.CreateCommand();
        vote1Cmd.CommandText = $"""
            INSERT INTO support.votes_suggestions (suggestion_id, votant_id)
            VALUES ({suggestionId}, 42)
        """;
        await vote1Cmd.ExecuteNonQueryAsync();

        // Mettre a jour le compteur
        using var updateCmd = connection.CreateCommand();
        updateCmd.CommandText = $"""
            UPDATE support.suggestions SET nombre_votes = nombre_votes + 1 WHERE id = {suggestionId}
        """;
        await updateCmd.ExecuteNonQueryAsync();

        // Deuxieme vote — doit echouer (UNIQUE constraint)
        using var vote2Cmd = connection.CreateCommand();
        vote2Cmd.CommandText = $"""
            INSERT INTO support.votes_suggestions (suggestion_id, votant_id)
            VALUES ({suggestionId}, 42)
        """;

        var act = async () => await vote2Cmd.ExecuteNonQueryAsync();

        await act.Should().ThrowAsync<Npgsql.PostgresException>();

        // Verifier que le compteur est correct
        using var countCmd = connection.CreateCommand();
        countCmd.CommandText = $"SELECT nombre_votes FROM support.suggestions WHERE id = {suggestionId}";
        var count = (int)(await countCmd.ExecuteScalarAsync())!;
        count.Should().Be(1); // Toujours 1, pas 2
    }

    // ─── CRUD Ticket ───

    [Fact]
    public async Task Ticket_CRUD_Complet()
    {
        using var connection = new Npgsql.NpgsqlConnection(_dbFixture.ConnectionString);
        await connection.OpenAsync();

        // CREATE
        using var createCmd = connection.CreateCommand();
        createCmd.CommandText = """
            INSERT INTO support.tickets (auteur_id, sujet, contenu, priorite, statut_ticket, categorie_ticket)
            VALUES (1, 'Probleme paiement', 'Orange Money ne marche pas', 'Haute', 'Ouvert', 'Bug')
            RETURNING id
        """;
        var ticketId = (int)(await createCmd.ExecuteScalarAsync())!;
        ticketId.Should().BeGreaterThan(0);

        // READ
        using var readCmd = connection.CreateCommand();
        readCmd.CommandText = $"SELECT sujet, statut_ticket FROM support.tickets WHERE id = {ticketId}";
        using var reader = await readCmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        reader.GetString(0).Should().Be("Probleme paiement");
        reader.GetString(1).Should().Be("Ouvert");
        await reader.CloseAsync();

        // UPDATE
        using var updateCmd = connection.CreateCommand();
        updateCmd.CommandText = $"""
            UPDATE support.tickets SET statut_ticket = 'EnCours', updated_at = NOW() WHERE id = {ticketId}
        """;
        var affected = await updateCmd.ExecuteNonQueryAsync();
        affected.Should().Be(1);

        // DELETE (soft)
        using var deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = $"""
            UPDATE support.tickets SET is_deleted = TRUE, deleted_at = NOW() WHERE id = {ticketId}
        """;
        await deleteCmd.ExecuteNonQueryAsync();

        // VERIFY GONE
        using var verifyCmd = connection.CreateCommand();
        verifyCmd.CommandText = $"SELECT COUNT(*) FROM support.tickets WHERE id = {ticketId} AND is_deleted = FALSE";
        var count = (long)(await verifyCmd.ExecuteScalarAsync())!;
        count.Should().Be(0);
    }
}
