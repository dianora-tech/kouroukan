import { useEvenementStore } from '../stores/evenement.store'
import type {
  Evenement,
  CreateEvenementPayload,
  UpdateEvenementPayload,
  EvenementFilters,
} from '../types/evenement.types'

export function useEvenement() {
  const store = useEvenementStore()
  const toast = useToast()
  const { t } = useI18n()

  const items = computed(() => store.items)
  const currentItem = computed(() => store.currentItem)
  const types = computed(() => store.types)
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

  const typeOptions = computed(() =>
    store.types.map(t => ({ label: t.name, value: t.id })),
  )

  const totalInscrits = computed(() =>
    store.items.reduce((sum, e) => sum + e.nombreInscrits, 0),
  )

  const totalTermines = computed(() =>
    store.items.filter(e => e.statutEvenement === 'Termine').length,
  )

  const totalPlanifies = computed(() =>
    store.items.filter(e => e.statutEvenement === 'Planifie' || e.statutEvenement === 'Valide').length,
  )

  async function fetchAll(params?: Partial<EvenementFilters & { page?: number, pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('bde.evenement.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<Evenement | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('bde.evenement.fetchError'), color: 'error' })
      return null
    }
  }

  async function fetchTypes(): Promise<void> {
    await store.fetchTypes()
  }

  async function create(payload: CreateEvenementPayload): Promise<Evenement | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('bde.evenement.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('bde.evenement.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateEvenementPayload): Promise<Evenement | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('bde.evenement.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('bde.evenement.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('bde.evenement.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('bde.evenement.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: EvenementFilters): void {
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
    types,
    loading,
    saving,
    isEmpty,
    hasFilters,
    pagination,
    paginatedData,
    typeOptions,
    totalInscrits,
    totalTermines,
    totalPlanifies,
    fetchAll,
    fetchById,
    fetchTypes,
    create,
    update,
    remove,
    setFilters,
    resetFilters,
    changePage,
  }
}
