import { defineStore } from 'pinia'
import type {
  JournalFinancier,
  CreateJournalFinancierPayload,
} from '../types/famille.types'
import { apiClient } from '~/core/api/client'

const API_PATH = '/api/finances/journal'

interface JournalFinancierState {
  items: JournalFinancier[]
  loading: boolean
  saving: boolean
  pagination: {
    page: number
    pageSize: number
    totalCount: number
    totalPages: number
  }
}

export const useJournalFinancierStore = defineStore('famille-journal-financier', {
  state: (): JournalFinancierState => ({
    items: [],
    loading: false,
    saving: false,
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
    async fetchAll(params?: { page?: number, pageSize?: number, enfantId?: number }): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.getPaginated<JournalFinancier>(API_PATH, {
          page: params?.page ?? this.pagination.page,
          pageSize: params?.pageSize ?? this.pagination.pageSize,
          search: params?.enfantId ? String(params.enfantId) : undefined,
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

    async create(payload: CreateJournalFinancierPayload): Promise<JournalFinancier | null> {
      this.saving = true
      try {
        const response = await apiClient.post<JournalFinancier>(API_PATH, payload)
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
  },
})
