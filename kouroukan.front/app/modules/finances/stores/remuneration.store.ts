import { defineStore } from 'pinia'
import type {
  RemunerationEnseignant,
  RemunerationFilters,
  CreateRemunerationPayload,
  UpdateRemunerationPayload,
} from '../types/remuneration.types'
import { apiClient } from '~/core/api/client'

const API_PATH = '/api/finances/remunerations-enseignants'

interface RemunerationState {
  items: RemunerationEnseignant[]
  currentItem: RemunerationEnseignant | null
  loading: boolean
  saving: boolean
  filters: RemunerationFilters
  pagination: {
    page: number
    pageSize: number
    totalCount: number
    totalPages: number
  }
}

export const useRemunerationStore = defineStore('finances-remuneration', {
  state: (): RemunerationState => ({
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
      !!(state.filters.search || state.filters.mois || state.filters.annee
        || state.filters.modeRemuneration || state.filters.statutPaiement),
  },

  actions: {
    async fetchAll(params?: Partial<RemunerationFilters & { page?: number, pageSize?: number }>): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.getPaginated<RemunerationEnseignant>(API_PATH, {
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

    async fetchById(id: number): Promise<RemunerationEnseignant | null> {
      this.loading = true
      try {
        const response = await apiClient.get<RemunerationEnseignant>(`${API_PATH}/${id}`)
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

    async create(payload: CreateRemunerationPayload): Promise<RemunerationEnseignant | null> {
      this.saving = true
      try {
        const response = await apiClient.post<RemunerationEnseignant>(API_PATH, payload)
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

    async update(id: number, payload: UpdateRemunerationPayload): Promise<RemunerationEnseignant | null> {
      this.saving = true
      try {
        const response = await apiClient.put<RemunerationEnseignant>(`${API_PATH}/${id}`, payload)
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

    setFilters(filters: RemunerationFilters): void {
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
