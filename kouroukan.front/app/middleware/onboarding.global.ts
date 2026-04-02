import { useAuthStore } from '~/core/stores/auth.store'
import { isOnboardingExemptRoute } from '~/core/auth/guards'

export default defineNuxtRouteMiddleware((to) => {
  if (import.meta.server) return

  const auth = useAuthStore()

  // Not authenticated → skip (auth middleware handles it)
  if (!auth.isAuthenticated) return

  // Only redirect directors (owners) who haven't completed onboarding
  if (auth.onboardingCompleted) return

  // Exempt routes → always accessible
  if (isOnboardingExemptRoute(to.path)) return

  // Only enforce onboarding for directors
  if (!auth.roles.includes('directeur')) return

  // Dashboard (/) is accessible — shows onboarding banner instead of blocking
  const rawPath = to.path.replace(/^\/[a-z]{2}(?=\/|$)/, '') || '/'
  if (rawPath === '/') return

  // Director with incomplete onboarding → redirect to /onboarding
  const localePath = useLocalePath()
  return navigateTo(localePath('/onboarding'))
})
