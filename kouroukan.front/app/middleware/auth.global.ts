import { useAuthStore } from '~/core/stores/auth.store'
import { isPublicRoute, getRequiredPermission } from '~/core/auth/guards'

/** Roles that belong to the establishment space */
const ESTABLISHMENT_ROLES = [
  'fondateur', 'admin_it', 'directeur', 'censeur', 'intendant',
  'responsable_admissions', 'chef_departement', 'personnel_admin', 'responsable_bde',
] as const

/** Shared routes accessible to all authenticated users */
const SHARED_ROUTES = ['/', '/profil', '/changer-mot-de-passe', '/onboarding']

function isSharedRoute(path: string): boolean {
  return SHARED_ROUTES.some(r => path === r)
}

/**
 * Returns the home route for a given user based on their primary role.
 */
function getHomeRoute(auth: ReturnType<typeof useAuthStore>): string {
  if (auth.hasRole('super_admin')) return '/admin'
  if (auth.hasRole('enseignant')) return '/enseignant'
  if (auth.hasRole('parent') || auth.hasRole('eleve')) return '/famille'
  return '/'
}

/**
 * Checks if the user is allowed to access a route based on their role space.
 */
function isInCorrectSpace(rawPath: string, auth: ReturnType<typeof useAuthStore>): boolean {
  // Super admin → only /admin (and shared routes)
  if (auth.hasRole('super_admin')) {
    return rawPath.startsWith('/admin') || isSharedRoute(rawPath)
  }

  // Enseignant → only /enseignant (and shared routes)
  if (auth.hasRole('enseignant')) {
    return rawPath.startsWith('/enseignant') || isSharedRoute(rawPath)
  }

  // Parent/Eleve → only /famille (and shared routes)
  if (auth.hasRole('parent') || auth.hasRole('eleve')) {
    return rawPath.startsWith('/famille') || isSharedRoute(rawPath)
  }

  // Establishment roles → NOT /admin, /enseignant, /famille
  if (ESTABLISHMENT_ROLES.some(r => auth.hasRole(r))) {
    return !rawPath.startsWith('/admin') && !rawPath.startsWith('/enseignant') && !rawPath.startsWith('/famille')
  }

  return true
}

export default defineNuxtRouteMiddleware((to) => {
  // Ne pas executer les checks cote serveur (pas de state Pinia persistee en SSR)
  if (import.meta.server) return

  const auth = useAuthStore()
  const localePath = useLocalePath()

  // Normaliser le path (supprimer le prefixe de locale /en/, /fr/)
  const rawPath = to.path.replace(/^\/[a-z]{2}(?=\/|$)/, '') || '/'

  // Public routes — skip auth
  if (isPublicRoute(rawPath)) return

  // Not authenticated → redirect to login
  if (!auth.isAuthenticated) {
    return navigateTo(localePath('/connexion'))
  }

  // Must change password → redirect to change password page
  if (auth.mustChangePassword && rawPath !== '/changer-mot-de-passe') {
    return navigateTo(localePath('/changer-mot-de-passe'))
  }

  // Dashboard root → redirect to correct home based on role
  if (rawPath === '/') {
    const home = getHomeRoute(auth)
    if (home !== '/') {
      return navigateTo(localePath(home))
    }
    return
  }

  // Check if user is in the correct space
  if (!isInCorrectSpace(rawPath, auth)) {
    const home = getHomeRoute(auth)
    return navigateTo(localePath(home))
  }

  // Check required permission for the route
  const requiredPermission = getRequiredPermission(rawPath)
  if (requiredPermission && !auth.hasPermission(requiredPermission)) {
    try {
      const { $i18n } = useNuxtApp()
      const t = $i18n.t.bind($i18n)
      useToast().add({
        title: t('errors.forbidden'),
        description: t('errors.insufficientPermissions'),
        color: 'error',
      })
    }
    catch {
      // toast non disponible
    }
    const home = getHomeRoute(auth)
    return navigateTo(localePath(home))
  }
})
