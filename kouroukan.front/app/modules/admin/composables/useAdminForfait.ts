import { useForfaitStore } from '../stores/forfait.store'
import type {
  Forfait,
  CreateForfaitPayload,
  UpdateForfaitPayload,
  UpdateTarifPayload,
  AdminFilters,
} from '../types/admin.types'

export function useAdminForfait() {
  const store = useForfaitStore()
  const toast = useToast()
  const { t } = useI18n()

  const items = computed(() => store.items)
  const currentItem = computed(() => store.currentItem)
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

  async function fetchAll(params?: Partial<AdminFilters & { page?: number, pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('admin.forfait.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<Forfait | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('admin.forfait.fetchError'), color: 'error' })
      return null
    }
  }

  async function create(payload: CreateForfaitPayload): Promise<Forfait | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('admin.forfait.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('admin.forfait.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateForfaitPayload): Promise<Forfait | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('admin.forfait.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('admin.forfait.updateError'), color: 'error' })
      return null
    }
  }

  async function updateTarif(payload: UpdateTarifPayload): Promise<Forfait | null> {
    try {
      const result = await store.updateTarif(payload)
      if (result) {
        toast.add({ title: t('admin.forfait.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('admin.forfait.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('admin.forfait.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('admin.forfait.deleteError'), color: 'error' })
      return false
    }
  }

  function changePage(page: number): void {
    fetchAll({ page })
  }

  return {
    items,
    currentItem,
    loading,
    saving,
    isEmpty,
    pagination,
    paginatedData,
    fetchAll,
    fetchById,
    create,
    update,
    updateTarif,
    remove,
    changePage,
  }
}
