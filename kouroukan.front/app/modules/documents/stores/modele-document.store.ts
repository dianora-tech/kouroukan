import { defineStore } from 'pinia'
import type {
  ModeleDocument,
  ModeleDocumentFilters,
  ModeleDocumentType,
  CreateModeleDocumentPayload,
  UpdateModeleDocumentPayload,
} from '../types/modele-document.types'
import { apiClient } from '~/core/api/client'

const API_PATH = '/api/documents/modelesdocuments'

interface ModeleDocumentState {
  items: ModeleDocument[]
  currentItem: ModeleDocument | null
  types: ModeleDocumentType[]
  loading: boolean
  saving: boolean
  filters: ModeleDocumentFilters
  pagination: {
    page: number
    pageSize: number
    totalCount: number
    totalPages: number
  }
}

export const useModeleDocumentStore = defineStore('documents-modele-document', {
  state: (): ModeleDocumentState => ({
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
      !!(state.filters.search || state.filters.typeId || state.filters.estActif !== undefined),
  },

  actions: {
    async fetchAll(params?: Partial<ModeleDocumentFilters & { page?: number, pageSize?: number }>): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.getPaginated<ModeleDocument>(API_PATH, {
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

    async fetchById(id: number): Promise<ModeleDocument | null> {
      this.loading = true
      try {
        const response = await apiClient.get<ModeleDocument>(`${API_PATH}/${id}`)
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
        const response = await apiClient.get<ModeleDocumentType[]>(`${API_PATH}/types`)
        if (response.success && response.data) {
          this.types = response.data
        }
      }
      catch {
        // Types will remain empty
      }
    },

    async create(payload: CreateModeleDocumentPayload): Promise<ModeleDocument | null> {
      this.saving = true
      try {
        const response = await apiClient.post<ModeleDocument>(API_PATH, payload)
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

    async update(id: number, payload: UpdateModeleDocumentPayload): Promise<ModeleDocument | null> {
      this.saving = true
      try {
        const response = await apiClient.put<ModeleDocument>(`${API_PATH}/${id}`, payload)
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

    setFilters(filters: ModeleDocumentFilters): void {
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
