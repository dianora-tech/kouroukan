-- ============================================================================
-- V022__indexes.sql
-- Kouroukan - Index de performance sur toutes les tables
-- ============================================================================

-- ============================================================================
-- AUTH
-- ============================================================================

-- Users
CREATE UNIQUE INDEX IF NOT EXISTS ix_users_email
    ON auth.users(email) WHERE is_deleted = FALSE;
CREATE UNIQUE INDEX IF NOT EXISTS ix_users_phone
    ON auth.users(phone_number) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_users_active
    ON auth.users(is_active) WHERE is_deleted = FALSE;

-- Roles
CREATE UNIQUE INDEX IF NOT EXISTS ix_roles_name
    ON auth.roles(name) WHERE is_deleted = FALSE;

-- Permissions
CREATE UNIQUE INDEX IF NOT EXISTS ix_permissions_name
    ON auth.permissions(name) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_permissions_module
    ON auth.permissions(module) WHERE is_deleted = FALSE;

-- User Roles
CREATE INDEX IF NOT EXISTS ix_user_roles_user_id
    ON auth.user_roles(user_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_user_roles_role_id
    ON auth.user_roles(role_id) WHERE is_deleted = FALSE;

-- Role Permissions
CREATE INDEX IF NOT EXISTS ix_role_permissions_role_id
    ON auth.role_permissions(role_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_role_permissions_permission_id
    ON auth.role_permissions(permission_id) WHERE is_deleted = FALSE;

-- Refresh Tokens
CREATE UNIQUE INDEX IF NOT EXISTS ix_refresh_tokens_token
    ON auth.refresh_tokens(token);
CREATE INDEX IF NOT EXISTS ix_refresh_tokens_user_id
    ON auth.refresh_tokens(user_id);

-- Companies
CREATE INDEX IF NOT EXISTS ix_companies_name
    ON auth.companies(name) WHERE is_deleted = FALSE;

-- User Companies
CREATE INDEX IF NOT EXISTS ix_user_companies_user_id
    ON auth.user_companies(user_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_user_companies_company_id
    ON auth.user_companies(company_id) WHERE is_deleted = FALSE;

-- CGU Versions
CREATE UNIQUE INDEX IF NOT EXISTS ix_cgu_versions_version
    ON auth.cgu_versions(version) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_cgu_versions_active
    ON auth.cgu_versions(est_active) WHERE is_deleted = FALSE AND est_active = TRUE;

-- ============================================================================
-- GEO
-- ============================================================================

CREATE UNIQUE INDEX IF NOT EXISTS ix_regions_code
    ON geo.regions(code) WHERE is_deleted = FALSE;
CREATE UNIQUE INDEX IF NOT EXISTS ix_prefectures_code
    ON geo.prefectures(code) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_prefectures_region_id
    ON geo.prefectures(region_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_sous_prefectures_prefecture_id
    ON geo.sous_prefectures(prefecture_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_user_locations_user_id
    ON geo.user_locations(user_id) WHERE is_deleted = FALSE;

-- ============================================================================
-- INSCRIPTIONS
-- ============================================================================

-- Annees scolaires
CREATE INDEX IF NOT EXISTS ix_annees_scolaires_active
    ON inscriptions.annees_scolaires(est_active) WHERE is_deleted = FALSE AND est_active = TRUE;

-- Eleves
CREATE UNIQUE INDEX IF NOT EXISTS ix_eleves_matricule
    ON inscriptions.eleves(numero_matricule) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_eleves_user_id
    ON inscriptions.eleves(user_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_eleves_parent_id
    ON inscriptions.eleves(parent_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_eleves_niveau_classe_id
    ON inscriptions.eleves(niveau_classe_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_eleves_classe_id
    ON inscriptions.eleves(classe_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_eleves_statut
    ON inscriptions.eleves(statut_inscription) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_eleves_nom_prenom
    ON inscriptions.eleves(last_name, first_name) WHERE is_deleted = FALSE;

-- Dossiers admission
CREATE INDEX IF NOT EXISTS ix_dossiers_admission_eleve_id
    ON inscriptions.dossiers_admission(eleve_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_dossiers_admission_annee
    ON inscriptions.dossiers_admission(annee_scolaire_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_dossiers_admission_statut
    ON inscriptions.dossiers_admission(statut_dossier) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_dossiers_admission_type_id
    ON inscriptions.dossiers_admission(type_id) WHERE is_deleted = FALSE;

-- Inscriptions
CREATE INDEX IF NOT EXISTS ix_inscriptions_eleve_id
    ON inscriptions.inscriptions(eleve_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_inscriptions_classe_id
    ON inscriptions.inscriptions(classe_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_inscriptions_annee
    ON inscriptions.inscriptions(annee_scolaire_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_inscriptions_statut
    ON inscriptions.inscriptions(statut_inscription) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_inscriptions_type_id
    ON inscriptions.inscriptions(type_id) WHERE is_deleted = FALSE;

-- ============================================================================
-- PEDAGOGIE
-- ============================================================================

-- Niveaux classes
CREATE UNIQUE INDEX IF NOT EXISTS ix_niveaux_classes_code
    ON pedagogie.niveaux_classes(code) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_niveaux_classes_cycle
    ON pedagogie.niveaux_classes(cycle_etude) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_niveaux_classes_ordre
    ON pedagogie.niveaux_classes(ordre) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_niveaux_classes_type_id
    ON pedagogie.niveaux_classes(type_id) WHERE is_deleted = FALSE;

-- Classes
CREATE INDEX IF NOT EXISTS ix_classes_niveau_classe_id
    ON pedagogie.classes(niveau_classe_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_classes_annee_scolaire_id
    ON pedagogie.classes(annee_scolaire_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_classes_enseignant_principal
    ON pedagogie.classes(enseignant_principal_id) WHERE is_deleted = FALSE;

-- Matieres
CREATE UNIQUE INDEX IF NOT EXISTS ix_matieres_code_niveau
    ON pedagogie.matieres(code, niveau_classe_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_matieres_niveau_classe_id
    ON pedagogie.matieres(niveau_classe_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_matieres_type_id
    ON pedagogie.matieres(type_id) WHERE is_deleted = FALSE;

-- Salles
CREATE INDEX IF NOT EXISTS ix_salles_type_id
    ON pedagogie.salles(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_salles_disponible
    ON pedagogie.salles(est_disponible) WHERE is_deleted = FALSE AND est_disponible = TRUE;

-- Seances
CREATE INDEX IF NOT EXISTS ix_seances_classe_id
    ON pedagogie.seances(classe_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_seances_enseignant_id
    ON pedagogie.seances(enseignant_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_seances_matiere_id
    ON pedagogie.seances(matiere_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_seances_salle_id
    ON pedagogie.seances(salle_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_seances_jour_heure
    ON pedagogie.seances(jour_semaine, heure_debut) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_seances_annee
    ON pedagogie.seances(annee_scolaire_id) WHERE is_deleted = FALSE;

-- Cahiers textes
CREATE INDEX IF NOT EXISTS ix_cahiers_textes_seance_id
    ON pedagogie.cahiers_textes(seance_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_cahiers_textes_date
    ON pedagogie.cahiers_textes(date_seance) WHERE is_deleted = FALSE;

-- ============================================================================
-- EVALUATIONS
-- ============================================================================

-- Evaluations
CREATE INDEX IF NOT EXISTS ix_evaluations_classe_id
    ON evaluations.evaluations(classe_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_evaluations_matiere_id
    ON evaluations.evaluations(matiere_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_evaluations_enseignant_id
    ON evaluations.evaluations(enseignant_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_evaluations_trimestre_annee
    ON evaluations.evaluations(trimestre, annee_scolaire_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_evaluations_type_id
    ON evaluations.evaluations(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_evaluations_user_id
    ON evaluations.evaluations(user_id) WHERE is_deleted = FALSE;

-- Notes
CREATE INDEX IF NOT EXISTS ix_notes_evaluation_id
    ON evaluations.notes(evaluation_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_notes_eleve_id
    ON evaluations.notes(eleve_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_notes_eleve_evaluation
    ON evaluations.notes(eleve_id, evaluation_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_notes_user_id
    ON evaluations.notes(user_id) WHERE is_deleted = FALSE;

-- Bulletins
CREATE INDEX IF NOT EXISTS ix_bulletins_eleve_id
    ON evaluations.bulletins(eleve_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_bulletins_classe_id
    ON evaluations.bulletins(classe_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_bulletins_trimestre_annee
    ON evaluations.bulletins(trimestre, annee_scolaire_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_bulletins_user_id
    ON evaluations.bulletins(user_id) WHERE is_deleted = FALSE;

-- ============================================================================
-- PRESENCES
-- ============================================================================

-- Appels
CREATE INDEX IF NOT EXISTS ix_appels_classe_id
    ON presences.appels(classe_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_appels_enseignant_id
    ON presences.appels(enseignant_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_appels_date
    ON presences.appels(date_appel) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_appels_user_id
    ON presences.appels(user_id) WHERE is_deleted = FALSE;

-- Absences
CREATE INDEX IF NOT EXISTS ix_absences_eleve_id
    ON presences.absences(eleve_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_absences_appel_id
    ON presences.absences(appel_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_absences_date
    ON presences.absences(date_absence) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_absences_justifiee
    ON presences.absences(est_justifiee) WHERE is_deleted = FALSE AND est_justifiee = FALSE;
CREATE INDEX IF NOT EXISTS ix_absences_type_id
    ON presences.absences(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_absences_user_id
    ON presences.absences(user_id) WHERE is_deleted = FALSE;

-- Badgeages
CREATE INDEX IF NOT EXISTS ix_badgeages_eleve_id
    ON presences.badgeages(eleve_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_badgeages_date
    ON presences.badgeages(date_badgeage) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_badgeages_type_id
    ON presences.badgeages(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_badgeages_user_id
    ON presences.badgeages(user_id) WHERE is_deleted = FALSE;

-- ============================================================================
-- FINANCES
-- ============================================================================

-- Factures
CREATE UNIQUE INDEX IF NOT EXISTS ix_factures_numero
    ON finances.factures(numero_facture) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_factures_eleve_id
    ON finances.factures(eleve_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_factures_parent_id
    ON finances.factures(parent_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_factures_statut
    ON finances.factures(statut_facture) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_factures_echeance
    ON finances.factures(date_echeance) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_factures_annee
    ON finances.factures(annee_scolaire_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_factures_type_id
    ON finances.factures(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_factures_user_id
    ON finances.factures(user_id) WHERE is_deleted = FALSE;

-- Paiements
CREATE UNIQUE INDEX IF NOT EXISTS ix_paiements_numero_recu
    ON finances.paiements(numero_recu) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_paiements_facture_id
    ON finances.paiements(facture_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_paiements_statut
    ON finances.paiements(statut_paiement) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_paiements_date
    ON finances.paiements(date_paiement) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_paiements_moyen
    ON finances.paiements(moyen_paiement) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_paiements_type_id
    ON finances.paiements(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_paiements_user_id
    ON finances.paiements(user_id) WHERE is_deleted = FALSE;

-- Depenses
CREATE UNIQUE INDEX IF NOT EXISTS ix_depenses_justificatif
    ON finances.depenses(numero_justificatif) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_depenses_statut
    ON finances.depenses(statut_depense) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_depenses_categorie
    ON finances.depenses(categorie) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_depenses_type_id
    ON finances.depenses(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_depenses_user_id
    ON finances.depenses(user_id) WHERE is_deleted = FALSE;

-- Remunerations enseignants
CREATE INDEX IF NOT EXISTS ix_remunerations_enseignant_id
    ON finances.remunerations_enseignants(enseignant_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_remunerations_mois_annee
    ON finances.remunerations_enseignants(mois, annee) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_remunerations_statut
    ON finances.remunerations_enseignants(statut_paiement) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_remunerations_user_id
    ON finances.remunerations_enseignants(user_id) WHERE is_deleted = FALSE;

-- ============================================================================
-- PERSONNEL
-- ============================================================================

-- Enseignants
CREATE UNIQUE INDEX IF NOT EXISTS ix_enseignants_matricule
    ON personnel.enseignants(matricule) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_enseignants_user_id
    ON personnel.enseignants(user_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_enseignants_statut
    ON personnel.enseignants(statut_enseignant) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_enseignants_type_id
    ON personnel.enseignants(type_id) WHERE is_deleted = FALSE;

-- Demandes conges
CREATE INDEX IF NOT EXISTS ix_demandes_conges_enseignant_id
    ON personnel.demandes_conges(enseignant_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_demandes_conges_statut
    ON personnel.demandes_conges(statut_demande) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_demandes_conges_dates
    ON personnel.demandes_conges(date_debut, date_fin) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_demandes_conges_type_id
    ON personnel.demandes_conges(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_demandes_conges_user_id
    ON personnel.demandes_conges(user_id) WHERE is_deleted = FALSE;

-- ============================================================================
-- COMMUNICATION
-- ============================================================================

-- Messages
CREATE INDEX IF NOT EXISTS ix_messages_expediteur_id
    ON communication.messages(expediteur_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_messages_destinataire_id
    ON communication.messages(destinataire_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_messages_est_lu
    ON communication.messages(est_lu) WHERE is_deleted = FALSE AND est_lu = FALSE;
CREATE INDEX IF NOT EXISTS ix_messages_type_id
    ON communication.messages(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_messages_user_id
    ON communication.messages(user_id) WHERE is_deleted = FALSE;

-- Notifications
CREATE INDEX IF NOT EXISTS ix_notifications_canal
    ON communication.notifications(canal) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_notifications_envoyee
    ON communication.notifications(est_envoyee) WHERE is_deleted = FALSE AND est_envoyee = FALSE;
CREATE INDEX IF NOT EXISTS ix_notifications_type_id
    ON communication.notifications(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_notifications_user_id
    ON communication.notifications(user_id) WHERE is_deleted = FALSE;

-- Annonces
CREATE INDEX IF NOT EXISTS ix_annonces_active
    ON communication.annonces(est_active) WHERE is_deleted = FALSE AND est_active = TRUE;
CREATE INDEX IF NOT EXISTS ix_annonces_dates
    ON communication.annonces(date_debut, date_fin) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_annonces_type_id
    ON communication.annonces(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_annonces_user_id
    ON communication.annonces(user_id) WHERE is_deleted = FALSE;

-- ============================================================================
-- BDE
-- ============================================================================

-- Associations
CREATE INDEX IF NOT EXISTS ix_associations_statut
    ON bde.associations(statut) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_associations_type_id
    ON bde.associations(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_associations_user_id
    ON bde.associations(user_id) WHERE is_deleted = FALSE;

-- Evenements
CREATE INDEX IF NOT EXISTS ix_evenements_association_id
    ON bde.evenements(association_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_evenements_date
    ON bde.evenements(date_evenement) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_evenements_statut
    ON bde.evenements(statut_evenement) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_evenements_type_id
    ON bde.evenements(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_evenements_user_id
    ON bde.evenements(user_id) WHERE is_deleted = FALSE;

-- Membres BDE
CREATE INDEX IF NOT EXISTS ix_membres_bde_association_id
    ON bde.membres_bde(association_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_membres_bde_eleve_id
    ON bde.membres_bde(eleve_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_membres_bde_user_id
    ON bde.membres_bde(user_id) WHERE is_deleted = FALSE;

-- Depenses BDE
CREATE INDEX IF NOT EXISTS ix_depenses_bde_association_id
    ON bde.depenses_bde(association_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_depenses_bde_statut
    ON bde.depenses_bde(statut_validation) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_depenses_bde_type_id
    ON bde.depenses_bde(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_depenses_bde_user_id
    ON bde.depenses_bde(user_id) WHERE is_deleted = FALSE;

-- ============================================================================
-- DOCUMENTS
-- ============================================================================

-- Modeles documents
CREATE UNIQUE INDEX IF NOT EXISTS ix_modeles_documents_code
    ON documents.modeles_documents(code) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_modeles_documents_actif
    ON documents.modeles_documents(est_actif) WHERE is_deleted = FALSE AND est_actif = TRUE;
CREATE INDEX IF NOT EXISTS ix_modeles_documents_type_id
    ON documents.modeles_documents(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_modeles_documents_user_id
    ON documents.modeles_documents(user_id) WHERE is_deleted = FALSE;

-- Documents generes
CREATE INDEX IF NOT EXISTS ix_documents_generes_modele_id
    ON documents.documents_generes(modele_document_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_documents_generes_eleve_id
    ON documents.documents_generes(eleve_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_documents_generes_enseignant_id
    ON documents.documents_generes(enseignant_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_documents_generes_statut
    ON documents.documents_generes(statut_signature) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_documents_generes_type_id
    ON documents.documents_generes(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_documents_generes_user_id
    ON documents.documents_generes(user_id) WHERE is_deleted = FALSE;

-- Signatures
CREATE INDEX IF NOT EXISTS ix_signatures_document_id
    ON documents.signatures(document_genere_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_signatures_signataire_id
    ON documents.signatures(signataire_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_signatures_statut
    ON documents.signatures(statut_signature) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_signatures_type_id
    ON documents.signatures(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_signatures_user_id
    ON documents.signatures(user_id) WHERE is_deleted = FALSE;

-- ============================================================================
-- SERVICES PREMIUM
-- ============================================================================

-- Services parents
CREATE UNIQUE INDEX IF NOT EXISTS ix_services_parents_code
    ON services.services_parents(code) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_services_parents_actif
    ON services.services_parents(est_actif) WHERE is_deleted = FALSE AND est_actif = TRUE;
CREATE INDEX IF NOT EXISTS ix_services_parents_type_id
    ON services.services_parents(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_services_parents_user_id
    ON services.services_parents(user_id) WHERE is_deleted = FALSE;

-- Souscriptions
CREATE INDEX IF NOT EXISTS ix_souscriptions_service_id
    ON services.souscriptions(service_parent_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_souscriptions_parent_id
    ON services.souscriptions(parent_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_souscriptions_statut
    ON services.souscriptions(statut_souscription) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_souscriptions_renouvellement
    ON services.souscriptions(date_prochain_renouvellement)
    WHERE is_deleted = FALSE AND renouvellement_auto = TRUE;
CREATE INDEX IF NOT EXISTS ix_souscriptions_user_id
    ON services.souscriptions(user_id) WHERE is_deleted = FALSE;

-- ============================================================================
-- SUPPORT
-- ============================================================================

-- Tickets
CREATE INDEX IF NOT EXISTS ix_tickets_auteur_id
    ON support.tickets(auteur_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_tickets_assigne_a_id
    ON support.tickets(assigne_a_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_tickets_statut
    ON support.tickets(statut_ticket) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_tickets_priorite
    ON support.tickets(priorite) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_tickets_module
    ON support.tickets(module_concerne) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_tickets_type_id
    ON support.tickets(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_tickets_user_id
    ON support.tickets(user_id) WHERE is_deleted = FALSE;

-- Reponses tickets
CREATE INDEX IF NOT EXISTS ix_reponses_tickets_ticket_id
    ON support.reponses_tickets(ticket_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_reponses_tickets_user_id
    ON support.reponses_tickets(user_id) WHERE is_deleted = FALSE;

-- Suggestions
CREATE INDEX IF NOT EXISTS ix_suggestions_auteur_id
    ON support.suggestions(auteur_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_suggestions_statut
    ON support.suggestions(statut_suggestion) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_suggestions_votes
    ON support.suggestions(nombre_votes DESC) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_suggestions_type_id
    ON support.suggestions(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_suggestions_user_id
    ON support.suggestions(user_id) WHERE is_deleted = FALSE;

-- Votes suggestions
CREATE INDEX IF NOT EXISTS ix_votes_suggestions_suggestion_id
    ON support.votes_suggestions(suggestion_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_votes_suggestions_votant_id
    ON support.votes_suggestions(votant_id) WHERE is_deleted = FALSE;

-- Articles aide
CREATE UNIQUE INDEX IF NOT EXISTS ix_articles_aide_slug
    ON support.articles_aide(slug) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_articles_aide_categorie
    ON support.articles_aide(categorie) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_articles_aide_publie
    ON support.articles_aide(est_publie) WHERE is_deleted = FALSE AND est_publie = TRUE;
CREATE INDEX IF NOT EXISTS ix_articles_aide_module
    ON support.articles_aide(module_concerne) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_articles_aide_type_id
    ON support.articles_aide(type_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_articles_aide_user_id
    ON support.articles_aide(user_id) WHERE is_deleted = FALSE;

-- Conversations IA
CREATE INDEX IF NOT EXISTS ix_conversations_ia_utilisateur_id
    ON support.conversations_ia(utilisateur_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS ix_conversations_ia_active
    ON support.conversations_ia(est_active) WHERE is_deleted = FALSE AND est_active = TRUE;

-- Messages IA
CREATE INDEX IF NOT EXISTS ix_messages_ia_conversation_id
    ON support.messages_ia(conversation_ia_id) WHERE is_deleted = FALSE;
