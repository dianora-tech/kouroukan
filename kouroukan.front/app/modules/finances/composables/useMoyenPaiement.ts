import { useMoyenPaiementStore } from '../stores/moyen-paiement.store'
import type {
  MoyenPaiement,
  CreateMoyenPaiementPayload,
  UpdateMoyenPaiementPayload,
  FinancesFilters,
} from '../types/finances.types'

export function useMoyenPaiement() {
  const store = useMoyenPaiementStore()
  const toast = useToast()
  const { t } = useI18n()

  const items = computed(() => store.items)
  const loading = computed(() => store.loading)
  const saving = computed(() => store.saving)
  const isEmpty = computed(() => store.isEmpty)
  const pagination = computed(() => store.pagination)

  const paginatedData = computed(() => ({
    items: store.items,
    totalCount: store.pagination.totalCount,
    pageNumber: store.pagination.page,
    pageSize: store.pagination.pageSize,
    totalPages: store.pagination.totalPages,
    hasNextPage: store.pagination.page < store.pagination.totalPages,
    hasPreviousPage: store.pagination.page > 1,
  }))

  async function fetchAll(params?: Partial<FinancesFilters & { page?: number; pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('admin.paiement.fetchError'), color: 'error' })
    }
  }

  async function create(payload: CreateMoyenPaiementPayload): Promise<MoyenPaiement | null> {
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

  async function update(id: number, payload: UpdateMoyenPaiementPayload): Promise<MoyenPaiement | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('admin.paiement.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('admin.paiement.updateError'), color: 'error' })
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

  function changePage(page: number): void {
    fetchAll({ page })
  }

  return {
    items,
    loading,
    saving,
    isEmpty,
    pagination,
    paginatedData,
    fetchAll,
    create,
    update,
    remove,
    changePage,
  }
}
