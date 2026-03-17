-- ============================================================================
-- V013__services_premium_tables.sql
-- Kouroukan - Module Services Premium (port 5010, schema services)
-- ============================================================================

-- ----------------------------------------------------------------------------
-- Tables de types
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS services.type_services_parents (
    id          INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name        VARCHAR(100)                NOT NULL,
    description VARCHAR(255)                NULL,
    created_at  TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    updated_at  TIMESTAMP WITH TIME ZONE    NULL,
    created_by  VARCHAR(100)                NOT NULL DEFAULT 'system',
    updated_by  VARCHAR(100)                NULL,
    is_deleted  BOOLEAN                     NOT NULL DEFAULT FALSE,
    deleted_at  TIMESTAMP WITH TIME ZONE    NULL,
    deleted_by  VARCHAR(100)                NULL
);

-- ----------------------------------------------------------------------------
-- services.services_parents
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS services.services_parents (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id             INT                         NOT NULL REFERENCES services.type_services_parents(id) ON DELETE NO ACTION,
    name                VARCHAR(200)                NOT NULL,
    description         VARCHAR(500)                NULL,
    code                VARCHAR(50)                 NOT NULL,
    tarif               NUMERIC(10,2)               NOT NULL,
    periodicite         VARCHAR(30)                 NOT NULL,   -- Mensuel/Annuel/Unite
    est_actif           BOOLEAN                     NOT NULL DEFAULT TRUE,
    periode_essai_jours INT                         NULL,
    tarif_degressif     BOOLEAN                     NOT NULL DEFAULT FALSE,
    user_id             INT                         NOT NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    -- Audit
    created_at          TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMP WITH TIME ZONE    NULL,
    created_by          VARCHAR(100)                NOT NULL DEFAULT 'system',
    updated_by          VARCHAR(100)                NULL,
    is_deleted          BOOLEAN                     NOT NULL DEFAULT FALSE,
    deleted_at          TIMESTAMP WITH TIME ZONE    NULL,
    deleted_by          VARCHAR(100)                NULL
);

-- ----------------------------------------------------------------------------
-- services.souscriptions
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS services.souscriptions (
    id                              INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    service_parent_id               INT                         NOT NULL REFERENCES services.services_parents(id) ON DELETE NO ACTION,
    parent_id                       INT                         NOT NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    date_debut                      DATE                        NOT NULL,
    date_fin                        DATE                        NULL,
    statut_souscription             VARCHAR(30)                 NOT NULL,   -- Active/Expiree/Resiliee/Essai
    montant_paye                    NUMERIC(10,2)               NOT NULL DEFAULT 0,
    renouvellement_auto             BOOLEAN                     NOT NULL DEFAULT FALSE,
    date_prochain_renouvellement    DATE                        NULL,
    user_id                         INT                         NOT NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    -- Audit
    created_at                      TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    updated_at                      TIMESTAMP WITH TIME ZONE    NULL,
    created_by                      VARCHAR(100)                NOT NULL DEFAULT 'system',
    updated_by                      VARCHAR(100)                NULL,
    is_deleted                      BOOLEAN                     NOT NULL DEFAULT FALSE,
    deleted_at                      TIMESTAMP WITH TIME ZONE    NULL,
    deleted_by                      VARCHAR(100)                NULL
);
