-- ============================================================================
-- V002__auth_tables.sql
-- Kouroukan - Tables d'authentification, roles, permissions, CGU
-- ============================================================================

-- ----------------------------------------------------------------------------
-- auth.users
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS auth.users (
    id              INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    first_name      VARCHAR(100)                NOT NULL,
    last_name       VARCHAR(100)                NOT NULL,
    email           VARCHAR(200)                NOT NULL,
    phone_number    VARCHAR(20)                 NOT NULL,
    password_hash   VARCHAR(500)                NOT NULL,
    is_active       BOOLEAN                     NOT NULL DEFAULT TRUE,
    last_login_at   TIMESTAMP WITH TIME ZONE    NULL,
    cgu_accepted_at TIMESTAMP WITH TIME ZONE    NULL,
    cgu_version     VARCHAR(20)                 NULL,
    -- Audit
    created_at      TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE    NULL,
    created_by      VARCHAR(100)                NOT NULL DEFAULT 'system',
    updated_by      VARCHAR(100)                NULL,
    -- Soft delete
    is_deleted      BOOLEAN                     NOT NULL DEFAULT FALSE,
    deleted_at      TIMESTAMP WITH TIME ZONE    NULL,
    deleted_by      VARCHAR(100)                NULL
);

-- ----------------------------------------------------------------------------
-- auth.roles
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS auth.roles (
    id              INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name            VARCHAR(100)                NOT NULL,
    description     VARCHAR(500)                NULL,
    -- Audit
    created_at      TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE    NULL,
    created_by      VARCHAR(100)                NOT NULL DEFAULT 'system',
    updated_by      VARCHAR(100)                NULL,
    -- Soft delete
    is_deleted      BOOLEAN                     NOT NULL DEFAULT FALSE,
    deleted_at      TIMESTAMP WITH TIME ZONE    NULL,
    deleted_by      VARCHAR(100)                NULL
);

-- ----------------------------------------------------------------------------
-- auth.permissions
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS auth.permissions (
    id              INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name            VARCHAR(200)                NOT NULL,
    description     VARCHAR(500)                NULL,
    module          VARCHAR(100)                NULL,
    -- Audit
    created_at      TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE    NULL,
    created_by      VARCHAR(100)                NOT NULL DEFAULT 'system',
    updated_by      VARCHAR(100)                NULL,
    -- Soft delete
    is_deleted      BOOLEAN                     NOT NULL DEFAULT FALSE,
    deleted_at      TIMESTAMP WITH TIME ZONE    NULL,
    deleted_by      VARCHAR(100)                NULL
);

-- ----------------------------------------------------------------------------
-- auth.user_roles
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS auth.user_roles (
    id              INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id         INT                         NOT NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    role_id         INT                         NOT NULL REFERENCES auth.roles(id) ON DELETE NO ACTION,
    -- Audit
    created_at      TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE    NULL,
    created_by      VARCHAR(100)                NOT NULL DEFAULT 'system',
    updated_by      VARCHAR(100)                NULL,
    -- Soft delete
    is_deleted      BOOLEAN                     NOT NULL DEFAULT FALSE,
    deleted_at      TIMESTAMP WITH TIME ZONE    NULL,
    deleted_by      VARCHAR(100)                NULL,
    -- Contrainte d'unicite
    CONSTRAINT uq_user_roles UNIQUE (user_id, role_id)
);

-- ----------------------------------------------------------------------------
-- auth.role_permissions
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS auth.role_permissions (
    id              INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    role_id         INT                         NOT NULL REFERENCES auth.roles(id) ON DELETE NO ACTION,
    permission_id   INT                         NOT NULL REFERENCES auth.permissions(id) ON DELETE NO ACTION,
    -- Audit
    created_at      TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE    NULL,
    created_by      VARCHAR(100)                NOT NULL DEFAULT 'system',
    updated_by      VARCHAR(100)                NULL,
    -- Soft delete
    is_deleted      BOOLEAN                     NOT NULL DEFAULT FALSE,
    deleted_at      TIMESTAMP WITH TIME ZONE    NULL,
    deleted_by      VARCHAR(100)                NULL,
    -- Contrainte d'unicite
    CONSTRAINT uq_role_permissions UNIQUE (role_id, permission_id)
);

-- ----------------------------------------------------------------------------
-- auth.refresh_tokens
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS auth.refresh_tokens (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id             INT                         NOT NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    token               VARCHAR(500)                NOT NULL,
    expires_at          TIMESTAMP WITH TIME ZONE    NOT NULL,
    is_revoked          BOOLEAN                     NOT NULL DEFAULT FALSE,
    revoked_at          TIMESTAMP WITH TIME ZONE    NULL,
    replaced_by_token   VARCHAR(500)                NULL,
    created_at          TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW()
);

-- ----------------------------------------------------------------------------
-- auth.companies (tenants / etablissements)
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS auth.companies (
    id              INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name            VARCHAR(200)                NOT NULL,
    description     VARCHAR(500)                NULL,
    address         VARCHAR(500)                NULL,
    phone_number    VARCHAR(20)                 NULL,
    email           VARCHAR(200)                NULL,
    -- Audit
    created_at      TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE    NULL,
    created_by      VARCHAR(100)                NOT NULL DEFAULT 'system',
    updated_by      VARCHAR(100)                NULL,
    -- Soft delete
    is_deleted      BOOLEAN                     NOT NULL DEFAULT FALSE,
    deleted_at      TIMESTAMP WITH TIME ZONE    NULL,
    deleted_by      VARCHAR(100)                NULL
);

-- ----------------------------------------------------------------------------
-- auth.user_companies
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS auth.user_companies (
    id              INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id         INT                         NOT NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    company_id      INT                         NOT NULL REFERENCES auth.companies(id) ON DELETE NO ACTION,
    role            VARCHAR(50)                 NOT NULL DEFAULT 'member',
    -- Audit
    created_at      TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE    NULL,
    created_by      VARCHAR(100)                NOT NULL DEFAULT 'system',
    updated_by      VARCHAR(100)                NULL,
    -- Soft delete
    is_deleted      BOOLEAN                     NOT NULL DEFAULT FALSE,
    deleted_at      TIMESTAMP WITH TIME ZONE    NULL,
    deleted_by      VARCHAR(100)                NULL,
    -- Contrainte d'unicite
    CONSTRAINT uq_user_companies UNIQUE (user_id, company_id)
);

-- ----------------------------------------------------------------------------
-- auth.cgu_versions
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS auth.cgu_versions (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    version             VARCHAR(20)                 NOT NULL,
    contenu             TEXT                        NOT NULL,
    date_publication    TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    est_active          BOOLEAN                     NOT NULL DEFAULT FALSE,
    -- Audit
    created_at          TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMP WITH TIME ZONE    NULL,
    created_by          VARCHAR(100)                NOT NULL DEFAULT 'system',
    updated_by          VARCHAR(100)                NULL,
    -- Soft delete
    is_deleted          BOOLEAN                     NOT NULL DEFAULT FALSE,
    deleted_at          TIMESTAMP WITH TIME ZONE    NULL,
    deleted_by          VARCHAR(100)                NULL
);
