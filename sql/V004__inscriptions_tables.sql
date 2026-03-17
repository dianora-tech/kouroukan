-- ============================================================================
-- V004__inscriptions_tables.sql
-- Kouroukan - Module Inscriptions (port 5001, schema inscriptions)
-- ============================================================================

-- ----------------------------------------------------------------------------
-- Tables de types
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS inscriptions.type_dossiers_admission (
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

CREATE TABLE IF NOT EXISTS inscriptions.type_inscriptions (
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
-- inscriptions.annees_scolaires
-- Aucune FK inter-module — doit etre creee en premier
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS inscriptions.annees_scolaires (
    id          INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    libelle     VARCHAR(20)                 NOT NULL,   -- Ex: 2025-2026
    date_debut  DATE                        NOT NULL,
    date_fin    DATE                        NOT NULL,
    est_active  BOOLEAN                     NOT NULL DEFAULT FALSE,
    -- Audit
    created_at  TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    updated_at  TIMESTAMP WITH TIME ZONE    NULL,
    created_by  VARCHAR(100)                NOT NULL DEFAULT 'system',
    updated_by  VARCHAR(100)                NULL,
    is_deleted  BOOLEAN                     NOT NULL DEFAULT FALSE,
    deleted_at  TIMESTAMP WITH TIME ZONE    NULL,
    deleted_by  VARCHAR(100)                NULL
);

-- ----------------------------------------------------------------------------
-- inscriptions.eleves
-- FK vers pedagogie.niveaux_classes et pedagogie.classes ajoutees dans V015
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS inscriptions.eleves (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    first_name          VARCHAR(100)                NOT NULL,
    last_name           VARCHAR(100)                NOT NULL,
    date_naissance      DATE                        NOT NULL,
    lieu_naissance      VARCHAR(200)                NOT NULL,
    genre               VARCHAR(10)                 NOT NULL,       -- M ou F
    nationalite         VARCHAR(50)                 NOT NULL,
    adresse             VARCHAR(500)                NULL,
    photo_url           VARCHAR(500)                NULL,
    numero_matricule    VARCHAR(50)                 NOT NULL,
    niveau_classe_id    INT                         NOT NULL,       -- FK → pedagogie.niveaux_classes (V015)
    classe_id           INT                         NULL,           -- FK → pedagogie.classes (V015)
    parent_id           INT                         NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    statut_inscription  VARCHAR(30)                 NOT NULL,       -- Prospect/PreInscrit/Inscrit/Radie
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
-- inscriptions.dossiers_admission
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS inscriptions.dossiers_admission (
    id                          INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id                     INT                         NOT NULL REFERENCES inscriptions.type_dossiers_admission(id) ON DELETE NO ACTION,
    eleve_id                    INT                         NOT NULL REFERENCES inscriptions.eleves(id) ON DELETE NO ACTION,
    annee_scolaire_id           INT                         NOT NULL REFERENCES inscriptions.annees_scolaires(id) ON DELETE NO ACTION,
    statut_dossier              VARCHAR(30)                 NOT NULL,   -- Prospect/PreInscrit/EnEtude/Convoque/Admis/Refuse/ListeAttente
    etape_actuelle              VARCHAR(50)                 NOT NULL,
    date_demande                DATE                        NOT NULL,
    date_decision               DATE                        NULL,
    motif_refus                 VARCHAR(500)                NULL,
    scoring_interne             NUMERIC(5,2)                NULL,
    commentaires                TEXT                        NULL,
    responsable_admission_id    INT                         NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
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

-- ----------------------------------------------------------------------------
-- inscriptions.inscriptions
-- FK vers pedagogie.classes ajoutee dans V015
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS inscriptions.inscriptions (
    id                      INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id                 INT                         NOT NULL REFERENCES inscriptions.type_inscriptions(id) ON DELETE NO ACTION,
    eleve_id                INT                         NOT NULL REFERENCES inscriptions.eleves(id) ON DELETE NO ACTION,
    classe_id               INT                         NOT NULL,   -- FK → pedagogie.classes (V015)
    annee_scolaire_id       INT                         NOT NULL REFERENCES inscriptions.annees_scolaires(id) ON DELETE NO ACTION,
    date_inscription        DATE                        NOT NULL,
    montant_inscription     NUMERIC(15,2)               NOT NULL,
    est_paye                BOOLEAN                     NOT NULL DEFAULT FALSE,
    est_redoublant          BOOLEAN                     NOT NULL DEFAULT FALSE,
    type_etablissement      VARCHAR(30)                 NULL,       -- Public/PriveLaique/PriveFrancoArabe/Communautaire/...
    serie_bac               VARCHAR(10)                 NULL,       -- SE/SM/SS/FA — uniquement lycee
    statut_inscription      VARCHAR(30)                 NOT NULL,   -- EnAttente/Validee/Annulee
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
