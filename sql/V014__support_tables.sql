-- ============================================================================
-- V014__support_tables.sql
-- Kouroukan - Module Support (port 5011, schema support)
-- ============================================================================

-- ----------------------------------------------------------------------------
-- Tables de types
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS support.type_tickets (
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

CREATE TABLE IF NOT EXISTS support.type_suggestions (
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

CREATE TABLE IF NOT EXISTS support.type_articles_aide (
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
-- support.tickets
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS support.tickets (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id             INT                         NOT NULL REFERENCES support.type_tickets(id) ON DELETE NO ACTION,
    auteur_id           INT                         NOT NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    sujet               VARCHAR(200)                NOT NULL,
    contenu             TEXT                        NOT NULL,
    priorite            VARCHAR(20)                 NOT NULL,   -- Basse/Moyenne/Haute/Critique
    statut_ticket       VARCHAR(30)                 NOT NULL,   -- Ouvert/EnCours/EnAttente/Resolu/Ferme
    categorie_ticket    VARCHAR(50)                 NOT NULL,   -- Bug/Question/Amelioration/Autre
    module_concerne     VARCHAR(50)                 NULL,
    assigne_a_id        INT                         NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    date_resolution     TIMESTAMP WITH TIME ZONE    NULL,
    note_satisfaction   INT                         NULL,       -- 1-5
    piece_jointe_url    VARCHAR(500)                NULL,
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
-- support.reponses_tickets
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS support.reponses_tickets (
    id              INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    ticket_id       INT                         NOT NULL REFERENCES support.tickets(id) ON DELETE NO ACTION,
    auteur_id       INT                         NOT NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    contenu         TEXT                        NOT NULL,
    est_reponse_ia  BOOLEAN                     NOT NULL DEFAULT FALSE,
    est_interne     BOOLEAN                     NOT NULL DEFAULT FALSE,
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
-- support.suggestions
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS support.suggestions (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id             INT                         NOT NULL REFERENCES support.type_suggestions(id) ON DELETE NO ACTION,
    auteur_id           INT                         NOT NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    titre               VARCHAR(200)                NOT NULL,
    contenu             TEXT                        NOT NULL,
    module_concerne     VARCHAR(50)                 NULL,
    statut_suggestion   VARCHAR(30)                 NOT NULL,   -- Soumise/EnRevue/Acceptee/Planifiee/Realisee/Rejetee
    nombre_votes        INT                         NOT NULL DEFAULT 0,
    commentaire_admin   TEXT                        NULL,
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
-- support.votes_suggestions
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS support.votes_suggestions (
    id              INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    suggestion_id   INT                         NOT NULL REFERENCES support.suggestions(id) ON DELETE NO ACTION,
    votant_id       INT                         NOT NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    -- Audit
    created_at      TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP WITH TIME ZONE    NULL,
    created_by      VARCHAR(100)                NOT NULL DEFAULT 'system',
    updated_by      VARCHAR(100)                NULL,
    is_deleted      BOOLEAN                     NOT NULL DEFAULT FALSE,
    deleted_at      TIMESTAMP WITH TIME ZONE    NULL,
    deleted_by      VARCHAR(100)                NULL,
    -- Un vote par suggestion par utilisateur
    CONSTRAINT uq_vote_suggestion UNIQUE (suggestion_id, votant_id)
);

-- ----------------------------------------------------------------------------
-- support.articles_aide
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS support.articles_aide (
    id              INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id         INT                         NOT NULL REFERENCES support.type_articles_aide(id) ON DELETE NO ACTION,
    titre           VARCHAR(200)                NOT NULL,
    contenu         TEXT                        NOT NULL,   -- Markdown
    slug            VARCHAR(200)                NOT NULL,
    categorie       VARCHAR(100)                NOT NULL,   -- Demarrage/Utilisation/FAQ/Mobile
    module_concerne VARCHAR(50)                 NULL,
    ordre           INT                         NOT NULL DEFAULT 0,
    est_publie      BOOLEAN                     NOT NULL DEFAULT FALSE,
    nombre_vues     INT                         NOT NULL DEFAULT 0,
    nombre_utile    INT                         NOT NULL DEFAULT 0,
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
-- support.conversations_ia
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS support.conversations_ia (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    utilisateur_id      INT                         NOT NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    titre               VARCHAR(200)                NULL,
    est_active          BOOLEAN                     NOT NULL DEFAULT TRUE,
    nombre_messages     INT                         NOT NULL DEFAULT 0,
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
-- support.messages_ia
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS support.messages_ia (
    id                      INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    conversation_ia_id      INT                         NOT NULL REFERENCES support.conversations_ia(id) ON DELETE NO ACTION,
    role                    VARCHAR(20)                 NOT NULL,   -- user/assistant/system
    contenu                 TEXT                        NOT NULL,
    contexte_articles_ids   TEXT                        NULL,       -- JSON array d'IDs d'articles
    tokens_utilises         INT                         NULL,
    -- Audit
    created_at              TIMESTAMP WITH TIME ZONE    NOT NULL DEFAULT NOW(),
    updated_at              TIMESTAMP WITH TIME ZONE    NULL,
    created_by              VARCHAR(100)                NOT NULL DEFAULT 'system',
    updated_by              VARCHAR(100)                NULL,
    is_deleted              BOOLEAN                     NOT NULL DEFAULT FALSE,
    deleted_at              TIMESTAMP WITH TIME ZONE    NULL,
    deleted_by              VARCHAR(100)                NULL
);
