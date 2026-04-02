import { useEleveStore } from '../stores/eleve.store'
import type { Eleve, CreateElevePayload, UpdateElevePayload, EleveFilters } from '../types/eleve.types'

export function useEleve() {
  const store = useEleveStore()
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

  async function fetchAll(params?: Partial<EleveFilters & { page?: number, pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('inscriptions.eleve.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<Eleve | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('inscriptions.eleve.fetchError'), color: 'error' })
      return null
    }
  }

  async function create(payload: CreateElevePayload): Promise<Eleve | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('inscriptions.eleve.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('inscriptions.eleve.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateElevePayload): Promise<Eleve | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('inscriptions.eleve.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('inscriptions.eleve.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('inscriptions.eleve.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('inscriptions.eleve.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: EleveFilters): void {
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
