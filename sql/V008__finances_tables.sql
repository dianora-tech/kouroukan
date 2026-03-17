-- ============================================================================
-- V008__finances_tables.sql
-- Kouroukan - Module Finances (port 5005, schema finances)
-- ============================================================================

-- ----------------------------------------------------------------------------
-- Tables de types
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS finances.type_factures (
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

CREATE TABLE IF NOT EXISTS finances.type_paiements (
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

CREATE TABLE IF NOT EXISTS finances.type_depenses (
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
-- finances.factures
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS finances.factures (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id             INT                         NOT NULL REFERENCES finances.type_factures(id) ON DELETE NO ACTION,
    eleve_id            INT                         NOT NULL REFERENCES inscriptions.eleves(id) ON DELETE NO ACTION,
    parent_id           INT                         NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    annee_scolaire_id   INT                         NOT NULL REFERENCES inscriptions.annees_scolaires(id) ON DELETE NO ACTION,
    montant_total       NUMERIC(15,2)               NOT NULL,
    montant_paye        NUMERIC(15,2)               NOT NULL DEFAULT 0,
    solde               NUMERIC(15,2)               NOT NULL,
    date_emission       DATE                        NOT NULL,
    date_echeance       DATE                        NOT NULL,
    statut_facture      VARCHAR(30)                 NOT NULL,   -- Emise/PartPaye/Payee/Echue/Annulee
    numero_facture      VARCHAR(50)                 NOT NULL,
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
-- finances.paiements
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS finances.paiements (
    id                      INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id                 INT                         NOT NULL REFERENCES finances.type_paiements(id) ON DELETE NO ACTION,
    facture_id              INT                         NOT NULL REFERENCES finances.factures(id) ON DELETE NO ACTION,
    montant_paye            NUMERIC(15,2)               NOT NULL,
    date_paiement           TIMESTAMP WITH TIME ZONE    NOT NULL,
    moyen_paiement          VARCHAR(30)                 NOT NULL,   -- OrangeMoney/SoutraMoney/MTNMoMo/Especes
    reference_mobile_money  VARCHAR(100)                NULL,
    statut_paiement         VARCHAR(30)                 NOT NULL,   -- EnAttente/Confirme/Echec/Rembourse
    caissier_id             INT                         NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    numero_recu             VARCHAR(50)                 NOT NULL,
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

-- ----------------------------------------------------------------------------
-- finances.depenses
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS finances.depenses (
    id                      INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    type_id                 INT                         NOT NULL REFERENCES finances.type_depenses(id) ON DELETE NO ACTION,
    montant                 NUMERIC(15,2)               NOT NULL,
    motif_depense           VARCHAR(500)                NOT NULL,
    categorie               VARCHAR(100)                NOT NULL,   -- Personnel/Fournitures/Maintenance/Evenements/BDE
    beneficiaire_nom        VARCHAR(200)                NOT NULL,
    beneficiaire_telephone  VARCHAR(20)                 NULL,
    beneficiaire_nif        VARCHAR(50)                 NULL,
    statut_depense          VARCHAR(30)                 NOT NULL,   -- Demande/ValideN1/ValideFinance/ValideDirection/Executee/Archivee
    date_demande            DATE                        NOT NULL,
    date_validation         DATE                        NULL,
    validateur_id           INT                         NULL REFERENCES auth.users(id) ON DELETE NO ACTION,
    piece_jointe_url        VARCHAR(500)                NULL,
    numero_justificatif     VARCHAR(50)                 NOT NULL,
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

-- ----------------------------------------------------------------------------
-- finances.remunerations_enseignants
-- FK vers personnel.enseignants ajoutee dans V015
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS finances.remunerations_enseignants (
    id                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    enseignant_id       INT                         NOT NULL,   -- FK → personnel.enseignants (V015)
    mois                INT                         NOT NULL,   -- 1-12
    annee               INT                         NOT NULL,
    mode_remuneration   VARCHAR(30)                 NOT NULL,   -- Forfait/Heures/Mixte
    montant_forfait     NUMERIC(15,2)               NULL,
    nombre_heures       NUMERIC(10,2)               NULL,
    taux_horaire        NUMERIC(15,2)               NULL,
    montant_total       NUMERIC(15,2)               NOT NULL,
    statut_paiement     VARCHAR(30)                 NOT NULL,   -- EnAttente/Valide/Paye
    date_validation     DATE                        NULL,
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
