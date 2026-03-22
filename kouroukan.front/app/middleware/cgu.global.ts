import { useAuthStore } from '~/core/stores/auth.store'
import { isCguExemptRoute } from '~/core/auth/guards'

export default defineNuxtRouteMiddleware(async (to) => {
  const auth = useAuthStore()
  const toast = useToast()
  const { $i18n } = useNuxtApp()
  const t = $i18n.t.bind($i18n)

  // 1. Not authenticated → skip (auth middleware handles it)
  if (!auth.isAuthenticated) return

  // 2. CGU-exempt routes → always accessible
  if (isCguExemptRoute(to.path)) return

  // 3. Check if CGU are up to date
  const isUpToDate = await auth.checkCgu()
  if (!isUpToDate) {
    toast.add({
      title: t('cgu.required'),
      description: t('cgu.mustAccept'),
      color: 'warning',
    })
    const localePath = useLocalePath()
    return navigateTo(localePath('/support/cgu'))
  }
})
