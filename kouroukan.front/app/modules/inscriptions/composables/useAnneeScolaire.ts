import { useAnneeScolaireStore } from '../stores/annee-scolaire.store'
import type {
  AnneeScolaire,
  CreateAnneeScolairePayload,
  UpdateAnneeScolairePayload,
  AnneeScolaireFilters,
} from '../types/annee-scolaire.types'

export function useAnneeScolaire() {
  const store = useAnneeScolaireStore()
  const toast = useToast()
  const { t } = useI18n()

  const items = computed(() => store.items)
  const currentItem = computed(() => store.currentItem)
  const loading = computed(() => store.loading)
  const saving = computed(() => store.saving)
  const isEmpty = computed(() => store.isEmpty)
  const hasFilters = computed(() => store.hasFilters)
  const pagination = computed(() => store.pagination)
  const activeYear = computed(() => store.activeYear)

  const paginatedData = computed(() => ({
    items: store.items,
    totalCount: store.pagination.totalCount,
    pageNumber: store.pagination.page,
    pageSize: store.pagination.pageSize,
    totalPages: store.pagination.totalPages,
    hasNextPage: store.pagination.page < store.pagination.totalPages,
    hasPreviousPage: store.pagination.page > 1,
  }))

  const selectOptions = computed(() =>
    store.items.map(a => ({ label: a.libelle, value: a.id })),
  )

  async function fetchAll(params?: Partial<AnneeScolaireFilters & { page?: number, pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('inscriptions.anneeScolaire.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<AnneeScolaire | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('inscriptions.anneeScolaire.fetchError'), color: 'error' })
      return null
    }
  }

  async function create(payload: CreateAnneeScolairePayload): Promise<AnneeScolaire | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('inscriptions.anneeScolaire.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('inscriptions.anneeScolaire.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateAnneeScolairePayload): Promise<AnneeScolaire | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('inscriptions.anneeScolaire.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('inscriptions.anneeScolaire.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('inscriptions.anneeScolaire.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('inscriptions.anneeScolaire.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: AnneeScolaireFilters): void {
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
    activeYear,
    selectOptions,
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
