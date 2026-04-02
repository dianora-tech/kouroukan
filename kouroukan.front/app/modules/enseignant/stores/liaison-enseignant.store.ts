import { defineStore } from 'pinia'
import { apiClient } from '~/core/api/client'
import type { LiaisonEnseignant } from '../types/enseignant.types'

const API_PATH = '/api/auth/liaisons-enseignant'

interface LiaisonEnseignantState {
  items: LiaisonEnseignant[]
  loading: boolean
  saving: boolean
}

export const useLiaisonEnseignantStore = defineStore('enseignant-liaison', {
  state: (): LiaisonEnseignantState => ({
    items: [],
    loading: false,
    saving: false,
  }),

  getters: {
    isEmpty: (state): boolean => state.items.length === 0,
    actives: (state): LiaisonEnseignant[] =>
      state.items.filter(l => l.statut === 'accepted' || l.statut === 'pending'),
    historique: (state): LiaisonEnseignant[] =>
      state.items.filter(l => l.statut === 'terminated'),
    pending: (state): LiaisonEnseignant[] =>
      state.items.filter(l => l.statut === 'pending'),
  },

  actions: {
    async fetchAll(): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.get<LiaisonEnseignant[]>(API_PATH)
        if (response.success && response.data) {
          this.items = response.data
        }
      }
      finally {
        this.loading = false
      }
    },

    async accept(id: number): Promise<boolean> {
      this.saving = true
      try {
        const response = await apiClient.put<LiaisonEnseignant>(`${API_PATH}/${id}/accept`, {})
        if (response.success) {
          await this.fetchAll()
          return true
        }
        return false
      }
      finally {
        this.saving = false
      }
    },

    async reject(id: number): Promise<boolean> {
      this.saving = true
      try {
        const response = await apiClient.put<LiaisonEnseignant>(`${API_PATH}/${id}/reject`, {})
        if (response.success) {
          await this.fetchAll()
          return true
        }
        return false
      }
      finally {
        this.saving = false
      }
    },
  },
})
