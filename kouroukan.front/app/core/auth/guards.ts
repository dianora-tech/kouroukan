import type { PermissionKey } from './rbac'

/**
 * Map of route patterns to their required permission.
 * Built dynamically from project modules.
 */
export const ROUTE_PERMISSIONS: Record<string, PermissionKey> = {
  '/inscriptions': 'inscriptions:read',
  '/pedagogie': 'pedagogie:read',
  '/evaluations': 'evaluations:read',
  '/presences': 'presences:read',
  '/finances': 'finances:read',
  '/personnel': 'personnel:read',
  '/communication': 'communication:read',
  '/bde': 'bde:read',
  '/documents': 'documents:read',
  '/services-premium': 'services-premium:read',
  '/support': 'support:read',
  '/parametres/utilisateurs': 'users:manage',
}

/** Routes accessible without authentication */
export const PUBLIC_ROUTES = [
  '/connexion',
  '/inscription',
  '/legal/cgu',
] as const

/** Routes accessible even if CGU not accepted */
export const CGU_EXEMPT_ROUTES = [
  '/support/cgu',
  '/connexion',
  '/inscription',
  '/legal/cgu',
  '/changer-mot-de-passe',
] as const

/**
 * Finds the required permission for a given route path.
 * Matches the most specific prefix.
 */
export function getRequiredPermission(path: string): PermissionKey | null {
  // Strip locale prefix (e.g. /en/inscriptions → /inscriptions)
  const cleanPath = path.replace(/^\/(fr|en)/, '') || '/'

  // Exact match first
  if (cleanPath in ROUTE_PERMISSIONS) {
    return ROUTE_PERMISSIONS[cleanPath]
  }

  // Prefix match — find longest matching prefix
  let bestMatch: string | null = null
  for (const pattern of Object.keys(ROUTE_PERMISSIONS)) {
    if (cleanPath.startsWith(pattern + '/') || cleanPath === pattern) {
      if (!bestMatch || pattern.length > bestMatch.length) {
        bestMatch = pattern
      }
    }
  }

  return bestMatch ? ROUTE_PERMISSIONS[bestMatch] : null
}

/**
 * Checks if a route is publicly accessible.
 */
export function isPublicRoute(path: string): boolean {
  const cleanPath = path.replace(/^\/(fr|en)/, '') || '/'
  return PUBLIC_ROUTES.some(route => cleanPath === route || cleanPath.startsWith(route + '/'))
}

/**
 * Checks if a route is exempt from CGU check.
 */
export function isCguExemptRoute(path: string): boolean {
  const cleanPath = path.replace(/^\/(fr|en)/, '') || '/'
  return CGU_EXEMPT_ROUTES.some(route => cleanPath === route || cleanPath.startsWith(route + '/'))
}
