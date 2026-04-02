import { defineStore } from 'pinia'
import type { Annonce, AnnonceFilters, CreateAnnoncePayload, UpdateAnnoncePayload } from '../types/annonce.types'
import { apiClient } from '~/core/api/client'
import type { PaginatedResult } from '~/core/api/types'

const API_PATH = '/api/communication/annonces'

interface AnnonceState {
  items: Annonce[]
  currentItem: Annonce | null
  types: { id: number, name: string }[]
  loading: boolean
  saving: boolean
  filters: AnnonceFilters
  pagination: {
    page: number
    pageSize: number
    totalCount: number
    totalPages: number
  }
}

export const useAnnonceStore = defineStore('communication-annonce', {
  state: (): AnnonceState => ({
    items: [],
    currentItem: null,
    types: [],
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
      !!(state.filters.search || state.filters.typeId || state.filters.estActive !== undefined
        || state.filters.cibleAudience || state.filters.priorite
        || state.filters.dateFrom || state.filters.dateTo),
  },

  actions: {
    async fetchAll(params?: Partial<AnnonceFilters & { page?: number, pageSize?: number }>): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.getPaginated<Annonce>(API_PATH, {
          page: params?.page ?? this.pagination.page,
          pageSize: params?.pageSize ?? this.pagination.pageSize,
          search: params?.search ?? this.filters.search,
          orderBy: 'createdAt',
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

    async fetchById(id: number): Promise<Annonce | null> {
      this.loading = true
      try {
        const response = await apiClient.get<Annonce>(`${API_PATH}/${id}`)
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

    async fetchTypes(): Promise<void> {
      try {
        const response = await apiClient.get<{ id: number, name: string }[]>(`${API_PATH}/types`)
        if (response.success && response.data) {
          this.types = response.data
        }
      }
      catch {
        // Types loading failure is non-blocking
      }
    },

    async create(payload: CreateAnnoncePayload): Promise<Annonce | null> {
      this.saving = true
      try {
        const response = await apiClient.post<Annonce>(API_PATH, payload)
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

    async update(id: number, payload: UpdateAnnoncePayload): Promise<Annonce | null> {
      this.saving = true
      try {
        const response = await apiClient.put<Annonce>(`${API_PATH}/${id}`, payload)
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

    setFilters(filters: AnnonceFilters): void {
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
