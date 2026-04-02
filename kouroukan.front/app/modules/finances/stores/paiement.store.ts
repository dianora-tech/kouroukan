import { defineStore } from 'pinia'
import type {
  Paiement,
  PaiementFilters,
  PaiementType,
  CreatePaiementPayload,
  UpdatePaiementPayload,
} from '../types/paiement.types'
import { apiClient } from '~/core/api/client'

const API_PATH = '/api/finances/paiements'

interface PaiementState {
  items: Paiement[]
  currentItem: Paiement | null
  types: PaiementType[]
  loading: boolean
  saving: boolean
  filters: PaiementFilters
  pagination: {
    page: number
    pageSize: number
    totalCount: number
    totalPages: number
  }
}

export const usePaiementStore = defineStore('finances-paiement', {
  state: (): PaiementState => ({
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
      !!(state.filters.search || state.filters.typeId || state.filters.moyenPaiement
        || state.filters.statutPaiement || state.filters.dateFrom || state.filters.dateTo),
  },

  actions: {
    async fetchAll(params?: Partial<PaiementFilters & { page?: number, pageSize?: number }>): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.getPaginated<Paiement>(API_PATH, {
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

    async fetchById(id: number): Promise<Paiement | null> {
      this.loading = true
      try {
        const response = await apiClient.get<Paiement>(`${API_PATH}/${id}`)
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
        const response = await apiClient.get<PaiementType[]>(`${API_PATH}/types`)
        if (response.success && response.data) {
          this.types = response.data
        }
      }
      catch {
        // Types will remain empty
      }
    },

    async create(payload: CreatePaiementPayload): Promise<Paiement | null> {
      this.saving = true
      try {
        const response = await apiClient.post<Paiement>(API_PATH, payload)
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

    async update(id: number, payload: UpdatePaiementPayload): Promise<Paiement | null> {
      this.saving = true
      try {
        const response = await apiClient.put<Paiement>(`${API_PATH}/${id}`, payload)
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

    setFilters(filters: PaiementFilters): void {
      this.filters = { ...filters }
      this.pagination.page = 1
    },

    resetFilters(): void {
      this.filters = {}
      this.pagination.page = 1
    },
  },

  persist: {
    pick: ['filters', 'types'],
  },
})
