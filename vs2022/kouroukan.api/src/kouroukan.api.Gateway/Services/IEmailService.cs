namespace Kouroukan.Api.Gateway.Services;

/// <summary>
/// Service d'envoi d'emails transactionnels.
/// Utilise la configuration SMTP stockee en base (support.email_config).
/// </summary>
public interface IEmailService
{
    /// <summary>Envoie un email HTML generique.</summary>
    Task SendEmailAsync(string to, string subject, string htmlBody, string? fromOverride = null, CancellationToken ct = default);

    // ── Emails d'authentification ──────────────────────────────────

    /// <summary>Email de bienvenue apres inscription d'un nouvel etablissement.</summary>
    Task SendWelcomeEmailAsync(string email, string firstName, string establishmentName, CancellationToken ct = default);

    /// <summary>Email de credentials pour un nouveau compte cree par un directeur.</summary>
    Task SendAccountCreatedEmailAsync(string email, string firstName, string temporaryPassword, string establishmentName, string role, CancellationToken ct = default);

    /// <summary>Confirmation de changement de mot de passe (securite).</summary>
    Task SendPasswordChangedEmailAsync(string email, string firstName, CancellationToken ct = default);

    // ── Emails de liaison enseignant ────────────────────────────────

    /// <summary>Notifie le directeur d'une demande de liaison enseignant.</summary>
    Task SendLiaisonRequestEmailAsync(string directorEmail, string directorName, string teacherName, string establishmentName, CancellationToken ct = default);

    /// <summary>Notifie l'enseignant que sa liaison a ete acceptee.</summary>
    Task SendLiaisonAcceptedEmailAsync(string teacherEmail, string teacherName, string establishmentName, CancellationToken ct = default);

    /// <summary>Notifie l'enseignant que sa liaison a ete refusee.</summary>
    Task SendLiaisonRejectedEmailAsync(string teacherEmail, string teacherName, string establishmentName, CancellationToken ct = default);

    /// <summary>Notifie l'enseignant que sa liaison a ete terminee.</summary>
    Task SendLiaisonTerminatedEmailAsync(string teacherEmail, string teacherName, string establishmentName, CancellationToken ct = default);

    /// <summary>Notifie l'enseignant que sa liaison a ete reintegree.</summary>
    Task SendLiaisonReintegratedEmailAsync(string teacherEmail, string teacherName, string establishmentName, CancellationToken ct = default);

    // ── Emails d'abonnement ─────────────────────────────────────────

    /// <summary>Confirmation de souscription a un forfait.</summary>
    Task SendSubscriptionConfirmationEmailAsync(string email, string firstName, string planName, string startDate, CancellationToken ct = default);

    /// <summary>Confirmation de resiliation d'abonnement.</summary>
    Task SendSubscriptionCancelledEmailAsync(string email, string firstName, string planName, CancellationToken ct = default);

    // ── Emails de gestion utilisateur ───────────────────────────────

    /// <summary>Notifie un utilisateur que son compte a ete supprime de l'etablissement.</summary>
    Task SendAccountRemovedEmailAsync(string email, string firstName, string establishmentName, CancellationToken ct = default);

    // ── Emails CGU ──────────────────────────────────────────────────

    /// <summary>Confirmation d'acceptation des CGU.</summary>
    Task SendCguAcceptedEmailAsync(string email, string firstName, string cguVersion, CancellationToken ct = default);

    // ── Emails admin abonnement ─────────────────────────────────────

    /// <summary>Notifie l'utilisateur/etablissement qu'un abonnement a ete cree par l'admin.</summary>
    Task SendAdminSubscriptionCreatedEmailAsync(string email, string firstName, string planName, string startDate, CancellationToken ct = default);

    /// <summary>Notifie l'utilisateur/etablissement qu'un abonnement a ete modifie par l'admin.</summary>
    Task SendAdminSubscriptionUpdatedEmailAsync(string email, string firstName, string planName, CancellationToken ct = default);

    /// <summary>Notifie l'utilisateur/etablissement qu'un abonnement a ete supprime par l'admin.</summary>
    Task SendAdminSubscriptionDeletedEmailAsync(string email, string firstName, string planName, CancellationToken ct = default);
}
