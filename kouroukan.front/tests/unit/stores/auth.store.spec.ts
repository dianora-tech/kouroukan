import { describe, it, expect, vi, beforeEach } from 'vitest'
import { useAuthStore } from '~/core/stores/auth.store'

describe('useAuthStore', () => {
  let store: ReturnType<typeof useAuthStore>

  beforeEach(() => {
    store = useAuthStore()
  })

  // ─── state initial ───
  describe('etat initial', () => {
    it('demarre sans utilisateur', () => {
      expect(store.user).toBeNull()
      expect(store.roles).toEqual([])
      expect(store.permissions).toEqual([])
      expect(store.cguAccepted).toBe(false)
      expect(store.cguVersion).toBeNull()
    })

    it('isAuthenticated retourne false', () => {
      expect(store.isAuthenticated).toBe(false)
    })

    it('isAdmin retourne false', () => {
      expect(store.isAdmin).toBe(false)
    })

    it('currentUserId retourne null', () => {
      expect(store.currentUserId).toBeNull()
    })
  })

  // ─── login ───
  describe('login()', () => {
    it('remplit user, roles et permissions apres login reussi', async () => {
      const mockUser = {
        id: 1,
        firstName: 'Ibrahima',
        lastName: 'Diallo',
        email: 'ibrahima@test.gn',
        phoneNumber: '+224620000000',
        isActive: true,
        lastLoginAt: null,
        roles: ['directeur'],
        permissions: ['inscriptions:read', 'inscriptions:create'],
        cguVersion: '1.0.0',
        cguAcceptedAt: '2025-01-01T00:00:00Z',
      }

      vi.stubGlobal('$fetch', vi.fn()
        .mockResolvedValueOnce({
          success: true,
          data: { accessToken: 'jwt-token-123', refreshToken: 'refresh-token' },
        })
        .mockResolvedValueOnce({
          success: true,
          data: mockUser,
        }),
      )

      await store.login('ibrahima@test.gn', 'password123')

      expect(store.user).toEqual(mockUser)
      expect(store.roles).toEqual(['directeur'])
      expect(store.permissions).toEqual(['inscriptions:read', 'inscriptions:create'])
      expect(store.isAuthenticated).toBe(true)
      expect(store.currentUserId).toBe(1)
      expect(store.cguAccepted).toBe(true)
      expect(store.cguVersion).toBe('1.0.0')
    })
  })

  // ─── logout ───
  describe('logout()', () => {
    it('vide le state apres logout', async () => {
      store.user = { id: 1 } as any
      store.roles = ['directeur'] as any
      store.permissions = ['inscriptions:read'] as any
      store.cguAccepted = true

      vi.stubGlobal('$fetch', vi.fn().mockResolvedValue({}))

      await store.logout()

      expect(store.user).toBeNull()
      expect(store.roles).toEqual([])
      expect(store.permissions).toEqual([])
      expect(store.cguAccepted).toBe(false)
    })
  })

  // ─── hasPermission ───
  describe('hasPermission()', () => {
    it('retourne true quand le role est super_admin', () => {
      store.roles = ['super_admin'] as any
      expect(store.hasPermission('inscriptions:create')).toBe(true)
    })

    it('retourne true quand la permission est dans la liste', () => {
      store.permissions = ['inscriptions:read', 'inscriptions:create'] as any
      expect(store.hasPermission('inscriptions:create')).toBe(true)
    })

    it('retourne false quand la permission est absente', () => {
      store.permissions = ['inscriptions:read'] as any
      expect(store.hasPermission('inscriptions:delete')).toBe(false)
    })
  })

  // ─── hasRole ───
  describe('hasRole()', () => {
    it('retourne true si le role est present', () => {
      store.roles = ['directeur', 'censeur'] as any
      expect(store.hasRole('directeur')).toBe(true)
    })

    it('retourne false si le role est absent', () => {
      store.roles = ['enseignant'] as any
      expect(store.hasRole('directeur')).toBe(false)
    })
  })

  // ─── isAdmin ───
  describe('isAdmin', () => {
    it('retourne true pour super_admin', () => {
      store.roles = ['super_admin'] as any
      expect(store.isAdmin).toBe(true)
    })

    it('retourne true pour admin_it', () => {
      store.roles = ['admin_it'] as any
      expect(store.isAdmin).toBe(true)
    })

    it('retourne false pour directeur', () => {
      store.roles = ['directeur'] as any
      expect(store.isAdmin).toBe(false)
    })
  })

  // ─── CGU ───
  describe('checkCgu()', () => {
    it('retourne true si la version CGU correspond', async () => {
      store.cguVersion = '1.0.0'
      vi.stubGlobal('$fetch', vi.fn().mockResolvedValue({
        success: true,
        data: { version: '1.0.0' },
      }))

      const result = await store.checkCgu()
      expect(result).toBe(true)
    })

    it('retourne false si la version CGU ne correspond pas', async () => {
      store.cguVersion = '0.9.0'
      vi.stubGlobal('$fetch', vi.fn().mockResolvedValue({
        success: true,
        data: { version: '1.0.0' },
      }))

      const result = await store.checkCgu()
      expect(result).toBe(false)
    })

    it('utilise le runtimeConfig en fallback si $fetch echoue', async () => {
      store.cguVersion = '1.0.0'
      vi.stubGlobal('$fetch', vi.fn().mockRejectedValue(new Error('network')))

      const result = await store.checkCgu()
      expect(result).toBe(true) // matches config.public.cguVersion = '1.0.0'
    })
  })

  describe('acceptCgu()', () => {
    it('met a jour cguVersion et cguAccepted', async () => {
      vi.stubGlobal('$fetch', vi.fn().mockResolvedValue({}))

      await store.acceptCgu('2.0.0')

      expect(store.cguVersion).toBe('2.0.0')
      expect(store.cguAccepted).toBe(true)
    })
  })
})
