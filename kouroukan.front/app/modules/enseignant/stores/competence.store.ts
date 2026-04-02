import { defineStore } from 'pinia'
import { apiClient } from '~/core/api/client'
import type {
  Competence,
  CreateCompetencePayload,
} from '../types/enseignant.types'

const API_PATH = '/api/pedagogie/competences-enseignant'

interface CompetenceState {
  items: Competence[]
  loading: boolean
  saving: boolean
}

export const useCompetenceStore = defineStore('enseignant-competence', {
  state: (): CompetenceState => ({
    items: [],
    loading: false,
    saving: false,
  }),

  getters: {
    isEmpty: (state): boolean => state.items.length === 0,
  },

  actions: {
    async fetchAll(params?: { userId?: number }): Promise<void> {
      this.loading = true
      try {
        const url = params?.userId ? `${API_PATH}?userId=${params.userId}` : API_PATH
        const response = await apiClient.get<Competence[]>(url)
        if (response.success && response.data) {
          this.items = response.data
        }
      }
      finally {
        this.loading = false
      }
    },

    async create(payload: CreateCompetencePayload): Promise<Competence | null> {
      this.saving = true
      try {
        const response = await apiClient.post<Competence>(API_PATH, payload)
        if (response.success && response.data) {
          await this.fetchAll()
          return response.data
        }
        return null
      }
      finally {
        this.saving = false
      }
    },

    async remove(id: number): Promise<boolean> {
      this.saving = true
      try {
        const response = await apiClient.delete(`${API_PATH}/${id}`)
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
