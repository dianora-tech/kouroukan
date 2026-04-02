import { useContenuIAStore } from '../stores/contenu-ia.store'
import type {
  ContenuIA,
  CreateContenuIAPayload,
  UpdateContenuIAPayload,
  AdminFilters,
} from '../types/admin.types'

export function useAdminContenuIA() {
  const store = useContenuIAStore()
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
      toast.add({ title: t('admin.contenuIA.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<ContenuIA | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('admin.contenuIA.fetchError'), color: 'error' })
      return null
    }
  }

  async function create(payload: CreateContenuIAPayload): Promise<ContenuIA | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('admin.contenuIA.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('admin.contenuIA.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateContenuIAPayload): Promise<ContenuIA | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('admin.contenuIA.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('admin.contenuIA.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('admin.contenuIA.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('admin.contenuIA.deleteError'), color: 'error' })
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
    remove,
    changePage,
  }
}
