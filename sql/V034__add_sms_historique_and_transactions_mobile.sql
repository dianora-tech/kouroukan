-- =============================================================================
-- V034 : Ajout des tables sms_historique et transactions_mobile
-- =============================================================================

-- Historique des envois SMS
CREATE TABLE IF NOT EXISTS support.sms_historique (
    id              SERIAL PRIMARY KEY,
    destinataire    VARCHAR(20)     NOT NULL,
    message         TEXT            NOT NULL,
    statut          VARCHAR(20)     NOT NULL DEFAULT 'en_attente',
    cout            INTEGER         NOT NULL DEFAULT 0,
    created_at      TIMESTAMPTZ     NOT NULL DEFAULT NOW(),
    created_by      VARCHAR(100)    NOT NULL DEFAULT 'system'
);

COMMENT ON TABLE support.sms_historique IS 'Historique des SMS envoyes depuis la plateforme';

-- Transactions Mobile Money (receptions/envois sur comptes admin)
CREATE TABLE IF NOT EXISTS finances.transactions_mobile (
    id              SERIAL PRIMARY KEY,
    compte_id       INTEGER         NOT NULL REFERENCES finances.comptes_admin(id),
    operateur       VARCHAR(50)     NOT NULL,
    type            VARCHAR(20)     NOT NULL,
    montant         INTEGER         NOT NULL,
    reference       VARCHAR(100),
    statut          VARCHAR(20)     NOT NULL DEFAULT 'en_attente',
    created_at      TIMESTAMPTZ     NOT NULL DEFAULT NOW(),
    created_by      VARCHAR(100)    NOT NULL DEFAULT 'system'
);

COMMENT ON TABLE finances.transactions_mobile IS 'Transactions Mobile Money sur les comptes admin';
