import { defineStore } from 'pinia'

export interface SyncOperation {
  id: string
  method: 'POST' | 'PUT' | 'DELETE'
  url: string
  body?: unknown
  createdAt: string
  retryCount: number
}

interface OfflineState {
  isOnline: boolean
  syncQueue: SyncOperation[]
  lastSyncAt: string | null
  syncInProgress: boolean
}

export const useOfflineStore = defineStore('offline', {
  state: (): OfflineState => ({
    isOnline: true,
    syncQueue: [],
    lastSyncAt: null,
    syncInProgress: false,
  }),

  getters: {
    pendingCount: (state): number => state.syncQueue.length,
    hasPendingSync: (state): boolean => state.syncQueue.length > 0,
  },

  actions: {
    initNetworkListener(): void {
      if (import.meta.server) return

      this.isOnline = navigator.onLine

      window.addEventListener('online', () => {
        this.isOnline = true
        this.processQueue()
      })

      window.addEventListener('offline', () => {
        this.isOnline = false
      })
    },

    enqueue(op: Omit<SyncOperation, 'id' | 'createdAt' | 'retryCount'>): void {
      this.syncQueue.push({
        ...op,
        id: crypto.randomUUID(),
        createdAt: new Date().toISOString(),
        retryCount: 0,
      })
    },

    async processQueue(): Promise<void> {
      if (!this.isOnline || this.syncInProgress || this.syncQueue.length === 0) return

      this.syncInProgress = true
      const processed: string[] = []

      for (const op of this.syncQueue) {
        try {
          await $fetch(op.url, {
            method: op.method,
            body: op.body,
          })
          processed.push(op.id)
        }
        catch {
          op.retryCount++
          if (op.retryCount >= 3) {
            processed.push(op.id)
          }
        }
      }

      this.syncQueue = this.syncQueue.filter(op => !processed.includes(op.id))
      this.lastSyncAt = new Date().toISOString()
      this.syncInProgress = false
    },

    clearQueue(): void {
      this.syncQueue = []
    },
  },

  persist: true,
})
