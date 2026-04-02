import { defineStore } from 'pinia'
import type {
  Souscription,
  SouscriptionFilters,
  CreateSouscriptionPayload,
  UpdateSouscriptionPayload,
} from '../types/souscription.types'
import { apiClient } from '~/core/api/client'

const API_PATH = '/api/services-premium/souscriptions'

interface SouscriptionState {
  items: Souscription[]
  currentItem: Souscription | null
  loading: boolean
  saving: boolean
  filters: SouscriptionFilters
  pagination: {
    page: number
    pageSize: number
    totalCount: number
    totalPages: number
  }
}

export const useSouscriptionStore = defineStore('services-premium-souscription', {
  state: (): SouscriptionState => ({
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
      !!(state.filters.search || state.filters.serviceParentId
        || state.filters.statutSouscription || state.filters.dateFrom || state.filters.dateTo),
  },

  actions: {
    async fetchAll(params?: Partial<SouscriptionFilters & { page?: number, pageSize?: number }>): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.getPaginated<Souscription>(API_PATH, {
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

    async fetchById(id: number): Promise<Souscription | null> {
      this.loading = true
      try {
        const response = await apiClient.get<Souscription>(`${API_PATH}/${id}`)
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

    async create(payload: CreateSouscriptionPayload): Promise<Souscription | null> {
      this.saving = true
      try {
        const response = await apiClient.post<Souscription>(API_PATH, payload)
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

    async update(id: number, payload: UpdateSouscriptionPayload): Promise<Souscription | null> {
      this.saving = true
      try {
        const response = await apiClient.put<Souscription>(`${API_PATH}/${id}`, payload)
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

    setFilters(filters: SouscriptionFilters): void {
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
