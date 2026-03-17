-- ============================================================================
-- V021__views.sql
-- Kouroukan - Vues resume par module
-- ============================================================================

-- ============================================================================
-- VUE : Inscriptions — Resume par utilisateur
-- ============================================================================
CREATE OR REPLACE VIEW inscriptions.vw_inscriptions_summary AS
SELECT
    u.id                    AS user_id,
    u.first_name || ' ' || u.last_name AS user_name,
    COUNT(DISTINCT e.id)    AS nb_eleves,
    COUNT(DISTINCT da.id)   AS nb_dossiers_admission,
    COUNT(DISTINCT i.id)    AS nb_inscriptions
FROM auth.users u
LEFT JOIN inscriptions.eleves e
    ON e.user_id = u.id AND e.is_deleted = FALSE
LEFT JOIN inscriptions.dossiers_admission da
    ON da.user_id = u.id AND da.is_deleted = FALSE
LEFT JOIN inscriptions.inscriptions i
    ON i.user_id = u.id AND i.is_deleted = FALSE
WHERE u.is_active = TRUE AND u.is_deleted = FALSE
GROUP BY u.id, u.first_name, u.last_name;

-- ============================================================================
-- VUE : Pedagogie — Resume par utilisateur
-- ============================================================================
CREATE OR REPLACE VIEW pedagogie.vw_pedagogie_summary AS
SELECT
    u.id                    AS user_id,
    u.first_name || ' ' || u.last_name AS user_name,
    COUNT(DISTINCT s.id)    AS nb_seances,
    COUNT(DISTINCT ct.id)   AS nb_cahiers_textes
FROM auth.users u
LEFT JOIN pedagogie.seances s
    ON s.enseignant_id IN (SELECT pe.id FROM personnel.enseignants pe WHERE pe.user_id = u.id)
    AND s.is_deleted = FALSE
LEFT JOIN pedagogie.cahiers_textes ct
    ON ct.seance_id = s.id AND ct.is_deleted = FALSE
WHERE u.is_active = TRUE AND u.is_deleted = FALSE
GROUP BY u.id, u.first_name, u.last_name;

-- ============================================================================
-- VUE : Evaluations — Resume par utilisateur
-- ============================================================================
CREATE OR REPLACE VIEW evaluations.vw_evaluations_summary AS
SELECT
    u.id                    AS user_id,
    u.first_name || ' ' || u.last_name AS user_name,
    COUNT(DISTINCT ev.id)   AS nb_evaluations,
    COUNT(DISTINCT n.id)    AS nb_notes,
    COUNT(DISTINCT b.id)    AS nb_bulletins
FROM auth.users u
LEFT JOIN evaluations.evaluations ev
    ON ev.user_id = u.id AND ev.is_deleted = FALSE
LEFT JOIN evaluations.notes n
    ON n.user_id = u.id AND n.is_deleted = FALSE
LEFT JOIN evaluations.bulletins b
    ON b.user_id = u.id AND b.is_deleted = FALSE
WHERE u.is_active = TRUE AND u.is_deleted = FALSE
GROUP BY u.id, u.first_name, u.last_name;

-- ============================================================================
-- VUE : Presences — Resume par utilisateur
-- ============================================================================
CREATE OR REPLACE VIEW presences.vw_presences_summary AS
SELECT
    u.id                    AS user_id,
    u.first_name || ' ' || u.last_name AS user_name,
    COUNT(DISTINCT ap.id)   AS nb_appels,
    COUNT(DISTINCT ab.id)   AS nb_absences,
    COUNT(DISTINCT bd.id)   AS nb_badgeages
FROM auth.users u
LEFT JOIN presences.appels ap
    ON ap.user_id = u.id AND ap.is_deleted = FALSE
LEFT JOIN presences.absences ab
    ON ab.user_id = u.id AND ab.is_deleted = FALSE
LEFT JOIN presences.badgeages bd
    ON bd.user_id = u.id AND bd.is_deleted = FALSE
WHERE u.is_active = TRUE AND u.is_deleted = FALSE
GROUP BY u.id, u.first_name, u.last_name;

-- ============================================================================
-- VUE : Finances — Resume par utilisateur
-- ============================================================================
CREATE OR REPLACE VIEW finances.vw_finances_summary AS
SELECT
    u.id                    AS user_id,
    u.first_name || ' ' || u.last_name AS user_name,
    COUNT(DISTINCT f.id)    AS nb_factures,
    COUNT(DISTINCT p.id)    AS nb_paiements,
    COUNT(DISTINCT d.id)    AS nb_depenses,
    COALESCE(SUM(DISTINCT f.montant_total), 0) AS total_facture,
    COALESCE(SUM(DISTINCT f.montant_paye), 0)  AS total_paye
FROM auth.users u
LEFT JOIN finances.factures f
    ON f.user_id = u.id AND f.is_deleted = FALSE
LEFT JOIN finances.paiements p
    ON p.user_id = u.id AND p.is_deleted = FALSE
LEFT JOIN finances.depenses d
    ON d.user_id = u.id AND d.is_deleted = FALSE
WHERE u.is_active = TRUE AND u.is_deleted = FALSE
GROUP BY u.id, u.first_name, u.last_name;

-- ============================================================================
-- VUE : Personnel — Resume par utilisateur
-- ============================================================================
CREATE OR REPLACE VIEW personnel.vw_personnel_summary AS
SELECT
    u.id                    AS user_id,
    u.first_name || ' ' || u.last_name AS user_name,
    COUNT(DISTINCT e.id)    AS nb_enseignants,
    COUNT(DISTINCT dc.id)   AS nb_demandes_conges
FROM auth.users u
LEFT JOIN personnel.enseignants e
    ON e.user_id = u.id AND e.is_deleted = FALSE
LEFT JOIN personnel.demandes_conges dc
    ON dc.user_id = u.id AND dc.is_deleted = FALSE
WHERE u.is_active = TRUE AND u.is_deleted = FALSE
GROUP BY u.id, u.first_name, u.last_name;

-- ============================================================================
-- VUE : Communication — Resume par utilisateur
-- ============================================================================
CREATE OR REPLACE VIEW communication.vw_communication_summary AS
SELECT
    u.id                    AS user_id,
    u.first_name || ' ' || u.last_name AS user_name,
    COUNT(DISTINCT m.id)    AS nb_messages_envoyes,
    COUNT(DISTINCT mr.id)   AS nb_messages_recus,
    COUNT(DISTINCT n.id)    AS nb_notifications
FROM auth.users u
LEFT JOIN communication.messages m
    ON m.expediteur_id = u.id AND m.is_deleted = FALSE
LEFT JOIN communication.messages mr
    ON mr.destinataire_id = u.id AND mr.is_deleted = FALSE
LEFT JOIN communication.notifications n
    ON n.user_id = u.id AND n.is_deleted = FALSE
WHERE u.is_active = TRUE AND u.is_deleted = FALSE
GROUP BY u.id, u.first_name, u.last_name;

-- ============================================================================
-- VUE : BDE — Resume par utilisateur
-- ============================================================================
CREATE OR REPLACE VIEW bde.vw_bde_summary AS
SELECT
    u.id                    AS user_id,
    u.first_name || ' ' || u.last_name AS user_name,
    COUNT(DISTINCT a.id)    AS nb_associations,
    COUNT(DISTINCT ev.id)   AS nb_evenements,
    COUNT(DISTINCT mb.id)   AS nb_membres,
    COUNT(DISTINCT db.id)   AS nb_depenses_bde
FROM auth.users u
LEFT JOIN bde.associations a
    ON a.user_id = u.id AND a.is_deleted = FALSE
LEFT JOIN bde.evenements ev
    ON ev.user_id = u.id AND ev.is_deleted = FALSE
LEFT JOIN bde.membres_bde mb
    ON mb.user_id = u.id AND mb.is_deleted = FALSE
LEFT JOIN bde.depenses_bde db
    ON db.user_id = u.id AND db.is_deleted = FALSE
WHERE u.is_active = TRUE AND u.is_deleted = FALSE
GROUP BY u.id, u.first_name, u.last_name;

-- ============================================================================
-- VUE : Documents — Resume par utilisateur
-- ============================================================================
CREATE OR REPLACE VIEW documents.vw_documents_summary AS
SELECT
    u.id                    AS user_id,
    u.first_name || ' ' || u.last_name AS user_name,
    COUNT(DISTINCT md.id)   AS nb_modeles,
    COUNT(DISTINCT dg.id)   AS nb_documents_generes,
    COUNT(DISTINCT sg.id)   AS nb_signatures
FROM auth.users u
LEFT JOIN documents.modeles_documents md
    ON md.user_id = u.id AND md.is_deleted = FALSE
LEFT JOIN documents.documents_generes dg
    ON dg.user_id = u.id AND dg.is_deleted = FALSE
LEFT JOIN documents.signatures sg
    ON sg.user_id = u.id AND sg.is_deleted = FALSE
WHERE u.is_active = TRUE AND u.is_deleted = FALSE
GROUP BY u.id, u.first_name, u.last_name;

-- ============================================================================
-- VUE : Services Premium — Resume par utilisateur
-- ============================================================================
CREATE OR REPLACE VIEW services.vw_services_premium_summary AS
SELECT
    u.id                    AS user_id,
    u.first_name || ' ' || u.last_name AS user_name,
    COUNT(DISTINCT sp.id)   AS nb_services,
    COUNT(DISTINCT s.id)    AS nb_souscriptions,
    COUNT(DISTINCT CASE WHEN s.statut_souscription = 'Active' THEN s.id END) AS nb_souscriptions_actives
FROM auth.users u
LEFT JOIN services.services_parents sp
    ON sp.user_id = u.id AND sp.is_deleted = FALSE
LEFT JOIN services.souscriptions s
    ON s.user_id = u.id AND s.is_deleted = FALSE
WHERE u.is_active = TRUE AND u.is_deleted = FALSE
GROUP BY u.id, u.first_name, u.last_name;

-- ============================================================================
-- VUE : Support — Resume par utilisateur
-- ============================================================================
CREATE OR REPLACE VIEW support.vw_support_summary AS
SELECT
    u.id                    AS user_id,
    u.first_name || ' ' || u.last_name AS user_name,
    COUNT(DISTINCT t.id)    AS nb_tickets,
    COUNT(DISTINCT CASE WHEN t.statut_ticket IN ('Ouvert', 'EnCours', 'EnAttente') THEN t.id END) AS nb_tickets_ouverts,
    COUNT(DISTINCT sg.id)   AS nb_suggestions,
    COUNT(DISTINCT ci.id)   AS nb_conversations_ia
FROM auth.users u
LEFT JOIN support.tickets t
    ON t.auteur_id = u.id AND t.is_deleted = FALSE
LEFT JOIN support.suggestions sg
    ON sg.auteur_id = u.id AND sg.is_deleted = FALSE
LEFT JOIN support.conversations_ia ci
    ON ci.utilisateur_id = u.id AND ci.is_deleted = FALSE
WHERE u.is_active = TRUE AND u.is_deleted = FALSE
GROUP BY u.id, u.first_name, u.last_name;

-- ============================================================================
-- VUE : Dashboard Directeur — Stats globales de l'etablissement
-- ============================================================================
CREATE OR REPLACE VIEW inscriptions.vw_dashboard_directeur AS
SELECT
    as2.id                  AS annee_scolaire_id,
    as2.libelle             AS annee_scolaire,
    COUNT(DISTINCT e.id)    AS total_eleves,
    COUNT(DISTINCT CASE WHEN e.genre = 'F' THEN e.id END) AS eleves_filles,
    COUNT(DISTINCT CASE WHEN e.genre = 'M' THEN e.id END) AS eleves_garcons,
    COUNT(DISTINCT CASE WHEN e.statut_inscription = 'Inscrit' THEN e.id END) AS eleves_inscrits,
    COUNT(DISTINCT CASE WHEN i.est_redoublant = TRUE THEN i.id END) AS nb_redoublants,
    COUNT(DISTINCT c.id)    AS total_classes
FROM inscriptions.annees_scolaires as2
LEFT JOIN inscriptions.inscriptions i
    ON i.annee_scolaire_id = as2.id AND i.is_deleted = FALSE AND i.statut_inscription = 'Validee'
LEFT JOIN inscriptions.eleves e
    ON e.id = i.eleve_id AND e.is_deleted = FALSE
LEFT JOIN pedagogie.classes c
    ON c.annee_scolaire_id = as2.id AND c.is_deleted = FALSE
WHERE as2.is_deleted = FALSE
GROUP BY as2.id, as2.libelle;

-- ============================================================================
-- VUE : Factures impayees — suivi des paiements
-- ============================================================================
CREATE OR REPLACE VIEW finances.vw_factures_impayees AS
SELECT
    f.id                    AS facture_id,
    f.numero_facture,
    e.first_name || ' ' || e.last_name AS eleve_nom,
    e.numero_matricule,
    f.montant_total,
    f.montant_paye,
    f.solde,
    f.date_echeance,
    f.statut_facture,
    tf.name                 AS type_facture,
    as2.libelle             AS annee_scolaire,
    CASE
        WHEN f.date_echeance < CURRENT_DATE THEN 'Echue'
        WHEN f.date_echeance < CURRENT_DATE + INTERVAL '7 days' THEN 'Bientot echue'
        ELSE 'En cours'
    END AS urgence
FROM finances.factures f
JOIN inscriptions.eleves e ON f.eleve_id = e.id
JOIN finances.type_factures tf ON f.type_id = tf.id
JOIN inscriptions.annees_scolaires as2 ON f.annee_scolaire_id = as2.id
WHERE f.statut_facture IN ('Emise', 'PartPaye', 'Echue')
  AND f.is_deleted = FALSE
  AND e.is_deleted = FALSE
ORDER BY f.date_echeance ASC;
