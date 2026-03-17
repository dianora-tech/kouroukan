-- ============================================================================
-- V016__seed_roles_permissions.sql
-- Kouroukan - Seed des roles, permissions et mapping
-- ============================================================================

-- ============================================================================
-- ROLES
-- ============================================================================
INSERT INTO auth.roles (name, description) VALUES
    ('super_admin',             'Maintenance SaaS, multi-tenant, configuration technique globale'),
    ('fondateur',               'Proprietaire legal - visibilite totale en lecture seule, AUCUNE action possible'),
    ('admin_it',                'Configuration technique, comptes utilisateurs, securite, integrations, sauvegardes'),
    ('directeur',               'Tous les modules operationnels, configuration pedagogique et administrative'),
    ('censeur',                 'Notes, absences, emplois du temps, conseils de classe (perimetre pedagogique)'),
    ('intendant',               'Facturation, paiements, depenses, budgets, rapports financiers (perimetre financier)'),
    ('responsable_admissions',  'Dossiers candidats, workflow d''admission, communication familles'),
    ('chef_departement',        'Enseignants du departement, planning, ressources pedagogiques'),
    ('enseignant',              'Notes, appels, cahier de textes, absences/conges propres pour ses classes assignees'),
    ('personnel_admin',         'Dossiers, inscriptions, correspondances, imprimes (secretariat)'),
    ('responsable_bde',         'Gestion activites, membres, budget BDE, evenements'),
    ('parent',                  'Notes, absences, bulletins, paiements, messagerie, services optionnels pour ses enfants'),
    ('eleve',                   'Cours, devoirs, notes, emploi du temps, activites BDE pour sa propre scolarite')
ON CONFLICT DO NOTHING;

-- ============================================================================
-- PERMISSIONS
-- ============================================================================
INSERT INTO auth.permissions (name, description, module) VALUES
    -- Module inscriptions
    ('inscriptions:read',           'Lecture des inscriptions',          'inscriptions'),
    ('inscriptions:create',         'Creation d''inscriptions',         'inscriptions'),
    ('inscriptions:update',         'Modification d''inscriptions',     'inscriptions'),
    ('inscriptions:delete',         'Suppression d''inscriptions',      'inscriptions'),
    -- Module pedagogie
    ('pedagogie:read',              'Lecture pedagogie',                 'pedagogie'),
    ('pedagogie:create',            'Creation pedagogie',               'pedagogie'),
    ('pedagogie:update',            'Modification pedagogie',           'pedagogie'),
    ('pedagogie:delete',            'Suppression pedagogie',            'pedagogie'),
    -- Module evaluations
    ('evaluations:read',            'Lecture des evaluations',           'evaluations'),
    ('evaluations:create',          'Creation d''evaluations',          'evaluations'),
    ('evaluations:update',          'Modification d''evaluations',      'evaluations'),
    ('evaluations:delete',          'Suppression d''evaluations',       'evaluations'),
    -- Module presences
    ('presences:read',              'Lecture des presences',             'presences'),
    ('presences:create',            'Creation de presences',             'presences'),
    ('presences:update',            'Modification de presences',         'presences'),
    ('presences:delete',            'Suppression de presences',          'presences'),
    -- Module finances
    ('finances:read',               'Lecture des finances',              'finances'),
    ('finances:create',             'Creation financiere',               'finances'),
    ('finances:update',             'Modification financiere',           'finances'),
    ('finances:delete',             'Suppression financiere',            'finances'),
    -- Module personnel
    ('personnel:read',              'Lecture du personnel',              'personnel'),
    ('personnel:create',            'Creation de personnel',             'personnel'),
    ('personnel:update',            'Modification de personnel',         'personnel'),
    ('personnel:delete',            'Suppression de personnel',          'personnel'),
    -- Module communication
    ('communication:read',          'Lecture des communications',        'communication'),
    ('communication:create',        'Creation de communications',        'communication'),
    ('communication:update',        'Modification de communications',    'communication'),
    ('communication:delete',        'Suppression de communications',     'communication'),
    -- Module BDE
    ('bde:read',                    'Lecture BDE',                       'bde'),
    ('bde:create',                  'Creation BDE',                      'bde'),
    ('bde:update',                  'Modification BDE',                  'bde'),
    ('bde:delete',                  'Suppression BDE',                   'bde'),
    -- Module documents
    ('documents:read',              'Lecture des documents',             'documents'),
    ('documents:create',            'Creation de documents',             'documents'),
    ('documents:update',            'Modification de documents',         'documents'),
    ('documents:delete',            'Suppression de documents',          'documents'),
    -- Module services-premium
    ('services-premium:read',       'Lecture services premium',          'services-premium'),
    ('services-premium:create',     'Creation services premium',         'services-premium'),
    ('services-premium:update',     'Modification services premium',     'services-premium'),
    ('services-premium:delete',     'Suppression services premium',      'services-premium'),
    -- Module support
    ('support:read',                'Lecture du support',                'support'),
    ('support:create',              'Creation dans le support',          'support'),
    ('support:update',              'Modification du support',           'support'),
    ('support:delete',              'Suppression du support',            'support'),
    ('support:manage',              'Gestion du support',                'support'),
    -- Administration
    ('users:manage',                'Gestion des utilisateurs',          'admin'),
    ('cache:reload',                'Rechargement du cache',             'admin'),
    ('settings:manage',             'Gestion des parametres',            'admin'),
    ('audit:read',                  'Lecture des audits',                'admin')
ON CONFLICT DO NOTHING;

-- ============================================================================
-- MAPPING ROLES → PERMISSIONS
-- ============================================================================

-- inscriptions:read
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','fondateur','admin_it','directeur','censeur','responsable_admissions','personnel_admin')
  AND p.name = 'inscriptions:read'
ON CONFLICT DO NOTHING;

-- inscriptions:create
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur','responsable_admissions','personnel_admin')
  AND p.name = 'inscriptions:create'
ON CONFLICT DO NOTHING;

-- inscriptions:update
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur','responsable_admissions','personnel_admin')
  AND p.name = 'inscriptions:update'
ON CONFLICT DO NOTHING;

-- inscriptions:delete
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur')
  AND p.name = 'inscriptions:delete'
ON CONFLICT DO NOTHING;

-- pedagogie:read
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','fondateur','admin_it','directeur','censeur','chef_departement','enseignant')
  AND p.name = 'pedagogie:read'
ON CONFLICT DO NOTHING;

-- pedagogie:create
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur','censeur')
  AND p.name = 'pedagogie:create'
ON CONFLICT DO NOTHING;

-- pedagogie:update
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur','censeur','chef_departement')
  AND p.name = 'pedagogie:update'
ON CONFLICT DO NOTHING;

-- pedagogie:delete
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur')
  AND p.name = 'pedagogie:delete'
ON CONFLICT DO NOTHING;

-- evaluations:read
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','fondateur','directeur','censeur','chef_departement','enseignant','parent','eleve')
  AND p.name = 'evaluations:read'
ON CONFLICT DO NOTHING;

-- evaluations:create
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur','censeur','enseignant')
  AND p.name = 'evaluations:create'
ON CONFLICT DO NOTHING;

-- evaluations:update
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur','censeur','enseignant')
  AND p.name = 'evaluations:update'
ON CONFLICT DO NOTHING;

-- evaluations:delete
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur','censeur')
  AND p.name = 'evaluations:delete'
ON CONFLICT DO NOTHING;

-- presences:read
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','fondateur','directeur','censeur','enseignant','parent')
  AND p.name = 'presences:read'
ON CONFLICT DO NOTHING;

-- presences:create
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur','censeur','enseignant')
  AND p.name = 'presences:create'
ON CONFLICT DO NOTHING;

-- presences:update
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur','censeur','enseignant')
  AND p.name = 'presences:update'
ON CONFLICT DO NOTHING;

-- presences:delete
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur')
  AND p.name = 'presences:delete'
ON CONFLICT DO NOTHING;

-- finances:read
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','fondateur','directeur','intendant','parent')
  AND p.name = 'finances:read'
ON CONFLICT DO NOTHING;

-- finances:create
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur','intendant')
  AND p.name = 'finances:create'
ON CONFLICT DO NOTHING;

-- finances:update
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur','intendant')
  AND p.name = 'finances:update'
ON CONFLICT DO NOTHING;

-- finances:delete
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur')
  AND p.name = 'finances:delete'
ON CONFLICT DO NOTHING;

-- personnel:read
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','fondateur','admin_it','directeur','censeur','chef_departement','enseignant')
  AND p.name = 'personnel:read'
ON CONFLICT DO NOTHING;

-- personnel:create
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur','admin_it')
  AND p.name = 'personnel:create'
ON CONFLICT DO NOTHING;

-- personnel:update
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur','admin_it','enseignant')
  AND p.name = 'personnel:update'
ON CONFLICT DO NOTHING;

-- personnel:delete
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur')
  AND p.name = 'personnel:delete'
ON CONFLICT DO NOTHING;

-- communication:read
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','fondateur','directeur','censeur','intendant','enseignant','personnel_admin','parent','eleve')
  AND p.name = 'communication:read'
ON CONFLICT DO NOTHING;

-- communication:create
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur','censeur','intendant','enseignant','personnel_admin')
  AND p.name = 'communication:create'
ON CONFLICT DO NOTHING;

-- communication:update
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur')
  AND p.name = 'communication:update'
ON CONFLICT DO NOTHING;

-- communication:delete
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur')
  AND p.name = 'communication:delete'
ON CONFLICT DO NOTHING;

-- bde:read
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','fondateur','directeur','responsable_bde','eleve')
  AND p.name = 'bde:read'
ON CONFLICT DO NOTHING;

-- bde:create
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur','responsable_bde')
  AND p.name = 'bde:create'
ON CONFLICT DO NOTHING;

-- bde:update
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur','responsable_bde')
  AND p.name = 'bde:update'
ON CONFLICT DO NOTHING;

-- bde:delete
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur')
  AND p.name = 'bde:delete'
ON CONFLICT DO NOTHING;

-- documents:read
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','fondateur','admin_it','directeur','censeur','intendant','enseignant','personnel_admin','parent')
  AND p.name = 'documents:read'
ON CONFLICT DO NOTHING;

-- documents:create
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','admin_it','directeur','censeur','intendant','personnel_admin')
  AND p.name = 'documents:create'
ON CONFLICT DO NOTHING;

-- documents:update
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','admin_it','directeur')
  AND p.name = 'documents:update'
ON CONFLICT DO NOTHING;

-- documents:delete
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur')
  AND p.name = 'documents:delete'
ON CONFLICT DO NOTHING;

-- services-premium:read
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','fondateur','directeur','intendant','parent')
  AND p.name = 'services-premium:read'
ON CONFLICT DO NOTHING;

-- services-premium:create
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur','intendant')
  AND p.name = 'services-premium:create'
ON CONFLICT DO NOTHING;

-- services-premium:update
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur','intendant')
  AND p.name = 'services-premium:update'
ON CONFLICT DO NOTHING;

-- services-premium:delete
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','directeur')
  AND p.name = 'services-premium:delete'
ON CONFLICT DO NOTHING;

-- support:read
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','fondateur','admin_it','directeur','censeur','intendant','enseignant','personnel_admin','responsable_bde','parent','eleve')
  AND p.name = 'support:read'
ON CONFLICT DO NOTHING;

-- support:create
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','admin_it','directeur','censeur','intendant','enseignant','personnel_admin','responsable_bde','parent','eleve')
  AND p.name = 'support:create'
ON CONFLICT DO NOTHING;

-- support:update
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','admin_it','directeur')
  AND p.name = 'support:update'
ON CONFLICT DO NOTHING;

-- support:delete
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','admin_it')
  AND p.name = 'support:delete'
ON CONFLICT DO NOTHING;

-- support:manage
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','admin_it','directeur')
  AND p.name = 'support:manage'
ON CONFLICT DO NOTHING;

-- users:manage
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','admin_it')
  AND p.name = 'users:manage'
ON CONFLICT DO NOTHING;

-- cache:reload
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','admin_it')
  AND p.name = 'cache:reload'
ON CONFLICT DO NOTHING;

-- settings:manage
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','admin_it','directeur')
  AND p.name = 'settings:manage'
ON CONFLICT DO NOTHING;

-- audit:read
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','fondateur','admin_it','directeur')
  AND p.name = 'audit:read'
ON CONFLICT DO NOTHING;
