import { describe, it, expect, vi, beforeEach } from 'vitest'
import { useInscriptionStore } from '~/modules/inscriptions/stores/inscription.store'
import { mockPaginatedResponse, mockApiResponse } from '../../helpers/api-mock'

// Mock apiClient
vi.mock('~/core/api/client', () => ({
  apiClient: {
    get: vi.fn(),
    getPaginated: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    delete: vi.fn(),
  },
}))

// Import mocked apiClient
import { apiClient } from '~/core/api/client'

const mockInscription = {
  id: 1,
  typeId: 1,
  eleveId: 10,
  classeId: 5,
  anneeScolaireId: 1,
  dateInscription: '2025-09-01',
  montantInscription: 150000,
  estPaye: false,
  estRedoublant: false,
  typeEtablissement: 'Public',
  serieBac: null,
  statutInscription: 'EnAttente',
  createdAt: '2025-09-01T00:00:00Z',
  updatedAt: null,
  createdBy: 'admin',
  updatedBy: null,
}

describe('useInscriptionStore', () => {
  let store: ReturnType<typeof useInscriptionStore>

  beforeEach(() => {
    vi.clearAllMocks()
    store = useInscriptionStore()
  })

  // ─── state initial ───
  describe('etat initial', () => {
    it('demarre sans items', () => {
      expect(store.items).toEqual([])
      expect(store.currentItem).toBeNull()
      expect(store.loading).toBe(false)
      expect(store.saving).toBe(false)
      expect(store.isEmpty).toBe(true)
    })
  })

  // ─── fetchAll ───
  describe('fetchAll()', () => {
    it('recupere les inscriptions et met a jour items et pagination', async () => {
      vi.mocked(apiClient.getPaginated).mockResolvedValue(
        mockPaginatedResponse([mockInscription, { ...mockInscription, id: 2 }]),
      )

      await store.fetchAll()

      expect(store.items).toHaveLength(2)
      expect(store.pagination.totalCount).toBe(2)
      expect(store.loading).toBe(false)
      expect(store.isEmpty).toBe(false)
    })

    it('passe loading a true pendant le chargement', async () => {
      let loadingDuringFetch = false
      vi.mocked(apiClient.getPaginated).mockImplementation(async () => {
        loadingDuringFetch = store.loading
        return mockPaginatedResponse([])
      })

      await store.fetchAll()

      expect(loadingDuringFetch).toBe(true)
      expect(store.loading).toBe(false)
    })

    it('utilise les parametres de filtre', async () => {
      vi.mocked(apiClient.getPaginated).mockResolvedValue(mockPaginatedResponse([]))

      store.filters = { search: 'Diallo' }
      await store.fetchAll({ page: 2, pageSize: 10 })

      expect(apiClient.getPaginated).toHaveBeenCalledWith(
        '/api/inscriptions/inscriptions',
        expect.objectContaining({
          page: 2,
          pageSize: 10,
          search: 'Diallo',
        }),
      )
    })
  })

  // ─── fetchById ───
  describe('fetchById()', () => {
    it('recupere une inscription par id', async () => {
      vi.mocked(apiClient.get).mockResolvedValue(mockApiResponse(mockInscription))

      const result = await store.fetchById(1)

      expect(result).toEqual(mockInscription)
      expect(store.currentItem).toEqual(mockInscription)
    })

    it('retourne null si la reponse echoue', async () => {
      vi.mocked(apiClient.get).mockResolvedValue(mockApiResponse(null, false))

      const result = await store.fetchById(999)

      expect(result).toBeNull()
    })
  })

  // ─── create ───
  describe('create()', () => {
    it('cree une inscription et rafraichit la liste', async () => {
      const payload = {
        typeId: 1,
        eleveId: 10,
        classeId: 5,
        anneeScolaireId: 1,
        dateInscription: '2025-09-01',
        montantInscription: 150000,
        estPaye: false,
        estRedoublant: false,
        statutInscription: 'EnAttente',
      }

      vi.mocked(apiClient.post).mockResolvedValue(mockApiResponse(mockInscription))
      vi.mocked(apiClient.getPaginated).mockResolvedValue(
        mockPaginatedResponse([mockInscription]),
      )

      const result = await store.create(payload as any)

      expect(result).toEqual(mockInscription)
      expect(apiClient.post).toHaveBeenCalledWith('/api/inscriptions/inscriptions', payload)
      expect(store.saving).toBe(false)
    })

    it('passe saving a true pendant la sauvegarde', async () => {
      let savingDuringCreate = false
      vi.mocked(apiClient.post).mockImplementation(async () => {
        savingDuringCreate = store.saving
        return mockApiResponse(mockInscription)
      })
      vi.mocked(apiClient.getPaginated).mockResolvedValue(mockPaginatedResponse([]))

      await store.create({} as any)

      expect(savingDuringCreate).toBe(true)
      expect(store.saving).toBe(false)
    })
  })

  // ─── update ───
  describe('update()', () => {
    it('met a jour une inscription', async () => {
      const updated = { ...mockInscription, estPaye: true }
      vi.mocked(apiClient.put).mockResolvedValue(mockApiResponse(updated))
      vi.mocked(apiClient.getPaginated).mockResolvedValue(mockPaginatedResponse([updated]))

      const result = await store.update(1, { estPaye: true } as any)

      expect(result).toEqual(updated)
      expect(apiClient.put).toHaveBeenCalledWith(
        '/api/inscriptions/inscriptions/1',
        { estPaye: true },
      )
    })
  })

  // ─── remove ───
  describe('remove()', () => {
    it('supprime une inscription', async () => {
      vi.mocked(apiClient.delete).mockResolvedValue({ success: true, data: undefined, message: null, errors: null })
      vi.mocked(apiClient.getPaginated).mockResolvedValue(mockPaginatedResponse([]))

      const result = await store.remove(1)

      expect(result).toBe(true)
      expect(apiClient.delete).toHaveBeenCalledWith('/api/inscriptions/inscriptions/1')
    })
  })

  // ─── filters ───
  describe('setFilters() / resetFilters()', () => {
    it('met a jour les filtres et remet la page a 1', () => {
      store.pagination.page = 5
      store.setFilters({ search: 'Camara' })

      expect(store.filters).toEqual({ search: 'Camara' })
      expect(store.pagination.page).toBe(1)
    })

    it('reset les filtres', () => {
      store.filters = { search: 'test' }
      store.resetFilters()

      expect(store.filters).toEqual({})
      expect(store.pagination.page).toBe(1)
    })

    it('hasFilters retourne true avec des filtres actifs', () => {
      store.filters = { search: 'test' }
      expect(store.hasFilters).toBe(true)
    })

    it('hasFilters retourne false sans filtres', () => {
      expect(store.hasFilters).toBe(false)
    })
  })

  // ─── mode offline ───
  describe('mode offline', () => {
    it('cree en mode offline via offlineStore.enqueue()', async () => {
      vi.mocked(apiClient.post).mockResolvedValue({
        success: true,
        data: null,
        message: 'queued_for_sync',
        errors: null,
      })
      vi.mocked(apiClient.getPaginated).mockResolvedValue(mockPaginatedResponse([]))

      const result = await store.create({ statutInscription: 'EnAttente' } as any)

      // apiClient.post was called - it handles offline internally
      expect(apiClient.post).toHaveBeenCalled()
    })
  })
})
