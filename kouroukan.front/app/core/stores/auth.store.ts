import { defineStore } from 'pinia'
import type { User } from '~/types/user'
import type { RoleName, PermissionKey } from '~/core/auth/rbac'
import { extractErrorMessage } from '~/core/api/client'

function showError(msg: string): void {
  try {
    useToast().add({ title: msg, color: 'error', icon: 'i-heroicons-exclamation-triangle' })
  }
  catch {
    // toast non disponible (SSR)
  }
}

/**
 * Mutex pour éviter les refresh concurrents.
 * Quand plusieurs requêtes reçoivent 401 simultanément,
 * seule la première lance le refresh, les autres attendent.
 */
let refreshPromise: Promise<boolean> | null = null

async function doRefresh(): Promise<boolean> {
  const auth = useAuthStore()
  if (!auth.refreshToken) return false

  try {
    const response = await $fetch<{
      success: boolean
      data: { accessToken: string, refreshToken: string }
    }>('/api/auth/refresh', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: { refreshToken: auth.refreshToken },
    })

    if (response?.success && response.data?.accessToken) {
      auth.accessToken = response.data.accessToken
      if (response.data.refreshToken) {
        auth.refreshToken = response.data.refreshToken
      }
      return true
    }
  }
  catch {
    // Refresh echoue
  }
  return false
}

/**
 * $fetch authentifie : ajoute le Bearer token automatiquement.
 * Si 401, tente un refresh (avec mutex) puis re-essaie une fois.
 */
export async function authFetch<T>(
  url: string,
  options: { method?: string, body?: unknown, headers?: Record<string, string> } = {},
): Promise<T> {
  const auth = useAuthStore()

  const makeHeaders = (): Record<string, string> => ({
    'Content-Type': 'application/json',
    ...(auth.accessToken ? { Authorization: `Bearer ${auth.accessToken}` } : {}),
    ...(options.headers ?? {}),
  })

  try {
    return await $fetch<T>(url, {
      method: (options.method as 'POST' | 'PUT' | 'GET' | 'DELETE') ?? 'GET',
      headers: makeHeaders(),
      body: options.body,
    })
  }
  catch (error: unknown) {
    const status = (error as { statusCode?: number })?.statusCode
    if (status !== 401) throw error

    // Mutex : si un refresh est déjà en cours, attendre son résultat
    if (!refreshPromise) {
      refreshPromise = doRefresh().finally(() => {
        refreshPromise = null
      })
    }

    const refreshed = await refreshPromise

    if (refreshed) {
      // Re-essayer avec le nouveau token
      return await $fetch<T>(url, {
        method: (options.method as 'POST' | 'PUT' | 'GET' | 'DELETE') ?? 'GET',
        headers: makeHeaders(),
        body: options.body,
      })
    }

    // Deconnecter l'utilisateur
    auth.$reset()
    await navigateTo('/connexion', { replace: true })
    throw error
  }
}

interface AuthState {
  user: User | null
  roles: RoleName[]
  permissions: PermissionKey[]
  lastLoginAt: string | null
  cguAccepted: boolean
  cguVersion: string | null
  mustChangePassword: boolean
  activeCompanyId: number | null
  accessToken: string | null
  refreshToken: string | null
  onboardingStep: number
  onboardingCompleted: boolean
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    user: null,
    roles: [],
    permissions: [],
    lastLoginAt: null,
    cguAccepted: false,
    cguVersion: null,
    mustChangePassword: false,
    activeCompanyId: null,
    accessToken: null,
    refreshToken: null,
    onboardingStep: 0,
    onboardingCompleted: false,
  }),

  getters: {
    isAuthenticated: (state): boolean => !!state.user,
    isAdmin: (state): boolean =>
      state.roles.includes('super_admin') || state.roles.includes('admin_it'),
    currentUserId: (state): number | null => state.user?.id ?? null,
    isCguUpToDate(): boolean {
      const config = useRuntimeConfig()
      return this.cguVersion === config.public.cguVersion
    },
    /** Modules souscrits par l'etablissement actif. */
    activeCompanyModules(): string[] {
      if (!this.user?.companies?.length) return []
      const company = this.activeCompanyId
        ? this.user.companies.find(c => c.id === this.activeCompanyId)
        : this.user.companies[0]
      return company?.modules ?? []
    },
  },

  actions: {
    async login(email: string, password: string, turnstileToken?: string | null): Promise<void> {
      let loginResponse: { success: boolean, data: { accessToken: string, refreshToken: string } }
      try {
        // 1. Login — get tokens
        loginResponse = await $fetch<{
          success: boolean
          data: {
            accessToken: string
            refreshToken: string
          }
        }>('/api/auth/login', {
          method: 'POST',
          body: { email, password, turnstileToken: turnstileToken ?? undefined },
        })
      }
      catch (error) {
        const msg = extractErrorMessage(error)
        showError(msg)
        throw error
      }

      if (!loginResponse?.success || !loginResponse.data?.accessToken) {
        const msg = 'Identifiants incorrects.'
        showError(msg)
        throw new Error(msg)
      }

      // Store tokens in Pinia state (persisted) and cookie
      const token = loginResponse.data.accessToken
      this.accessToken = token
      this.refreshToken = loginResponse.data.refreshToken ?? null
      try {
        const tokenCookie = useCookie('auth.token')
        tokenCookie.value = token
      }
      catch {
        // cookie not available
      }

      // 2. Fetch user profile with the token
      const profileResponse = await $fetch<{
        success: boolean
        data: User
      }>('/api/auth/me', {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })

      if (profileResponse?.success && profileResponse.data) {
        const user = profileResponse.data
        this.user = user
        this.roles = (user.roles ?? []) as RoleName[]
        this.permissions = (user.permissions ?? []) as PermissionKey[]
        this.lastLoginAt = new Date().toISOString()
        this.cguVersion = user.cguVersion ?? null
        this.cguAccepted = !!user.cguAcceptedAt
        this.mustChangePassword = user.mustChangePassword ?? false
        this.onboardingStep = user.onboardingStep ?? 0
        this.onboardingCompleted = !!user.onboardingCompletedAt
      }
    },

    async logout(): Promise<void> {
      try {
        await $fetch('/api/auth/logout', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            ...(this.accessToken ? { Authorization: `Bearer ${this.accessToken}` } : {}),
          },
          body: {},
        })
      }
      catch {
        // Ignore logout errors
      }
      finally {
        // Nettoyage local
        try {
          const tokenCookie = useCookie('auth.token')
          tokenCookie.value = null
          const authTokensCookie = useCookie('auth-tokens')
          authTokensCookie.value = null
        }
        catch {
          // cookie non disponible
        }
        // Nettoyer localStorage
        if (import.meta.client) {
          try {
            localStorage.removeItem('auth-profile')
          }
          catch {
            // noop
          }
        }
        this.$reset()
        await navigateTo('/connexion', { replace: true })
      }
    },

    async refreshProfile(): Promise<void> {
      const response = await $fetch<{
        success: boolean
        data: User
      }>('/api/auth/me')

      if (response?.success && response.data) {
        const user = response.data
        this.user = user
        this.roles = (user.roles ?? []) as RoleName[]
        this.permissions = (user.permissions ?? []) as PermissionKey[]
        this.cguVersion = user.cguVersion ?? null
        this.cguAccepted = !!user.cguAcceptedAt
        this.mustChangePassword = user.mustChangePassword ?? false
        this.onboardingStep = user.onboardingStep ?? 0
        this.onboardingCompleted = !!user.onboardingCompletedAt
      }
    },

    async changePassword(currentPassword: string, newPassword: string): Promise<void> {
      try {
        await authFetch('/api/auth/change-password', {
          method: 'POST',
          body: { currentPassword, newPassword },
        })
        this.mustChangePassword = false
      }
      catch (error) {
        const msg = extractErrorMessage(error)
        showError(msg)
        throw error
      }
    },

    async fetchPermissions(): Promise<void> {
      const response = await $fetch<{
        success: boolean
        data: { permissions: string[] }
      }>('/api/auth/permissions')

      if (response?.success && response.data) {
        this.permissions = response.data.permissions as PermissionKey[]
      }
    },

    async checkCgu(): Promise<boolean> {
      try {
        const data = await $fetch<{
          success: boolean
          data: { version: string }
        }>('/api/auth/cgu/active')

        if (data?.success && data.data) {
          return this.cguVersion === data.data.version
        }
      }
      catch {
        // Fallback to local config check
        const config = useRuntimeConfig()
        return this.cguVersion === config.public.cguVersion
      }
      return false
    },

    async acceptCgu(version: string): Promise<void> {
      try {
        const response = await authFetch<{
          success: boolean
          data: {
            accessToken: string
            refreshToken: string
          }
        }>('/api/auth/cgu/accept', {
          method: 'POST',
        })

        // Update tokens with new cguVersion claim
        if (response?.success && response.data?.accessToken) {
          this.accessToken = response.data.accessToken
          if (response.data.refreshToken) {
            this.refreshToken = response.data.refreshToken
          }
          try {
            const tokenCookie = useCookie('auth.token')
            tokenCookie.value = response.data.accessToken
          }
          catch {
            // cookie non disponible
          }
        }

        this.cguVersion = version
        this.cguAccepted = true
      }
      catch (error) {
        const msg = extractErrorMessage(error)
        showError(msg)
        throw error
      }
    },

    async updatePreferences(locale: string, theme: string): Promise<void> {
      try {
        await authFetch('/api/auth/preferences', {
          method: 'PUT',
          body: { locale, theme },
        })
        if (this.user) {
          this.user.preferredLocale = locale
          this.user.preferredTheme = theme
        }
      }
      catch (error) {
        const msg = extractErrorMessage(error)
        showError(msg)
        throw error
      }
    },

    hasPermission(permission: PermissionKey): boolean {
      if (this.roles.includes('super_admin')) return true
      return this.permissions.includes(permission)
    },

    hasRole(role: RoleName): boolean {
      return this.roles.includes(role)
    },

    /** Verifie si le module est souscrit par l'etablissement actif. */
    hasModule(slug: string): boolean {
      // Super admins ont acces a tout
      if (this.roles.includes('super_admin')) return true
      // Support est toujours accessible
      if (slug === 'support') return true
      return this.activeCompanyModules.includes(slug)
    },

    async uploadAvatar(file: File): Promise<string> {
      const formData = new FormData()
      formData.append('file', file)

      try {
        const response = await $fetch<{
          success: boolean
          data: { avatarUrl: string }
          message: string
        }>('/api/auth/avatar', {
          method: 'POST',
          headers: {
            ...(this.accessToken ? { Authorization: `Bearer ${this.accessToken}` } : {}),
          },
          body: formData,
        })

        if (response?.success && response.data?.avatarUrl) {
          if (this.user) {
            this.user.avatarUrl = response.data.avatarUrl
          }
          return response.data.avatarUrl
        }
        throw new Error('Upload echoue')
      }
      catch (error) {
        const msg = extractErrorMessage(error)
        showError(msg)
        throw error
      }
    },
  },

  persist: [
    {
      // Tokens dans des cookies (accessibles SSR + client, légers < 4KB)
      key: 'auth-tokens',
      pick: ['accessToken', 'refreshToken'],
      storage: piniaPluginPersistedstate.cookies({
        maxAge: 60 * 60 * 24 * 7, // 7 jours
      }),
    },
    {
      // Profil utilisateur dans localStorage (trop volumineux pour un cookie)
      key: 'auth-profile',
      pick: ['user', 'roles', 'permissions', 'lastLoginAt', 'cguAccepted', 'cguVersion', 'mustChangePassword', 'activeCompanyId', 'onboardingStep', 'onboardingCompleted'],
      storage: piniaPluginPersistedstate.localStorage(),
    },
  ],
})
