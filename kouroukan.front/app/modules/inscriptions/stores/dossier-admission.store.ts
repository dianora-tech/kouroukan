import { defineStore } from 'pinia'
import type {
  DossierAdmission,
  DossierAdmissionFilters,
  DossierAdmissionType,
  CreateDossierAdmissionPayload,
  UpdateDossierAdmissionPayload,
} from '../types/dossier-admission.types'
import { apiClient } from '~/core/api/client'

const API_PATH = '/api/inscriptions/dossiers-admission'

interface DossierAdmissionState {
  items: DossierAdmission[]
  currentItem: DossierAdmission | null
  types: DossierAdmissionType[]
  loading: boolean
  saving: boolean
  filters: DossierAdmissionFilters
  pagination: {
    page: number
    pageSize: number
    totalCount: number
    totalPages: number
  }
}

export const useDossierAdmissionStore = defineStore('inscriptions-dossier-admission', {
  state: (): DossierAdmissionState => ({
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
      !!(state.filters.search || state.filters.typeId || state.filters.statutDossier
        || state.filters.anneeScolaireId || state.filters.dateFrom || state.filters.dateTo),
  },

  actions: {
    async fetchAll(params?: Partial<DossierAdmissionFilters & { page?: number, pageSize?: number }>): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.getPaginated<DossierAdmission>(API_PATH, {
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

    async fetchById(id: number): Promise<DossierAdmission | null> {
      this.loading = true
      try {
        const response = await apiClient.get<DossierAdmission>(`${API_PATH}/${id}`)
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
        const response = await apiClient.get<DossierAdmissionType[]>(`${API_PATH}/types`)
        if (response.success && response.data) {
          this.types = response.data
        }
      }
      catch {
        // Types will remain empty
      }
    },

    async create(payload: CreateDossierAdmissionPayload): Promise<DossierAdmission | null> {
      this.saving = true
      try {
        const response = await apiClient.post<DossierAdmission>(API_PATH, payload)
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

    async update(id: number, payload: UpdateDossierAdmissionPayload): Promise<DossierAdmission | null> {
      this.saving = true
      try {
        const response = await apiClient.put<DossierAdmission>(`${API_PATH}/${id}`, payload)
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

    setFilters(filters: DossierAdmissionFilters): void {
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
