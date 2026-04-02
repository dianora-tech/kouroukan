import { defineStore } from 'pinia'
import { apiClient } from '~/core/api/client'
import type {
  ContenuIA,
  CreateContenuIAPayload,
  UpdateContenuIAPayload,
  AdminFilters,
} from '../types/admin.types'

const API_PATH = '/api/admin/contenu-ia'

interface ContenuIAState {
  items: ContenuIA[]
  currentItem: ContenuIA | null
  loading: boolean
  saving: boolean
  filters: AdminFilters
  pagination: {
    page: number
    pageSize: number
    totalCount: number
    totalPages: number
  }
}

export const useContenuIAStore = defineStore('admin-contenu-ia', {
  state: (): ContenuIAState => ({
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
  },

  actions: {
    async fetchAll(params?: Partial<AdminFilters & { page?: number; pageSize?: number }>): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.getPaginated<ContenuIA>(API_PATH, {
          page: params?.page ?? this.pagination.page,
          pageSize: params?.pageSize ?? this.pagination.pageSize,
          search: params?.search ?? this.filters.search,
        })
        if (response.success && response.data) {
          this.items = response.data.items
          const totalCount = response.data.totalCount ?? 0
          const ps = response.data.pageSize ?? this.pagination.pageSize
          this.pagination = {
            page: response.data.pageNumber ?? response.data.page ?? 1,
            pageSize: ps,
            totalCount,
            totalPages: response.data.totalPages ?? (Math.ceil(totalCount / ps) || 1),
          }
        }
      }
      finally {
        this.loading = false
      }
    },

    async fetchById(id: number): Promise<ContenuIA | null> {
      this.loading = true
      try {
        const response = await apiClient.get<ContenuIA>(`${API_PATH}/${id}`)
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

    async create(payload: CreateContenuIAPayload): Promise<ContenuIA | null> {
      this.saving = true
      try {
        const response = await apiClient.post<ContenuIA>(API_PATH, payload)
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

    async update(id: number, payload: UpdateContenuIAPayload): Promise<ContenuIA | null> {
      this.saving = true
      try {
        const response = await apiClient.put<ContenuIA>(`${API_PATH}/${id}`, payload)
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

    setFilters(filters: AdminFilters): void {
      this.filters = { ...filters }
      this.pagination.page = 1
    },

    resetFilters(): void {
      this.filters = {}
      this.pagination.page = 1
    },
  },
})
