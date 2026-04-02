import { useLiaisonEnseignantStore } from '../stores/liaison-enseignant.store'
import type { LiaisonEnseignant } from '../types/enseignant.types'

export function useEnseignantLiaison() {
  const store = useLiaisonEnseignantStore()
  const toast = useToast()
  const { t } = useI18n()

  const items = computed(() => store.items)
  const actives = computed(() => store.actives)
  const historique = computed(() => store.historique)
  const pending = computed(() => store.pending)
  const loading = computed(() => store.loading)
  const saving = computed(() => store.saving)
  const isEmpty = computed(() => store.isEmpty)

  async function fetchAll(): Promise<void> {
    try {
      await store.fetchAll()
    }
    catch {
      toast.add({ title: t('enseignant.etablissements.fetchError'), color: 'error' })
    }
  }

  async function accept(id: number): Promise<boolean> {
    try {
      const success = await store.accept(id)
      if (success) {
        toast.add({ title: t('enseignant.etablissements.acceptSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('enseignant.etablissements.acceptError'), color: 'error' })
      return false
    }
  }

  async function reject(id: number): Promise<boolean> {
    try {
      const success = await store.reject(id)
      if (success) {
        toast.add({ title: t('enseignant.etablissements.rejectSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('enseignant.etablissements.rejectError'), color: 'error' })
      return false
    }
  }

  return {
    items,
    actives,
    historique,
    pending,
    loading,
    saving,
    isEmpty,
    fetchAll,
    accept,
    reject,
  }
}
