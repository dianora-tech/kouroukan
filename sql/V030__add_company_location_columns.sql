-- =============================================================================
-- V030 : Ajout des colonnes de localisation sur auth.companies
-- Permet de stocker region, prefecture, sous-prefecture pour chaque établissement
-- =============================================================================

ALTER TABLE auth.companies ADD COLUMN IF NOT EXISTS region_code VARCHAR(10);
ALTER TABLE auth.companies ADD COLUMN IF NOT EXISTS prefecture_code VARCHAR(10);
ALTER TABLE auth.companies ADD COLUMN IF NOT EXISTS sous_prefecture_code VARCHAR(10);
