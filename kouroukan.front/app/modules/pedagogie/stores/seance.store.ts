import { defineStore } from 'pinia'
import { apiClient } from '~/core/api/client'
import type { Seance, SeanceFilters, CreateSeancePayload, UpdateSeancePayload } from '../types/seance.types'

const API_PATH = '/api/pedagogie/seances'

interface SeanceState {
  items: Seance[]
  currentItem: Seance | null
  loading: boolean
  saving: boolean
  filters: SeanceFilters
  pagination: {
    page: number
    pageSize: number
    totalCount: number
    totalPages: number
  }
}

export const useSeanceStore = defineStore('pedagogie-seance', {
  state: (): SeanceState => ({
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
      !!(state.filters.search || state.filters.classeId || state.filters.enseignantId
        || state.filters.jourSemaine || state.filters.anneeScolaireId),
  },

  actions: {
    async fetchAll(params?: Partial<SeanceFilters & { page?: number; pageSize?: number }>): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.getPaginated<Seance>(API_PATH, {
          page: params?.page ?? this.pagination.page,
          pageSize: params?.pageSize ?? this.pagination.pageSize,
          search: params?.search ?? this.filters.search,
          orderBy: 'jourSemaine',
          orderDirection: 'asc',
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

    async fetchById(id: number): Promise<Seance | null> {
      this.loading = true
      try {
        const response = await apiClient.get<Seance>(`${API_PATH}/${id}`)
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

    async create(payload: CreateSeancePayload): Promise<Seance | null> {
      this.saving = true
      try {
        const response = await apiClient.post<Seance>(API_PATH, payload)
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

    async update(id: number, payload: UpdateSeancePayload): Promise<Seance | null> {
      this.saving = true
      try {
        const response = await apiClient.put<Seance>(`${API_PATH}/${id}`, payload)
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

    setFilters(filters: SeanceFilters): void {
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
