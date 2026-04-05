import { defineStore } from 'pinia'
import type { MessageFamille } from '../types/famille.types'
import { apiClient } from '~/core/api/client'

interface CommunicationState {
  items: MessageFamille[]
  loading: boolean
  pagination: {
    page: number
    pageSize: number
    totalCount: number
    totalPages: number
  }
}

export const useCommunicationStore = defineStore('famille-communication', {
  state: (): CommunicationState => ({
    items: [],
    loading: false,
    pagination: {
      page: 1,
      pageSize: 20,
      totalCount: 0,
      totalPages: 0,
    },
  }),

  actions: {
    async fetchAll(params?: { page?: number, pageSize?: number }): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.getPaginated<MessageFamille>('/api/communication/messages', {
          page: params?.page ?? this.pagination.page,
          pageSize: params?.pageSize ?? this.pagination.pageSize,
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

    async markAsRead(id: number): Promise<void> {
      try {
        await apiClient.put(`/api/communication/messages/${id}/read`)
        const item = this.items.find(m => m.id === id)
        if (item) item.lu = true
      }
      catch {
        // silently fail
      }
    },
  },
})
