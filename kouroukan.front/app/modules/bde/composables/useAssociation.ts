import { useAssociationStore } from '../stores/association.store'
import type {
  Association,
  CreateAssociationPayload,
  UpdateAssociationPayload,
  AssociationFilters,
} from '../types/association.types'

export function useAssociation() {
  const store = useAssociationStore()
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

  const totalBudget = computed(() =>
    store.items.reduce((sum, a) => sum + a.budgetAnnuel, 0),
  )

  const totalActives = computed(() =>
    store.items.filter(a => a.statut === 'Active').length,
  )

  async function fetchAll(params?: Partial<AssociationFilters & { page?: number, pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('bde.association.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<Association | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('bde.association.fetchError'), color: 'error' })
      return null
    }
  }

  async function fetchTypes(): Promise<void> {
    await store.fetchTypes()
  }

  async function create(payload: CreateAssociationPayload): Promise<Association | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('bde.association.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('bde.association.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateAssociationPayload): Promise<Association | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('bde.association.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('bde.association.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('bde.association.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('bde.association.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: AssociationFilters): void {
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
    totalBudget,
    totalActives,
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
