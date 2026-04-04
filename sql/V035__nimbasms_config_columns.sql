-- =============================================================================
-- V035 : Ajout colonnes NimbaSMS à sms_config + colonne envoi test
-- =============================================================================

ALTER TABLE support.sms_config ADD COLUMN IF NOT EXISTS service_id VARCHAR(200) NOT NULL DEFAULT '';
ALTER TABLE support.sms_config ADD COLUMN IF NOT EXISTS cout_unitaire INTEGER NOT NULL DEFAULT 200;

-- Renommer api_key -> on garde api_key pour stocker le secret_token NimbaSMS
-- service_id = ACCOUNT_SID / SERVICE_ID NimbaSMS
-- api_key = AUTH_TOKEN / SECRET_TOKEN NimbaSMS
-- sender_name = Nom expediteur approuvé NimbaSMS

COMMENT ON COLUMN support.sms_config.service_id IS 'NimbaSMS SERVICE_ID (ACCOUNT_SID)';
COMMENT ON COLUMN support.sms_config.api_key IS 'NimbaSMS SECRET_TOKEN (AUTH_TOKEN)';
COMMENT ON COLUMN support.sms_config.sender_name IS 'Nom expediteur approuve sur NimbaSMS';
COMMENT ON COLUMN support.sms_config.cout_unitaire IS 'Cout unitaire SMS en GNF';
