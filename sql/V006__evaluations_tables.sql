-- ============================================================================
-- V006__evaluations_tables.sql
-- Kouroukan - Module Evaluations (port 5003, schema evaluations)
-- ============================================================================

-- ----------------------------------------------------------------------------
-- Tables de types
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS evaluations.type_evaluations (
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
-- evaluations.evaluations
-- FK vers personnel.enseignants ajoutee dans V015
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS evaluations.evaluations (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id             INT                         NOT NULL REFERENCES evaluations.type_evaluations(id) ON DELETE NO ACTION,
    matiere_id          INT                         NOT NULL REFERENCES pedagogie.matieres(id) ON DELETE NO ACTION,
    classe_id           INT                         NOT NULL REFERENCES pedagogie.classes(id) ON DELETE NO ACTION,
    enseignant_id       INT                         NOT NULL,   -- FK → personnel.enseignants (V015)
    date_evaluation     DATE                        NOT NULL,
    coefficient         NUMERIC(5,2)                NOT NULL,
    note_maximale       NUMERIC(5,2)                NOT NULL,   -- 20, 10...
    trimestre           INT                         NOT NULL,   -- 1, 2, 3 ou Semestre
    annee_scolaire_id   INT                         NOT NULL REFERENCES inscriptions.annees_scolaires(id) ON DELETE NO ACTION,
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
-- evaluations.notes
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS evaluations.notes (
    id              INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    evaluation_id   INT                         NOT NULL REFERENCES evaluations.evaluations(id) ON DELETE NO ACTION,
    eleve_id        INT                         NOT NULL REFERENCES inscriptions.eleves(id) ON DELETE NO ACTION,
    valeur          NUMERIC(5,2)                NOT NULL,
    commentaire     VARCHAR(500)                NULL,
    date_saisie     TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
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
-- evaluations.bulletins
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS evaluations.bulletins (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    eleve_id            INT                         NOT NULL REFERENCES inscriptions.eleves(id) ON DELETE NO ACTION,
    classe_id           INT                         NOT NULL REFERENCES pedagogie.classes(id) ON DELETE NO ACTION,
    trimestre           INT                         NOT NULL,
    annee_scolaire_id   INT                         NOT NULL REFERENCES inscriptions.annees_scolaires(id) ON DELETE NO ACTION,
    moyenne_generale    NUMERIC(5,2)                NOT NULL,
    rang                INT                         NULL,
    appreciation        VARCHAR(500)                NULL,
    est_publie          BOOLEAN                     NOT NULL DEFAULT FALSE,
    date_generation     TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    chemin_fichier_pdf  VARCHAR(500)                NULL,
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
