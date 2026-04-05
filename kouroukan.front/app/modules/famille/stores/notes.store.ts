import { defineStore } from 'pinia'
import type { NoteFamille } from '../types/famille.types'
import { apiClient } from '~/core/api/client'

interface NotesState {
  items: NoteFamille[]
  loading: boolean
}

export const useNotesStore = defineStore('famille-notes', {
  state: (): NotesState => ({
    items: [],
    loading: false,
  }),

  actions: {
    async fetchByEnfant(enfantId: number): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.get<NoteFamille[]>(`/api/evaluations/notes?eleveId=${enfantId}`)
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
