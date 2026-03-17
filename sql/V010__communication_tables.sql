-- ============================================================================
-- V010__communication_tables.sql
-- Kouroukan - Module Communication (port 5007, schema communication)
-- ============================================================================

-- ----------------------------------------------------------------------------
-- Tables de types
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS communication.type_messages (
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

CREATE TABLE IF NOT EXISTS communication.type_notifications (
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

CREATE TABLE IF NOT EXISTS communication.type_annonces (
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
-- communication.messages
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS communication.messages (
    id                      INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id                 INT                         NOT NULL REFERENCES communication.type_messages(id) ON DELETE NO ACTION,
    expediteur_id           INT                         NOT NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    destinataire_id         INT                         NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    sujet                   VARCHAR(200)                NOT NULL,
    contenu                 TEXT                        NOT NULL,
    est_lu                  BOOLEAN                     NOT NULL DEFAULT FALSE,
    date_lecture            TIMESTAMP WITH TIME ZONE    NULL,
    groupe_destinataire     VARCHAR(100)                NULL,   -- Groupe cible (classe, niveau, role)
    user_id                 INT                         NOT NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    -- Audit
    created_at              TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    updated_at              TIMESTAMP WITH TIME ZONE    NULL,
    created_by              VARCHAR(100)                NOT NULL DEFAULT 'system',
    updated_by              VARCHAR(100)                NULL,
    is_deleted              BOOLEAN                     NOT NULL DEFAULT FALSE,
    deleted_at              TIMESTAMP WITH TIME ZONE    NULL,
    deleted_by              VARCHAR(100)                NULL
);

-- ----------------------------------------------------------------------------
-- communication.notifications
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS communication.notifications (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id             INT                         NOT NULL REFERENCES communication.type_notifications(id) ON DELETE NO ACTION,
    destinataires_ids   TEXT                        NOT NULL,   -- JSON array d'IDs
    contenu             VARCHAR(500)                NOT NULL,
    canal               VARCHAR(30)                 NOT NULL,   -- Push/SMS/Email/InApp
    est_envoyee         BOOLEAN                     NOT NULL DEFAULT FALSE,
    date_envoi          TIMESTAMP WITH TIME ZONE    NULL,
    lien_action         VARCHAR(500)                NULL,       -- Deep link
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
-- communication.annonces
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS communication.annonces (
    id              INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id         INT                         NOT NULL REFERENCES communication.type_annonces(id) ON DELETE NO ACTION,
    name            VARCHAR(200)                NOT NULL,   -- Titre de l'annonce
    contenu         TEXT                        NOT NULL,
    date_debut      DATE                        NOT NULL,
    date_fin        DATE                        NULL,
    est_active      BOOLEAN                     NOT NULL DEFAULT TRUE,
    cible_audience  VARCHAR(100)                NOT NULL,   -- Tous/Parents/Enseignants/Eleves
    priorite        INT                         NOT NULL DEFAULT 3,   -- 1=haute
    user_id         INT                         NOT NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    -- Audit
    created_at      TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE    NULL,
    created_by      VARCHAR(100)                NOT NULL DEFAULT 'system',
    updated_by      VARCHAR(100)                NULL,
    is_deleted      BOOLEAN                     NOT NULL DEFAULT FALSE,
    deleted_at      TIMESTAMP WITH TIME ZONE    NULL,
    deleted_by      VARCHAR(100)                NULL
);
