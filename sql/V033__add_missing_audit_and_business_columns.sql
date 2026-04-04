-- =============================================================================
-- V033 : Ajouter les colonnes manquantes pour aligner les DTOs avec le schema
-- =============================================================================
-- Certaines tables creees dans V029 avaient un schema minimal. Les DTOs
-- implementent IAuditableEntity + ISoftDeletable et necessitent toutes les
-- colonnes d'audit (updated_by, deleted_at, deleted_by, etc.).
-- Sans ces colonnes, les SELECT/INSERT/UPDATE/DELETE via GnDapper echouent.
-- =============================================================================

-- ── 1. pedagogie.affectations_enseignant ─────────────────────────────────────
-- Manque : updated_by, deleted_at, deleted_by
ALTER TABLE pedagogie.affectations_enseignant
  ADD COLUMN IF NOT EXISTS updated_by  VARCHAR(100) NULL,
  ADD COLUMN IF NOT EXISTS deleted_at  TIMESTAMPTZ  NULL,
  ADD COLUMN IF NOT EXISTS deleted_by  VARCHAR(100) NULL;

-- ── 2. pedagogie.competences_enseignant ──────────────────────────────────────
-- Manque : updated_at, updated_by, created_by, deleted_at, deleted_by
ALTER TABLE pedagogie.competences_enseignant
  ADD COLUMN IF NOT EXISTS created_by  VARCHAR(100) NOT NULL DEFAULT 'system',
  ADD COLUMN IF NOT EXISTS updated_at  TIMESTAMPTZ  NULL,
  ADD COLUMN IF NOT EXISTS updated_by  VARCHAR(100) NULL,
  ADD COLUMN IF NOT EXISTS deleted_at  TIMESTAMPTZ  NULL,
  ADD COLUMN IF NOT EXISTS deleted_by  VARCHAR(100) NULL;

-- ── 3. inscriptions.transferts ───────────────────────────────────────────────
-- Manque : updated_at, updated_by, deleted_at, deleted_by
ALTER TABLE inscriptions.transferts
  ADD COLUMN IF NOT EXISTS updated_at  TIMESTAMPTZ  NULL,
  ADD COLUMN IF NOT EXISTS updated_by  VARCHAR(100) NULL,
  ADD COLUMN IF NOT EXISTS deleted_at  TIMESTAMPTZ  NULL,
  ADD COLUMN IF NOT EXISTS deleted_by  VARCHAR(100) NULL;

-- ── 4. inscriptions.radiations ───────────────────────────────────────────────
-- Manque : updated_at, updated_by, deleted_at, deleted_by
ALTER TABLE inscriptions.radiations
  ADD COLUMN IF NOT EXISTS updated_at  TIMESTAMPTZ  NULL,
  ADD COLUMN IF NOT EXISTS updated_by  VARCHAR(100) NULL,
  ADD COLUMN IF NOT EXISTS deleted_at  TIMESTAMPTZ  NULL,
  ADD COLUMN IF NOT EXISTS deleted_by  VARCHAR(100) NULL;

-- ── 5. inscriptions.liaisons_parent ──────────────────────────────────────────
-- Manque : updated_at, updated_by, deleted_at, deleted_by
ALTER TABLE inscriptions.liaisons_parent
  ADD COLUMN IF NOT EXISTS updated_at  TIMESTAMPTZ  NULL,
  ADD COLUMN IF NOT EXISTS updated_by  VARCHAR(100) NULL,
  ADD COLUMN IF NOT EXISTS deleted_at  TIMESTAMPTZ  NULL,
  ADD COLUMN IF NOT EXISTS deleted_by  VARCHAR(100) NULL;

-- ── 6. finances.journal_financier ────────────────────────────────────────────
-- Manque : updated_at, updated_by, deleted_at, deleted_by
ALTER TABLE finances.journal_financier
  ADD COLUMN IF NOT EXISTS updated_at  TIMESTAMPTZ  NULL,
  ADD COLUMN IF NOT EXISTS updated_by  VARCHAR(100) NULL,
  ADD COLUMN IF NOT EXISTS deleted_at  TIMESTAMPTZ  NULL,
  ADD COLUMN IF NOT EXISTS deleted_by  VARCHAR(100) NULL;

-- ── 7. finances.moyens_paiement ──────────────────────────────────────────────
-- Manque : deleted_at, deleted_by
ALTER TABLE finances.moyens_paiement
  ADD COLUMN IF NOT EXISTS deleted_at  TIMESTAMPTZ  NULL,
  ADD COLUMN IF NOT EXISTS deleted_by  VARCHAR(100) NULL;

-- ── 8. finances.paliers_familiaux ────────────────────────────────────────────
-- Manque : updated_at, updated_by, is_deleted, deleted_at, deleted_by
ALTER TABLE finances.paliers_familiaux
  ADD COLUMN IF NOT EXISTS updated_at  TIMESTAMPTZ  NULL,
  ADD COLUMN IF NOT EXISTS updated_by  VARCHAR(100) NULL,
  ADD COLUMN IF NOT EXISTS is_deleted  BOOLEAN      NOT NULL DEFAULT FALSE,
  ADD COLUMN IF NOT EXISTS deleted_at  TIMESTAMPTZ  NULL,
  ADD COLUMN IF NOT EXISTS deleted_by  VARCHAR(100) NULL;

-- ── 9. inscriptions.annees_scolaires ─────────────────────────────────────────
-- Manque : code, description, statut, date_rentree, nombre_periodes, type_periode
ALTER TABLE inscriptions.annees_scolaires
  ADD COLUMN IF NOT EXISTS code             VARCHAR(20)  NULL,
  ADD COLUMN IF NOT EXISTS description      TEXT         NULL,
  ADD COLUMN IF NOT EXISTS statut           VARCHAR(20)  NOT NULL DEFAULT 'preparation',
  ADD COLUMN IF NOT EXISTS date_rentree     DATE         NULL,
  ADD COLUMN IF NOT EXISTS nombre_periodes  INTEGER      NOT NULL DEFAULT 3,
  ADD COLUMN IF NOT EXISTS type_periode     VARCHAR(20)  NOT NULL DEFAULT 'trimestre';
