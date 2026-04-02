import { defineStore } from 'pinia'
import type {
  AnneeScolaire,
  AnneeScolaireFilters,
  CreateAnneeScolairePayload,
  UpdateAnneeScolairePayload,
} from '../types/annee-scolaire.types'
import { apiClient } from '~/core/api/client'

const API_PATH = '/api/inscriptions/annees-scolaires'

interface AnneeScolaireState {
  items: AnneeScolaire[]
  currentItem: AnneeScolaire | null
  loading: boolean
  saving: boolean
  filters: AnneeScolaireFilters
  pagination: {
    page: number
    pageSize: number
    totalCount: number
    totalPages: number
  }
}

export const useAnneeScolaireStore = defineStore('inscriptions-annee-scolaire', {
  state: (): AnneeScolaireState => ({
    items: [],
    currentItem: null,
    loading: false,
    saving: false,
    filters: {},
    pagination: {
      page: 1,
      pageSize: 20,
      totalCount: 0,
      totalPages: 0,
    },
  }),

  getters: {
    isEmpty: (state): boolean => state.items.length === 0,
    hasFilters: (state): boolean =>
      !!(state.filters.search || state.filters.dateFrom || state.filters.dateTo),
    activeYear: (state): AnneeScolaire | undefined =>
      state.items.find(a => a.statut === 'active'),
  },

  actions: {
    async fetchAll(params?: Partial<AnneeScolaireFilters & { page?: number, pageSize?: number }>): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.getPaginated<AnneeScolaire>(API_PATH, {
          page: params?.page ?? this.pagination.page,
          pageSize: params?.pageSize ?? this.pagination.pageSize,
          search: params?.search ?? this.filters.search,
          orderBy: 'date_debut',
          orderDirection: 'desc',
        })
        if (response.success && response.data) {
          this.items = response.data.items
          this.pagination = {
            page: response.data.pageNumber,
            pageSize: response.data.pageSize,
            totalCount: response.data.totalCount,
            totalPages: response.data.totalPages,
          }
        }
      }
      finally {
        this.loading = false
      }
    },

    async fetchById(id: number): Promise<AnneeScolaire | null> {
      this.loading = true
      try {
        const response = await apiClient.get<AnneeScolaire>(`${API_PATH}/${id}`)
        if (response.success && response.data) {
          this.currentItem = response.data
          return response.data
        }
        return null
      }
      finally {
        this.loading = false
      }
    },

    async create(payload: CreateAnneeScolairePayload): Promise<AnneeScolaire | null> {
      this.saving = true
      try {
        const response = await apiClient.post<AnneeScolaire>(API_PATH, payload)
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

    async update(id: number, payload: UpdateAnneeScolairePayload): Promise<AnneeScolaire | null> {
      this.saving = true
      try {
        const response = await apiClient.put<AnneeScolaire>(`${API_PATH}/${id}`, payload)
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

    setFilters(filters: AnneeScolaireFilters): void {
      this.filters = { ...filters }
      this.pagination.page = 1
    },

    resetFilters(): void {
      this.filters = {}
      this.pagination.page = 1
    },
  },

  persist: {
    pick: ['filters'],
  },
})
