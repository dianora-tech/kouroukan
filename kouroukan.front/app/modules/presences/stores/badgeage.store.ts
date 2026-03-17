import { defineStore } from 'pinia'
import { apiClient } from '~/core/api/client'
import type {
  Badgeage,
  BadgeageFilters,
  BadgeageType,
  CreateBadgeagePayload,
  UpdateBadgeagePayload,
} from '../types/badgeage.types'

const API_PATH = '/api/presences/badgeages'

interface BadgeageState {
  items: Badgeage[]
  currentItem: Badgeage | null
  types: BadgeageType[]
  loading: boolean
  saving: boolean
  filters: BadgeageFilters
  pagination: {
    page: number
    pageSize: number
    totalCount: number
    totalPages: number
  }
}

export const useBadgeageStore = defineStore('presences-badgeage', {
  state: (): BadgeageState => ({
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
      !!(state.filters.search || state.filters.typeId
        || state.filters.eleveId || state.filters.pointAcces
        || state.filters.methodeBadgeage
        || state.filters.dateFrom || state.filters.dateTo),
  },

  actions: {
    async fetchAll(params?: Partial<BadgeageFilters & { page?: number; pageSize?: number }>): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.getPaginated<Badgeage>(API_PATH, {
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

    async fetchById(id: number): Promise<Badgeage | null> {
      this.loading = true
      try {
        const response = await apiClient.get<Badgeage>(`${API_PATH}/${id}`)
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
        const response = await apiClient.get<BadgeageType[]>(`${API_PATH}/types`)
        if (response.success && response.data) {
          this.types = response.data
        }
      }
      catch {
        // Types will remain empty
      }
    },

    async create(payload: CreateBadgeagePayload): Promise<Badgeage | null> {
      this.saving = true
      try {
        const response = await apiClient.post<Badgeage>(API_PATH, payload)
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

    async update(id: number, payload: UpdateBadgeagePayload): Promise<Badgeage | null> {
      this.saving = true
      try {
        const response = await apiClient.put<Badgeage>(`${API_PATH}/${id}`, payload)
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

    setFilters(filters: BadgeageFilters): void {
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
