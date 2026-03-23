import { useAuthStore } from '~/core/stores/auth.store'
import { isPublicRoute, getRequiredPermission } from '~/core/auth/guards'

export default defineNuxtRouteMiddleware((to) => {
  // Ne pas executer les checks cote serveur (pas de state Pinia persistee en SSR)
  if (import.meta.server) return

  const auth = useAuthStore()
  const localePath = useLocalePath()

  // Normaliser le path (supprimer le prefixe de locale /en/, /fr/)
  const rawPath = to.path.replace(/^\/[a-z]{2}(?=\/|$)/, '') || '/'

  // Public routes — skip auth
  if (isPublicRoute(rawPath)) return

  // Dashboard root — always accessible to authenticated users
  if (rawPath === '/') {
    if (!auth.isAuthenticated) {
      return navigateTo(localePath('/connexion'))
    }
    return
  }

  // Not authenticated → redirect to login
  if (!auth.isAuthenticated) {
    return navigateTo(localePath('/connexion'))
  }

  // Must change password → redirect to change password page
  if (auth.mustChangePassword && rawPath !== '/changer-mot-de-passe') {
    return navigateTo(localePath('/changer-mot-de-passe'))
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
    return navigateTo(localePath('/'))
  }
})
