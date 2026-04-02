import { apiClient } from '~/core/api/client'

export interface ForfaitPlanDto {
  id: number
  code: string
  nom: string
  description?: string
  prixMensuel: number
  prixVacances: number
  periodeEssaiJours: number
  limiteEleves?: number
  estGratuit: boolean
}

export interface AbonnementHistoryDto {
  id: number
  forfaitNom: string
  dateDebut: string
  dateFin?: string
  montant: number
  statut: string
  createdAt: string
}

export function useForfaitSubscription() {
  const toast = useToast()
  const { t } = useI18n()
  const { formatDate } = useFormatDate()

  const availablePlans = ref<ForfaitPlanDto[]>([])
  const subscriptionHistory = ref<AbonnementHistoryDto[]>([])
  const loading = ref(false)
  const saving = ref(false)

  async function fetchPlans(type: string): Promise<void> {
    loading.value = true
    try {
      const response = await apiClient.get<ForfaitPlanDto[]>(`/api/forfait/plans?type=${encodeURIComponent(type)}`)
      if (response.success && response.data) {
        availablePlans.value = response.data
      }
    }
    catch {
      toast.add({
        title: t('forfait.fetchPlansError'),
        color: 'error',
        icon: 'i-heroicons-exclamation-triangle',
      })
    }
    finally {
      loading.value = false
    }
  }

  async function fetchHistory(): Promise<void> {
    loading.value = true
    try {
      const response = await apiClient.get<AbonnementHistoryDto[]>('/api/forfait/history')
      if (response.success && response.data) {
        subscriptionHistory.value = response.data
      }
    }
    catch {
      toast.add({
        title: t('forfait.fetchHistoryError'),
        color: 'error',
        icon: 'i-heroicons-exclamation-triangle',
      })
    }
    finally {
      loading.value = false
    }
  }

  async function subscribe(forfaitId: number, eleveId?: number): Promise<boolean> {
    saving.value = true
    try {
      const body: { forfaitId: number; eleveId?: number } = { forfaitId }
      if (eleveId !== undefined) {
        body.eleveId = eleveId
      }
      const response = await apiClient.post<void>('/api/forfait/subscribe', body)
      if (response.success) {
        toast.add({
          title: t('forfait.subscribeSuccess'),
          color: 'success',
          icon: 'i-heroicons-check-circle',
        })
        return true
      }
      return false
    }
    catch {
      toast.add({
        title: t('forfait.subscribeError'),
        color: 'error',
        icon: 'i-heroicons-exclamation-triangle',
      })
      return false
    }
    finally {
      saving.value = false
    }
  }

  async function cancel(abonnementId: number): Promise<boolean> {
    saving.value = true
    try {
      const response = await apiClient.post<void>('/api/forfait/cancel', { abonnementId })
      if (response.success) {
        toast.add({
          title: t('forfait.cancelSuccess'),
          color: 'success',
          icon: 'i-heroicons-check-circle',
        })
        return true
      }
      return false
    }
    catch {
      toast.add({
        title: t('forfait.cancelError'),
        color: 'error',
        icon: 'i-heroicons-exclamation-triangle',
      })
      return false
    }
    finally {
      saving.value = false
    }
  }

  function formatMontant(montant: number): string {
    return new Intl.NumberFormat('fr-GN', {
      style: 'decimal',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0,
    }).format(montant) + ' GNF'
  }

  return {
    availablePlans,
    subscriptionHistory,
    loading,
    saving,
    fetchPlans,
    fetchHistory,
    subscribe,
    cancel,
    formatMontant,
    formatDate,
  }
}
