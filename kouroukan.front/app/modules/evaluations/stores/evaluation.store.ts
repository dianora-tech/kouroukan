import { defineStore } from 'pinia'
import type {
  Evaluation,
  EvaluationFilters,
  EvaluationType,
  CreateEvaluationPayload,
  UpdateEvaluationPayload,
} from '../types/evaluation.types'
import { apiClient } from '~/core/api/client'

const API_PATH = '/api/evaluations/evaluations'

interface EvaluationState {
  items: Evaluation[]
  currentItem: Evaluation | null
  types: EvaluationType[]
  loading: boolean
  saving: boolean
  filters: EvaluationFilters
  pagination: {
    page: number
    pageSize: number
    totalCount: number
    totalPages: number
  }
}

export const useEvaluationStore = defineStore('evaluations-evaluation', {
  state: (): EvaluationState => ({
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
      !!(state.filters.search || state.filters.typeId || state.filters.matiereId
        || state.filters.classeId || state.filters.enseignantId
        || state.filters.trimestre || state.filters.anneeScolaireId
        || state.filters.dateFrom || state.filters.dateTo),
  },

  actions: {
    async fetchAll(params?: Partial<EvaluationFilters & { page?: number, pageSize?: number }>): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.getPaginated<Evaluation>(API_PATH, {
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

    async fetchById(id: number): Promise<Evaluation | null> {
      this.loading = true
      try {
        const response = await apiClient.get<Evaluation>(`${API_PATH}/${id}`)
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
        const response = await apiClient.get<EvaluationType[]>(`${API_PATH}/types`)
        if (response.success && response.data) {
          this.types = response.data
        }
      }
      catch {
        // Types will remain empty
      }
    },

    async create(payload: CreateEvaluationPayload): Promise<Evaluation | null> {
      this.saving = true
      try {
        const response = await apiClient.post<Evaluation>(API_PATH, payload)
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

    async update(id: number, payload: UpdateEvaluationPayload): Promise<Evaluation | null> {
      this.saving = true
      try {
        const response = await apiClient.put<Evaluation>(`${API_PATH}/${id}`, payload)
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

    setFilters(filters: EvaluationFilters): void {
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
