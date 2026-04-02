import { defineStore } from 'pinia'
import type {
  MoyenPaiement,
  CreateMoyenPaiementPayload,
  UpdateMoyenPaiementPayload,
  FinancesFilters,
} from '../types/finances.types'
import { apiClient } from '~/core/api/client'

const API_PATH = '/api/finances/moyens-paiement'

interface MoyenPaiementState {
  items: MoyenPaiement[]
  currentItem: MoyenPaiement | null
  loading: boolean
  saving: boolean
  filters: FinancesFilters
  pagination: {
    page: number
    pageSize: number
    totalCount: number
    totalPages: number
  }
}

export const useMoyenPaiementStore = defineStore('finances-moyen-paiement', {
  state: (): MoyenPaiementState => ({
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
    async fetchAll(params?: Partial<FinancesFilters & { page?: number, pageSize?: number }>): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.getPaginated<MoyenPaiement>(API_PATH, {
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

    async create(payload: CreateMoyenPaiementPayload): Promise<MoyenPaiement | null> {
      this.saving = true
      try {
        const response = await apiClient.post<MoyenPaiement>(API_PATH, payload)
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

    async update(id: number, payload: UpdateMoyenPaiementPayload): Promise<MoyenPaiement | null> {
      this.saving = true
      try {
        const response = await apiClient.put<MoyenPaiement>(`${API_PATH}/${id}`, payload)
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

    setFilters(filters: FinancesFilters): void {
      this.filters = { ...filters }
      this.pagination.page = 1
    },

    resetFilters(): void {
      this.filters = {}
      this.pagination.page = 1
    },
  },
})
