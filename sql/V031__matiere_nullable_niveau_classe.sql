-- Remove coefficient, nombre_heures, and niveau_classe_id from matières
-- These fields don't belong at the matière reference level

-- Drop indexes referencing niveau_classe_id
DROP INDEX IF EXISTS pedagogie.ix_matieres_code_niveau;
DROP INDEX IF EXISTS pedagogie.ix_matieres_niveau_classe_id;

-- Drop the columns
ALTER TABLE pedagogie.matieres DROP COLUMN IF EXISTS niveau_classe_id;
ALTER TABLE pedagogie.matieres DROP COLUMN IF EXISTS coefficient;
ALTER TABLE pedagogie.matieres DROP COLUMN IF EXISTS nombre_heures;

-- Add description column if missing
ALTER TABLE pedagogie.matieres ADD COLUMN IF NOT EXISTS description VARCHAR(500) NULL;

-- Recreate unique index on code only
CREATE UNIQUE INDEX IF NOT EXISTS ix_matieres_code ON pedagogie.matieres(code) WHERE is_deleted = FALSE;
