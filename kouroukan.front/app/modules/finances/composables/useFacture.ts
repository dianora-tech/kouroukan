import { useFactureStore } from '../stores/facture.store'
import type {
  Facture,
  CreateFacturePayload,
  UpdateFacturePayload,
  FactureFilters,
} from '../types/facture.types'

export function useFacture() {
  const store = useFactureStore()
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
    store.items.reduce((sum, f) => sum + f.montantTotal, 0),
  )

  const totalPaye = computed(() =>
    store.items.reduce((sum, f) => sum + f.montantPaye, 0),
  )

  const totalSolde = computed(() =>
    store.items.reduce((sum, f) => sum + f.solde, 0),
  )

  const totalEchues = computed(() =>
    store.items.filter(f => f.statutFacture === 'Echue').length,
  )

  async function fetchAll(params?: Partial<FactureFilters & { page?: number, pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('finances.facture.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<Facture | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('finances.facture.fetchError'), color: 'error' })
      return null
    }
  }

  async function fetchTypes(): Promise<void> {
    await store.fetchTypes()
  }

  async function create(payload: CreateFacturePayload): Promise<Facture | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('finances.facture.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('finances.facture.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateFacturePayload): Promise<Facture | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('finances.facture.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('finances.facture.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('finances.facture.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('finances.facture.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: FactureFilters): void {
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
    totalPaye,
    totalSolde,
    totalEchues,
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
