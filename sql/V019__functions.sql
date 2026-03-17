-- ============================================================================
-- V019__functions.sql
-- Kouroukan - Fonctions et procedures stockees PostgreSQL
-- ============================================================================

-- ============================================================================
-- Fonction : recuperer un utilisateur avec ses roles et permissions
-- ============================================================================
CREATE OR REPLACE FUNCTION auth.get_user_with_roles_and_permissions(p_user_id INT)
RETURNS TABLE (
    user_data       JSON,
    roles           TEXT[],
    permissions     TEXT[]
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        row_to_json(u.*) AS user_data,
        ARRAY(
            SELECT r.name
            FROM auth.roles r
            JOIN auth.user_roles ur ON r.id = ur.role_id
            WHERE ur.user_id = p_user_id
              AND ur.is_deleted = FALSE
              AND r.is_deleted = FALSE
        ) AS roles,
        ARRAY(
            SELECT DISTINCT p.name
            FROM auth.permissions p
            JOIN auth.role_permissions rp ON p.id = rp.permission_id
            JOIN auth.user_roles ur ON rp.role_id = ur.role_id
            WHERE ur.user_id = p_user_id
              AND ur.is_deleted = FALSE
              AND rp.is_deleted = FALSE
              AND p.is_deleted = FALSE
        ) AS permissions
    FROM auth.users u
    WHERE u.id = p_user_id
      AND u.is_active = TRUE
      AND u.is_deleted = FALSE;
END;
$$ LANGUAGE plpgsql;

-- ============================================================================
-- Fonction : verifier si un utilisateur a accepte la version actuelle des CGU
-- ============================================================================
CREATE OR REPLACE FUNCTION auth.check_cgu_acceptance(p_user_id INT)
RETURNS TABLE (
    doit_accepter   BOOLEAN,
    version_actuelle VARCHAR(20),
    version_user    VARCHAR(20)
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        CASE
            WHEN u.cgu_version IS NULL THEN TRUE
            WHEN u.cgu_version != cv.version THEN TRUE
            ELSE FALSE
        END AS doit_accepter,
        cv.version AS version_actuelle,
        u.cgu_version AS version_user
    FROM auth.users u
    CROSS JOIN (
        SELECT version
        FROM auth.cgu_versions
        WHERE est_active = TRUE
          AND is_deleted = FALSE
        ORDER BY date_publication DESC
        LIMIT 1
    ) cv
    WHERE u.id = p_user_id
      AND u.is_deleted = FALSE;
END;
$$ LANGUAGE plpgsql;

-- ============================================================================
-- Fonction : accepter les CGU pour un utilisateur
-- ============================================================================
CREATE OR REPLACE FUNCTION auth.accept_cgu(p_user_id INT)
RETURNS VOID AS $$
DECLARE
    v_version VARCHAR(20);
BEGIN
    SELECT version INTO v_version
    FROM auth.cgu_versions
    WHERE est_active = TRUE
      AND is_deleted = FALSE
    ORDER BY date_publication DESC
    LIMIT 1;

    IF v_version IS NULL THEN
        RAISE EXCEPTION 'Aucune version active des CGU trouvee';
    END IF;

    UPDATE auth.users
    SET cgu_accepted_at = NOW(),
        cgu_version = v_version,
        updated_at = NOW(),
        updated_by = 'system'
    WHERE id = p_user_id
      AND is_deleted = FALSE;
END;
$$ LANGUAGE plpgsql;

-- ============================================================================
-- Fonction : calculer la moyenne d'un eleve pour un trimestre donne
-- ============================================================================
CREATE OR REPLACE FUNCTION evaluations.calculer_moyenne_eleve(
    p_eleve_id          INT,
    p_classe_id         INT,
    p_trimestre         INT,
    p_annee_scolaire_id INT
)
RETURNS NUMERIC(5,2) AS $$
DECLARE
    v_moyenne NUMERIC(5,2);
BEGIN
    SELECT
        CASE
            WHEN SUM(m.coefficient) = 0 THEN 0
            ELSE ROUND(
                SUM((n.valeur / e.note_maximale * 20) * m.coefficient) / SUM(m.coefficient),
                2
            )
        END INTO v_moyenne
    FROM evaluations.notes n
    JOIN evaluations.evaluations e ON n.evaluation_id = e.id
    JOIN pedagogie.matieres m ON e.matiere_id = m.id
    WHERE n.eleve_id = p_eleve_id
      AND e.classe_id = p_classe_id
      AND e.trimestre = p_trimestre
      AND e.annee_scolaire_id = p_annee_scolaire_id
      AND n.is_deleted = FALSE
      AND e.is_deleted = FALSE;

    RETURN COALESCE(v_moyenne, 0);
END;
$$ LANGUAGE plpgsql;

-- ============================================================================
-- Fonction : calculer le solde d'une facture et mettre a jour le statut
-- ============================================================================
CREATE OR REPLACE FUNCTION finances.recalculer_solde_facture(p_facture_id INT)
RETURNS VOID AS $$
DECLARE
    v_total_paye NUMERIC(15,2);
    v_montant_total NUMERIC(15,2);
BEGIN
    -- Calculer le total des paiements confirmes
    SELECT COALESCE(SUM(montant_paye), 0) INTO v_total_paye
    FROM finances.paiements
    WHERE facture_id = p_facture_id
      AND statut_paiement = 'Confirme'
      AND is_deleted = FALSE;

    -- Recuperer le montant total de la facture
    SELECT montant_total INTO v_montant_total
    FROM finances.factures
    WHERE id = p_facture_id
      AND is_deleted = FALSE;

    -- Mettre a jour la facture
    UPDATE finances.factures
    SET montant_paye = v_total_paye,
        solde = v_montant_total - v_total_paye,
        statut_facture = CASE
            WHEN v_total_paye >= v_montant_total THEN 'Payee'
            WHEN v_total_paye > 0 THEN 'PartPaye'
            ELSE statut_facture
        END,
        updated_at = NOW()
    WHERE id = p_facture_id
      AND is_deleted = FALSE;
END;
$$ LANGUAGE plpgsql;

-- ============================================================================
-- Fonction : compter les absences non justifiees d'un eleve
-- ============================================================================
CREATE OR REPLACE FUNCTION presences.compter_absences_non_justifiees(
    p_eleve_id          INT,
    p_annee_scolaire_id INT
)
RETURNS INT AS $$
DECLARE
    v_count INT;
BEGIN
    SELECT COUNT(*) INTO v_count
    FROM presences.absences a
    JOIN presences.appels ap ON a.appel_id = ap.id
    JOIN pedagogie.seances s ON ap.seance_id = s.id
    WHERE a.eleve_id = p_eleve_id
      AND s.annee_scolaire_id = p_annee_scolaire_id
      AND a.est_justifiee = FALSE
      AND a.is_deleted = FALSE
      AND ap.is_deleted = FALSE;

    RETURN COALESCE(v_count, 0);
END;
$$ LANGUAGE plpgsql;

-- ============================================================================
-- Fonction : calculer la remuneration mensuelle d'un enseignant
-- ============================================================================
CREATE OR REPLACE FUNCTION finances.calculer_remuneration_enseignant(
    p_enseignant_id INT,
    p_mois          INT,
    p_annee         INT
)
RETURNS NUMERIC(15,2) AS $$
DECLARE
    v_mode          VARCHAR(30);
    v_forfait       NUMERIC(15,2);
    v_heures        NUMERIC(10,2);
    v_taux          NUMERIC(15,2);
    v_total         NUMERIC(15,2);
BEGIN
    -- Recuperer le mode de remuneration
    SELECT mode_remuneration, montant_forfait
    INTO v_mode, v_forfait
    FROM personnel.enseignants
    WHERE id = p_enseignant_id
      AND is_deleted = FALSE;

    IF v_mode = 'Forfait' THEN
        v_total := COALESCE(v_forfait, 0);
    ELSIF v_mode = 'Heures' THEN
        -- Compter les heures effectuees dans le mois
        SELECT COUNT(*) * 1.0, -- nombre de seances = heures approximatives
               COALESCE(MAX(nc.taux_horaire_enseignant), 0)
        INTO v_heures, v_taux
        FROM pedagogie.seances s
        JOIN pedagogie.classes c ON s.classe_id = c.id
        JOIN pedagogie.niveaux_classes nc ON c.niveau_classe_id = nc.id
        WHERE s.enseignant_id = p_enseignant_id
          AND s.is_deleted = FALSE;

        v_total := COALESCE(v_heures * v_taux, 0);
    ELSE -- Mixte
        v_total := COALESCE(v_forfait, 0);
    END IF;

    RETURN v_total;
END;
$$ LANGUAGE plpgsql;

-- ============================================================================
-- Fonction utilitaire : soft delete generique
-- ============================================================================
CREATE OR REPLACE FUNCTION public.soft_delete(
    p_schema    VARCHAR,
    p_table     VARCHAR,
    p_id        INT,
    p_user      VARCHAR DEFAULT 'system'
)
RETURNS VOID AS $$
BEGIN
    EXECUTE format(
        'UPDATE %I.%I SET is_deleted = TRUE, deleted_at = NOW(), deleted_by = $1 WHERE id = $2',
        p_schema, p_table
    ) USING p_user, p_id;
END;
$$ LANGUAGE plpgsql;
