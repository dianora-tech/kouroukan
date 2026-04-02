import { defineStore } from 'pinia'
import type { Matiere, MatiereType, MatiereFilters, CreateMatierePayload, UpdateMatierePayload } from '../types/matiere.types'
import { apiClient } from '~/core/api/client'

const API_PATH = '/api/pedagogie/matieres'

interface MatiereState {
  items: Matiere[]
  currentItem: Matiere | null
  types: MatiereType[]
  loading: boolean
  saving: boolean
  filters: MatiereFilters
  pagination: {
    page: number
    pageSize: number
    totalCount: number
    totalPages: number
  }
}

export const useMatiereStore = defineStore('pedagogie-matiere', {
  state: (): MatiereState => ({
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
      !!(state.filters.search || state.filters.typeId || state.filters.niveauClasseId),
  },

  actions: {
    async fetchAll(params?: Partial<MatiereFilters & { page?: number, pageSize?: number }>): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.getPaginated<Matiere>(API_PATH, {
          page: params?.page ?? this.pagination.page,
          pageSize: params?.pageSize ?? this.pagination.pageSize,
          search: params?.search ?? this.filters.search,
          orderBy: 'name',
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

    async fetchById(id: number): Promise<Matiere | null> {
      this.loading = true
      try {
        const response = await apiClient.get<Matiere>(`${API_PATH}/${id}`)
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
        const response = await apiClient.get<MatiereType[]>(`${API_PATH}/types`)
        if (response.success && response.data) {
          this.types = response.data
        }
      }
      catch {
        // Types loading failure is non-critical
      }
    },

    async create(payload: CreateMatierePayload): Promise<Matiere | null> {
      this.saving = true
      try {
        const response = await apiClient.post<Matiere>(API_PATH, payload)
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

    async update(id: number, payload: UpdateMatierePayload): Promise<Matiere | null> {
      this.saving = true
      try {
        const response = await apiClient.put<Matiere>(`${API_PATH}/${id}`, payload)
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

    setFilters(filters: MatiereFilters): void {
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
