namespace Kouroukan.Api.Gateway.Models;

/// <summary>
/// Rapport de deploiement envoye par GitHub Actions apres chaque livraison.
/// </summary>
public class DeploymentReportRequest
{
    /// <summary>Environnement cible : "test" ou "production".</summary>
    public string Environment { get; set; } = string.Empty;

    /// <summary>Branche deployee (develop, main).</summary>
    public string Branch { get; set; } = string.Empty;

    /// <summary>SHA du commit deploye.</summary>
    public string CommitSha { get; set; } = string.Empty;

    /// <summary>Message du dernier commit.</summary>
    public string CommitMessage { get; set; } = string.Empty;

    /// <summary>Auteur du dernier commit.</summary>
    public string CommitAuthor { get; set; } = string.Empty;

    /// <summary>Version de l'application (ex: v1.2).</summary>
    public string AppVersion { get; set; } = string.Empty;

    /// <summary>Liste des commits deployes depuis le dernier deploiement.</summary>
    public List<string> Commits { get; set; } = new();

    /// <summary>Resultat global : "success" ou "failure".</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Duree totale du deploiement en secondes.</summary>
    public int DeploymentDurationSeconds { get; set; }

    /// <summary>Resultats des health checks (domaine → code HTTP).</summary>
    public Dictionary<string, string> HealthChecks { get; set; } = new();

    /// <summary>Erreurs rencontrees pendant le deploiement.</summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>Services impactes par les changements (detectes par le CI).</summary>
    public List<string> ImpactedServices { get; set; } = new();

    /// <summary>URL du run GitHub Actions.</summary>
    public string WorkflowRunUrl { get; set; } = string.Empty;

    /// <summary>Timestamp de debut du deploiement (UTC ISO 8601).</summary>
    public string DeploymentStartedAt { get; set; } = string.Empty;

    /// <summary>Couverture de code par composant (nom → pourcentage).</summary>
    public Dictionary<string, string> CodeCoverage { get; set; } = new();
}
