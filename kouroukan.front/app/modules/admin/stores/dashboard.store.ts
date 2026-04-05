import { defineStore } from 'pinia'
import { apiClient } from '~/core/api/client'

export interface DashboardKpi {
  totalEtablissements: number
  totalEnseignants: number
  totalParents: number
  totalEleves: number
}

export interface RevenuMensuel {
  mois: string
  montant: number
}

export interface RegionStat {
  nom: string
  count: number
  pct: number
}

export interface UsageStat {
  label: string
  value: string
  trend: string
}

export interface ForfaitStats {
  totalEtablissements: number
  etablissementsAvecForfait: number
  tauxEtablissements: number
  totalEnseignants: number
  enseignantsAvecForfait: number
  tauxEnseignants: number
  totalParents: number
  parentsAvecForfait: number
  tauxParents: number
}

interface DashboardState {
  kpis: DashboardKpi | null
  revenus: RevenuMensuel[]
  regions: RegionStat[]
  usage: UsageStat[]
  forfaitStats: ForfaitStats | null
  loading: boolean
  forfaitStatsLoading: boolean
}

export const useDashboardStore = defineStore('admin-dashboard', {
  state: (): DashboardState => ({
    kpis: null,
    revenus: [],
    regions: [],
    usage: [],
    forfaitStats: null,
    loading: false,
    forfaitStatsLoading: false,
  }),

  actions: {
    async fetchKpis(): Promise<void> {
      this.loading = true
      try {
        const response = await apiClient.get<DashboardKpi>('/api/admin/stats/dashboard')
        if (response.success && response.data) {
          this.kpis = response.data
        }
      }
      catch {
        // silently fail
      }
      finally {
        this.loading = false
      }
    },

    async fetchRevenus(): Promise<void> {
      try {
        const response = await apiClient.get<RevenuMensuel[]>('/api/admin/stats/revenus')
        if (response.success && response.data) {
          this.revenus = response.data
        }
      }
      catch {
        // silently fail
      }
    },

    async fetchRegions(): Promise<void> {
      try {
        const response = await apiClient.get<RegionStat[]>('/api/admin/stats/regions')
        if (response.success && response.data) {
          this.regions = response.data
        }
      }
      catch {
        // silently fail
      }
    },

    async fetchUsage(): Promise<void> {
      try {
        const response = await apiClient.get<UsageStat[]>('/api/admin/stats/usage')
        if (response.success && response.data) {
          this.usage = response.data
        }
      }
      catch {
        // silently fail
      }
    },

    async fetchForfaitStats(): Promise<void> {
      this.forfaitStatsLoading = true
      try {
        const response = await apiClient.get<ForfaitStats>('/api/admin/stats/forfaits')
        if (response.success && response.data) {
          this.forfaitStats = response.data
        }
      }
      catch {
        // silently fail
      }
      finally {
        this.forfaitStatsLoading = false
      }
    },

    async fetchAll(): Promise<void> {
      await Promise.all([
        this.fetchKpis(),
        this.fetchRevenus(),
        this.fetchRegions(),
        this.fetchUsage(),
        this.fetchForfaitStats(),
      ])
    },
  },
})
