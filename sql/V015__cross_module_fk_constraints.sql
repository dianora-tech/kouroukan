-- ============================================================================
-- V015__cross_module_fk_constraints.sql
-- Kouroukan - Contraintes FK inter-modules
-- Ajoutees apres la creation de toutes les tables pour eviter les deps circulaires
-- ============================================================================

-- ============================================================================
-- Module INSCRIPTIONS → PEDAGOGIE
-- ============================================================================

-- inscriptions.eleves.niveau_classe_id → pedagogie.niveaux_classes
ALTER TABLE inscriptions.eleves
    ADD CONSTRAINT fk_eleves_niveau_classe
    FOREIGN KEY (niveau_classe_id) REFERENCES pedagogie.niveaux_classes(id)
    ON DELETE NO ACTION;

-- inscriptions.eleves.classe_id → pedagogie.classes
ALTER TABLE inscriptions.eleves
    ADD CONSTRAINT fk_eleves_classe
    FOREIGN KEY (classe_id) REFERENCES pedagogie.classes(id)
    ON DELETE NO ACTION;

-- inscriptions.inscriptions.classe_id → pedagogie.classes
ALTER TABLE inscriptions.inscriptions
    ADD CONSTRAINT fk_inscriptions_classe
    FOREIGN KEY (classe_id) REFERENCES pedagogie.classes(id)
    ON DELETE NO ACTION;

-- ============================================================================
-- Module PEDAGOGIE → PERSONNEL
-- ============================================================================

-- pedagogie.classes.enseignant_principal_id → personnel.enseignants
ALTER TABLE pedagogie.classes
    ADD CONSTRAINT fk_classes_enseignant_principal
    FOREIGN KEY (enseignant_principal_id) REFERENCES personnel.enseignants(id)
    ON DELETE NO ACTION;

-- pedagogie.seances.enseignant_id → personnel.enseignants
ALTER TABLE pedagogie.seances
    ADD CONSTRAINT fk_seances_enseignant
    FOREIGN KEY (enseignant_id) REFERENCES personnel.enseignants(id)
    ON DELETE NO ACTION;

-- ============================================================================
-- Module EVALUATIONS → PERSONNEL
-- ============================================================================

-- evaluations.evaluations.enseignant_id → personnel.enseignants
ALTER TABLE evaluations.evaluations
    ADD CONSTRAINT fk_evaluations_enseignant
    FOREIGN KEY (enseignant_id) REFERENCES personnel.enseignants(id)
    ON DELETE NO ACTION;

-- ============================================================================
-- Module PRESENCES → PERSONNEL
-- ============================================================================

-- presences.appels.enseignant_id → personnel.enseignants
ALTER TABLE presences.appels
    ADD CONSTRAINT fk_appels_enseignant
    FOREIGN KEY (enseignant_id) REFERENCES personnel.enseignants(id)
    ON DELETE NO ACTION;

-- ============================================================================
-- Module FINANCES → PERSONNEL
-- ============================================================================

-- finances.remunerations_enseignants.enseignant_id → personnel.enseignants
ALTER TABLE finances.remunerations_enseignants
    ADD CONSTRAINT fk_remunerations_enseignant
    FOREIGN KEY (enseignant_id) REFERENCES personnel.enseignants(id)
    ON DELETE NO ACTION;

-- ============================================================================
-- Module DOCUMENTS → PERSONNEL
-- ============================================================================

-- documents.documents_generes.enseignant_id → personnel.enseignants
ALTER TABLE documents.documents_generes
    ADD CONSTRAINT fk_documents_generes_enseignant
    FOREIGN KEY (enseignant_id) REFERENCES personnel.enseignants(id)
    ON DELETE NO ACTION;
