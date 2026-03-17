-- ============================================================================
-- V007__presences_tables.sql
-- Kouroukan - Module Presences (port 5004, schema presences)
-- ============================================================================

-- ----------------------------------------------------------------------------
-- Tables de types
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS presences.type_absences (
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

CREATE TABLE IF NOT EXISTS presences.type_badgeages (
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
-- presences.appels
-- FK vers personnel.enseignants ajoutee dans V015
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS presences.appels (
    id              INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    classe_id       INT                         NOT NULL REFERENCES pedagogie.classes(id) ON DELETE NO ACTION,
    enseignant_id   INT                         NOT NULL,   -- FK → personnel.enseignants (V015)
    seance_id       INT                         NULL REFERENCES pedagogie.seances(id) ON DELETE NO ACTION,
    date_appel      DATE                        NOT NULL,
    heure_appel     TIME                        NOT NULL,
    est_cloture     BOOLEAN                     NOT NULL DEFAULT FALSE,
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

-- ----------------------------------------------------------------------------
-- presences.absences
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS presences.absences (
    id                      INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id                 INT                         NOT NULL REFERENCES presences.type_absences(id) ON DELETE NO ACTION,
    eleve_id                INT                         NOT NULL REFERENCES inscriptions.eleves(id) ON DELETE NO ACTION,
    appel_id                INT                         NULL REFERENCES presences.appels(id) ON DELETE NO ACTION,
    date_absence            DATE                        NOT NULL,
    heure_debut             TIME                        NULL,
    heure_fin               TIME                        NULL,
    est_justifiee           BOOLEAN                     NOT NULL DEFAULT FALSE,
    motif_justification     VARCHAR(500)                NULL,
    piece_jointe_url        VARCHAR(500)                NULL,
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
-- presences.badgeages
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS presences.badgeages (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id             INT                         NOT NULL REFERENCES presences.type_badgeages(id) ON DELETE NO ACTION,
    eleve_id            INT                         NOT NULL REFERENCES inscriptions.eleves(id) ON DELETE NO ACTION,
    date_badgeage       DATE                        NOT NULL,
    heure_badgeage      TIME                        NOT NULL,
    point_acces         VARCHAR(100)                NOT NULL,   -- Entree/Sortie/Cantine/Biblio
    methode_badgeage    VARCHAR(20)                 NOT NULL,   -- NFC/QRCode/Manuel
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
