import { defineStore } from 'pinia'
import type { AffectationEnseignant } from '../types/enseignant.types'
import { apiClient } from '~/core/api/client'

const API_PATH = '/api/pedagogie/affectations-enseignant'

interface AffectationState {
  items: AffectationEnseignant[]
  loading: boolean
}

export const useAffectationStore = defineStore('enseignant-affectation', {
  state: (): AffectationState => ({
    items: [],
    loading: false,
  }),

  getters: {
    isEmpty: (state): boolean => state.items.length === 0,
  },

  actions: {
    async fetchAll(params?: { etablissementId?: number }): Promise<void> {
      this.loading = true
      try {
        const url = params?.etablissementId
          ? `${API_PATH}?etablissementId=${params.etablissementId}`
          : API_PATH
        const response = await apiClient.get<AffectationEnseignant[]>(url)
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
