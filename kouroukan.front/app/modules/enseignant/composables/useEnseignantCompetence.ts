import { useCompetenceStore } from '../stores/competence.store'
import type {
  Competence,
  CreateCompetencePayload,
} from '../types/enseignant.types'

export function useEnseignantCompetence() {
  const store = useCompetenceStore()
  const toast = useToast()
  const { t } = useI18n()

  const items = computed(() => store.items)
  const loading = computed(() => store.loading)
  const saving = computed(() => store.saving)
  const isEmpty = computed(() => store.isEmpty)

  async function fetchAll(params?: { userId?: number }): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('enseignant.competences.fetchError'), color: 'error' })
    }
  }

  async function create(payload: CreateCompetencePayload): Promise<Competence | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('enseignant.competences.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('enseignant.competences.createError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('enseignant.competences.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('enseignant.competences.deleteError'), color: 'error' })
      return false
    }
  }

  return {
    items,
    loading,
    saving,
    isEmpty,
    fetchAll,
    create,
    remove,
  }
}
