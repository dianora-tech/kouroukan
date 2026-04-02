-- =============================================================================
-- V029: Platform Restructure — 4 Espaces, Forfaits, Liaisons, Paiements
-- =============================================================================

-- ── 1. Forfaits ──────────────────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS auth.forfaits (
  id SERIAL PRIMARY KEY,
  code VARCHAR(50) NOT NULL UNIQUE,
  nom VARCHAR(200) NOT NULL,
  description TEXT,
  prix_mensuel INTEGER NOT NULL DEFAULT 0,
  prix_vacances INTEGER NOT NULL DEFAULT 0,
  periode_essai_jours INTEGER NOT NULL DEFAULT 30,
  type_cible VARCHAR(20) NOT NULL, -- 'etablissement', 'enseignant', 'parent'
  est_gratuit BOOLEAN NOT NULL DEFAULT FALSE,
  limite_eleves INTEGER, -- NULL = illimité
  est_actif BOOLEAN NOT NULL DEFAULT TRUE,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ,
  created_by VARCHAR(100) NOT NULL DEFAULT 'system',
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

-- Seed forfaits par défaut
INSERT INTO auth.forfaits (code, nom, description, prix_mensuel, prix_vacances, type_cible, est_gratuit, limite_eleves) VALUES
  ('etab-public-gratuit', 'Établissement Public Gratuit', 'Gratuit pour les établissements publics jusqu''à 30 élèves', 0, 0, 'etablissement', TRUE, 30),
  ('etab-standard', 'Établissement Standard', 'Forfait standard pour les établissements', 100000, 50000, 'etablissement', FALSE, NULL),
  ('etab-premium', 'Établissement Premium', 'Forfait premium avec toutes les fonctionnalités', 180000, 90000, 'etablissement', FALSE, NULL),
  ('enseignant', 'Enseignant', 'Forfait enseignant pour accéder à l''espace personnel', 30000, 15000, 'enseignant', FALSE, NULL),
  ('parent', 'Parent', 'Forfait parent par élève', 5000, 0, 'parent', FALSE, NULL)
ON CONFLICT (code) DO NOTHING;

-- Historique des tarifs (versionné par date d'effet)
CREATE TABLE IF NOT EXISTS auth.forfait_tarifs (
  id SERIAL PRIMARY KEY,
  forfait_id INTEGER NOT NULL REFERENCES auth.forfaits(id),
  prix_mensuel INTEGER NOT NULL,
  prix_vacances INTEGER NOT NULL,
  date_effet DATE NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  created_by VARCHAR(100) NOT NULL DEFAULT 'system'
);

-- ── 2. Abonnements ───────────────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS auth.abonnements (
  id SERIAL PRIMARY KEY,
  forfait_id INTEGER NOT NULL REFERENCES auth.forfaits(id),
  company_id INTEGER REFERENCES auth.companies(id),
  user_id INTEGER REFERENCES auth.users(id),
  date_debut DATE NOT NULL,
  date_fin DATE,
  date_essai_fin DATE,
  est_actif BOOLEAN NOT NULL DEFAULT TRUE,
  geste_commercial_id INTEGER,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ,
  created_by VARCHAR(100) NOT NULL DEFAULT 'system',
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

-- ── 3. Gestes commerciaux ────────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS auth.gestes_commerciaux (
  id SERIAL PRIMARY KEY,
  nom VARCHAR(200) NOT NULL,
  description TEXT,
  type_cible VARCHAR(30) NOT NULL, -- 'global', 'utilisateur', 'tranche_age', 'genre', 'categorie', 'famille'
  cible_valeur VARCHAR(200), -- userId, '6-12', 'F', 'public', companyId, etc.
  forfait_id INTEGER REFERENCES auth.forfaits(id),
  reduction_pourcent INTEGER DEFAULT 0,
  reduction_montant INTEGER DEFAULT 0,
  date_debut DATE NOT NULL,
  date_fin DATE,
  est_actif BOOLEAN NOT NULL DEFAULT TRUE,
  company_id INTEGER REFERENCES auth.companies(id), -- NULL = geste admin global, sinon geste établissement
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  created_by VARCHAR(100) NOT NULL DEFAULT 'system',
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

-- ── 4. Compétences enseignant ────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS pedagogie.competences_enseignant (
  id SERIAL PRIMARY KEY,
  user_id INTEGER NOT NULL REFERENCES auth.users(id),
  matiere_id INTEGER NOT NULL REFERENCES pedagogie.matieres(id),
  cycle_etude VARCHAR(20) NOT NULL, -- 'prescolaire', 'primaire', 'college', 'lycee'
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
  UNIQUE(user_id, matiere_id, cycle_etude)
);

-- ── 5. Liaisons enseignant ↔ établissement ───────────────────────────────────

CREATE TABLE IF NOT EXISTS auth.liaisons_enseignant (
  id SERIAL PRIMARY KEY,
  user_id INTEGER NOT NULL REFERENCES auth.users(id),
  company_id INTEGER NOT NULL REFERENCES auth.companies(id),
  statut VARCHAR(20) NOT NULL DEFAULT 'pending', -- 'pending', 'accepted', 'rejected', 'terminated'
  date_debut DATE,
  date_fin DATE,
  motif_fin TEXT,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ,
  created_by VARCHAR(100) NOT NULL DEFAULT 'system',
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

-- ── 6. Affectations enseignant → matière/classe ──────────────────────────────

CREATE TABLE IF NOT EXISTS pedagogie.affectations_enseignant (
  id SERIAL PRIMARY KEY,
  liaison_id INTEGER NOT NULL REFERENCES auth.liaisons_enseignant(id),
  classe_id INTEGER NOT NULL REFERENCES pedagogie.classes(id),
  matiere_id INTEGER NOT NULL REFERENCES pedagogie.matieres(id),
  annee_scolaire_id INTEGER NOT NULL,
  est_active BOOLEAN NOT NULL DEFAULT TRUE,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ,
  created_by VARCHAR(100) NOT NULL DEFAULT 'system',
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

-- ── 7. Transferts d'élèves ───────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS inscriptions.transferts (
  id SERIAL PRIMARY KEY,
  eleve_id INTEGER NOT NULL,
  company_origine_id INTEGER NOT NULL REFERENCES auth.companies(id),
  company_cible_id INTEGER REFERENCES auth.companies(id),
  statut VARCHAR(20) NOT NULL DEFAULT 'pending',
  motif TEXT,
  documents JSONB DEFAULT '[]',
  date_demande DATE NOT NULL,
  date_traitement DATE,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  created_by VARCHAR(100) NOT NULL DEFAULT 'system',
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

-- ── 8. Radiations d'élèves ──────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS inscriptions.radiations (
  id SERIAL PRIMARY KEY,
  eleve_id INTEGER NOT NULL,
  company_id INTEGER NOT NULL REFERENCES auth.companies(id),
  motif TEXT NOT NULL,
  documents JSONB DEFAULT '[]',
  date_radiation DATE NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  created_by VARCHAR(100) NOT NULL DEFAULT 'system',
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

-- ── 9. Liaisons parent ↔ élève ───────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS inscriptions.liaisons_parent (
  id SERIAL PRIMARY KEY,
  parent_user_id INTEGER NOT NULL REFERENCES auth.users(id),
  eleve_id INTEGER NOT NULL,
  company_id INTEGER NOT NULL REFERENCES auth.companies(id),
  statut VARCHAR(20) NOT NULL DEFAULT 'active',
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  created_by VARCHAR(100) NOT NULL DEFAULT 'system',
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

-- ── 10. QR codes utilisateurs ────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS auth.qr_codes (
  id SERIAL PRIMARY KEY,
  user_id INTEGER NOT NULL REFERENCES auth.users(id),
  code_unique VARCHAR(100) NOT NULL UNIQUE,
  est_actif BOOLEAN NOT NULL DEFAULT TRUE,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Ajouter identifiant unique aux users s'il n'existe pas
ALTER TABLE auth.users ADD COLUMN IF NOT EXISTS identifiant_unique VARCHAR(20) UNIQUE;

-- ── 11. Moyens de paiement établissement ─────────────────────────────────────

CREATE SCHEMA IF NOT EXISTS finances;

CREATE TABLE IF NOT EXISTS finances.moyens_paiement (
  id SERIAL PRIMARY KEY,
  company_id INTEGER NOT NULL REFERENCES auth.companies(id),
  operateur VARCHAR(50) NOT NULL, -- 'orange_money', 'mtn_momo', 'soutra_money'
  numero_compte VARCHAR(50) NOT NULL,
  code_marchand VARCHAR(100),
  libelle VARCHAR(200),
  est_actif BOOLEAN NOT NULL DEFAULT TRUE,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ,
  created_by VARCHAR(100) NOT NULL DEFAULT 'system',
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

-- ── 12. Comptes Mobile Money super admin ─────────────────────────────────────

CREATE TABLE IF NOT EXISTS finances.comptes_admin (
  id SERIAL PRIMARY KEY,
  operateur VARCHAR(50) NOT NULL,
  numero_compte VARCHAR(50) NOT NULL,
  code_marchand VARCHAR(100),
  libelle VARCHAR(200),
  est_actif BOOLEAN NOT NULL DEFAULT TRUE,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ,
  created_by VARCHAR(100) NOT NULL DEFAULT 'system',
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

-- ── 13. Journal financier catégorisé ─────────────────────────────────────────

CREATE TABLE IF NOT EXISTS finances.journal_financier (
  id SERIAL PRIMARY KEY,
  company_id INTEGER NOT NULL REFERENCES auth.companies(id),
  type VARCHAR(20) NOT NULL, -- 'encaissement', 'decaissement'
  categorie VARCHAR(50) NOT NULL, -- 'scolarite', 'salaire', 'depense', 'fourniture', etc.
  montant INTEGER NOT NULL,
  description TEXT,
  reference_externe VARCHAR(100),
  mode_paiement VARCHAR(20) NOT NULL DEFAULT 'mobile_money', -- 'mobile_money', 'especes', 'virement'
  date_operation DATE NOT NULL,
  eleve_id INTEGER,
  parent_user_id INTEGER,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  created_by VARCHAR(100) NOT NULL DEFAULT 'system',
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

-- ── 14. Paliers scolarité familiale ──────────────────────────────────────────

CREATE TABLE IF NOT EXISTS finances.paliers_familiaux (
  id SERIAL PRIMARY KEY,
  company_id INTEGER NOT NULL REFERENCES auth.companies(id),
  rang_enfant INTEGER NOT NULL, -- 1, 2, 3, 4+
  reduction_pourcent INTEGER NOT NULL DEFAULT 0,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  created_by VARCHAR(100) NOT NULL DEFAULT 'system',
  UNIQUE(company_id, rang_enfant)
);

-- ── 15. Abonnements Communication Parent ↔ Établissement ─────────────────────

CREATE TABLE IF NOT EXISTS communication.abonnements_parent (
  id SERIAL PRIMARY KEY,
  parent_user_id INTEGER NOT NULL REFERENCES auth.users(id),
  company_id INTEGER NOT NULL REFERENCES auth.companies(id),
  est_actif BOOLEAN NOT NULL DEFAULT TRUE,
  date_debut DATE NOT NULL,
  date_fin DATE, -- NULL = actif
  prix_mensuel INTEGER NOT NULL DEFAULT 5000, -- 5000 GNF/mois/établissement
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ,
  created_by VARCHAR(100) NOT NULL DEFAULT 'system',
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
  UNIQUE(parent_user_id, company_id)
);

-- ── 16. Options supplémentaires (abonnables par le parent) ───────────────────

CREATE TABLE IF NOT EXISTS auth.options_services (
  id SERIAL PRIMARY KEY,
  code VARCHAR(50) NOT NULL UNIQUE,
  nom VARCHAR(200) NOT NULL,
  description TEXT,
  prix_mensuel INTEGER NOT NULL DEFAULT 0,
  type_cible VARCHAR(20) NOT NULL DEFAULT 'parent', -- 'parent', 'enseignant', 'etablissement'
  est_disponible BOOLEAN NOT NULL DEFAULT TRUE, -- désactivable globalement
  icone VARCHAR(100),
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

-- Options prédéfinies
INSERT INTO auth.options_services (code, nom, description, prix_mensuel, type_cible, est_disponible) VALUES
  ('communication-etablissement', 'Communication Établissement', 'Recevez les annonces, notifications et messages de l''établissement', 5000, 'parent', TRUE),
  ('traceur-gps', 'Traceur GPS Enfant', 'Suivez en temps réel la position de votre enfant sur une carte', 15000, 'parent', FALSE)
ON CONFLICT (code) DO NOTHING;

-- Abonnements aux options par utilisateur
CREATE TABLE IF NOT EXISTS auth.abonnements_options (
  id SERIAL PRIMARY KEY,
  user_id INTEGER NOT NULL REFERENCES auth.users(id),
  option_id INTEGER NOT NULL REFERENCES auth.options_services(id),
  company_id INTEGER REFERENCES auth.companies(id), -- pour les options liées à un établissement
  est_actif BOOLEAN NOT NULL DEFAULT TRUE,
  date_debut DATE NOT NULL,
  date_fin DATE,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ,
  created_by VARCHAR(100) NOT NULL DEFAULT 'system',
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
  UNIQUE(user_id, option_id, company_id)
);

-- ── 17. Traceurs GPS (désactivé pour le moment) ─────────────────────────────

CREATE TABLE IF NOT EXISTS presences.traceurs_gps (
  id SERIAL PRIMARY KEY,
  eleve_id INTEGER NOT NULL,
  company_id INTEGER NOT NULL REFERENCES auth.companies(id),
  code_traceur VARCHAR(100) UNIQUE,
  est_actif BOOLEAN NOT NULL DEFAULT FALSE, -- désactivé par défaut
  derniere_position_lat DOUBLE PRECISION,
  derniere_position_lng DOUBLE PRECISION,
  derniere_synchro TIMESTAMPTZ,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

-- ── 18. Modèle de document par établissement ─────────────────────────────────

CREATE SCHEMA IF NOT EXISTS documents;

CREATE TABLE IF NOT EXISTS documents.modeles_etablissement (
  id SERIAL PRIMARY KEY,
  company_id INTEGER NOT NULL REFERENCES auth.companies(id),
  logo_url VARCHAR(500),
  nom_etablissement VARCHAR(300),
  adresse VARCHAR(500),
  telephone VARCHAR(50),
  email VARCHAR(200),
  pied_de_page TEXT,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ,
  created_by VARCHAR(100) NOT NULL DEFAULT 'system'
);

-- ── 16. Vérification de documents ────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS documents.verifications (
  id SERIAL PRIMARY KEY,
  code_verification VARCHAR(100) NOT NULL UNIQUE,
  mot_de_passe_hash VARCHAR(200) NOT NULL,
  document_type VARCHAR(50) NOT NULL,
  document_id INTEGER NOT NULL,
  company_id INTEGER NOT NULL REFERENCES auth.companies(id),
  date_emission DATE NOT NULL,
  metadata JSONB DEFAULT '{}',
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ── 17. Configuration email SMTP ─────────────────────────────────────────────

CREATE SCHEMA IF NOT EXISTS support;

CREATE TABLE IF NOT EXISTS support.email_config (
  id SERIAL PRIMARY KEY,
  smtp_host VARCHAR(200) NOT NULL,
  smtp_port INTEGER NOT NULL DEFAULT 587,
  smtp_user VARCHAR(200) NOT NULL,
  smtp_password VARCHAR(500) NOT NULL,
  email_expediteur VARCHAR(200) NOT NULL,
  nom_expediteur VARCHAR(100) NOT NULL DEFAULT 'Kouroukan',
  est_actif BOOLEAN NOT NULL DEFAULT TRUE,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ
);

-- ── 18. Configuration NimbaSMS ───────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS support.sms_config (
  id SERIAL PRIMARY KEY,
  api_key VARCHAR(200) NOT NULL,
  api_secret VARCHAR(200),
  sender_name VARCHAR(20) NOT NULL DEFAULT 'Kouroukan',
  solde_actuel INTEGER DEFAULT 0,
  derniere_synchro TIMESTAMPTZ,
  est_actif BOOLEAN NOT NULL DEFAULT TRUE,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ
);

-- ── 19. Contenu IA administré ────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS support.contenu_ia (
  id SERIAL PRIMARY KEY,
  rubrique VARCHAR(50) NOT NULL,
  titre VARCHAR(200) NOT NULL,
  contenu TEXT NOT NULL,
  est_actif BOOLEAN NOT NULL DEFAULT TRUE,
  ordre INTEGER NOT NULL DEFAULT 0,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ,
  created_by VARCHAR(100) NOT NULL DEFAULT 'system',
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

-- ── 20. Ajouter type_compte aux users ────────────────────────────────────────

ALTER TABLE auth.users ADD COLUMN IF NOT EXISTS type_compte VARCHAR(20) NOT NULL DEFAULT 'etablissement';
-- type_compte: 'super_admin', 'etablissement', 'enseignant', 'parent', 'eleve'

-- ── 21. Ajouter photo de profil si pas encore ────────────────────────────────

ALTER TABLE auth.users ADD COLUMN IF NOT EXISTS avatar_url VARCHAR(500);
