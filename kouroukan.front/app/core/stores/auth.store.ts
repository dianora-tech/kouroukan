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
  },

  actions: {
    async login(email: string, password: string): Promise<void> {
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
          body: { email, password },
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

      // Store the token in Pinia state (persisted) and cookie
      const token = loginResponse.data.accessToken
      this.accessToken = token
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
      }
    },

    async logout(): Promise<void> {
      try {
        await $fetch('/api/auth/logout', { method: 'POST' })
      }
      catch {
        // Ignore logout errors
      }
      finally {
        // Clear sidebase/nuxt-auth token
        try {
          const { signOut } = useAuth()
          await signOut({ callbackUrl: '/connexion', redirect: false })
        }
        catch {
          // sidebase auth not available, clear manual token
          useState('auth-token').value = null
        }
        this.$reset()
        await navigateTo('/connexion', { replace: true })
      }
    },

    async refreshToken(): Promise<void> {
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
      }
    },

    async changePassword(currentPassword: string, newPassword: string): Promise<void> {
      try {
        await $fetch('/api/auth/change-password', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            ...(this.accessToken ? { Authorization: `Bearer ${this.accessToken}` } : {}),
          },
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
        const token = this.accessToken

        const response = await $fetch<{
          success: boolean
          data: {
            accessToken: string
            refreshToken: string
          }
        }>('/api/auth/cgu/accept', {
          method: 'POST',
          headers: {
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
          },
        })

        // Update token with new cguVersion claim
        if (response?.success && response.data?.accessToken) {
          this.accessToken = response.data.accessToken
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

    hasPermission(permission: PermissionKey): boolean {
      if (this.roles.includes('super_admin')) return true
      return this.permissions.includes(permission)
    },

    hasRole(role: RoleName): boolean {
      return this.roles.includes(role)
    },
  },

  persist: {
    pick: ['user', 'roles', 'permissions', 'lastLoginAt', 'cguAccepted', 'cguVersion', 'mustChangePassword', 'activeCompanyId', 'accessToken'],
  },
})
