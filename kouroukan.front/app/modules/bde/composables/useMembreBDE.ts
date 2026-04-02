import { useMembreBDEStore } from '../stores/membre-bde.store'
import type {
  MembreBDE,
  CreateMembreBDEPayload,
  UpdateMembreBDEPayload,
  MembreBDEFilters,
} from '../types/membre-bde.types'

export function useMembreBDE() {
  const store = useMembreBDEStore()
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

  const totalActifs = computed(() =>
    store.items.filter(m => m.estActif).length,
  )

  const totalCotisations = computed(() =>
    store.items.reduce((sum, m) => sum + (m.montantCotisation ?? 0), 0),
  )

  async function fetchAll(params?: Partial<MembreBDEFilters & { page?: number, pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('bde.membreBde.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<MembreBDE | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('bde.membreBde.fetchError'), color: 'error' })
      return null
    }
  }

  async function create(payload: CreateMembreBDEPayload): Promise<MembreBDE | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('bde.membreBde.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('bde.membreBde.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateMembreBDEPayload): Promise<MembreBDE | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('bde.membreBde.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('bde.membreBde.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('bde.membreBde.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('bde.membreBde.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: MembreBDEFilters): void {
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
    totalActifs,
    totalCotisations,
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
