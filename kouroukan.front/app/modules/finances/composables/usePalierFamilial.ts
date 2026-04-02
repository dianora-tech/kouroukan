import { usePalierFamilialStore } from '../stores/palier-familial.store'
import type {
  PalierFamilial,
  CreatePalierFamilialPayload,
} from '../types/finances.types'

export function usePalierFamilial() {
  const store = usePalierFamilialStore()
  const toast = useToast()
  const { t } = useI18n()

  const items = computed(() => store.items)
  const loading = computed(() => store.loading)
  const saving = computed(() => store.saving)
  const isEmpty = computed(() => store.isEmpty)
  const pagination = computed(() => store.pagination)

  async function fetchAll(params?: { page?: number; pageSize?: number }): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('admin.paiement.fetchError'), color: 'error' })
    }
  }

  async function create(payload: CreatePalierFamilialPayload): Promise<PalierFamilial | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('admin.paiement.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('admin.paiement.createError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('admin.paiement.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('admin.paiement.deleteError'), color: 'error' })
      return false
    }
  }

  return {
    items,
    loading,
    saving,
    isEmpty,
    pagination,
    fetchAll,
    create,
    remove,
  }
}
