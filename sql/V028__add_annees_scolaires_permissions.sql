-- ============================================================================
-- V028__add_annees_scolaires_permissions.sql
-- Kouroukan - Permissions dediees aux annees scolaires
-- ============================================================================

-- ============================================================================
-- PERMISSIONS
-- ============================================================================
INSERT INTO auth.permissions (name, description, module) VALUES
    ('annees-scolaires:read',       'Lecture des annees scolaires',          'inscriptions'),
    ('annees-scolaires:create',     'Creation d''annees scolaires',          'inscriptions'),
    ('annees-scolaires:update',     'Modification d''annees scolaires',      'inscriptions'),
    ('annees-scolaires:delete',     'Suppression d''annees scolaires',       'inscriptions')
ON CONFLICT DO NOTHING;

-- ============================================================================
-- MAPPING ROLES → PERMISSIONS
-- ============================================================================

-- annees-scolaires:read
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin','fondateur','admin_it','directeur','censeur','responsable_admissions','personnel_admin')
  AND p.name = 'annees-scolaires:read'
ON CONFLICT DO NOTHING;

-- annees-scolaires:create
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin')
  AND p.name = 'annees-scolaires:create'
ON CONFLICT DO NOTHING;

-- annees-scolaires:update
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin')
  AND p.name = 'annees-scolaires:update'
ON CONFLICT DO NOTHING;

-- annees-scolaires:delete
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin')
  AND p.name = 'annees-scolaires:delete'
ON CONFLICT DO NOTHING;
