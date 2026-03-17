-- ============================================================================
-- V012__documents_tables.sql
-- Kouroukan - Module Documents (port 5009, schema documents)
-- ============================================================================

-- ----------------------------------------------------------------------------
-- Tables de types
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS documents.type_modeles_documents (
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

CREATE TABLE IF NOT EXISTS documents.type_documents_generes (
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

CREATE TABLE IF NOT EXISTS documents.type_signatures (
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
-- documents.modeles_documents
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS documents.modeles_documents (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id             INT                         NOT NULL REFERENCES documents.type_modeles_documents(id) ON DELETE NO ACTION,
    name                VARCHAR(200)                NOT NULL,
    code                VARCHAR(50)                 NOT NULL,
    contenu             TEXT                        NOT NULL,   -- Template HTML/Mustache
    logo_url            VARCHAR(500)                NULL,
    couleur_primaire    VARCHAR(7)                  NULL,       -- Hex
    texte_pied_page     VARCHAR(500)                NULL,
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
-- documents.documents_generes
-- FK vers personnel.enseignants ajoutee dans V015
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS documents.documents_generes (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id             INT                         NOT NULL REFERENCES documents.type_documents_generes(id) ON DELETE NO ACTION,
    modele_document_id  INT                         NOT NULL REFERENCES documents.modeles_documents(id) ON DELETE NO ACTION,
    eleve_id            INT                         NULL REFERENCES inscriptions.eleves(id) ON DELETE NO ACTION,
    enseignant_id       INT                         NULL,       -- FK → personnel.enseignants (V015)
    donnees_json        TEXT                        NOT NULL,   -- Donnees de merge JSON
    date_generation     TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    statut_signature    VARCHAR(30)                 NOT NULL,   -- EnAttente/EnCours/Signe/Refuse
    chemin_fichier      VARCHAR(500)                NULL,       -- Chemin du PDF genere
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
-- documents.signatures
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS documents.signatures (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id             INT                         NOT NULL REFERENCES documents.type_signatures(id) ON DELETE NO ACTION,
    document_genere_id  INT                         NOT NULL REFERENCES documents.documents_generes(id) ON DELETE NO ACTION,
    signataire_id       INT                         NOT NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    ordre_signature     INT                         NOT NULL,
    date_signature      TIMESTAMP WITH TIME ZONE    NULL,
    statut_signature    VARCHAR(30)                 NOT NULL,   -- EnAttente/Signe/Refuse/Delegue
    niveau_signature    VARCHAR(30)                 NOT NULL,   -- Basique/Avancee
    motif_refus         VARCHAR(500)                NULL,
    est_validee         BOOLEAN                     NOT NULL DEFAULT FALSE,
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
