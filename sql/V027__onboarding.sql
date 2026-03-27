-- V027: Ajouter le suivi de l'onboarding sur les etablissements
ALTER TABLE auth.companies
    ADD COLUMN IF NOT EXISTS onboarding_step INT NOT NULL DEFAULT 0,
    ADD COLUMN IF NOT EXISTS onboarding_completed_at TIMESTAMP NULL;
