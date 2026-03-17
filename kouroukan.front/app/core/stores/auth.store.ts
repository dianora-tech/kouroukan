import { defineStore } from 'pinia'
import type { User } from '~/types/user'
import type { RoleName, PermissionKey } from '~/core/auth/rbac'

interface AuthState {
  user: User | null
  roles: RoleName[]
  permissions: PermissionKey[]
  lastLoginAt: string | null
  cguAccepted: boolean
  cguVersion: string | null
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    user: null,
    roles: [],
    permissions: [],
    lastLoginAt: null,
    cguAccepted: false,
    cguVersion: null,
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
      const { data } = await useFetch<{
        success: boolean
        data: {
          accessToken: string
          user: User
        }
      }>('/api/auth/login', {
        method: 'POST',
        body: { email, password },
      })

      if (data.value?.success && data.value.data) {
        const { user } = data.value.data
        this.user = user
        this.roles = (user.roles ?? []) as RoleName[]
        this.permissions = (user.permissions ?? []) as PermissionKey[]
        this.lastLoginAt = new Date().toISOString()
        this.cguVersion = user.cguVersion ?? null
        this.cguAccepted = !!user.cguAcceptedAt
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
        this.$reset()
        navigateTo('/connexion')
      }
    },

    async refreshToken(): Promise<void> {
      const { data } = await useFetch<{
        success: boolean
        data: User
      }>('/api/auth/me')

      if (data.value?.success && data.value.data) {
        const user = data.value.data
        this.user = user
        this.roles = (user.roles ?? []) as RoleName[]
        this.permissions = (user.permissions ?? []) as PermissionKey[]
        this.cguVersion = user.cguVersion ?? null
        this.cguAccepted = !!user.cguAcceptedAt
      }
    },

    async fetchPermissions(): Promise<void> {
      const { data } = await useFetch<{
        success: boolean
        data: { permissions: string[] }
      }>('/api/auth/permissions')

      if (data.value?.success && data.value.data) {
        this.permissions = data.value.data.permissions as PermissionKey[]
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
      await $fetch('/api/auth/cgu/accept', {
        method: 'POST',
        body: { version },
      })
      this.cguVersion = version
      this.cguAccepted = true
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
    pick: ['user', 'roles', 'permissions', 'lastLoginAt', 'cguAccepted', 'cguVersion'],
  },
})
