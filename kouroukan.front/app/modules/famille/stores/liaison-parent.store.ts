import { defineStore } from 'pinia'
import type { LiaisonParent } from '../types/famille.types'
import { apiClient } from '~/core/api/client'

const API_PATH = '/api/inscriptions/liaisons-parent'

interface LiaisonParentState {
  items: LiaisonParent[]
  loading: boolean
}

export const useLiaisonParentStore = defineStore('famille-liaison-parent', {
  state: (): LiaisonParentState => ({
    items: [],
    loading: false,
  }),

  getters: {
    isEmpty: (state): boolean => state.items.length === 0,
  },

  actions: {
    async fetchAll(): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.get<LiaisonParent[]>(API_PATH)
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
