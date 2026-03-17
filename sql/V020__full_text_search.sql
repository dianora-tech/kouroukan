-- ============================================================================
-- V020__full_text_search.sql
-- Kouroukan - Recherche full-text PostgreSQL (tsvector/tsquery)
-- ============================================================================

-- ============================================================================
-- 1. SUPPORT : Articles d'aide — recherche full-text francais
-- ============================================================================

-- Ajouter la colonne tsvector pour la recherche
ALTER TABLE support.articles_aide
    ADD COLUMN IF NOT EXISTS search_vector tsvector;

-- Fonction trigger pour mettre a jour le vecteur de recherche
CREATE OR REPLACE FUNCTION support.update_articles_aide_search_vector()
RETURNS trigger AS $$
BEGIN
    NEW.search_vector :=
        setweight(to_tsvector('french', COALESCE(NEW.titre, '')), 'A') ||
        setweight(to_tsvector('french', COALESCE(NEW.contenu, '')), 'B') ||
        setweight(to_tsvector('french', COALESCE(NEW.categorie, '')), 'C');
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger avant INSERT ou UPDATE
DROP TRIGGER IF EXISTS trg_update_articles_aide_search ON support.articles_aide;
CREATE TRIGGER trg_update_articles_aide_search
    BEFORE INSERT OR UPDATE ON support.articles_aide
    FOR EACH ROW EXECUTE FUNCTION support.update_articles_aide_search_vector();

-- Index GIN pour la recherche full-text
CREATE INDEX IF NOT EXISTS idx_articles_aide_search
    ON support.articles_aide USING GIN(search_vector);

-- ============================================================================
-- 2. SUPPORT : Tickets — recherche full-text
-- ============================================================================

ALTER TABLE support.tickets
    ADD COLUMN IF NOT EXISTS search_vector tsvector;

CREATE OR REPLACE FUNCTION support.update_tickets_search_vector()
RETURNS trigger AS $$
BEGIN
    NEW.search_vector :=
        setweight(to_tsvector('french', COALESCE(NEW.sujet, '')), 'A') ||
        setweight(to_tsvector('french', COALESCE(NEW.contenu, '')), 'B');
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_update_tickets_search ON support.tickets;
CREATE TRIGGER trg_update_tickets_search
    BEFORE INSERT OR UPDATE ON support.tickets
    FOR EACH ROW EXECUTE FUNCTION support.update_tickets_search_vector();

CREATE INDEX IF NOT EXISTS idx_tickets_search
    ON support.tickets USING GIN(search_vector);

-- ============================================================================
-- 3. SUPPORT : Suggestions — recherche full-text
-- ============================================================================

ALTER TABLE support.suggestions
    ADD COLUMN IF NOT EXISTS search_vector tsvector;

CREATE OR REPLACE FUNCTION support.update_suggestions_search_vector()
RETURNS trigger AS $$
BEGIN
    NEW.search_vector :=
        setweight(to_tsvector('french', COALESCE(NEW.titre, '')), 'A') ||
        setweight(to_tsvector('french', COALESCE(NEW.contenu, '')), 'B');
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_update_suggestions_search ON support.suggestions;
CREATE TRIGGER trg_update_suggestions_search
    BEFORE INSERT OR UPDATE ON support.suggestions
    FOR EACH ROW EXECUTE FUNCTION support.update_suggestions_search_vector();

CREATE INDEX IF NOT EXISTS idx_suggestions_search
    ON support.suggestions USING GIN(search_vector);

-- ============================================================================
-- 4. INSCRIPTIONS : Eleves — recherche par nom/prenom/matricule
-- ============================================================================

ALTER TABLE inscriptions.eleves
    ADD COLUMN IF NOT EXISTS search_vector tsvector;

CREATE OR REPLACE FUNCTION inscriptions.update_eleves_search_vector()
RETURNS trigger AS $$
BEGIN
    NEW.search_vector :=
        setweight(to_tsvector('french', COALESCE(NEW.first_name, '')), 'A') ||
        setweight(to_tsvector('french', COALESCE(NEW.last_name, '')), 'A') ||
        setweight(to_tsvector('simple', COALESCE(NEW.numero_matricule, '')), 'B') ||
        setweight(to_tsvector('french', COALESCE(NEW.lieu_naissance, '')), 'C');
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_update_eleves_search ON inscriptions.eleves;
CREATE TRIGGER trg_update_eleves_search
    BEFORE INSERT OR UPDATE ON inscriptions.eleves
    FOR EACH ROW EXECUTE FUNCTION inscriptions.update_eleves_search_vector();

CREATE INDEX IF NOT EXISTS idx_eleves_search
    ON inscriptions.eleves USING GIN(search_vector);

-- ============================================================================
-- 5. COMMUNICATION : Messages — recherche par sujet/contenu
-- ============================================================================

ALTER TABLE communication.messages
    ADD COLUMN IF NOT EXISTS search_vector tsvector;

CREATE OR REPLACE FUNCTION communication.update_messages_search_vector()
RETURNS trigger AS $$
BEGIN
    NEW.search_vector :=
        setweight(to_tsvector('french', COALESCE(NEW.sujet, '')), 'A') ||
        setweight(to_tsvector('french', COALESCE(NEW.contenu, '')), 'B');
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_update_messages_search ON communication.messages;
CREATE TRIGGER trg_update_messages_search
    BEFORE INSERT OR UPDATE ON communication.messages
    FOR EACH ROW EXECUTE FUNCTION communication.update_messages_search_vector();

CREATE INDEX IF NOT EXISTS idx_messages_search
    ON communication.messages USING GIN(search_vector);

-- ============================================================================
-- 6. Fonction utilitaire de recherche full-text
-- ============================================================================

-- Recherche generique dans les articles d'aide
CREATE OR REPLACE FUNCTION support.rechercher_articles(
    p_query     TEXT,
    p_limit     INT DEFAULT 20
)
RETURNS TABLE (
    id          INT,
    titre       VARCHAR(200),
    slug        VARCHAR(200),
    categorie   VARCHAR(100),
    rang        REAL
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        a.id,
        a.titre,
        a.slug,
        a.categorie,
        ts_rank(a.search_vector, plainto_tsquery('french', p_query)) AS rang
    FROM support.articles_aide a
    WHERE a.search_vector @@ plainto_tsquery('french', p_query)
      AND a.est_publie = TRUE
      AND a.is_deleted = FALSE
    ORDER BY rang DESC
    LIMIT p_limit;
END;
$$ LANGUAGE plpgsql;

-- Recherche d'eleves par nom/prenom/matricule
CREATE OR REPLACE FUNCTION inscriptions.rechercher_eleves(
    p_query     TEXT,
    p_limit     INT DEFAULT 50
)
RETURNS TABLE (
    id                  INT,
    first_name          VARCHAR(100),
    last_name           VARCHAR(100),
    numero_matricule    VARCHAR(50),
    statut_inscription  VARCHAR(30),
    rang                REAL
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        e.id,
        e.first_name,
        e.last_name,
        e.numero_matricule,
        e.statut_inscription,
        ts_rank(e.search_vector, plainto_tsquery('french', p_query)) AS rang
    FROM inscriptions.eleves e
    WHERE e.search_vector @@ plainto_tsquery('french', p_query)
      AND e.is_deleted = FALSE
    ORDER BY rang DESC
    LIMIT p_limit;
END;
$$ LANGUAGE plpgsql;
