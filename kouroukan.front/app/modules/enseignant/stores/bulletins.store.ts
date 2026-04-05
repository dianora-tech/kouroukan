import { defineStore } from 'pinia'
import type { BulletinEnseignant } from '../types/enseignant.types'
import { apiClient } from '~/core/api/client'

interface BulletinsState {
  items: BulletinEnseignant[]
  loading: boolean
}

export const useBulletinsStore = defineStore('enseignant-bulletins', {
  state: (): BulletinsState => ({
    items: [],
    loading: false,
  }),

  actions: {
    async fetchAll(): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.get<BulletinEnseignant[]>('/api/evaluations/bulletins')
        if (response.success && response.data) {
          this.items = response.data
        }
      }
      finally {
        this.loading = false
      }
    },
  },
})
