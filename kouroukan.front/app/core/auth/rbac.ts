export const ROLES = [
  'super_admin',
  'fondateur',
  'admin_it',
  'directeur',
  'censeur',
  'intendant',
  'responsable_admissions',
  'chef_departement',
  'enseignant',
  'personnel_admin',
  'responsable_bde',
  'parent',
  'eleve',
] as const

export type RoleName = (typeof ROLES)[number]

export const PERMISSIONS = {
  // Module inscriptions
  'inscriptions:read': ['super_admin', 'fondateur', 'admin_it', 'directeur', 'censeur', 'responsable_admissions', 'personnel_admin'],
  'inscriptions:create': ['super_admin', 'directeur', 'responsable_admissions', 'personnel_admin'],
  'inscriptions:update': ['super_admin', 'directeur', 'responsable_admissions', 'personnel_admin'],
  'inscriptions:delete': ['super_admin', 'directeur'],

  // Annees scolaires (CRUD reserve au super_admin)
  'annees-scolaires:read': ['super_admin', 'fondateur', 'admin_it', 'directeur', 'censeur', 'responsable_admissions', 'personnel_admin'],
  'annees-scolaires:create': ['super_admin'],
  'annees-scolaires:update': ['super_admin'],
  'annees-scolaires:delete': ['super_admin'],

  // Module pedagogie
  'pedagogie:read': ['super_admin', 'fondateur', 'admin_it', 'directeur', 'censeur', 'chef_departement', 'enseignant'],
  'pedagogie:create': ['super_admin', 'directeur', 'censeur'],
  'pedagogie:update': ['super_admin', 'directeur', 'censeur', 'chef_departement'],
  'pedagogie:delete': ['super_admin', 'directeur'],

  // Module evaluations
  'evaluations:read': ['super_admin', 'fondateur', 'directeur', 'censeur', 'chef_departement', 'enseignant', 'parent', 'eleve'],
  'evaluations:create': ['super_admin', 'directeur', 'censeur', 'enseignant'],
  'evaluations:update': ['super_admin', 'directeur', 'censeur', 'enseignant'],
  'evaluations:delete': ['super_admin', 'directeur', 'censeur'],

  // Module presences
  'presences:read': ['super_admin', 'fondateur', 'directeur', 'censeur', 'enseignant', 'parent'],
  'presences:create': ['super_admin', 'directeur', 'censeur', 'enseignant'],
  'presences:update': ['super_admin', 'directeur', 'censeur', 'enseignant'],
  'presences:delete': ['super_admin', 'directeur'],

  // Module finances
  'finances:read': ['super_admin', 'fondateur', 'directeur', 'intendant', 'parent'],
  'finances:create': ['super_admin', 'directeur', 'intendant'],
  'finances:update': ['super_admin', 'directeur', 'intendant'],
  'finances:delete': ['super_admin', 'directeur'],

  // Module personnel
  'personnel:read': ['super_admin', 'fondateur', 'admin_it', 'directeur', 'censeur', 'chef_departement', 'enseignant'],
  'personnel:create': ['super_admin', 'directeur', 'admin_it'],
  'personnel:update': ['super_admin', 'directeur', 'admin_it', 'enseignant'],
  'personnel:delete': ['super_admin', 'directeur'],

  // Module communication
  'communication:read': ['super_admin', 'fondateur', 'directeur', 'censeur', 'intendant', 'enseignant', 'personnel_admin', 'parent', 'eleve'],
  'communication:create': ['super_admin', 'directeur', 'censeur', 'intendant', 'enseignant', 'personnel_admin'],
  'communication:update': ['super_admin', 'directeur'],
  'communication:delete': ['super_admin', 'directeur'],

  // Module BDE
  'bde:read': ['super_admin', 'fondateur', 'directeur', 'responsable_bde', 'eleve'],
  'bde:create': ['super_admin', 'directeur', 'responsable_bde'],
  'bde:update': ['super_admin', 'directeur', 'responsable_bde'],
  'bde:delete': ['super_admin', 'directeur'],

  // Module documents
  'documents:read': ['super_admin', 'fondateur', 'admin_it', 'directeur', 'censeur', 'intendant', 'enseignant', 'personnel_admin', 'parent'],
  'documents:create': ['super_admin', 'admin_it', 'directeur', 'censeur', 'intendant', 'personnel_admin'],
  'documents:update': ['super_admin', 'admin_it', 'directeur'],
  'documents:delete': ['super_admin', 'directeur'],

  // Module services-premium
  'services-premium:read': ['super_admin', 'fondateur', 'directeur', 'intendant', 'parent'],
  'services-premium:create': ['super_admin', 'directeur', 'intendant'],
  'services-premium:update': ['super_admin', 'directeur', 'intendant'],
  'services-premium:delete': ['super_admin', 'directeur'],

  // Module support
  'support:read': ['super_admin', 'fondateur', 'admin_it', 'directeur', 'censeur', 'intendant', 'enseignant', 'personnel_admin', 'responsable_bde', 'parent', 'eleve'],
  'support:create': ['super_admin', 'admin_it', 'directeur', 'censeur', 'intendant', 'enseignant', 'personnel_admin', 'responsable_bde', 'parent', 'eleve'],
  'support:update': ['super_admin', 'admin_it', 'directeur'],
  'support:delete': ['super_admin', 'admin_it'],
  'support:manage': ['super_admin', 'admin_it', 'directeur'],

  // Administration
  'users:manage': ['super_admin', 'admin_it', 'directeur'],
  'cache:reload': ['super_admin', 'admin_it'],
  'settings:manage': ['super_admin', 'admin_it', 'directeur'],
  'audit:read': ['super_admin', 'fondateur', 'admin_it', 'directeur'],
} as const satisfies Record<string, readonly RoleName[]>

export type PermissionKey = keyof typeof PERMISSIONS
