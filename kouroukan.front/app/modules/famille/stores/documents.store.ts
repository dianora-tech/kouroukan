import { defineStore } from 'pinia'
import type { DocumentFamille } from '../types/famille.types'
import { apiClient } from '~/core/api/client'

interface DocumentsState {
  items: DocumentFamille[]
  loading: boolean
}

export const useDocumentsStore = defineStore('famille-documents', {
  state: (): DocumentsState => ({
    items: [],
    loading: false,
  }),

  actions: {
    async fetchByEnfant(enfantId: number): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.get<DocumentFamille[]>(`/api/documents/documents-generes?eleveId=${enfantId}`)
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
