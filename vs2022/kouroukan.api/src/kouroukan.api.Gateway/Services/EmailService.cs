using System.Net;
using System.Net.Mail;
using Dapper;
using GnDapper.Connection;

namespace Kouroukan.Api.Gateway.Services;

/// <summary>
/// Service d'envoi d'emails transactionnels via SMTP.
/// Lit la configuration depuis support.email_config (cache 5 min).
/// Ne lance jamais d'exception au niveau appelant — les erreurs sont loguees.
/// </summary>
public sealed class EmailService : IEmailService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<EmailService> _logger;

    // Cache simple de la config SMTP
    private SmtpConfig? _cachedConfig;
    private DateTime _cacheExpiry = DateTime.MinValue;
    private readonly SemaphoreSlim _cacheLock = new(1, 1);

    private const string AppUrl = "https://app.kouroukan.dianora.org";
    private const string SupportEmail = "support@kouroukan.dianora.org";

    public EmailService(IDbConnectionFactory connectionFactory, ILogger<EmailService> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    // ═══════════════════════════════════════════════════════════════════════
    // ENVOI GENERIQUE
    // ═══════════════════════════════════════════════════════════════════════

    public async Task SendEmailAsync(string to, string subject, string htmlBody, string? fromOverride = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(to) || to.EndsWith("@kouroukan.gn", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogDebug("Email ignore — adresse invalide ou placeholder : {To}", to);
            return;
        }

        var config = await GetSmtpConfigAsync(ct);
        if (config is null)
        {
            _logger.LogWarning("Aucune configuration SMTP — email non envoye a {To}", to);
            return;
        }

        try
        {
            using var client = new SmtpClient(config.Host, config.Port)
            {
                Credentials = new NetworkCredential(config.Username, config.Password),
                EnableSsl = true,
                Timeout = 15000 // 15 secondes
            };

            var fromAddr = string.IsNullOrEmpty(fromOverride) ? config.FromEmail : fromOverride;
            var message = new MailMessage(
                new MailAddress(fromAddr, config.FromName),
                new MailAddress(to))
            {
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            await client.SendMailAsync(message, ct);
            _logger.LogInformation("Email envoye a {To} — Sujet: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Echec envoi email a {To} — Sujet: {Subject}", to, subject);
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // EMAILS D'AUTHENTIFICATION
    // ═══════════════════════════════════════════════════════════════════════

    public Task SendWelcomeEmailAsync(string email, string firstName, string establishmentName, CancellationToken ct = default)
    {
        var body = WrapInLayout($"""
            <h2 style="color:#16a34a;margin:0 0 16px">Bienvenue sur Kouroukan !</h2>
            <p>Bonjour <strong>{Escape(firstName)}</strong>,</p>
            <p>Felicitations ! Votre etablissement <strong>{Escape(establishmentName)}</strong> a ete cree avec succes sur la plateforme Kouroukan.</p>
            <h3 style="color:#333;margin:24px 0 12px">Prochaines etapes</h3>
            <ol style="color:#555;line-height:1.8">
                <li>Completez l'assistant de configuration (onboarding)</li>
                <li>Ajoutez vos classes et niveaux scolaires</li>
                <li>Creez les comptes de votre personnel (enseignants, censeurs...)</li>
                <li>Commencez les inscriptions des eleves</li>
            </ol>
            <div style="text-align:center;margin:32px 0">
                <a href="{AppUrl}/connexion" style="background:#16a34a;color:#fff;padding:14px 32px;text-decoration:none;border-radius:8px;font-weight:600;display:inline-block">
                    Acceder a ma plateforme
                </a>
            </div>
            """);

        return SendEmailAsync(email, "Bienvenue sur Kouroukan — Votre etablissement est pret", body, null, ct);
    }

    public Task SendAccountCreatedEmailAsync(string email, string firstName, string temporaryPassword, string establishmentName, string role, CancellationToken ct = default)
    {
        var roleLabel = TranslateRole(role);
        var body = WrapInLayout($"""
            <h2 style="color:#16a34a;margin:0 0 16px">Votre compte a ete cree</h2>
            <p>Bonjour <strong>{Escape(firstName)}</strong>,</p>
            <p>Un compte a ete cree pour vous sur la plateforme Kouroukan par l'etablissement <strong>{Escape(establishmentName)}</strong>.</p>
            <table style="width:100%;border-collapse:collapse;margin:24px 0;background:#f9fafb;border-radius:8px">
                <tr>
                    <td style="padding:12px 16px;border-bottom:1px solid #e5e7eb;color:#666;width:40%">Role</td>
                    <td style="padding:12px 16px;border-bottom:1px solid #e5e7eb;font-weight:600">{Escape(roleLabel)}</td>
                </tr>
                <tr>
                    <td style="padding:12px 16px;border-bottom:1px solid #e5e7eb;color:#666">Email / Identifiant</td>
                    <td style="padding:12px 16px;border-bottom:1px solid #e5e7eb;font-weight:600">{Escape(email)}</td>
                </tr>
                <tr>
                    <td style="padding:12px 16px;color:#666">Mot de passe temporaire</td>
                    <td style="padding:12px 16px;font-weight:600;font-family:monospace;font-size:16px;color:#dc2626">{Escape(temporaryPassword)}</td>
                </tr>
            </table>
            <p style="color:#dc2626;font-weight:600">&#9888; Vous devrez changer ce mot de passe lors de votre premiere connexion.</p>
            <div style="text-align:center;margin:32px 0">
                <a href="{AppUrl}/connexion" style="background:#16a34a;color:#fff;padding:14px 32px;text-decoration:none;border-radius:8px;font-weight:600;display:inline-block">
                    Me connecter
                </a>
            </div>
            """);

        return SendEmailAsync(email, "Votre compte Kouroukan a ete cree", body, null, ct);
    }

    public Task SendPasswordChangedEmailAsync(string email, string firstName, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow.ToString("dd/MM/yyyy a HH:mm UTC");
        var body = WrapInLayout($"""
            <h2 style="color:#16a34a;margin:0 0 16px">Mot de passe modifie</h2>
            <p>Bonjour <strong>{Escape(firstName)}</strong>,</p>
            <p>Votre mot de passe a ete modifie avec succes le <strong>{now}</strong>.</p>
            <div style="background:#fef3c7;border-left:4px solid #f59e0b;padding:16px;margin:24px 0;border-radius:0 8px 8px 0">
                <p style="margin:0;color:#92400e">
                    <strong>&#9888; Ce n'etait pas vous ?</strong><br>
                    Si vous n'etes pas a l'origine de ce changement, contactez immediatement notre support a
                    <a href="mailto:{SupportEmail}" style="color:#16a34a">{SupportEmail}</a>.
                </p>
            </div>
            """);

        return SendEmailAsync(email, "Votre mot de passe Kouroukan a ete modifie", body, null, ct);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // EMAILS DE LIAISON ENSEIGNANT
    // ═══════════════════════════════════════════════════════════════════════

    public Task SendLiaisonRequestEmailAsync(string directorEmail, string directorName, string teacherName, string establishmentName, CancellationToken ct = default)
    {
        var body = WrapInLayout($"""
            <h2 style="color:#16a34a;margin:0 0 16px">Nouvelle demande de liaison</h2>
            <p>Bonjour <strong>{Escape(directorName)}</strong>,</p>
            <p>L'enseignant <strong>{Escape(teacherName)}</strong> souhaite etre rattache a votre etablissement <strong>{Escape(establishmentName)}</strong>.</p>
            <p>Veuillez examiner cette demande et l'accepter ou la refuser depuis votre espace.</p>
            <div style="text-align:center;margin:32px 0">
                <a href="{AppUrl}" style="background:#16a34a;color:#fff;padding:14px 32px;text-decoration:none;border-radius:8px;font-weight:600;display:inline-block">
                    Voir la demande
                </a>
            </div>
            """);

        return SendEmailAsync(directorEmail, $"Nouvelle demande de liaison enseignant — {teacherName}", body, null, ct);
    }

    public Task SendLiaisonAcceptedEmailAsync(string teacherEmail, string teacherName, string establishmentName, CancellationToken ct = default)
    {
        var body = WrapInLayout($"""
            <h2 style="color:#16a34a;margin:0 0 16px">Liaison acceptee</h2>
            <p>Bonjour <strong>{Escape(teacherName)}</strong>,</p>
            <p>Votre demande de liaison avec l'etablissement <strong>{Escape(establishmentName)}</strong> a ete <span style="color:#16a34a;font-weight:600">acceptee</span>.</p>
            <p>Vous pouvez desormais acceder aux donnees de l'etablissement depuis votre espace.</p>
            <div style="text-align:center;margin:32px 0">
                <a href="{AppUrl}" style="background:#16a34a;color:#fff;padding:14px 32px;text-decoration:none;border-radius:8px;font-weight:600;display:inline-block">
                    Acceder a mon espace
                </a>
            </div>
            """);

        return SendEmailAsync(teacherEmail, $"Votre liaison avec {establishmentName} a ete acceptee", body, null, ct);
    }

    public Task SendLiaisonRejectedEmailAsync(string teacherEmail, string teacherName, string establishmentName, CancellationToken ct = default)
    {
        var body = WrapInLayout($"""
            <h2 style="color:#e11d48;margin:0 0 16px">Liaison non acceptee</h2>
            <p>Bonjour <strong>{Escape(teacherName)}</strong>,</p>
            <p>Votre demande de liaison avec l'etablissement <strong>{Escape(establishmentName)}</strong> n'a pas ete acceptee.</p>
            <p>Si vous pensez qu'il s'agit d'une erreur, nous vous invitons a contacter directement l'etablissement ou notre support a <a href="mailto:{SupportEmail}" style="color:#16a34a">{SupportEmail}</a>.</p>
            """);

        return SendEmailAsync(teacherEmail, $"Votre demande de liaison avec {establishmentName}", body, null, ct);
    }

    public Task SendLiaisonTerminatedEmailAsync(string teacherEmail, string teacherName, string establishmentName, CancellationToken ct = default)
    {
        var body = WrapInLayout($"""
            <h2 style="color:#f59e0b;margin:0 0 16px">Liaison terminee</h2>
            <p>Bonjour <strong>{Escape(teacherName)}</strong>,</p>
            <p>Votre liaison avec l'etablissement <strong>{Escape(establishmentName)}</strong> a ete terminee par le directeur.</p>
            <p>Vous n'avez plus acces aux donnees de cet etablissement. Si vous avez des questions, contactez <a href="mailto:{SupportEmail}" style="color:#16a34a">{SupportEmail}</a>.</p>
            """);

        return SendEmailAsync(teacherEmail, $"Votre liaison avec {establishmentName} a ete terminee", body, null, ct);
    }

    public Task SendLiaisonReintegratedEmailAsync(string teacherEmail, string teacherName, string establishmentName, CancellationToken ct = default)
    {
        var body = WrapInLayout($"""
            <h2 style="color:#16a34a;margin:0 0 16px">Liaison reintegree</h2>
            <p>Bonjour <strong>{Escape(teacherName)}</strong>,</p>
            <p>Bonne nouvelle ! Votre liaison avec l'etablissement <strong>{Escape(establishmentName)}</strong> a ete <span style="color:#16a34a;font-weight:600">reintegree</span>.</p>
            <p>Vous avez de nouveau acces aux donnees de l'etablissement depuis votre espace.</p>
            <div style="text-align:center;margin:32px 0">
                <a href="{AppUrl}" style="background:#16a34a;color:#fff;padding:14px 32px;text-decoration:none;border-radius:8px;font-weight:600;display:inline-block">
                    Acceder a mon espace
                </a>
            </div>
            """);

        return SendEmailAsync(teacherEmail, $"Votre liaison avec {establishmentName} a ete reintegree", body, null, ct);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // EMAILS D'ABONNEMENT
    // ═══════════════════════════════════════════════════════════════════════

    public Task SendSubscriptionConfirmationEmailAsync(string email, string firstName, string planName, string startDate, CancellationToken ct = default)
    {
        var body = WrapInLayout($"""
            <h2 style="color:#16a34a;margin:0 0 16px">Abonnement confirme</h2>
            <p>Bonjour <strong>{Escape(firstName)}</strong>,</p>
            <p>Votre souscription au forfait <strong>{Escape(planName)}</strong> a ete confirmee avec succes.</p>
            <table style="width:100%;border-collapse:collapse;margin:24px 0;background:#f9fafb;border-radius:8px">
                <tr>
                    <td style="padding:12px 16px;border-bottom:1px solid #e5e7eb;color:#666;width:40%">Forfait</td>
                    <td style="padding:12px 16px;border-bottom:1px solid #e5e7eb;font-weight:600">{Escape(planName)}</td>
                </tr>
                <tr>
                    <td style="padding:12px 16px;color:#666">Date de debut</td>
                    <td style="padding:12px 16px;font-weight:600">{Escape(startDate)}</td>
                </tr>
            </table>
            <p>Vous pouvez gerer votre abonnement depuis les parametres de votre compte.</p>
            <div style="text-align:center;margin:32px 0">
                <a href="{AppUrl}" style="background:#16a34a;color:#fff;padding:14px 32px;text-decoration:none;border-radius:8px;font-weight:600;display:inline-block">
                    Mon espace
                </a>
            </div>
            """);

        return SendEmailAsync(email, $"Abonnement confirme — Forfait {planName}", body, null, ct);
    }

    public Task SendSubscriptionCancelledEmailAsync(string email, string firstName, string planName, CancellationToken ct = default)
    {
        var body = WrapInLayout($"""
            <h2 style="color:#f59e0b;margin:0 0 16px">Abonnement resilie</h2>
            <p>Bonjour <strong>{Escape(firstName)}</strong>,</p>
            <p>Votre abonnement au forfait <strong>{Escape(planName)}</strong> a ete resilie.</p>
            <p>Vous conservez l'acces a vos fonctionnalites jusqu'a la fin de la periode en cours. Vos donnees restent accessibles.</p>
            <p>Pour vous reabonner a tout moment, rendez-vous dans les parametres de votre compte.</p>
            <div style="text-align:center;margin:32px 0">
                <a href="{AppUrl}" style="background:#16a34a;color:#fff;padding:14px 32px;text-decoration:none;border-radius:8px;font-weight:600;display:inline-block">
                    Mon espace
                </a>
            </div>
            """);

        return SendEmailAsync(email, $"Confirmation de resiliation — Forfait {planName}", body, null, ct);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // EMAILS DE GESTION UTILISATEUR
    // ═══════════════════════════════════════════════════════════════════════

    public Task SendAccountRemovedEmailAsync(string email, string firstName, string establishmentName, CancellationToken ct = default)
    {
        var body = WrapInLayout($"""
            <h2 style="color:#e11d48;margin:0 0 16px">Compte retire de l'etablissement</h2>
            <p>Bonjour <strong>{Escape(firstName)}</strong>,</p>
            <p>Votre compte a ete retire de l'etablissement <strong>{Escape(establishmentName)}</strong> par le directeur.</p>
            <p>Vous n'avez plus acces aux donnees de cet etablissement. Si vous pensez qu'il s'agit d'une erreur, veuillez contacter directement l'etablissement ou notre support.</p>
            <div style="text-align:center;margin:32px 0">
                <a href="mailto:{SupportEmail}" style="background:#16a34a;color:#fff;padding:14px 32px;text-decoration:none;border-radius:8px;font-weight:600;display:inline-block">
                    Contacter le support
                </a>
            </div>
            """);

        return SendEmailAsync(email, $"Votre compte a ete retire de {establishmentName}", body, null, ct);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // EMAILS CGU
    // ═══════════════════════════════════════════════════════════════════════

    public Task SendCguAcceptedEmailAsync(string email, string firstName, string cguVersion, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow.ToString("dd/MM/yyyy a HH:mm UTC");
        var body = WrapInLayout($"""
            <h2 style="color:#16a34a;margin:0 0 16px">Conditions generales acceptees</h2>
            <p>Bonjour <strong>{Escape(firstName)}</strong>,</p>
            <p>Nous confirmons que vous avez accepte les Conditions Generales d'Utilisation (CGU) version <strong>{Escape(cguVersion)}</strong> le <strong>{now}</strong>.</p>
            <p>Vous pouvez consulter les CGU a tout moment depuis votre espace utilisateur.</p>
            <div style="text-align:center;margin:32px 0">
                <a href="{AppUrl}" style="background:#16a34a;color:#fff;padding:14px 32px;text-decoration:none;border-radius:8px;font-weight:600;display:inline-block">
                    Acceder a mon espace
                </a>
            </div>
            """);

        return SendEmailAsync(email, "Confirmation d'acceptation des CGU Kouroukan", body, null, ct);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // EMAILS ADMIN ABONNEMENT
    // ═══════════════════════════════════════════════════════════════════════

    public Task SendAdminSubscriptionCreatedEmailAsync(string email, string firstName, string planName, string startDate, CancellationToken ct = default)
    {
        var body = WrapInLayout($"""
            <h2 style="color:#16a34a;margin:0 0 16px">Nouvel abonnement active</h2>
            <p>Bonjour <strong>{Escape(firstName)}</strong>,</p>
            <p>Un abonnement au forfait <strong>{Escape(planName)}</strong> a ete active pour votre compte par l'equipe Kouroukan.</p>
            <table style="width:100%;border-collapse:collapse;margin:24px 0;background:#f9fafb;border-radius:8px">
                <tr>
                    <td style="padding:12px 16px;border-bottom:1px solid #e5e7eb;color:#666;width:40%">Forfait</td>
                    <td style="padding:12px 16px;border-bottom:1px solid #e5e7eb;font-weight:600">{Escape(planName)}</td>
                </tr>
                <tr>
                    <td style="padding:12px 16px;color:#666">Date de debut</td>
                    <td style="padding:12px 16px;font-weight:600">{Escape(startDate)}</td>
                </tr>
            </table>
            <p>Vous pouvez gerer votre abonnement depuis les parametres de votre compte.</p>
            <div style="text-align:center;margin:32px 0">
                <a href="{AppUrl}" style="background:#16a34a;color:#fff;padding:14px 32px;text-decoration:none;border-radius:8px;font-weight:600;display:inline-block">
                    Mon espace
                </a>
            </div>
            """);

        return SendEmailAsync(email, $"Abonnement active — Forfait {planName}", body, null, ct);
    }

    public Task SendAdminSubscriptionUpdatedEmailAsync(string email, string firstName, string planName, CancellationToken ct = default)
    {
        var body = WrapInLayout($"""
            <h2 style="color:#f59e0b;margin:0 0 16px">Abonnement modifie</h2>
            <p>Bonjour <strong>{Escape(firstName)}</strong>,</p>
            <p>Votre abonnement au forfait <strong>{Escape(planName)}</strong> a ete modifie par l'equipe Kouroukan.</p>
            <p>Consultez les details de votre abonnement depuis votre espace pour voir les changements.</p>
            <div style="text-align:center;margin:32px 0">
                <a href="{AppUrl}" style="background:#16a34a;color:#fff;padding:14px 32px;text-decoration:none;border-radius:8px;font-weight:600;display:inline-block">
                    Voir mon abonnement
                </a>
            </div>
            """);

        return SendEmailAsync(email, $"Votre abonnement Kouroukan a ete modifie — Forfait {planName}", body, null, ct);
    }

    public Task SendAdminSubscriptionDeletedEmailAsync(string email, string firstName, string planName, CancellationToken ct = default)
    {
        var body = WrapInLayout($"""
            <h2 style="color:#e11d48;margin:0 0 16px">Abonnement supprime</h2>
            <p>Bonjour <strong>{Escape(firstName)}</strong>,</p>
            <p>Votre abonnement au forfait <strong>{Escape(planName)}</strong> a ete supprime par l'equipe Kouroukan.</p>
            <p>Si vous pensez qu'il s'agit d'une erreur ou si vous avez des questions, contactez notre support a <a href="mailto:{SupportEmail}" style="color:#16a34a">{SupportEmail}</a>.</p>
            <div style="text-align:center;margin:32px 0">
                <a href="mailto:{SupportEmail}" style="background:#16a34a;color:#fff;padding:14px 32px;text-decoration:none;border-radius:8px;font-weight:600;display:inline-block">
                    Contacter le support
                </a>
            </div>
            """);

        return SendEmailAsync(email, $"Votre abonnement Kouroukan a ete supprime — Forfait {planName}", body, null, ct);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // EMAIL DE RAPPORT DE DEPLOIEMENT
    // ═══════════════════════════════════════════════════════════════════════

    public Task SendDeploymentReportEmailAsync(string to, Models.DeploymentReportRequest report, CancellationToken ct = default)
    {
        var isSuccess = report.Status.Equals("success", StringComparison.OrdinalIgnoreCase);
        var statusIcon = isSuccess ? "&#9989;" : "&#10060;";
        var statusLabel = isSuccess ? "Deploiement reussi" : "Echec du deploiement";
        var statusColor = isSuccess ? "#16a34a" : "#dc2626";
        var envLabel = report.Environment.Equals("production", StringComparison.OrdinalIgnoreCase) ? "PRODUCTION" : "TEST";
        var envColor = report.Environment.Equals("production", StringComparison.OrdinalIgnoreCase) ? "#dc2626" : "#2563eb";

        var duration = report.DeploymentDurationSeconds > 0
            ? $"{report.DeploymentDurationSeconds / 60}m {report.DeploymentDurationSeconds % 60}s"
            : "N/A";

        // Health checks table rows
        var healthChecksRows = "";
        foreach (var (domain, code) in report.HealthChecks)
        {
            var hcIcon = code.StartsWith("2") ? "&#9989;" : (code == "000" ? "&#9888;" : "&#10060;");
            var hcColor = code.StartsWith("2") ? "#16a34a" : (code == "000" ? "#f59e0b" : "#dc2626");
            healthChecksRows += $"""
                <tr>
                    <td style="padding:8px 12px;border-bottom:1px solid #e5e7eb;font-family:monospace;font-size:13px">{Escape(domain)}</td>
                    <td style="padding:8px 12px;border-bottom:1px solid #e5e7eb;text-align:center;color:{hcColor};font-weight:600">{hcIcon} HTTP {Escape(code)}</td>
                </tr>
                """;
        }

        // Commits list
        var commitsHtml = "";
        if (report.Commits.Count > 0)
        {
            commitsHtml = "<ul style=\"margin:0;padding-left:20px;color:#555;font-size:13px;line-height:1.8\">";
            foreach (var commit in report.Commits.Take(15))
            {
                commitsHtml += $"<li><code style=\"background:#f3f4f6;padding:2px 6px;border-radius:4px;font-size:12px\">{Escape(commit)}</code></li>";
            }
            if (report.Commits.Count > 15)
                commitsHtml += $"<li style=\"color:#9ca3af\">... et {report.Commits.Count - 15} autres commits</li>";
            commitsHtml += "</ul>";
        }
        else
        {
            commitsHtml = "<p style=\"color:#9ca3af;font-style:italic\">Aucun commit detecte</p>";
        }

        // Errors
        var errorsHtml = "";
        if (report.Errors.Count > 0)
        {
            errorsHtml = """
                <h3 style="color:#dc2626;margin:24px 0 12px">&#9888; Erreurs detectees</h3>
                <div style="background:#fef2f2;border-left:4px solid #dc2626;padding:12px 16px;border-radius:0 8px 8px 0;margin-bottom:16px">
                """;
            foreach (var error in report.Errors)
            {
                errorsHtml += $"<p style=\"margin:4px 0;color:#991b1b;font-size:13px\">&bull; {Escape(error)}</p>";
            }
            errorsHtml += "</div>";
        }

        // Impacted services
        var servicesHtml = "";
        if (report.ImpactedServices.Count > 0)
        {
            servicesHtml = "<div style=\"margin:8px 0\">";
            foreach (var svc in report.ImpactedServices)
            {
                servicesHtml += $"<span style=\"display:inline-block;background:#dbeafe;color:#1e40af;padding:4px 10px;border-radius:12px;font-size:12px;font-weight:600;margin:2px 4px 2px 0\">{Escape(svc)}</span>";
            }
            servicesHtml += "</div>";
        }
        else
        {
            servicesHtml = "<span style=\"color:#9ca3af;font-size:13px\">Tous les services</span>";
        }

        var body = WrapInDeploymentLayout($"""
            <!-- Status banner -->
            <div style="background:{statusColor};color:#fff;padding:16px 24px;border-radius:8px;text-align:center;margin-bottom:24px">
                <span style="font-size:28px">{statusIcon}</span>
                <h2 style="margin:8px 0 4px;font-size:20px">{statusLabel}</h2>
                <span style="background:{envColor};color:#fff;padding:4px 16px;border-radius:12px;font-size:13px;font-weight:700;letter-spacing:1px">{Escape(envLabel)}</span>
            </div>

            <!-- Resume -->
            <table style="width:100%;border-collapse:collapse;margin:0 0 24px;background:#f9fafb;border-radius:8px">
                <tr>
                    <td style="padding:10px 16px;border-bottom:1px solid #e5e7eb;color:#666;width:35%">Branche</td>
                    <td style="padding:10px 16px;border-bottom:1px solid #e5e7eb;font-weight:600">{Escape(report.Branch)}</td>
                </tr>
                <tr>
                    <td style="padding:10px 16px;border-bottom:1px solid #e5e7eb;color:#666">Version</td>
                    <td style="padding:10px 16px;border-bottom:1px solid #e5e7eb;font-weight:600;font-family:monospace">{Escape(report.AppVersion)}</td>
                </tr>
                <tr>
                    <td style="padding:10px 16px;border-bottom:1px solid #e5e7eb;color:#666">Commit</td>
                    <td style="padding:10px 16px;border-bottom:1px solid #e5e7eb;font-family:monospace;font-size:13px">{Escape(report.CommitSha.Length > 7 ? report.CommitSha[..7] : report.CommitSha)} — {Escape(report.CommitMessage)}</td>
                </tr>
                <tr>
                    <td style="padding:10px 16px;border-bottom:1px solid #e5e7eb;color:#666">Auteur</td>
                    <td style="padding:10px 16px;border-bottom:1px solid #e5e7eb">{Escape(report.CommitAuthor)}</td>
                </tr>
                <tr>
                    <td style="padding:10px 16px;border-bottom:1px solid #e5e7eb;color:#666">Duree</td>
                    <td style="padding:10px 16px;border-bottom:1px solid #e5e7eb;font-weight:600">{duration}</td>
                </tr>
                <tr>
                    <td style="padding:10px 16px;color:#666">Debut</td>
                    <td style="padding:10px 16px">{Escape(report.DeploymentStartedAt)}</td>
                </tr>
            </table>

            <!-- Services impactes -->
            <h3 style="color:#333;margin:0 0 8px">Services impactes</h3>
            {servicesHtml}

            <!-- Health checks -->
            <h3 style="color:#333;margin:24px 0 12px">Health checks</h3>
            <table style="width:100%;border-collapse:collapse;background:#f9fafb;border-radius:8px">
                <tr style="background:#e5e7eb">
                    <th style="padding:8px 12px;text-align:left;font-size:13px;color:#374151">Domaine</th>
                    <th style="padding:8px 12px;text-align:center;font-size:13px;color:#374151">Statut</th>
                </tr>
                {healthChecksRows}
            </table>

            {errorsHtml}

            <!-- Couverture de code -->
            {BuildCoverageHtml(report)}

            <!-- Commits deployes -->
            <h3 style="color:#333;margin:24px 0 12px">Commits deployes</h3>
            {commitsHtml}

            <!-- Actions link -->
            <div style="text-align:center;margin:32px 0 8px">
                <a href="{Escape(report.WorkflowRunUrl)}" style="background:#24292f;color:#fff;padding:12px 28px;text-decoration:none;border-radius:8px;font-weight:600;display:inline-block;font-size:14px">
                    Voir les logs GitHub Actions
                </a>
            </div>
            """);

        var subject = isSuccess
            ? $"{statusIcon} Deploiement {envLabel} reussi — {report.AppVersion}"
            : $"{statusIcon} ECHEC deploiement {envLabel} — {report.AppVersion}";

        return SendEmailAsync(to, subject, body, "noreply@kouroukan.dianora.org", ct);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // TEMPLATE HTML
    // ═══════════════════════════════════════════════════════════════════════

    private static string WrapInLayout(string content)
    {
        return $"""
            <!DOCTYPE html>
            <html lang="fr">
            <head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"></head>
            <body style="margin:0;padding:0;background:#f3f4f6;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,sans-serif">
                <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background:#f3f4f6;padding:32px 16px">
                    <tr><td align="center">
                        <table role="presentation" width="600" cellpadding="0" cellspacing="0" style="max-width:600px;width:100%">
                            <!-- Header -->
                            <tr><td style="background:#16a34a;padding:24px 32px;border-radius:12px 12px 0 0;text-align:center">
                                <h1 style="margin:0;color:#fff;font-size:24px;font-weight:700;letter-spacing:1px">KOUROUKAN</h1>
                                <p style="margin:4px 0 0;color:rgba(255,255,255,0.8);font-size:13px">Plateforme de gestion scolaire</p>
                            </td></tr>
                            <!-- Body -->
                            <tr><td style="background:#fff;padding:32px;font-size:15px;line-height:1.6;color:#333">
                                {content}
                            </td></tr>
                            <!-- Footer -->
                            <tr><td style="background:#f9fafb;padding:24px 32px;border-radius:0 0 12px 12px;border-top:1px solid #e5e7eb;text-align:center;font-size:12px;color:#9ca3af">
                                <p style="margin:0">Cet email a ete envoye automatiquement par la plateforme Kouroukan.</p>
                                <p style="margin:8px 0 0">
                                    <a href="mailto:{SupportEmail}" style="color:#16a34a;text-decoration:none">{SupportEmail}</a>
                                    &nbsp;|&nbsp;
                                    <a href="{AppUrl}" style="color:#16a34a;text-decoration:none">app.kouroukan.dianora.org</a>
                                </p>
                                <p style="margin:8px 0 0;color:#d1d5db">&copy; {DateTime.UtcNow.Year} Kouroukan — Dianora Technology</p>
                            </td></tr>
                        </table>
                    </td></tr>
                </table>
            </body>
            </html>
            """;
    }

    private static string BuildCoverageHtml(Models.DeploymentReportRequest report)
    {
        if (report.CodeCoverage.Count == 0) return "";

        var rows = "";
        foreach (var (component, pct) in report.CodeCoverage)
        {
            // Parse percentage for color
            var color = "#16a34a"; // green
            if (double.TryParse(pct.TrimEnd('%'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var val))
            {
                if (val < 50) color = "#dc2626"; // red
                else if (val < 75) color = "#f59e0b"; // amber
            }

            rows += $"""
                <tr>
                    <td style="padding:8px 12px;border-bottom:1px solid #e5e7eb;font-size:13px">{Escape(component)}</td>
                    <td style="padding:8px 12px;border-bottom:1px solid #e5e7eb;text-align:center">
                        <span style="background:{color};color:#fff;padding:3px 10px;border-radius:10px;font-size:12px;font-weight:700">{Escape(pct)}</span>
                    </td>
                </tr>
                """;
        }

        return $"""
            <h3 style="color:#333;margin:24px 0 12px">Couverture de code</h3>
            <table style="width:100%;border-collapse:collapse;background:#f9fafb;border-radius:8px">
                <tr style="background:#e5e7eb">
                    <th style="padding:8px 12px;text-align:left;font-size:13px;color:#374151">Composant</th>
                    <th style="padding:8px 12px;text-align:center;font-size:13px;color:#374151">Couverture</th>
                </tr>
                {rows}
            </table>
            """;
    }

    /// <summary>Layout specifique pour les emails de deploiement (header sombre).</summary>
    private static string WrapInDeploymentLayout(string content)
    {
        return $"""
            <!DOCTYPE html>
            <html lang="fr">
            <head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"></head>
            <body style="margin:0;padding:0;background:#f3f4f6;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,sans-serif">
                <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background:#f3f4f6;padding:32px 16px">
                    <tr><td align="center">
                        <table role="presentation" width="640" cellpadding="0" cellspacing="0" style="max-width:640px;width:100%">
                            <!-- Header -->
                            <tr><td style="background:#1e293b;padding:20px 32px;border-radius:12px 12px 0 0;text-align:center">
                                <h1 style="margin:0;color:#fff;font-size:20px;font-weight:700;letter-spacing:1px">KOUROUKAN</h1>
                                <p style="margin:4px 0 0;color:rgba(255,255,255,0.6);font-size:12px">Rapport de deploiement</p>
                            </td></tr>
                            <!-- Body -->
                            <tr><td style="background:#fff;padding:28px 32px;font-size:14px;line-height:1.6;color:#333">
                                {content}
                            </td></tr>
                            <!-- Footer -->
                            <tr><td style="background:#f9fafb;padding:20px 32px;border-radius:0 0 12px 12px;border-top:1px solid #e5e7eb;text-align:center;font-size:11px;color:#9ca3af">
                                <p style="margin:0">Email automatique genere par le pipeline CI/CD Kouroukan.</p>
                                <p style="margin:6px 0 0">&copy; {DateTime.UtcNow.Year} Kouroukan — Dianora Technology</p>
                            </td></tr>
                        </table>
                    </td></tr>
                </table>
            </body>
            </html>
            """;
    }

    private static string TranslateRole(string role)
    {
        return role.ToLowerInvariant() switch
        {
            "directeur" => "Directeur",
            "censeur" => "Censeur",
            "enseignant" => "Enseignant",
            "surveillant" => "Surveillant General",
            "secretaire" => "Secretaire",
            "comptable" => "Comptable",
            "bibliothecaire" => "Bibliothecaire",
            "parent" => "Parent d'eleve",
            _ => role
        };
    }

    private static string Escape(string? value)
    {
        return System.Net.WebUtility.HtmlEncode(value ?? string.Empty);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // CONFIG SMTP (avec cache 5 minutes)
    // ═══════════════════════════════════════════════════════════════════════

    private async Task<SmtpConfig?> GetSmtpConfigAsync(CancellationToken ct)
    {
        if (_cachedConfig is not null && DateTime.UtcNow < _cacheExpiry)
            return _cachedConfig;

        await _cacheLock.WaitAsync(ct);
        try
        {
            // Double-check after lock
            if (_cachedConfig is not null && DateTime.UtcNow < _cacheExpiry)
                return _cachedConfig;

            using var connection = _connectionFactory.CreateConnection();
            _cachedConfig = await connection.QuerySingleOrDefaultAsync<SmtpConfig>(
                """
                SELECT smtp_host AS Host, smtp_port AS Port,
                       smtp_user AS Username, smtp_password AS Password,
                       email_expediteur AS FromEmail, nom_expediteur AS FromName
                FROM support.email_config
                WHERE est_actif = TRUE
                LIMIT 1
                """);

            _cacheExpiry = DateTime.UtcNow.AddMinutes(5);
            return _cachedConfig;
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    /// <summary>Invalide le cache SMTP (utile apres mise a jour de la config).</summary>
    public void InvalidateCache()
    {
        _cacheExpiry = DateTime.MinValue;
        _cachedConfig = null;
    }

    private sealed class SmtpConfig
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = "Kouroukan";
    }
}
