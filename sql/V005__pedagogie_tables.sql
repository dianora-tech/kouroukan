-- ============================================================================
-- V005__pedagogie_tables.sql
-- Kouroukan - Module Pedagogie (port 5002, schema pedagogie)
-- ============================================================================

-- ----------------------------------------------------------------------------
-- Tables de types
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS pedagogie.type_niveaux_classes (
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

CREATE TABLE IF NOT EXISTS pedagogie.type_matieres (
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

CREATE TABLE IF NOT EXISTS pedagogie.type_salles (
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
-- pedagogie.niveaux_classes
-- Nomenclature guineenne : PS→GS, CP1→CM2, 7E→10E, 11E→TLE, L1→M2
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS pedagogie.niveaux_classes (
    id                      INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id                 INT                         NOT NULL REFERENCES pedagogie.type_niveaux_classes(id) ON DELETE NO ACTION,
    name                    VARCHAR(200)                NOT NULL,   -- Ex: "Petite Section", "7eme annee"
    code                    VARCHAR(20)                 NOT NULL,   -- Ex: PS, CP1, 7E, TLE, L1
    ordre                   INT                         NOT NULL,   -- Tri global (1=PS ... 21=M2)
    cycle_etude             VARCHAR(50)                 NOT NULL,   -- Prescolaire/Primaire/College/Lycee/ETFP.../Universite
    age_officiel_entree     INT                         NULL,       -- Age officiel (source ProDEG)
    ministere_tutelle       VARCHAR(20)                 NULL,       -- MENA/METFP-ET/MESRS
    examen_sortie           VARCHAR(20)                 NULL,       -- CEE/BEPC/BU/CQP/BEP/CAP/BTS/Licence/Master/Doctorat
    taux_horaire_enseignant NUMERIC(15,2)               NULL,       -- Taux horaire par defaut (GNF)
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
-- pedagogie.classes
-- FK vers personnel.enseignants ajoutee dans V015
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS pedagogie.classes (
    id                      INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name                    VARCHAR(200)                NOT NULL,   -- Ex: "7E-A", "CM1-B"
    niveau_classe_id        INT                         NOT NULL REFERENCES pedagogie.niveaux_classes(id) ON DELETE NO ACTION,
    capacite                INT                         NOT NULL,
    annee_scolaire_id       INT                         NOT NULL REFERENCES inscriptions.annees_scolaires(id) ON DELETE NO ACTION,
    enseignant_principal_id INT                         NULL,       -- FK → personnel.enseignants (V015)
    effectif                INT                         NOT NULL DEFAULT 0,
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
-- pedagogie.matieres
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS pedagogie.matieres (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id             INT                         NOT NULL REFERENCES pedagogie.type_matieres(id) ON DELETE NO ACTION,
    name                VARCHAR(200)                NOT NULL,   -- Ex: "Mathematiques"
    code                VARCHAR(20)                 NOT NULL,   -- Ex: MATH, FR, HG
    coefficient         NUMERIC(5,2)                NOT NULL,
    nombre_heures       INT                         NOT NULL,   -- Volume horaire hebdomadaire
    niveau_classe_id    INT                         NOT NULL REFERENCES pedagogie.niveaux_classes(id) ON DELETE NO ACTION,
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
-- pedagogie.salles
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS pedagogie.salles (
    id              INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id         INT                         NOT NULL REFERENCES pedagogie.type_salles(id) ON DELETE NO ACTION,
    name            VARCHAR(200)                NOT NULL,   -- Ex: "Salle 101"
    capacite        INT                         NOT NULL,
    batiment        VARCHAR(100)                NULL,
    equipements     TEXT                        NULL,
    est_disponible  BOOLEAN                     NOT NULL DEFAULT TRUE,
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
-- pedagogie.seances
-- FK vers personnel.enseignants ajoutee dans V015
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS pedagogie.seances (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    matiere_id          INT                         NOT NULL REFERENCES pedagogie.matieres(id) ON DELETE NO ACTION,
    classe_id           INT                         NOT NULL REFERENCES pedagogie.classes(id) ON DELETE NO ACTION,
    enseignant_id       INT                         NOT NULL,   -- FK → personnel.enseignants (V015)
    salle_id            INT                         NOT NULL REFERENCES pedagogie.salles(id) ON DELETE NO ACTION,
    jour_semaine        INT                         NOT NULL,   -- 1=Lundi ... 6=Samedi
    heure_debut         TIME                        NOT NULL,
    heure_fin           TIME                        NOT NULL,
    annee_scolaire_id   INT                         NOT NULL REFERENCES inscriptions.annees_scolaires(id) ON DELETE NO ACTION,
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
-- pedagogie.cahiers_textes
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS pedagogie.cahiers_textes (
    id              INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    seance_id       INT                         NOT NULL REFERENCES pedagogie.seances(id) ON DELETE NO ACTION,
    contenu         TEXT                        NOT NULL,
    date_seance     DATE                        NOT NULL,
    travail_a_faire TEXT                        NULL,
    -- Audit
    created_at      TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE    NULL,
    created_by      VARCHAR(100)                NOT NULL DEFAULT 'system',
    updated_by      VARCHAR(100)                NULL,
    is_deleted      BOOLEAN                     NOT NULL DEFAULT FALSE,
    deleted_at      TIMESTAMP WITH TIME ZONE    NULL,
    deleted_by      VARCHAR(100)                NULL
);
