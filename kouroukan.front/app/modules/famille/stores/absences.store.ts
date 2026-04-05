import { defineStore } from 'pinia'
import type { AbsenceFamille } from '../types/famille.types'
import { apiClient } from '~/core/api/client'

interface AbsencesState {
  items: AbsenceFamille[]
  loading: boolean
}

export const useAbsencesStore = defineStore('famille-absences', {
  state: (): AbsencesState => ({
    items: [],
    loading: false,
  }),

  actions: {
    async fetchByEnfant(enfantId: number): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.get<AbsenceFamille[]>(`/api/presences/absences?eleveId=${enfantId}`)
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
