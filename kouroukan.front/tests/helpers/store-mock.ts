import { vi } from 'vitest'
import type { SyncOperation } from '~/core/stores/offline.store'

/**
 * Cree un mock du store offline.
 */
export function createOfflineStoreMock(overrides: Partial<{
  isOnline: boolean
  syncQueue: SyncOperation[]
  lastSyncAt: string | null
  syncInProgress: boolean
}> = {}) {
  return {
    isOnline: true,
    syncQueue: [],
    lastSyncAt: null,
    syncInProgress: false,
    pendingCount: 0,
    hasPendingSync: false,
    initNetworkListener: vi.fn(),
    enqueue: vi.fn(),
    processQueue: vi.fn(),
    clearQueue: vi.fn(),
    ...overrides,
  }
}

/**
 * Cree un mock du store auth.
 */
export function createAuthStoreMock(overrides: Record<string, unknown> = {}) {
  return {
    user: null,
    roles: [],
    permissions: [],
    lastLoginAt: null,
    cguAccepted: false,
    cguVersion: null,
    isAuthenticated: false,
    isAdmin: false,
    currentUserId: null,
    isCguUpToDate: false,
    login: vi.fn(),
    logout: vi.fn(),
    refreshToken: vi.fn(),
    fetchPermissions: vi.fn(),
    checkCgu: vi.fn(),
    acceptCgu: vi.fn(),
    hasPermission: vi.fn().mockReturnValue(false),
    hasRole: vi.fn().mockReturnValue(false),
    $reset: vi.fn(),
    ...overrides,
  }
}
