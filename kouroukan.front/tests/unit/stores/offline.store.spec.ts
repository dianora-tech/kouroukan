import { describe, it, expect, vi, beforeEach } from 'vitest'
import { useOfflineStore } from '~/core/stores/offline.store'

describe('useOfflineStore', () => {
  let store: ReturnType<typeof useOfflineStore>

  beforeEach(() => {
    store = useOfflineStore()
  })

  // ─── state initial ───
  describe('etat initial', () => {
    it('demarre en mode online', () => {
      expect(store.isOnline).toBe(true)
    })

    it('la queue de sync est vide', () => {
      expect(store.syncQueue).toEqual([])
      expect(store.pendingCount).toBe(0)
      expect(store.hasPendingSync).toBe(false)
    })
  })

  // ─── enqueue ───
  describe('enqueue()', () => {
    it('ajoute une operation a la queue', () => {
      store.enqueue({ method: 'POST', url: '/api/test', body: { name: 'Test' } })

      expect(store.syncQueue).toHaveLength(1)
      expect(store.pendingCount).toBe(1)
      expect(store.hasPendingSync).toBe(true)
      expect(store.syncQueue[0]).toMatchObject({
        method: 'POST',
        url: '/api/test',
        body: { name: 'Test' },
        retryCount: 0,
      })
    })

    it('attribue un id unique et un createdAt', () => {
      store.enqueue({ method: 'DELETE', url: '/api/test/1' })

      expect(store.syncQueue[0].id).toBeDefined()
      expect(store.syncQueue[0].createdAt).toBeDefined()
    })
  })

  // ─── processQueue ───
  describe('processQueue()', () => {
    it('ne traite pas si offline', async () => {
      store.isOnline = false
      store.enqueue({ method: 'POST', url: '/api/test', body: {} })

      await store.processQueue()

      expect(store.syncQueue).toHaveLength(1)
    })

    it('ne traite pas si la queue est vide', async () => {
      await store.processQueue()
      expect(store.syncInProgress).toBe(false)
    })

    it('ne traite pas si un sync est deja en cours', async () => {
      store.syncInProgress = true
      store.enqueue({ method: 'POST', url: '/api/test', body: {} })

      await store.processQueue()

      expect(store.syncQueue).toHaveLength(1)
    })

    it('retire les operations reussies de la queue', async () => {
      vi.stubGlobal('$fetch', vi.fn().mockResolvedValue({}))
      store.enqueue({ method: 'POST', url: '/api/test', body: { a: 1 } })
      store.enqueue({ method: 'PUT', url: '/api/test/1', body: { a: 2 } })

      await store.processQueue()

      expect(store.syncQueue).toHaveLength(0)
      expect(store.lastSyncAt).toBeDefined()
      expect(store.syncInProgress).toBe(false)
    })

    it('retire les operations apres 3 echecs', async () => {
      vi.stubGlobal('$fetch', vi.fn().mockRejectedValue(new Error('500')))

      store.syncQueue = [{
        id: 'test-id',
        method: 'POST',
        url: '/api/test',
        body: {},
        createdAt: new Date().toISOString(),
        retryCount: 2, // deja 2 echecs, le 3eme = suppression
      }]

      await store.processQueue()

      expect(store.syncQueue).toHaveLength(0)
    })

    it('garde les operations echouees avec moins de 3 tentatives', async () => {
      vi.stubGlobal('$fetch', vi.fn().mockRejectedValue(new Error('500')))

      store.syncQueue = [{
        id: 'test-id',
        method: 'POST',
        url: '/api/test',
        body: {},
        createdAt: new Date().toISOString(),
        retryCount: 0,
      }]

      await store.processQueue()

      expect(store.syncQueue).toHaveLength(1)
      expect(store.syncQueue[0].retryCount).toBe(1)
    })
  })

  // ─── clearQueue ───
  describe('clearQueue()', () => {
    it('vide la queue', () => {
      store.enqueue({ method: 'POST', url: '/api/test', body: {} })
      store.clearQueue()
      expect(store.syncQueue).toEqual([])
    })
  })
})
