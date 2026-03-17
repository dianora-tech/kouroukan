import { useEnseignantStore } from '../stores/enseignant.store'
import type {
  Enseignant,
  CreateEnseignantPayload,
  UpdateEnseignantPayload,
  EnseignantFilters,
} from '../types/enseignant.types'

export function useEnseignant() {
  const store = useEnseignantStore()
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

  const totalActifs = computed(() =>
    store.items.filter(e => e.statutEnseignant === 'Actif').length,
  )

  const totalEnConge = computed(() =>
    store.items.filter(e => e.statutEnseignant === 'EnConge').length,
  )

  const moyenneSoldeConges = computed(() => {
    if (store.items.length === 0) return 0
    return Math.round(store.items.reduce((sum, e) => sum + e.soldeCongesAnnuel, 0) / store.items.length)
  })

  async function fetchAll(params?: Partial<EnseignantFilters & { page?: number; pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('personnel.enseignant.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<Enseignant | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('personnel.enseignant.fetchError'), color: 'error' })
      return null
    }
  }

  async function fetchTypes(): Promise<void> {
    await store.fetchTypes()
  }

  async function create(payload: CreateEnseignantPayload): Promise<Enseignant | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('personnel.enseignant.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('personnel.enseignant.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateEnseignantPayload): Promise<Enseignant | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('personnel.enseignant.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('personnel.enseignant.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('personnel.enseignant.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('personnel.enseignant.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: EnseignantFilters): void {
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
    totalActifs,
    totalEnConge,
    moyenneSoldeConges,
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
