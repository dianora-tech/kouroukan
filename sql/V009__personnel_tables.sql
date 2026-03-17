-- ============================================================================
-- V009__personnel_tables.sql
-- Kouroukan - Module Personnel (port 5006, schema personnel)
-- ============================================================================

-- ----------------------------------------------------------------------------
-- Tables de types
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS personnel.type_enseignants (
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

CREATE TABLE IF NOT EXISTS personnel.type_demandes_conges (
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
-- personnel.enseignants
-- Table centrale referencee par pedagogie, evaluations, presences, finances
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS personnel.enseignants (
    id                      INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id                 INT                         NOT NULL REFERENCES personnel.type_enseignants(id) ON DELETE NO ACTION,
    user_id                 INT                         NOT NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    matricule               VARCHAR(50)                 NOT NULL,
    specialite              VARCHAR(200)                NOT NULL,
    date_embauche           DATE                        NOT NULL,
    mode_remuneration       VARCHAR(30)                 NOT NULL,   -- Forfait/Heures/Mixte
    montant_forfait         NUMERIC(15,2)               NULL,
    telephone               VARCHAR(20)                 NOT NULL,   -- +224 xxx
    email                   VARCHAR(200)                NULL,
    statut_enseignant       VARCHAR(30)                 NOT NULL,   -- Actif/EnConge/Suspendu/Inactif
    solde_conges_annuel     INT                         NOT NULL DEFAULT 0,
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
-- personnel.demandes_conges
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS personnel.demandes_conges (
    id                          INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id                     INT                         NOT NULL REFERENCES personnel.type_demandes_conges(id) ON DELETE NO ACTION,
    enseignant_id               INT                         NOT NULL REFERENCES personnel.enseignants(id) ON DELETE NO ACTION,
    date_debut                  DATE                        NOT NULL,
    date_fin                    DATE                        NOT NULL,
    motif                       VARCHAR(500)                NOT NULL,
    statut_demande              VARCHAR(30)                 NOT NULL,   -- Soumise/ApprouveeN1/ApprouveeDirection/Refusee
    piece_jointe_url            VARCHAR(500)                NULL,
    commentaire_validateur      VARCHAR(500)                NULL,
    validateur_id               INT                         NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    date_validation             DATE                        NULL,
    impact_paie                 BOOLEAN                     NOT NULL DEFAULT FALSE,
    user_id                     INT                         NOT NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    -- Audit
    created_at                  TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    updated_at                  TIMESTAMP WITH TIME ZONE    NULL,
    created_by                  VARCHAR(100)                NOT NULL DEFAULT 'system',
    updated_by                  VARCHAR(100)                NULL,
    is_deleted                  BOOLEAN                     NOT NULL DEFAULT FALSE,
    deleted_at                  TIMESTAMP WITH TIME ZONE    NULL,
    deleted_by                  VARCHAR(100)                NULL
);
