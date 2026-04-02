import { defineStore } from 'pinia'
import type {
  EmailConfig,
  UpdateEmailConfigPayload,
  SendTestEmailPayload,
} from '../types/admin.types'
import { apiClient } from '~/core/api/client'

const API_PATH = '/api/admin/email-config'

interface EmailConfigState {
  config: EmailConfig | null
  loading: boolean
  saving: boolean
  testSending: boolean
}

export const useEmailConfigStore = defineStore('admin-email-config', {
  state: (): EmailConfigState => ({
    config: null,
    loading: false,
    saving: false,
    testSending: false,
  }),

  actions: {
    async fetch(): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.get<EmailConfig>(API_PATH)
        if (response.success && response.data) {
          this.config = response.data
        }
      }
      finally {
        this.loading = false
      }
    },

    async update(payload: UpdateEmailConfigPayload): Promise<boolean> {
      this.saving = true
      try {
        const response = await apiClient.put<EmailConfig>(API_PATH, payload)
        if (response.success) {
          // Refresh config from server after update
          await this.fetch()
          return true
        }
        return false
      }
      finally {
        this.saving = false
      }
    },

    async sendTest(payload: SendTestEmailPayload): Promise<boolean> {
      this.testSending = true
      try {
        const response = await apiClient.post<void>(`${API_PATH}/test`, payload)
        return response.success
      }
      finally {
        this.testSending = false
      }
    },
  },
})
