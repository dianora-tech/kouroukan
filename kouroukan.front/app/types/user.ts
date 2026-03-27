import type { RoleName, PermissionKey } from '~/core/auth/rbac'

export interface User {
  id: number
  firstName: string
  lastName: string
  email: string
  phoneNumber: string | null
  isActive: boolean
  lastLoginAt: string | null
  roles: RoleName[]
  permissions: PermissionKey[]
  cguVersion: string | null
  cguAcceptedAt: string | null
  mustChangePassword: boolean
  companies: { id: number, name: string, role: string, modules?: string[] }[]
  preferredLocale?: string
  preferredTheme?: string
  avatarUrl?: string | null
  onboardingStep: number
  onboardingCompletedAt: string | null
}

export interface LoginPayload {
  email: string
  password: string
}

export interface RegisterPayload {
  firstName: string
  lastName: string
  email: string
  phoneNumber: string
  password: string
  confirmPassword: string
}

export interface AuthTokens {
  accessToken: string
  refreshToken: string
  expiresAt: string
}
