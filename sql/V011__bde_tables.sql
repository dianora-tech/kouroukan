-- ============================================================================
-- V011__bde_tables.sql
-- Kouroukan - Module BDE (port 5008, schema bde)
-- ============================================================================

-- ----------------------------------------------------------------------------
-- Tables de types
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS bde.type_associations (
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

CREATE TABLE IF NOT EXISTS bde.type_evenements (
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

CREATE TABLE IF NOT EXISTS bde.type_depenses_bde (
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
-- bde.associations
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS bde.associations (
    id              INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id         INT                         NOT NULL REFERENCES bde.type_associations(id) ON DELETE NO ACTION,
    name            VARCHAR(200)                NOT NULL,
    description     VARCHAR(500)                NULL,
    sigle           VARCHAR(50)                 NULL,
    annee_scolaire  VARCHAR(20)                 NOT NULL,
    statut          VARCHAR(30)                 NOT NULL,   -- Active/Suspendue/Dissoute
    budget_annuel   NUMERIC(15,2)               NOT NULL DEFAULT 0,
    superviseur_id  INT                         NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
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
-- bde.evenements
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS bde.evenements (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id             INT                         NOT NULL REFERENCES bde.type_evenements(id) ON DELETE NO ACTION,
    name                VARCHAR(200)                NOT NULL,
    description         VARCHAR(500)                NULL,
    association_id      INT                         NOT NULL REFERENCES bde.associations(id) ON DELETE NO ACTION,
    date_evenement      TIMESTAMP WITH TIME ZONE    NOT NULL,
    lieu                VARCHAR(200)                NOT NULL,
    capacite            INT                         NULL,
    tarif_entree        NUMERIC(10,2)               NULL,
    nombre_inscrits     INT                         NOT NULL DEFAULT 0,
    statut_evenement    VARCHAR(30)                 NOT NULL,   -- Planifie/Valide/EnCours/Termine/Annule
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
-- bde.membres_bde
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS bde.membres_bde (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    association_id      INT                         NOT NULL REFERENCES bde.associations(id) ON DELETE NO ACTION,
    eleve_id            INT                         NOT NULL REFERENCES inscriptions.eleves(id) ON DELETE NO ACTION,
    role_bde            VARCHAR(50)                 NOT NULL,   -- President/Tresorier/Secretaire/RespPole/Membre
    date_adhesion       DATE                        NOT NULL,
    montant_cotisation  NUMERIC(10,2)               NULL,
    est_actif           BOOLEAN                     NOT NULL DEFAULT TRUE,
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
-- bde.depenses_bde
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS bde.depenses_bde (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id             INT                         NOT NULL REFERENCES bde.type_depenses_bde(id) ON DELETE NO ACTION,
    association_id      INT                         NOT NULL REFERENCES bde.associations(id) ON DELETE NO ACTION,
    montant             NUMERIC(15,2)               NOT NULL,
    motif               VARCHAR(500)                NOT NULL,
    categorie           VARCHAR(100)                NOT NULL,   -- Materiel/Location/Prestataire/Remboursement
    statut_validation   VARCHAR(30)                 NOT NULL,   -- Demandee/ValideTresorier/ValideSuper/Refusee
    validateur_id       INT                         NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
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
