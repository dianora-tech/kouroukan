import { apiClient } from '~/core/api/client'
import { useAuthStore } from '~/core/stores/auth.store'

export interface ForfaitStatusDto {
  forfaitId: number | null
  forfaitNom: string | null
  type: 'gratuit' | 'standard' | 'premium' | null
  statut: 'actif' | 'essai' | 'expire' | 'annule' | null
  dateDebut: string | null
  dateFin: string | null
  dateFinEssai: string | null
  limiteEleves: number | null
  nombreEleves: number | null
}

const LOCKED_FEATURES_ENSEIGNANT = ['emploi-du-temps', 'heures', 'bulletins', 'etablissements']
const LOCKED_FEATURES_FAMILLE = ['notes', 'emploi-du-temps', 'absences', 'communication', 'paiements', 'documents']

// Shared state across all usages (singleton pattern)
const forfaitStatus = ref<ForfaitStatusDto | null>(null)
const loaded = ref(false)
const loading = ref(false)

export function useForfaitGating() {
  const auth = useAuthStore()

  const userType = computed<'enseignant' | 'famille' | 'etablissement' | 'admin'>(() => {
    if (auth.hasRole('super_admin')) return 'admin'
    if (auth.hasRole('enseignant')) return 'enseignant'
    if (auth.hasRole('parent') || auth.hasRole('eleve')) return 'famille'
    return 'etablissement'
  })

  const hasForfait = computed<boolean>(() => {
    if (!forfaitStatus.value) return false
    return (
      forfaitStatus.value.type !== 'gratuit'
      && forfaitStatus.value.type !== null
      && (forfaitStatus.value.statut === 'actif' || forfaitStatus.value.statut === 'essai')
    )
  })

  const isInTrial = computed<boolean>(() => {
    if (!forfaitStatus.value) return false
    return forfaitStatus.value.statut === 'essai'
  })

  const limiteEleves = computed<number | null>(() => {
    return forfaitStatus.value?.limiteEleves ?? null
  })

  const nombreEleves = computed<number | null>(() => {
    return forfaitStatus.value?.nombreEleves ?? null
  })

  function isFeatureLocked(featureSlug: string): boolean {
    // Admin and etablissement never have menu-level locking
    if (userType.value === 'admin') return false
    if (userType.value === 'etablissement') return false

    // If user has an active paid forfait, nothing is locked
    if (hasForfait.value) return false

    if (userType.value === 'enseignant') {
      return LOCKED_FEATURES_ENSEIGNANT.includes(featureSlug)
    }

    if (userType.value === 'famille') {
      return LOCKED_FEATURES_FAMILLE.includes(featureSlug)
    }

    return false
  }

  async function fetchForfaitStatus(): Promise<void> {
    if (loaded.value || loading.value) return

    // Only fetch for enseignant and famille
    if (userType.value !== 'enseignant' && userType.value !== 'famille') return

    loading.value = true
    try {
      const response = await apiClient.get<ForfaitStatusDto>('/api/forfait/me')
      if (response.success && response.data) {
        forfaitStatus.value = response.data
      }
      else {
        forfaitStatus.value = null
      }
    }
    catch {
      // On error, default to no forfait (locked state)
      forfaitStatus.value = null
    }
    finally {
      loading.value = false
      loaded.value = true
    }
  }

  return {
    hasForfait,
    isInTrial,
    forfaitStatus: computed(() => forfaitStatus.value),
    limiteEleves,
    nombreEleves,
    loading: computed(() => loading.value),
    fetchForfaitStatus,
    isFeatureLocked,
    userType,
  }
}
