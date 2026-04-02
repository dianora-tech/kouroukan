namespace Kouroukan.Api.Gateway.Models;

/// <summary>
/// DTO retourne par GET /api/auth/onboarding/status.
/// </summary>
public class OnboardingStatusDto
{
    /// <summary>Etape actuelle (0 = pas commence, 1-6 = en cours, 7 = termine).</summary>
    public int CurrentStep { get; set; }

    /// <summary>Date de completion (null si pas termine).</summary>
    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// Request pour PUT /api/auth/onboarding/step.
/// </summary>
public class UpdateOnboardingStepRequest
{
    /// <summary>Numero de l'etape completee (1-6).</summary>
    public int Step { get; set; }
}

/// <summary>
/// Request pour PUT /api/companies/{id}.
/// </summary>
public class UpdateCompanyRequest
{
    /// <summary>Nom de l'etablissement.</summary>
    public string? Name { get; set; }

    /// <summary>Description.</summary>
    public string? Description { get; set; }

    /// <summary>Adresse.</summary>
    public string? Address { get; set; }

    /// <summary>Telephone.</summary>
    public string? PhoneNumber { get; set; }

    /// <summary>Email.</summary>
    public string? Email { get; set; }
}
