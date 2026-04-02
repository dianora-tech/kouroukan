import { useSeanceStore } from '../stores/seance.store'
import type { Seance, CreateSeancePayload, UpdateSeancePayload, SeanceFilters } from '../types/seance.types'

export function useSeance() {
  const store = useSeanceStore()
  const toast = useToast()
  const { t } = useI18n()

  const items = computed(() => store.items)
  const currentItem = computed(() => store.currentItem)
  const loading = computed(() => store.loading)
  const saving = computed(() => store.saving)
  const isEmpty = computed(() => store.isEmpty)
  const hasFilters = computed(() => store.hasFilters)
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

  async function fetchAll(params?: Partial<SeanceFilters & { page?: number, pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('pedagogie.seance.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<Seance | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('pedagogie.seance.fetchError'), color: 'error' })
      return null
    }
  }

  async function create(payload: CreateSeancePayload): Promise<Seance | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('pedagogie.seance.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('pedagogie.seance.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateSeancePayload): Promise<Seance | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('pedagogie.seance.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('pedagogie.seance.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('pedagogie.seance.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('pedagogie.seance.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: SeanceFilters): void {
    store.setFilters(filters)
    fetchAll()
  }

  function resetFilters(): void {
    store.resetFilters()
    fetchAll()
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
    hasFilters,
    pagination,
    paginatedData,
    fetchAll,
    fetchById,
    create,
    update,
    remove,
    setFilters,
    resetFilters,
    changePage,
  }
}
