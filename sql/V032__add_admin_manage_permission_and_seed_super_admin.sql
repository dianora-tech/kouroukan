-- ============================================================================
-- V032 : Ajouter la permission admin:manage et creer le super admin
-- ============================================================================
-- La permission admin:manage est requise par le AdminController.
-- Le super admin (admin@kouroukan.gn) est le compte initial de la plateforme.
-- ============================================================================

-- 1. Creer la permission admin:manage si elle n'existe pas
INSERT INTO auth.permissions (name, description, module)
VALUES ('admin:manage', 'Gestion de la plateforme (administration globale)', 'admin')
ON CONFLICT DO NOTHING;

-- 2. Attribuer admin:manage aux roles super_admin et admin_it
INSERT INTO auth.role_permissions (role_id, permission_id)
SELECT r.id, p.id FROM auth.roles r, auth.permissions p
WHERE r.name IN ('super_admin', 'admin_it')
  AND p.name = 'admin:manage'
ON CONFLICT DO NOTHING;

-- 3. Creer le compte super admin initial
-- Le mot de passe est un placeholder volontaire : 'CHANGE_ME_AT_DEPLOY'.
-- Il sera remplace au deploiement par le script scripts/init-super-admin.sh
-- qui genere un vrai hash Argon2id (format {salt_base64}:{hash_base64}).
--
-- Le script est idempotent : il ne met a jour QUE les lignes dont le
-- password_hash vaut exactement 'CHANGE_ME_AT_DEPLOY', donc il est sans
-- danger de le relancer plusieurs fois et il n'ecrase jamais un hash reel.
--
-- IMPORTANT : Ne PAS deployer en production sans executer le script d'init.
--
-- Note : ON CONFLICT (email) requiert un index unique non-partiel sur email.
-- V022 ne cree qu'un index partiel (WHERE is_deleted = FALSE), donc on
-- insere conditionnellement via un DO block.
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM auth.users WHERE email = 'admin@kouroukan.gn') THEN
        INSERT INTO auth.users (
            email, password_hash, first_name, last_name,
            is_active, type_compte, identifiant_unique,
            cgu_accepted_at, cgu_version,
            created_at, created_by
        )
        VALUES (
            'admin@kouroukan.gn',
            -- Placeholder hash — sera regenere au deploiement via script d'init
            'CHANGE_ME_AT_DEPLOY',
            'Admin', 'Kouroukan',
            TRUE, 'super_admin', 'SUPER-ADMIN-001',
            NOW(), '1.0',
            NOW(), 'system'
        );
    END IF;
END
$$;

-- 4. Attribuer le role super_admin a l'utilisateur admin@kouroukan.gn
INSERT INTO auth.user_roles (user_id, role_id, created_at, created_by)
SELECT u.id, r.id, NOW(), 'system'
FROM auth.users u, auth.roles r
WHERE u.email = 'admin@kouroukan.gn'
  AND r.name = 'super_admin'
ON CONFLICT DO NOTHING;
