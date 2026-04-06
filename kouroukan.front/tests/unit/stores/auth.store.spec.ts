import { describe, it, expect, vi, beforeEach } from 'vitest'
import { useAuthStore, authFetch } from '~/core/stores/auth.store'

// Helper : un user mock complet
function mockUser(overrides: Record<string, unknown> = {}) {
  return {
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
    mustChangePassword: false,
    onboardingStep: 3,
    onboardingCompletedAt: '2025-01-01T00:00:00Z',
    avatarUrl: null,
    preferredLocale: 'fr',
    preferredTheme: 'dark',
    companies: [
      { id: 10, modules: ['inscriptions', 'pedagogie'] },
      { id: 20, modules: ['finances'] },
    ],
    ...overrides,
  }
}

describe('useAuthStore', () => {
  let store: ReturnType<typeof useAuthStore>

  beforeEach(() => {
    store = useAuthStore()
    vi.mocked(navigateTo).mockReset()
    vi.mocked($fetch).mockReset()
  })

  // ═══════════════════════════════════════════════════════════════
  // State initial
  // ═══════════════════════════════════════════════════════════════
  describe('etat initial', () => {
    it('demarre sans utilisateur', () => {
      expect(store.user).toBeNull()
      expect(store.roles).toEqual([])
      expect(store.permissions).toEqual([])
      expect(store.cguAccepted).toBe(false)
      expect(store.cguVersion).toBeNull()
      expect(store.mustChangePassword).toBe(false)
      expect(store.activeCompanyId).toBeNull()
      expect(store.accessToken).toBeNull()
      expect(store.refreshToken).toBeNull()
      expect(store.onboardingStep).toBe(0)
      expect(store.onboardingCompleted).toBe(false)
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

    it('isCguUpToDate retourne false quand cguVersion est null', () => {
      expect(store.isCguUpToDate).toBe(false)
    })

    it('isCguUpToDate retourne true quand cguVersion correspond au config', () => {
      store.cguVersion = '1.0.0' // matches useRuntimeConfig().public.cguVersion
      expect(store.isCguUpToDate).toBe(true)
    })

    it('isCguUpToDate retourne false quand cguVersion differe', () => {
      store.cguVersion = '0.9.0'
      expect(store.isCguUpToDate).toBe(false)
    })
  })

  // ═══════════════════════════════════════════════════════════════
  // activeCompanyModules getter
  // ═══════════════════════════════════════════════════════════════
  describe('activeCompanyModules', () => {
    it('retourne un tableau vide sans user', () => {
      expect(store.activeCompanyModules).toEqual([])
    })

    it('retourne un tableau vide si user sans companies', () => {
      store.user = { id: 1, companies: [] } as any
      expect(store.activeCompanyModules).toEqual([])
    })

    it('retourne les modules de la premiere company par defaut', () => {
      store.user = mockUser() as any
      expect(store.activeCompanyModules).toEqual(['inscriptions', 'pedagogie'])
    })

    it('retourne les modules de la company active', () => {
      store.user = mockUser() as any
      store.activeCompanyId = 20
      expect(store.activeCompanyModules).toEqual(['finances'])
    })

    it('retourne un tableau vide si activeCompanyId ne correspond a rien', () => {
      store.user = mockUser() as any
      store.activeCompanyId = 999
      expect(store.activeCompanyModules).toEqual([])
    })
  })

  // ═══════════════════════════════════════════════════════════════
  // login()
  // ═══════════════════════════════════════════════════════════════
  describe('login()', () => {
    it('remplit user, roles et permissions apres login reussi', async () => {
      const user = mockUser()
      vi.mocked($fetch)
        .mockResolvedValueOnce({
          success: true,
          data: { accessToken: 'jwt-token-123', refreshToken: 'refresh-token' },
        })
        .mockResolvedValueOnce({ success: true, data: user })

      await store.login('ibrahima@test.gn', 'password123')

      expect(store.user).toEqual(user)
      expect(store.roles).toEqual(['directeur'])
      expect(store.permissions).toEqual(['inscriptions:read', 'inscriptions:create'])
      expect(store.isAuthenticated).toBe(true)
      expect(store.currentUserId).toBe(1)
      expect(store.cguAccepted).toBe(true)
      expect(store.cguVersion).toBe('1.0.0')
      expect(store.accessToken).toBe('jwt-token-123')
      expect(store.refreshToken).toBe('refresh-token')
      expect(store.mustChangePassword).toBe(false)
      expect(store.onboardingStep).toBe(3)
      expect(store.onboardingCompleted).toBe(true)
      expect(store.lastLoginAt).toBeTruthy()
    })

    it('gere un user sans refreshToken', async () => {
      vi.mocked($fetch)
        .mockResolvedValueOnce({
          success: true,
          data: { accessToken: 'jwt-token' },
        })
        .mockResolvedValueOnce({
          success: true,
          data: mockUser({ cguAcceptedAt: null, onboardingCompletedAt: null }),
        })

      await store.login('test@test.gn', 'pass')
      expect(store.refreshToken).toBeNull()
      expect(store.cguAccepted).toBe(false)
      expect(store.onboardingCompleted).toBe(false)
    })

    it('lance une erreur si $fetch echoue au login', async () => {
      vi.mocked($fetch).mockRejectedValueOnce(new Error('Network error'))

      await expect(store.login('test@test.gn', 'pass')).rejects.toThrow('Network error')
      expect(store.user).toBeNull()
    })

    it('lance une erreur si le login retourne success=false', async () => {
      vi.mocked($fetch).mockResolvedValueOnce({ success: false, data: null })

      await expect(store.login('test@test.gn', 'pass')).rejects.toThrow('Identifiants incorrects.')
    })

    it('lance une erreur si le login retourne un token vide', async () => {
      vi.mocked($fetch).mockResolvedValueOnce({
        success: true,
        data: { accessToken: '' },
      })

      await expect(store.login('test@test.gn', 'pass')).rejects.toThrow('Identifiants incorrects.')
    })

    it('passe le turnstileToken au body de la requete login', async () => {
      const user = mockUser()
      vi.mocked($fetch)
        .mockResolvedValueOnce({
          success: true,
          data: { accessToken: 'jwt-token-123', refreshToken: 'refresh-token' },
        })
        .mockResolvedValueOnce({ success: true, data: user })

      await store.login('ibrahima@test.gn', 'password123', 'cf-turnstile-token-abc')

      expect($fetch).toHaveBeenCalledWith('/api/auth/login', expect.objectContaining({
        body: { email: 'ibrahima@test.gn', password: 'password123', turnstileToken: 'cf-turnstile-token-abc' },
      }))
    })

    it('ne passe pas turnstileToken si null', async () => {
      const user = mockUser()
      vi.mocked($fetch)
        .mockResolvedValueOnce({
          success: true,
          data: { accessToken: 'jwt-token-123', refreshToken: 'refresh-token' },
        })
        .mockResolvedValueOnce({ success: true, data: user })

      await store.login('ibrahima@test.gn', 'password123', null)

      expect($fetch).toHaveBeenCalledWith('/api/auth/login', expect.objectContaining({
        body: { email: 'ibrahima@test.gn', password: 'password123' },
      }))
    })
  })

  // ═══════════════════════════════════════════════════════════════
  // logout()
  // ═══════════════════════════════════════════════════════════════
  describe('logout()', () => {
    it('vide le state apres logout', async () => {
      store.user = { id: 1 } as any
      store.roles = ['directeur'] as any
      store.permissions = ['inscriptions:read'] as any
      store.cguAccepted = true
      store.accessToken = 'token'

      vi.mocked($fetch).mockResolvedValue({})

      await store.logout()

      expect(store.user).toBeNull()
      expect(store.roles).toEqual([])
      expect(store.permissions).toEqual([])
      expect(store.cguAccepted).toBe(false)
      expect(store.accessToken).toBeNull()
      expect(navigateTo).toHaveBeenCalledWith('/connexion', { replace: true })
    })

    it('vide le state meme si $fetch echoue', async () => {
      store.user = { id: 1 } as any
      store.accessToken = 'token'
      vi.mocked($fetch).mockRejectedValue(new Error('network'))

      await store.logout()

      expect(store.user).toBeNull()
      expect(navigateTo).toHaveBeenCalledWith('/connexion', { replace: true })
    })

    it('vide le state sans token (pas de header Authorization)', async () => {
      store.user = { id: 1 } as any
      store.accessToken = null
      vi.mocked($fetch).mockResolvedValue({})

      await store.logout()

      expect(store.user).toBeNull()
    })
  })

  // ═══════════════════════════════════════════════════════════════
  // refreshToken() (la méthode action)
  // ═══════════════════════════════════════════════════════════════
  describe('refreshProfile()', () => {
    it('recharge le profil utilisateur', async () => {
      const user = mockUser()
      vi.mocked($fetch).mockResolvedValueOnce({ success: true, data: user })

      await store.refreshProfile()

      expect(store.user).toEqual(user)
      expect(store.roles).toEqual(['directeur'])
      expect(store.permissions).toEqual(['inscriptions:read', 'inscriptions:create'])
    })

    it('ne fait rien si la reponse n est pas success', async () => {
      vi.mocked($fetch).mockResolvedValueOnce({ success: false })

      await store.refreshProfile()

      expect(store.user).toBeNull()
    })
  })

  // ═══════════════════════════════════════════════════════════════
  // changePassword()
  // ═══════════════════════════════════════════════════════════════
  describe('changePassword()', () => {
    it('met mustChangePassword a false apres succes', async () => {
      store.mustChangePassword = true
      vi.mocked($fetch).mockResolvedValueOnce({})

      await store.changePassword('old', 'new')

      expect(store.mustChangePassword).toBe(false)
    })

    it('lance une erreur si le changement echoue', async () => {
      store.mustChangePassword = true
      vi.mocked($fetch).mockRejectedValueOnce({ statusCode: 400, data: { message: 'Mot de passe trop faible' } })

      await expect(store.changePassword('old', 'weak')).rejects.toBeTruthy()
      expect(store.mustChangePassword).toBe(true)
    })
  })

  // ═══════════════════════════════════════════════════════════════
  // fetchPermissions()
  // ═══════════════════════════════════════════════════════════════
  describe('fetchPermissions()', () => {
    it('charge les permissions', async () => {
      vi.mocked($fetch).mockResolvedValueOnce({
        success: true,
        data: { permissions: ['finances:read', 'finances:create'] },
      })

      await store.fetchPermissions()

      expect(store.permissions).toEqual(['finances:read', 'finances:create'])
    })

    it('ne modifie pas les permissions si reponse invalide', async () => {
      store.permissions = ['existing'] as any
      vi.mocked($fetch).mockResolvedValueOnce({ success: false })

      await store.fetchPermissions()

      expect(store.permissions).toEqual(['existing'])
    })
  })

  // ═══════════════════════════════════════════════════════════════
  // checkCgu()
  // ═══════════════════════════════════════════════════════════════
  describe('checkCgu()', () => {
    it('retourne true si la version CGU correspond', async () => {
      store.cguVersion = '1.0.0'
      vi.mocked($fetch).mockResolvedValue({
        success: true,
        data: { version: '1.0.0' },
      })

      expect(await store.checkCgu()).toBe(true)
    })

    it('retourne false si la version CGU ne correspond pas', async () => {
      store.cguVersion = '0.9.0'
      vi.mocked($fetch).mockResolvedValue({
        success: true,
        data: { version: '1.0.0' },
      })

      expect(await store.checkCgu()).toBe(false)
    })

    it('retourne false si la reponse n est pas success', async () => {
      vi.mocked($fetch).mockResolvedValue({ success: false })

      expect(await store.checkCgu()).toBe(false)
    })

    it('utilise le runtimeConfig en fallback si $fetch echoue', async () => {
      store.cguVersion = '1.0.0'
      vi.mocked($fetch).mockRejectedValue(new Error('network'))

      expect(await store.checkCgu()).toBe(true)
    })

    it('retourne false en fallback si la version ne correspond pas', async () => {
      store.cguVersion = '0.5.0'
      vi.mocked($fetch).mockRejectedValue(new Error('network'))

      expect(await store.checkCgu()).toBe(false)
    })
  })

  // ═══════════════════════════════════════════════════════════════
  // acceptCgu()
  // ═══════════════════════════════════════════════════════════════
  describe('acceptCgu()', () => {
    it('met a jour cguVersion, cguAccepted et les tokens', async () => {
      vi.mocked($fetch).mockResolvedValue({
        success: true,
        data: { accessToken: 'new-token', refreshToken: 'new-refresh' },
      })

      await store.acceptCgu('2.0.0')

      expect(store.cguVersion).toBe('2.0.0')
      expect(store.cguAccepted).toBe(true)
      expect(store.accessToken).toBe('new-token')
      expect(store.refreshToken).toBe('new-refresh')
    })

    it('accepte sans mise a jour des tokens si reponse vide', async () => {
      vi.mocked($fetch).mockResolvedValue({})

      await store.acceptCgu('2.0.0')

      expect(store.cguVersion).toBe('2.0.0')
      expect(store.cguAccepted).toBe(true)
      expect(store.accessToken).toBeNull()
    })

    it('accepte avec accessToken mais sans refreshToken', async () => {
      vi.mocked($fetch).mockResolvedValue({
        success: true,
        data: { accessToken: 'new-token' },
      })

      await store.acceptCgu('2.0.0')

      expect(store.accessToken).toBe('new-token')
      expect(store.refreshToken).toBeNull()
    })

    it('lance une erreur si acceptCgu echoue', async () => {
      vi.mocked($fetch).mockRejectedValue({ statusCode: 500, data: { message: 'Erreur serveur' } })

      await expect(store.acceptCgu('2.0.0')).rejects.toBeTruthy()
      expect(store.cguAccepted).toBe(false)
    })
  })

  // ═══════════════════════════════════════════════════════════════
  // updatePreferences()
  // ═══════════════════════════════════════════════════════════════
  describe('updatePreferences()', () => {
    it('met a jour locale et theme sur le user', async () => {
      store.user = mockUser() as any
      vi.mocked($fetch).mockResolvedValueOnce({})

      await store.updatePreferences('en', 'light')

      expect(store.user!.preferredLocale).toBe('en')
      expect(store.user!.preferredTheme).toBe('light')
    })

    it('ne plante pas si user est null', async () => {
      store.user = null
      vi.mocked($fetch).mockResolvedValueOnce({})

      await store.updatePreferences('en', 'light')
      // no crash
    })

    it('lance une erreur si la requete echoue', async () => {
      store.user = mockUser() as any
      vi.mocked($fetch).mockRejectedValueOnce({ statusCode: 500, data: { message: 'Erreur' } })

      await expect(store.updatePreferences('en', 'light')).rejects.toBeTruthy()
    })
  })

  // ═══════════════════════════════════════════════════════════════
  // hasPermission()
  // ═══════════════════════════════════════════════════════════════
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

  // ═══════════════════════════════════════════════════════════════
  // hasRole()
  // ═══════════════════════════════════════════════════════════════
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

  // ═══════════════════════════════════════════════════════════════
  // hasModule()
  // ═══════════════════════════════════════════════════════════════
  describe('hasModule()', () => {
    it('retourne true pour super_admin quel que soit le module', () => {
      store.roles = ['super_admin'] as any
      expect(store.hasModule('any-module')).toBe(true)
    })

    it('retourne true pour le module support (toujours accessible)', () => {
      store.roles = ['directeur'] as any
      expect(store.hasModule('support')).toBe(true)
    })

    it('retourne true si le module est dans les modules de la company', () => {
      store.user = mockUser() as any
      store.roles = ['directeur'] as any
      expect(store.hasModule('inscriptions')).toBe(true)
    })

    it('retourne false si le module n est pas souscrit', () => {
      store.user = mockUser() as any
      store.roles = ['directeur'] as any
      expect(store.hasModule('unknown-module')).toBe(false)
    })

    it('retourne false sans user ni companies', () => {
      store.roles = ['directeur'] as any
      expect(store.hasModule('inscriptions')).toBe(false)
    })
  })

  // ═══════════════════════════════════════════════════════════════
  // isAdmin
  // ═══════════════════════════════════════════════════════════════
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

  // ═══════════════════════════════════════════════════════════════
  // uploadAvatar()
  // ═══════════════════════════════════════════════════════════════
  describe('uploadAvatar()', () => {
    it('upload un avatar et met a jour user.avatarUrl', async () => {
      store.user = mockUser() as any
      store.accessToken = 'jwt-token'

      vi.mocked($fetch).mockResolvedValueOnce({
        success: true,
        data: { avatarUrl: '/avatars/1/new-avatar.jpg' },
      })

      const file = new File([''], 'avatar.jpg', { type: 'image/jpeg' })
      const result = await store.uploadAvatar(file)

      expect(result).toBe('/avatars/1/new-avatar.jpg')
      expect(store.user!.avatarUrl).toBe('/avatars/1/new-avatar.jpg')
    })

    it('upload sans user (ne plante pas)', async () => {
      store.user = null
      store.accessToken = 'jwt-token'

      vi.mocked($fetch).mockResolvedValueOnce({
        success: true,
        data: { avatarUrl: '/avatars/1/new-avatar.jpg' },
      })

      const file = new File([''], 'avatar.jpg', { type: 'image/jpeg' })
      const result = await store.uploadAvatar(file)

      expect(result).toBe('/avatars/1/new-avatar.jpg')
    })

    it('upload sans accessToken (pas de header Authorization)', async () => {
      store.user = mockUser() as any
      store.accessToken = null

      vi.mocked($fetch).mockResolvedValueOnce({
        success: true,
        data: { avatarUrl: '/avatars/1/img.jpg' },
      })

      const file = new File([''], 'avatar.jpg', { type: 'image/jpeg' })
      await store.uploadAvatar(file)

      const call = vi.mocked($fetch).mock.calls[0]
      expect((call[1] as any).headers).toEqual({})
    })

    it('lance une erreur si success=false', async () => {
      store.accessToken = 'jwt-token'
      vi.mocked($fetch).mockResolvedValueOnce({ success: false })

      const file = new File([''], 'avatar.jpg', { type: 'image/jpeg' })
      await expect(store.uploadAvatar(file)).rejects.toThrow('Upload echoue')
    })

    it('lance une erreur si $fetch echoue', async () => {
      store.accessToken = 'jwt-token'
      vi.mocked($fetch).mockRejectedValueOnce(new Error('network'))

      const file = new File([''], 'avatar.jpg', { type: 'image/jpeg' })
      await expect(store.uploadAvatar(file)).rejects.toThrow()
    })
  })
})

// ═══════════════════════════════════════════════════════════════
// authFetch() (fonction exportee)
// ═══════════════════════════════════════════════════════════════
describe('authFetch()', () => {
  let store: ReturnType<typeof useAuthStore>

  beforeEach(() => {
    store = useAuthStore()
    vi.mocked($fetch).mockReset()
    vi.mocked(navigateTo).mockReset()
  })

  it('fait un appel GET avec le token', async () => {
    store.accessToken = 'my-token'
    vi.mocked($fetch).mockResolvedValueOnce({ data: 'ok' })

    const result = await authFetch('/api/test')

    expect($fetch).toHaveBeenCalledWith('/api/test', expect.objectContaining({
      method: 'GET',
      headers: expect.objectContaining({
        Authorization: 'Bearer my-token',
      }),
    }))
    expect(result).toEqual({ data: 'ok' })
  })

  it('fait un appel sans token si non connecte', async () => {
    store.accessToken = null
    vi.mocked($fetch).mockResolvedValueOnce({ data: 'ok' })

    await authFetch('/api/test')

    const headers = vi.mocked($fetch).mock.calls[0][1]?.headers as Record<string, string>
    expect(headers.Authorization).toBeUndefined()
  })

  it('propage les erreurs non-401', async () => {
    store.accessToken = 'token'
    vi.mocked($fetch).mockRejectedValueOnce({ statusCode: 500 })

    await expect(authFetch('/api/test')).rejects.toEqual({ statusCode: 500 })
  })

  it('tente un refresh sur 401 et re-essaie', async () => {
    store.accessToken = 'old-token'
    store.refreshToken = 'refresh-token'

    vi.mocked($fetch)
      // 1. Premier appel -> 401
      .mockRejectedValueOnce({ statusCode: 401 })
      // 2. Appel refresh -> success
      .mockResolvedValueOnce({
        success: true,
        data: { accessToken: 'new-token', refreshToken: 'new-refresh' },
      })
      // 3. Retry -> success
      .mockResolvedValueOnce({ data: 'retry-ok' })

    const result = await authFetch('/api/test')

    expect(result).toEqual({ data: 'retry-ok' })
    expect(store.accessToken).toBe('new-token')
    expect(store.refreshToken).toBe('new-refresh')
  })

  it('deconnecte si le refresh echoue', async () => {
    store.accessToken = 'old-token'
    store.refreshToken = 'refresh-token'
    store.user = { id: 1 } as any

    vi.mocked($fetch)
      .mockRejectedValueOnce({ statusCode: 401 })
      // Refresh echoue
      .mockRejectedValueOnce(new Error('refresh failed'))

    await expect(authFetch('/api/test')).rejects.toEqual({ statusCode: 401 })
    expect(store.user).toBeNull()
    expect(navigateTo).toHaveBeenCalledWith('/connexion', { replace: true })
  })

  it('deconnecte si pas de refreshToken', async () => {
    store.accessToken = 'token'
    store.refreshToken = null
    store.user = { id: 1 } as any

    vi.mocked($fetch).mockRejectedValueOnce({ statusCode: 401 })

    await expect(authFetch('/api/test')).rejects.toEqual({ statusCode: 401 })
    expect(store.user).toBeNull()
    expect(navigateTo).toHaveBeenCalledWith('/connexion', { replace: true })
  })

  it('passe les headers personnalises', async () => {
    store.accessToken = 'token'
    vi.mocked($fetch).mockResolvedValueOnce({})

    await authFetch('/api/test', {
      method: 'POST',
      body: { key: 'value' },
      headers: { 'X-Custom': 'header' },
    })

    expect($fetch).toHaveBeenCalledWith('/api/test', expect.objectContaining({
      method: 'POST',
      body: { key: 'value' },
      headers: expect.objectContaining({
        'X-Custom': 'header',
        'Authorization': 'Bearer token',
      }),
    }))
  })

  it('refresh avec nouveau refreshToken partiel (sans refreshToken dans la reponse)', async () => {
    store.accessToken = 'old'
    store.refreshToken = 'old-refresh'

    vi.mocked($fetch)
      .mockRejectedValueOnce({ statusCode: 401 })
      .mockResolvedValueOnce({
        success: true,
        data: { accessToken: 'new-token' },
      })
      .mockResolvedValueOnce({ data: 'ok' })

    await authFetch('/api/test')

    expect(store.accessToken).toBe('new-token')
    expect(store.refreshToken).toBe('old-refresh') // not replaced
  })
})
