import { defineStore } from 'pinia'
import { apiClient } from '~/core/api/client'

interface UserListItem {
  id: number
  firstName: string
  lastName: string
  email: string
  phoneNumber: string
  role: string
  isActive: boolean
  createdAt: string
}

interface CreateUserPayload {
  firstName: string
  lastName: string
  phoneNumber: string
  email?: string
  role: string
  existingUserId?: number
}

interface CreateUserResult {
  userId: number
  temporaryPassword: string
}

interface UserSearchResult {
  id: number
  firstName: string
  lastName: string
}

interface UsersState {
  items: UserListItem[]
  loading: boolean
  saving: boolean
  searchResult: UserSearchResult | null
  searching: boolean
}

export const useUsersStore = defineStore('users', {
  state: (): UsersState => ({
    items: [],
    loading: false,
    saving: false,
    searchResult: null,
    searching: false,
  }),

  actions: {
    async fetchUsers(): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.get<UserListItem[]>('/api/users')
        if (response.success && response.data) {
          this.items = response.data
        }
      }
      finally {
        this.loading = false
      }
    },

    async createUser(payload: CreateUserPayload): Promise<CreateUserResult> {
      this.saving = true
      try {
        const response = await apiClient.post<CreateUserResult>('/api/users', payload)
        if (response.success && response.data) {
          await this.fetchUsers()
          return response.data
        }
        throw new Error(response.message || 'Erreur lors de la creation')
      }
      finally {
        this.saving = false
      }
    },

    async searchUser(query: string): Promise<UserSearchResult | null> {
      this.searching = true
      this.searchResult = null
      try {
        const response = await apiClient.get<UserSearchResult>(`/api/users/search?q=${encodeURIComponent(query)}`)
        if (response.success && response.data) {
          this.searchResult = response.data
          return response.data
        }
        return null
      }
      catch {
        return null
      }
      finally {
        this.searching = false
      }
    },

    async deleteUser(userId: number): Promise<void> {
      await apiClient.delete(`/api/users/${userId}`)
      await this.fetchUsers()
    },
  },
})
