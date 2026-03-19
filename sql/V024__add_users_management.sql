-- ============================================================================
-- V024__add_users_management.sql
-- Kouroukan - Gestion des comptes utilisateurs par le directeur
-- ============================================================================

-- 1. Ajouter must_change_password pour les comptes crees par un admin
ALTER TABLE auth.users
    ADD COLUMN IF NOT EXISTS must_change_password BOOLEAN NOT NULL DEFAULT FALSE;

-- 2. Donner la permission users:manage au role directeur
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name = 'directeur'
  AND p.name = 'users:manage'
ON CONFLICT DO NOTHING;
