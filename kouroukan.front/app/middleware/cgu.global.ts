import { useAuthStore } from '~/core/stores/auth.store'
import { isCguExemptRoute } from '~/core/auth/guards'

// Cache client-side pour eviter de re-verifier les CGU a chaque navigation
let lastCguCheck: { result: boolean, timestamp: number } | null = null
const CGU_CHECK_TTL_MS = 5 * 60 * 1000 // 5 minutes

export default defineNuxtRouteMiddleware(async (to) => {
  // Ne pas executer cote serveur (pas de token disponible en SSR)
  if (import.meta.server) return

  const auth = useAuthStore()

  // 1. Not authenticated → skip (auth middleware handles it)
  if (!auth.isAuthenticated) return

  // 2. CGU-exempt routes → always accessible
  if (isCguExemptRoute(to.path)) return

  // 3. Si les CGU sont deja acceptees dans le store, pas besoin de verifier
  if (auth.cguAccepted) return

  // 4. Utiliser le cache pour eviter les appels reseau repetitifs
  const now = Date.now()
  if (lastCguCheck && (now - lastCguCheck.timestamp) < CGU_CHECK_TTL_MS) {
    if (lastCguCheck.result) return
  }

  // 5. Check if CGU are up to date (appel reseau)
  const isUpToDate = await auth.checkCgu()
  lastCguCheck = { result: isUpToDate, timestamp: now }

  if (!isUpToDate) {
    const toast = useToast()
    const { $i18n } = useNuxtApp()
    const t = $i18n.t.bind($i18n)
    toast.add({
      title: t('cgu.required'),
      description: t('cgu.mustAccept'),
      color: 'warning',
    })
    const localePath = useLocalePath()
    return navigateTo(localePath('/support/cgu'))
  }
})
