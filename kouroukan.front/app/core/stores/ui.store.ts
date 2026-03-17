import { defineStore } from 'pinia'

interface UiState {
  sidebarCollapsed: boolean
  currentLocale: string
  dataSavingMode: boolean
}

export const useUiStore = defineStore('ui', {
  state: (): UiState => ({
    sidebarCollapsed: false,
    currentLocale: 'fr',
    dataSavingMode: true, // Enabled by default for low connectivity (Guinea 2G/3G)
  }),

  actions: {
    toggleSidebar(): void {
      this.sidebarCollapsed = !this.sidebarCollapsed
    },

    setLocale(locale: string): void {
      this.currentLocale = locale
    },

    toggleDataSaving(): void {
      this.dataSavingMode = !this.dataSavingMode
    },
  },

  persist: true,
})
