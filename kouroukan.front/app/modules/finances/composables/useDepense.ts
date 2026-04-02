import { useDepenseStore } from '../stores/depense.store'
import type {
  Depense,
  CreateDepensePayload,
  UpdateDepensePayload,
  DepenseFilters,
} from '../types/depense.types'

export function useDepense() {
  const store = useDepenseStore()
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

  const totalExecutees = computed(() =>
    store.items.filter(d => d.statutDepense === 'Executee').length,
  )

  const totalEnAttente = computed(() =>
    store.items.filter(d => ['Demande', 'ValideN1', 'ValideFinance'].includes(d.statutDepense)).length,
  )

  async function fetchAll(params?: Partial<DepenseFilters & { page?: number, pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('finances.depense.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<Depense | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('finances.depense.fetchError'), color: 'error' })
      return null
    }
  }

  async function fetchTypes(): Promise<void> {
    await store.fetchTypes()
  }

  async function create(payload: CreateDepensePayload): Promise<Depense | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('finances.depense.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('finances.depense.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateDepensePayload): Promise<Depense | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('finances.depense.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('finances.depense.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('finances.depense.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('finances.depense.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: DepenseFilters): void {
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
    totalExecutees,
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
