import { defineStore } from 'pinia'
import type { HeureEnseignant } from '../types/enseignant.types'
import { apiClient } from '~/core/api/client'

interface HeuresState {
  items: HeureEnseignant[]
  loading: boolean
}

export const useHeuresStore = defineStore('enseignant-heures', {
  state: (): HeuresState => ({
    items: [],
    loading: false,
  }),

  getters: {
    totalHeuresMois: (state): number => {
      return state.items.reduce((sum, h) => sum + h.duree, 0)
    },
  },

  actions: {
    async fetchAll(params?: { etablissementId?: number }): Promise<void> {
      this.loading = true
      try {
        const url = params?.etablissementId
          ? `/api/pedagogie/seances?etablissementId=${params.etablissementId}`
          : '/api/pedagogie/seances'
        const response = await apiClient.get<HeureEnseignant[]>(url)
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
