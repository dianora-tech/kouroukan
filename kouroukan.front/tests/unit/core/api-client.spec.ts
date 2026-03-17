import { describe, it, expect, vi, beforeEach } from 'vitest'
import { ApiClient } from '~/core/api/client'

// Mock useOfflineStore
vi.mock('~/core/stores/offline.store', () => ({
  useOfflineStore: vi.fn(),
}))

import { useOfflineStore } from '~/core/stores/offline.store'

describe('ApiClient', () => {
  let client: ApiClient

  beforeEach(() => {
    vi.clearAllMocks()
    client = new ApiClient()

    // Default: online
    vi.mocked(useOfflineStore).mockReturnValue({
      isOnline: true,
      enqueue: vi.fn(),
    } as any)
  })

  // ─── Intercepteur auth ───
  describe('headers', () => {
    it('inclut Content-Type application/json', async () => {
      vi.stubGlobal('$fetch', vi.fn().mockResolvedValue({ success: true, data: {} }))

      await client.get('/api/test')

      expect($fetch).toHaveBeenCalledWith('/api/test', expect.objectContaining({
        headers: expect.objectContaining({
          'Content-Type': 'application/json',
        }),
      }))
    })
  })

  // ─── Intercepteur retry ───
  describe('retry sur erreur 500', () => {
    it('reessaie 3 fois sur erreur serveur', async () => {
      const error = { statusCode: 500 }
      vi.stubGlobal('$fetch', vi.fn()
        .mockRejectedValueOnce(error)
        .mockRejectedValueOnce(error)
        .mockResolvedValueOnce({ success: true, data: 'ok' }),
      )

      const result = await client.get('/api/test')

      expect($fetch).toHaveBeenCalledTimes(3)
      expect(result.data).toBe('ok')
    })

    it('ne reessaie pas sur erreur client 400', async () => {
      vi.stubGlobal('$fetch', vi.fn().mockRejectedValue({ statusCode: 400 }))

      await expect(client.get('/api/test')).rejects.toEqual({ statusCode: 400 })
      expect($fetch).toHaveBeenCalledTimes(1)
    })

    it('ne reessaie pas sur erreur 404', async () => {
      vi.stubGlobal('$fetch', vi.fn().mockRejectedValue({ statusCode: 404 }))

      await expect(client.get('/api/test')).rejects.toEqual({ statusCode: 404 })
      expect($fetch).toHaveBeenCalledTimes(1)
    })
  })

  // ─── Intercepteur offline ───
  describe('mode offline', () => {
    it('lance une erreur sur GET quand offline', async () => {
      vi.mocked(useOfflineStore).mockReturnValue({
        isOnline: false,
        enqueue: vi.fn(),
      } as any)

      await expect(client.get('/api/test')).rejects.toThrow('offline')
    })

    it('enqueue un POST quand offline', async () => {
      const enqueueMock = vi.fn()
      vi.mocked(useOfflineStore).mockReturnValue({
        isOnline: false,
        enqueue: enqueueMock,
      } as any)

      const result = await client.post('/api/test', { name: 'test' })

      expect(enqueueMock).toHaveBeenCalledWith({
        method: 'POST',
        url: '/api/test',
        body: { name: 'test' },
      })
      expect(result.message).toBe('queued_for_sync')
    })

    it('enqueue un PUT quand offline', async () => {
      const enqueueMock = vi.fn()
      vi.mocked(useOfflineStore).mockReturnValue({
        isOnline: false,
        enqueue: enqueueMock,
      } as any)

      const result = await client.put('/api/test/1', { name: 'updated' })

      expect(enqueueMock).toHaveBeenCalledWith({
        method: 'PUT',
        url: '/api/test/1',
        body: { name: 'updated' },
      })
    })

    it('enqueue un DELETE quand offline', async () => {
      const enqueueMock = vi.fn()
      vi.mocked(useOfflineStore).mockReturnValue({
        isOnline: false,
        enqueue: enqueueMock,
      } as any)

      await client.delete('/api/test/1')

      expect(enqueueMock).toHaveBeenCalledWith({
        method: 'DELETE',
        url: '/api/test/1',
      })
    })
  })

  // ─── Intercepteur CGU ───
  describe('intercepteur CGU', () => {
    it('redirige vers /support/cgu si code CGU_NOT_ACCEPTED', async () => {
      vi.stubGlobal('$fetch', vi.fn().mockResolvedValue({
        success: false,
        data: null,
        code: 'CGU_NOT_ACCEPTED',
      }))

      await client.get('/api/test')

      expect(navigateTo).toHaveBeenCalledWith('/support/cgu')
    })

    it('ne redirige pas si le code est absent', async () => {
      vi.stubGlobal('$fetch', vi.fn().mockResolvedValue({
        success: true,
        data: {},
      }))

      await client.get('/api/test')

      expect(navigateTo).not.toHaveBeenCalled()
    })
  })

  // ─── getPaginated ───
  describe('getPaginated()', () => {
    it('construit la query string correctement', async () => {
      vi.stubGlobal('$fetch', vi.fn().mockResolvedValue({
        success: true,
        data: { items: [], totalCount: 0, pageNumber: 1, pageSize: 20, totalPages: 0, hasNextPage: false, hasPreviousPage: false },
      }))

      await client.getPaginated('/api/inscriptions', { page: 2, pageSize: 10, search: 'test' })

      expect($fetch).toHaveBeenCalledWith(
        expect.stringContaining('page=2'),
        expect.any(Object),
      )
    })
  })
})
