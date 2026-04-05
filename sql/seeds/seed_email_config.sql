-- =============================================================================
-- Seed : Configuration SMTP Resend pour l'envoi d'emails
-- =============================================================================
-- A executer sur l'environnement de test uniquement.
-- La configuration production sera ajoutee separement.
-- =============================================================================

INSERT INTO support.email_config (
    smtp_host,
    smtp_port,
    smtp_user,
    smtp_password,
    email_expediteur,
    nom_expediteur,
    est_actif
)
SELECT
    'smtp.resend.com',
    587,
    'resend',
    're_ho2Rd6eg_EUicRE5ieZSPoZGHLLFKfbm1',
    'noreply@kouroukan.dianora.org',
    'Kouroukan',
    TRUE
WHERE NOT EXISTS (
    SELECT 1 FROM support.email_config WHERE email_expediteur = 'noreply@kouroukan.dianora.org'
);
