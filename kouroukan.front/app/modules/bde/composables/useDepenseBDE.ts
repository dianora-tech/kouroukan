import { useDepenseBDEStore } from '../stores/depense-bde.store'
import type {
  DepenseBDE,
  CreateDepenseBDEPayload,
  UpdateDepenseBDEPayload,
  DepenseBDEFilters,
} from '../types/depense-bde.types'

export function useDepenseBDE() {
  const store = useDepenseBDEStore()
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

  const totalMontant = computed(() =>
    store.items.reduce((sum, d) => sum + d.montant, 0),
  )

  const totalValidees = computed(() =>
    store.items.filter(d => d.statutValidation === 'ValideTresorier' || d.statutValidation === 'ValideSuper').length,
  )

  const totalEnAttente = computed(() =>
    store.items.filter(d => d.statutValidation === 'Demandee').length,
  )

  async function fetchAll(params?: Partial<DepenseBDEFilters & { page?: number, pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('bde.depenseBde.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<DepenseBDE | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('bde.depenseBde.fetchError'), color: 'error' })
      return null
    }
  }

  async function fetchTypes(): Promise<void> {
    await store.fetchTypes()
  }

  async function create(payload: CreateDepenseBDEPayload): Promise<DepenseBDE | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('bde.depenseBde.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('bde.depenseBde.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateDepenseBDEPayload): Promise<DepenseBDE | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('bde.depenseBde.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('bde.depenseBde.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('bde.depenseBde.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('bde.depenseBde.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: DepenseBDEFilters): void {
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
    totalMontant,
    totalValidees,
    totalEnAttente,
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
