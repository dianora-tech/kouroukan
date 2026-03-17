import { useAuthStore } from '~/core/stores/auth.store'
import { isPublicRoute, getRequiredPermission } from '~/core/auth/guards'

export default defineNuxtRouteMiddleware((to) => {
  const auth = useAuthStore()
  const { $i18n } = useNuxtApp()
  const t = $i18n.t.bind($i18n)
  const toast = useToast()
  const path = to.path

  // Public routes — skip auth
  if (isPublicRoute(path)) return

  // Dashboard root — always accessible to authenticated users
  if (path === '/' || path === '/fr' || path === '/en') {
    if (!auth.isAuthenticated) {
      return navigateTo('/connexion')
    }
    return
  }

  // Not authenticated → redirect to login
  if (!auth.isAuthenticated) {
    return navigateTo('/connexion')
  }

  // Check required permission for the route
  const requiredPermission = getRequiredPermission(path)
  if (requiredPermission && !auth.hasPermission(requiredPermission)) {
    toast.add({
      title: t('errors.forbidden'),
      description: t('errors.insufficientPermissions'),
      color: 'error',
    })
    return navigateTo('/')
  }
})
