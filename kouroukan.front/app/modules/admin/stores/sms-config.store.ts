import { defineStore } from 'pinia'
import type {
  SmsConfig,
  UpdateSmsConfigPayload,
  SendTestSmsPayload,
  SmsEnvoi,
} from '../types/admin.types'
import { apiClient } from '~/core/api/client'

const API_PATH = '/api/admin/sms-config'

interface SmsConfigState {
  config: SmsConfig | null
  historique: SmsEnvoi[]
  loading: boolean
  saving: boolean
  testSending: boolean
  syncing: boolean
  pagination: {
    page: number
    pageSize: number
    totalCount: number
    totalPages: number
  }
}

export const useSmsConfigStore = defineStore('admin-sms-config', {
  state: (): SmsConfigState => ({
    config: null,
    historique: [],
    loading: false,
    saving: false,
    testSending: false,
    syncing: false,
    pagination: {
      page: 1,
      pageSize: 20,
      totalCount: 0,
      totalPages: 0,
    },
  }),

  actions: {
    async fetch(): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.get<SmsConfig>(API_PATH)
        if (response.success && response.data) {
          this.config = response.data
        }
      }
      finally {
        this.loading = false
      }
    },

    async fetchHistorique(params?: { page?: number, pageSize?: number }): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.getPaginated<SmsEnvoi>(`${API_PATH}/historique`, {
          page: params?.page ?? this.pagination.page,
          pageSize: params?.pageSize ?? this.pagination.pageSize,
        })
        if (response.success && response.data) {
          this.historique = response.data.items
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

    async update(payload: UpdateSmsConfigPayload): Promise<boolean> {
      this.saving = true
      try {
        const response = await apiClient.put<SmsConfig>(API_PATH, payload)
        if (response.success) {
          await this.fetch()
          return true
        }
        return false
      }
      finally {
        this.saving = false
      }
    },

    async sendTest(payload: SendTestSmsPayload): Promise<boolean> {
      this.testSending = true
      try {
        const response = await apiClient.post<void>(`${API_PATH}/test`, payload)
        if (response.success) {
          await this.fetchHistorique()
        }
        return response.success
      }
      finally {
        this.testSending = false
      }
    },

    async syncBalance(): Promise<boolean> {
      this.syncing = true
      try {
        const response = await apiClient.post<SmsConfig>(`${API_PATH}/sync-balance`, {})
        if (response.success && response.data) {
          this.config = response.data
          return true
        }
        return false
      }
      finally {
        this.syncing = false
      }
    },
  },
})
