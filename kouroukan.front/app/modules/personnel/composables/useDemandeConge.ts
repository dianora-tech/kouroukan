import { useDemandeCongeStore } from '../stores/demandeConge.store'
import type {
  DemandeConge,
  CreateDemandeCongePayload,
  UpdateDemandeCongePayload,
  DemandeCongeFilters,
} from '../types/demandeConge.types'

export function useDemandeConge() {
  const store = useDemandeCongeStore()
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

  const totalSoumises = computed(() =>
    store.items.filter(d => d.statutDemande === 'Soumise').length,
  )

  const totalApprouvees = computed(() =>
    store.items.filter(d => d.statutDemande === 'ApprouveeN1' || d.statutDemande === 'ApprouveeDirection').length,
  )

  const totalAvecImpactPaie = computed(() =>
    store.items.filter(d => d.impactPaie).length,
  )

  async function fetchAll(params?: Partial<DemandeCongeFilters & { page?: number, pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('personnel.demandeConge.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<DemandeConge | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('personnel.demandeConge.fetchError'), color: 'error' })
      return null
    }
  }

  async function fetchTypes(): Promise<void> {
    await store.fetchTypes()
  }

  async function create(payload: CreateDemandeCongePayload): Promise<DemandeConge | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('personnel.demandeConge.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('personnel.demandeConge.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateDemandeCongePayload): Promise<DemandeConge | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('personnel.demandeConge.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('personnel.demandeConge.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('personnel.demandeConge.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('personnel.demandeConge.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: DemandeCongeFilters): void {
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
    totalSoumises,
    totalApprouvees,
    totalAvecImpactPaie,
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
