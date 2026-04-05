import { defineStore } from 'pinia'
import type { SeanceFamille } from '../types/famille.types'
import { apiClient } from '~/core/api/client'

interface EmploiDuTempsState {
  items: SeanceFamille[]
  loading: boolean
}

export const useEmploiDuTempsStore = defineStore('famille-emploi-du-temps', {
  state: (): EmploiDuTempsState => ({
    items: [],
    loading: false,
  }),

  actions: {
    async fetchByEnfant(enfantId: number): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.get<SeanceFamille[]>(`/api/pedagogie/seances?eleveId=${enfantId}`)
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
