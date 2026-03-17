-- ============================================================================
-- V003__geo_tables.sql
-- Kouroukan - Tables geographiques (Guinee - Conakry)
-- ============================================================================

-- ----------------------------------------------------------------------------
-- geo.regions (8 regions administratives de Guinee)
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS geo.regions (
    id          INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name        VARCHAR(100)                NOT NULL,
    code        VARCHAR(10)                 NOT NULL,
    -- Audit
    created_at  TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    updated_at  TIMESTAMP WITH TIME ZONE    NULL,
    created_by  VARCHAR(100)                NOT NULL DEFAULT 'system',
    updated_by  VARCHAR(100)                NULL,
    -- Soft delete
    is_deleted  BOOLEAN                     NOT NULL DEFAULT FALSE,
    deleted_at  TIMESTAMP WITH TIME ZONE    NULL,
    deleted_by  VARCHAR(100)                NULL
);

-- ----------------------------------------------------------------------------
-- geo.prefectures
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS geo.prefectures (
    id          INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name        VARCHAR(100)                NOT NULL,
    code        VARCHAR(10)                 NOT NULL,
    region_id   INT                         NOT NULL REFERENCES geo.regions(id) ON DELETE NO ACTION,
    -- Audit
    created_at  TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    updated_at  TIMESTAMP WITH TIME ZONE    NULL,
    created_by  VARCHAR(100)                NOT NULL DEFAULT 'system',
    updated_by  VARCHAR(100)                NULL,
    -- Soft delete
    is_deleted  BOOLEAN                     NOT NULL DEFAULT FALSE,
    deleted_at  TIMESTAMP WITH TIME ZONE    NULL,
    deleted_by  VARCHAR(100)                NULL
);

-- ----------------------------------------------------------------------------
-- geo.sous_prefectures
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS geo.sous_prefectures (
    id              INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name            VARCHAR(100)                NOT NULL,
    code            VARCHAR(20)                 NULL,
    prefecture_id   INT                         NOT NULL REFERENCES geo.prefectures(id) ON DELETE NO ACTION,
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
-- geo.user_locations
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS geo.user_locations (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id             INT                         NOT NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    sous_prefecture_id  INT                         NOT NULL REFERENCES geo.sous_prefectures(id) ON DELETE NO ACTION,
    address             VARCHAR(500)                NULL,
    latitude            NUMERIC(9,6)                NULL,
    longitude           NUMERIC(9,6)                NULL,
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
