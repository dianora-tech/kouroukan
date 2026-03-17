import { useRemunerationStore } from '../stores/remuneration.store'
import type {
  RemunerationEnseignant,
  CreateRemunerationPayload,
  UpdateRemunerationPayload,
  RemunerationFilters,
} from '../types/remuneration.types'

export function useRemuneration() {
  const store = useRemunerationStore()
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

  const totalMontant = computed(() =>
    store.items.reduce((sum, r) => sum + r.montantTotal, 0),
  )

  const totalPayes = computed(() =>
    store.items.filter(r => r.statutPaiement === 'Paye').length,
  )

  const totalHeures = computed(() =>
    store.items.reduce((sum, r) => sum + (r.nombreHeures ?? 0), 0),
  )

  async function fetchAll(params?: Partial<RemunerationFilters & { page?: number; pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('finances.remuneration.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<RemunerationEnseignant | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('finances.remuneration.fetchError'), color: 'error' })
      return null
    }
  }

  async function create(payload: CreateRemunerationPayload): Promise<RemunerationEnseignant | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('finances.remuneration.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('finances.remuneration.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateRemunerationPayload): Promise<RemunerationEnseignant | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('finances.remuneration.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('finances.remuneration.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('finances.remuneration.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('finances.remuneration.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: RemunerationFilters): void {
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
    totalMontant,
    totalPayes,
    totalHeures,
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
