-- ============================================================================
-- V018__seed_reference_data.sql
-- Kouroukan - Seed des donnees de reference (types, geo, niveaux)
-- ============================================================================

-- ============================================================================
-- 1. DONNEES GEOGRAPHIQUES (8 regions + prefectures)
-- ============================================================================

INSERT INTO geo.regions (name, code) VALUES
    ('Conakry',    'CKY'),
    ('Boke',       'BOK'),
    ('Faranah',    'FAR'),
    ('Kankan',     'KAN'),
    ('Kindia',     'KIN'),
    ('Labe',       'LAB'),
    ('Mamou',      'MAM'),
    ('Nzerekore',  'NZR')
ON CONFLICT DO NOTHING;

-- Prefectures par region
-- Region Conakry (5 communes)
INSERT INTO geo.prefectures (name, code, region_id) VALUES
    ('Kaloum',    'KLM', (SELECT id FROM geo.regions WHERE code = 'CKY')),
    ('Dixinn',    'DXN', (SELECT id FROM geo.regions WHERE code = 'CKY')),
    ('Matam',     'MTM', (SELECT id FROM geo.regions WHERE code = 'CKY')),
    ('Ratoma',    'RTM', (SELECT id FROM geo.regions WHERE code = 'CKY')),
    ('Matoto',    'MTO', (SELECT id FROM geo.regions WHERE code = 'CKY'))
ON CONFLICT DO NOTHING;

-- Region Boke
INSERT INTO geo.prefectures (name, code, region_id) VALUES
    ('Boke',      'BOK', (SELECT id FROM geo.regions WHERE code = 'BOK')),
    ('Boffa',     'BFA', (SELECT id FROM geo.regions WHERE code = 'BOK')),
    ('Fria',      'FRI', (SELECT id FROM geo.regions WHERE code = 'BOK')),
    ('Gaoual',    'GAO', (SELECT id FROM geo.regions WHERE code = 'BOK')),
    ('Koundara',  'KND', (SELECT id FROM geo.regions WHERE code = 'BOK'))
ON CONFLICT DO NOTHING;

-- Region Faranah
INSERT INTO geo.prefectures (name, code, region_id) VALUES
    ('Faranah',       'FAR', (SELECT id FROM geo.regions WHERE code = 'FAR')),
    ('Dabola',        'DAB', (SELECT id FROM geo.regions WHERE code = 'FAR')),
    ('Dinguiraye',    'DIN', (SELECT id FROM geo.regions WHERE code = 'FAR')),
    ('Kissidougou',   'KIS', (SELECT id FROM geo.regions WHERE code = 'FAR'))
ON CONFLICT DO NOTHING;

-- Region Kankan
INSERT INTO geo.prefectures (name, code, region_id) VALUES
    ('Kankan',        'KAN', (SELECT id FROM geo.regions WHERE code = 'KAN')),
    ('Kerouane',      'KER', (SELECT id FROM geo.regions WHERE code = 'KAN')),
    ('Kouroussa',     'KOU', (SELECT id FROM geo.regions WHERE code = 'KAN')),
    ('Mandiana',      'MAN', (SELECT id FROM geo.regions WHERE code = 'KAN')),
    ('Siguiri',       'SIG', (SELECT id FROM geo.regions WHERE code = 'KAN'))
ON CONFLICT DO NOTHING;

-- Region Kindia
INSERT INTO geo.prefectures (name, code, region_id) VALUES
    ('Kindia',        'KIN', (SELECT id FROM geo.regions WHERE code = 'KIN')),
    ('Coyah',         'COY', (SELECT id FROM geo.regions WHERE code = 'KIN')),
    ('Dubreka',       'DUB', (SELECT id FROM geo.regions WHERE code = 'KIN')),
    ('Forecariah',    'FOR', (SELECT id FROM geo.regions WHERE code = 'KIN')),
    ('Telimele',      'TEL', (SELECT id FROM geo.regions WHERE code = 'KIN'))
ON CONFLICT DO NOTHING;

-- Region Labe
INSERT INTO geo.prefectures (name, code, region_id) VALUES
    ('Labe',          'LAB', (SELECT id FROM geo.regions WHERE code = 'LAB')),
    ('Koubia',        'KBA', (SELECT id FROM geo.regions WHERE code = 'LAB')),
    ('Lelouma',       'LEL', (SELECT id FROM geo.regions WHERE code = 'LAB')),
    ('Mali',          'MAL', (SELECT id FROM geo.regions WHERE code = 'LAB')),
    ('Tougue',        'TOU', (SELECT id FROM geo.regions WHERE code = 'LAB'))
ON CONFLICT DO NOTHING;

-- Region Mamou
INSERT INTO geo.prefectures (name, code, region_id) VALUES
    ('Mamou',         'MAM', (SELECT id FROM geo.regions WHERE code = 'MAM')),
    ('Dalaba',        'DAL', (SELECT id FROM geo.regions WHERE code = 'MAM')),
    ('Pita',          'PIT', (SELECT id FROM geo.regions WHERE code = 'MAM'))
ON CONFLICT DO NOTHING;

-- Region Nzerekore
INSERT INTO geo.prefectures (name, code, region_id) VALUES
    ('Nzerekore',     'NZR', (SELECT id FROM geo.regions WHERE code = 'NZR')),
    ('Beyla',         'BEY', (SELECT id FROM geo.regions WHERE code = 'NZR')),
    ('Gueckedou',     'GUE', (SELECT id FROM geo.regions WHERE code = 'NZR')),
    ('Lola',          'LOL', (SELECT id FROM geo.regions WHERE code = 'NZR')),
    ('Macenta',       'MAC', (SELECT id FROM geo.regions WHERE code = 'NZR')),
    ('Yomou',         'YOM', (SELECT id FROM geo.regions WHERE code = 'NZR'))
ON CONFLICT DO NOTHING;

-- ============================================================================
-- 2. TYPES DE REFERENCE — MODULE INSCRIPTIONS
-- ============================================================================

INSERT INTO inscriptions.type_dossiers_admission (name, description) VALUES
    ('Admission directe',      'Admission standard sans examen'),
    ('Transfert',              'Transfert depuis un autre etablissement'),
    ('Redoublement',           'Redoublement — taux tres eleve en Guinee'),
    ('Admission sur examen',   'Admission apres passage d''un examen'),
    ('Admission post-CEE',     'Passage primaire vers college apres CEE'),
    ('Admission post-BEPC',    'Passage college vers lycee ou ETFP Type A apres BEPC'),
    ('Admission post-BAC',     'Passage lycee vers universite ou ETFP Type B apres BU'),
    ('Passerelle ETFP',        'Orientation depuis l''enseignement general vers ETFP')
ON CONFLICT DO NOTHING;

INSERT INTO inscriptions.type_inscriptions (name, description) VALUES
    ('Nouvelle inscription',   'Premiere inscription dans l''etablissement'),
    ('Reinscription',          'Reinscription pour une nouvelle annee'),
    ('Transfert entrant',      'Eleve transifere d''un autre etablissement'),
    ('Redoublement',           'Inscription pour redoublement')
ON CONFLICT DO NOTHING;

-- ============================================================================
-- 3. TYPES DE REFERENCE — MODULE PEDAGOGIE
-- ============================================================================

-- Types de niveaux (cycles d'etude)
INSERT INTO pedagogie.type_niveaux_classes (name, description) VALUES
    ('Prescolaire',         '3-5 ans, 3 niveaux, MENA'),
    ('Primaire',            '6-11 ans, 6 niveaux CP1-CM2, MENA, sanctionne par CEE'),
    ('College',             '12-15 ans, 4 niveaux 7E-10E, MENA, sanctionne par BEPC'),
    ('Lycee',               '16-18 ans, 3 niveaux 11E-TLE, MENA, sanctionne par BU'),
    ('ETFP_PostPrimaire',   'Post-primaire apres CM2, 9-18 mois, METFP-ET, sanctionne par CQP'),
    ('ETFP_TypeA',          'Apres BEPC, 2-3 ans, METFP-ET, sanctionne par BEP/CAP'),
    ('ETFP_TypeB',          'Apres Bac/Type A, 2-3 ans, METFP-ET, sanctionne par BTS'),
    ('ENF',                 'Education Non Formelle, 9-15 ans non scolarises, 3 ans, MENA'),
    ('Universite',          'Systeme LMD depuis 2008, MESRS')
ON CONFLICT DO NOTHING;

-- Types de matieres
INSERT INTO pedagogie.type_matieres (name, description) VALUES
    ('Scientifique',  'Matieres scientifiques'),
    ('Litteraire',    'Matieres litteraires'),
    ('Artistique',    'Matieres artistiques'),
    ('Sportive',      'Education physique et sportive'),
    ('Technique',     'Matieres techniques et professionnelles')
ON CONFLICT DO NOTHING;

-- Types de salles
INSERT INTO pedagogie.type_salles (name, description) VALUES
    ('Salle de classe',     'Salle de cours standard'),
    ('Laboratoire',         'Laboratoire scientifique'),
    ('Salle informatique',  'Salle equipee d''ordinateurs'),
    ('Amphitheatre',        'Grand amphitheatre'),
    ('Gymnase',             'Salle de sport'),
    ('Bibliotheque',        'Bibliotheque / centre de documentation')
ON CONFLICT DO NOTHING;

-- ============================================================================
-- 4. NIVEAUX DE CLASSES — Nomenclature guineenne officielle (ProDEG 2020-2029)
-- ============================================================================

-- PRESCOLAIRE (3-5 ans, MENA)
INSERT INTO pedagogie.niveaux_classes (type_id, name, code, ordre, cycle_etude, age_officiel_entree, ministere_tutelle, examen_sortie, taux_horaire_enseignant) VALUES
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'Prescolaire'), 'Petite Section',  'PS',  1,  'Prescolaire', 3,  'MENA', NULL,  12000),
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'Prescolaire'), 'Moyenne Section', 'MS',  2,  'Prescolaire', 4,  'MENA', NULL,  12000),
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'Prescolaire'), 'Grande Section',  'GS',  3,  'Prescolaire', 5,  'MENA', NULL,  12000)
ON CONFLICT DO NOTHING;

-- PRIMAIRE (6-11 ans, MENA, sanctionne par CEE)
INSERT INTO pedagogie.niveaux_classes (type_id, name, code, ordre, cycle_etude, age_officiel_entree, ministere_tutelle, examen_sortie, taux_horaire_enseignant) VALUES
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'Primaire'), 'CP1 (1ere annee)', 'CP1', 4,  'Primaire', 6,  'MENA', NULL,  15000),
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'Primaire'), 'CP2 (2eme annee)', 'CP2', 5,  'Primaire', 7,  'MENA', NULL,  15000),
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'Primaire'), 'CE1 (3eme annee)', 'CE1', 6,  'Primaire', 8,  'MENA', NULL,  15000),
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'Primaire'), 'CE2 (4eme annee)', 'CE2', 7,  'Primaire', 9,  'MENA', NULL,  15000),
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'Primaire'), 'CM1 (5eme annee)', 'CM1', 8,  'Primaire', 10, 'MENA', NULL,  18000),
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'Primaire'), 'CM2 (6eme annee)', 'CM2', 9,  'Primaire', 11, 'MENA', 'CEE', 18000)
ON CONFLICT DO NOTHING;

-- COLLEGE / 1er cycle secondaire (12-15 ans, MENA, sanctionne par BEPC)
INSERT INTO pedagogie.niveaux_classes (type_id, name, code, ordre, cycle_etude, age_officiel_entree, ministere_tutelle, examen_sortie, taux_horaire_enseignant) VALUES
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'College'), '7eme annee',  '7E',  10, 'College', 12, 'MENA', NULL,   22000),
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'College'), '8eme annee',  '8E',  11, 'College', 13, 'MENA', NULL,   22000),
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'College'), '9eme annee',  '9E',  12, 'College', 14, 'MENA', NULL,   22000),
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'College'), '10eme annee', '10E', 13, 'College', 15, 'MENA', 'BEPC', 22000)
ON CONFLICT DO NOTHING;

-- LYCEE / 2nd cycle secondaire (16-18 ans, MENA, sanctionne par BU)
INSERT INTO pedagogie.niveaux_classes (type_id, name, code, ordre, cycle_etude, age_officiel_entree, ministere_tutelle, examen_sortie, taux_horaire_enseignant) VALUES
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'Lycee'), '11eme annee', '11E', 14, 'Lycee', 16, 'MENA', NULL, 28000),
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'Lycee'), '12eme annee', '12E', 15, 'Lycee', 17, 'MENA', NULL, 28000),
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'Lycee'), 'Terminale',   'TLE', 16, 'Lycee', 18, 'MENA', 'BU', 28000)
ON CONFLICT DO NOTHING;

-- UNIVERSITE / Systeme LMD (MESRS)
INSERT INTO pedagogie.niveaux_classes (type_id, name, code, ordre, cycle_etude, age_officiel_entree, ministere_tutelle, examen_sortie, taux_horaire_enseignant) VALUES
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'Universite'), 'Licence 1', 'L1', 17, 'Universite', 19, 'MESRS', NULL,      35000),
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'Universite'), 'Licence 2', 'L2', 18, 'Universite', 20, 'MESRS', NULL,      35000),
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'Universite'), 'Licence 3', 'L3', 19, 'Universite', 21, 'MESRS', 'Licence', 35000),
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'Universite'), 'Master 1',  'M1', 20, 'Universite', 22, 'MESRS', NULL,      45000),
    ((SELECT id FROM pedagogie.type_niveaux_classes WHERE name = 'Universite'), 'Master 2',  'M2', 21, 'Universite', 23, 'MESRS', 'Master',  45000)
ON CONFLICT DO NOTHING;

-- ============================================================================
-- 5. TYPES DE REFERENCE — MODULE EVALUATIONS
-- ============================================================================

INSERT INTO evaluations.type_evaluations (name, description) VALUES
    ('Devoir surveille',        'Devoir surveille en classe'),
    ('Interrogation',           'Interrogation ecrite ou orale'),
    ('Examen trimestriel',      'Examen de fin de trimestre'),
    ('Examen semestriel',       'Examen de fin de semestre (universite)'),
    ('Examen final',            'Examen de fin d''annee'),
    ('TP/TD',                   'Travaux pratiques ou diriges'),
    ('CEE',                     'Certificat d''Etudes Elementaires (fin CM2)'),
    ('BEPC',                    'Brevet d''Etudes du Premier Cycle (fin 10eme)'),
    ('Baccalaureat Unique',     'BU (fin Terminale) — series SE/SM/SS'),
    ('Concours d''entree',      'Concours d''entree ETFP ou universite')
ON CONFLICT DO NOTHING;

-- ============================================================================
-- 6. TYPES DE REFERENCE — MODULE PRESENCES
-- ============================================================================

INSERT INTO presences.type_absences (name, description) VALUES
    ('Non justifiee',       'Absence sans justification'),
    ('Maladie',             'Absence pour raison medicale'),
    ('Evenement familial',  'Absence pour evenement familial'),
    ('Retard',              'Retard en cours')
ON CONFLICT DO NOTHING;

INSERT INTO presences.type_badgeages (name, description) VALUES
    ('Entree etablissement',   'Badgeage a l''entree de l''etablissement'),
    ('Sortie etablissement',   'Badgeage a la sortie de l''etablissement'),
    ('Cantine',                'Badgeage a la cantine'),
    ('Bibliotheque',           'Badgeage a la bibliotheque'),
    ('Salle de sport',         'Badgeage a la salle de sport')
ON CONFLICT DO NOTHING;

-- ============================================================================
-- 7. TYPES DE REFERENCE — MODULE FINANCES
-- ============================================================================

INSERT INTO finances.type_factures (name, description) VALUES
    ('Frais d''inscription',   'Frais d''inscription annuels'),
    ('Frais de scolarite',     'Frais de scolarite trimestriels ou annuels'),
    ('Frais de cantine',       'Frais de restauration scolaire'),
    ('Frais de transport',     'Frais de transport scolaire'),
    ('Frais d''activites',     'Frais d''activites extra-scolaires')
ON CONFLICT DO NOTHING;

INSERT INTO finances.type_paiements (name, description) VALUES
    ('Orange Money',   'Paiement via Orange Money'),
    ('Soutra Money',   'Paiement via Soutra Money'),
    ('MTN MoMo',       'Paiement via MTN Mobile Money'),
    ('Especes',        'Paiement en especes a la caisse')
ON CONFLICT DO NOTHING;

INSERT INTO finances.type_depenses (name, description) VALUES
    ('Personnel',      'Depenses liees au personnel'),
    ('Fournitures',    'Achat de fournitures'),
    ('Maintenance',    'Maintenance et reparations'),
    ('Evenements',     'Organisation d''evenements'),
    ('BDE',            'Depenses du bureau des etudiants'),
    ('Equipements',    'Achat d''equipements')
ON CONFLICT DO NOTHING;

-- ============================================================================
-- 8. TYPES DE REFERENCE — MODULE PERSONNEL
-- ============================================================================

INSERT INTO personnel.type_enseignants (name, description) VALUES
    ('Permanent',    'Enseignant permanent titulaire'),
    ('Vacataire',    'Enseignant vacataire'),
    ('Contractuel',  'Enseignant sous contrat'),
    ('Stagiaire',    'Enseignant stagiaire en formation')
ON CONFLICT DO NOTHING;

INSERT INTO personnel.type_demandes_conges (name, description) VALUES
    ('Conge annuel',            'Conge annuel legal'),
    ('Conge maladie',           'Conge pour raison medicale'),
    ('Formation',               'Conge pour formation professionnelle'),
    ('Evenement familial',      'Conge pour evenement familial (mariage, deces, etc.)'),
    ('Maternite/Paternite',     'Conge maternite ou paternite')
ON CONFLICT DO NOTHING;

-- ============================================================================
-- 9. TYPES DE REFERENCE — MODULE COMMUNICATION
-- ============================================================================

INSERT INTO communication.type_messages (name, description) VALUES
    ('Personnel',   'Message individuel entre deux utilisateurs'),
    ('Groupe',      'Message envoye a un groupe'),
    ('Diffusion',   'Message de diffusion a large audience')
ON CONFLICT DO NOTHING;

INSERT INTO communication.type_notifications (name, description) VALUES
    ('Absence',     'Notification d''absence'),
    ('Note',        'Notification de nouvelle note'),
    ('Paiement',    'Notification de paiement'),
    ('Evenement',   'Notification d''evenement'),
    ('Alerte',      'Alerte importante'),
    ('Systeme',     'Notification systeme')
ON CONFLICT DO NOTHING;

INSERT INTO communication.type_annonces (name, description) VALUES
    ('Information',      'Annonce d''information generale'),
    ('Urgente',          'Annonce urgente prioritaire'),
    ('Evenement',        'Annonce d''evenement'),
    ('Administrative',   'Annonce administrative')
ON CONFLICT DO NOTHING;

-- ============================================================================
-- 10. TYPES DE REFERENCE — MODULE BDE
-- ============================================================================

INSERT INTO bde.type_associations (name, description) VALUES
    ('BDE',                  'Bureau des etudiants principal'),
    ('Club sportif',         'Association sportive'),
    ('Club culturel',        'Association culturelle'),
    ('Association solidaire','Association de solidarite'),
    ('Club scientifique',    'Club a vocation scientifique')
ON CONFLICT DO NOTHING;

INSERT INTO bde.type_evenements (name, description) VALUES
    ('Soiree',       'Soiree ou gala'),
    ('Sortie',       'Sortie pedagogique ou de loisir'),
    ('Competition',  'Competition sportive ou academique'),
    ('Atelier',      'Atelier ou workshop'),
    ('Conference',   'Conference ou seminaire')
ON CONFLICT DO NOTHING;

INSERT INTO bde.type_depenses_bde (name, description) VALUES
    ('Materiel',       'Achat de materiel'),
    ('Location',       'Location de salle ou equipement'),
    ('Prestataire',    'Paiement de prestataire'),
    ('Remboursement',  'Remboursement de frais')
ON CONFLICT DO NOTHING;

-- ============================================================================
-- 11. TYPES DE REFERENCE — MODULE DOCUMENTS
-- ============================================================================

INSERT INTO documents.type_modeles_documents (name, description) VALUES
    ('Bulletin de notes',           'Modele de bulletin scolaire'),
    ('Releve de notes',             'Modele de releve de notes'),
    ('Attestation de scolarite',    'Attestation de frequentation scolaire'),
    ('Attestation de reussite',     'Attestation de reussite a un examen'),
    ('Justificatif de depense',     'Modele de justificatif de depense'),
    ('Contrat d''inscription',      'Contrat d''inscription scolaire'),
    ('Recu de paiement',            'Modele de recu de paiement'),
    ('Convocation',                 'Modele de convocation'),
    ('Attestation de travail',      'Attestation de travail pour enseignant'),
    ('Demande de conge',            'Formulaire de demande de conge')
ON CONFLICT DO NOTHING;

INSERT INTO documents.type_documents_generes (name, description) VALUES
    ('Bulletin',       'Bulletin de notes genere'),
    ('Attestation',    'Attestation generee'),
    ('Certificat',     'Certificat genere'),
    ('Recu',           'Recu de paiement genere'),
    ('Contrat',        'Contrat genere'),
    ('Convocation',    'Convocation generee')
ON CONFLICT DO NOTHING;

INSERT INTO documents.type_signatures (name, description) VALUES
    ('Basique',   'Signature electronique basique'),
    ('Avancee',   'Signature electronique avancee avec verification')
ON CONFLICT DO NOTHING;

-- ============================================================================
-- 12. TYPES DE REFERENCE — MODULE SERVICES PREMIUM
-- ============================================================================

INSERT INTO services.type_services_parents (name, description) VALUES
    ('Notifications SMS',           'Notifications par SMS des evenements scolaires'),
    ('Alertes absence',             'Alertes immediates en cas d''absence'),
    ('Graphiques progression',      'Graphiques de progression academique de l''enfant'),
    ('Rapport hebdomadaire',        'Rapport hebdomadaire envoye aux parents'),
    ('Contenus pedagogiques',       'Acces a des contenus pedagogiques supplementaires'),
    ('Badge NFC enfant',            'Badge NFC pour le badgeage de l''enfant'),
    ('Consultation enseignant',     'Prise de rendez-vous avec les enseignants'),
    ('Archive bulletins',           'Archivage numerique des bulletins')
ON CONFLICT DO NOTHING;

-- ============================================================================
-- 13. TYPES DE REFERENCE — MODULE SUPPORT
-- ============================================================================

INSERT INTO support.type_tickets (name, description) VALUES
    ('Bug / Dysfonctionnement',     'Signalement d''un bug ou dysfonctionnement'),
    ('Question / Comment faire',    'Question sur l''utilisation de la plateforme'),
    ('Demande d''amelioration',     'Proposition d''amelioration fonctionnelle'),
    ('Probleme de paiement',        'Probleme lie aux paiements'),
    ('Probleme de connexion',       'Probleme de connexion ou d''authentification'),
    ('Autre',                       'Autre type de demande')
ON CONFLICT DO NOTHING;

INSERT INTO support.type_suggestions (name, description) VALUES
    ('Nouvelle fonctionnalite',     'Suggestion de nouvelle fonctionnalite'),
    ('Amelioration existante',      'Amelioration d''une fonctionnalite existante'),
    ('Interface utilisateur',       'Amelioration de l''interface'),
    ('Performance',                 'Amelioration des performances'),
    ('Contenu pedagogique',         'Suggestion de contenu pedagogique')
ON CONFLICT DO NOTHING;

INSERT INTO support.type_articles_aide (name, description) VALUES
    ('Guide de demarrage',  'Guide pour commencer a utiliser la plateforme'),
    ('Tutoriel',            'Tutoriel pas a pas'),
    ('FAQ',                 'Questions frequemment posees'),
    ('Depannage',           'Guide de depannage et resolution de problemes'),
    ('Bonnes pratiques',    'Conseils et bonnes pratiques d''utilisation')
ON CONFLICT DO NOTHING;
