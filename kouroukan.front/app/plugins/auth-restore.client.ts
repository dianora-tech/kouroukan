import { useAuthStore } from '~/core/stores/auth.store'

/**
 * Plugin client-side qui restaure le profil utilisateur au chargement.
 *
 * Les tokens sont persistés en cookies (accessibles SSR + client).
 * Le profil (user, roles, permissions) est persisté en localStorage (client only).
 *
 * Si on a un token valide mais pas de profil (ex: cookie présent mais
 * localStorage vidé), on recharge le profil depuis l'API.
 */
export default defineNuxtPlugin({
  name: 'auth-restore',
  parallel: false,
  async setup() {
    const auth = useAuthStore()

    // Migration : supprimer l'ancien cookie unique "auth" (trop volumineux)
    if (document.cookie.includes('auth=')) {
      document.cookie = 'auth=; max-age=0; path=/'
    }

    // Si on a un token mais pas de user, recharger le profil
    if (auth.accessToken && !auth.user) {
      try {
        const response = await $fetch<{
          success: boolean
          data: {
            id: number
            firstName: string
            lastName: string
            email: string
            roles: string[]
            permissions: string[]
            cguVersion: string | null
            cguAcceptedAt: string | null
            mustChangePassword: boolean
            onboardingStep: number
            onboardingCompletedAt: string | null
            avatarUrl: string | null
            companies: unknown[]
            preferredLocale: string | null
            preferredTheme: string | null
          }
        }>('/api/auth/me', {
          headers: {
            Authorization: `Bearer ${auth.accessToken}`,
          },
        })

        if (response?.success && response.data) {
          const user = response.data
          auth.user = user as any
          auth.roles = (user.roles ?? []) as any
          auth.permissions = (user.permissions ?? []) as any
          auth.cguVersion = user.cguVersion ?? null
          auth.cguAccepted = !!user.cguAcceptedAt
          auth.mustChangePassword = user.mustChangePassword ?? false
          auth.onboardingStep = user.onboardingStep ?? 0
          auth.onboardingCompleted = !!user.onboardingCompletedAt
        }
      }
      catch {
        // Token invalide ou expiré → reset
        auth.$reset()
      }
    }
  },
})
