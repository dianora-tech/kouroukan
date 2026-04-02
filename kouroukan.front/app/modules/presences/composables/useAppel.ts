import { useAppelStore } from '../stores/appel.store'
import type {
  Appel,
  CreateAppelPayload,
  UpdateAppelPayload,
  AppelFilters,
} from '../types/appel.types'

export function useAppel() {
  const store = useAppelStore()
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

  const totalClotures = computed(() =>
    store.items.filter(a => a.estCloture).length,
  )

  const totalEnCours = computed(() =>
    store.items.filter(a => !a.estCloture).length,
  )

  async function fetchAll(params?: Partial<AppelFilters & { page?: number, pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('presences.appel.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<Appel | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('presences.appel.fetchError'), color: 'error' })
      return null
    }
  }

  async function create(payload: CreateAppelPayload): Promise<Appel | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('presences.appel.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('presences.appel.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateAppelPayload): Promise<Appel | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('presences.appel.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('presences.appel.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('presences.appel.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('presences.appel.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: AppelFilters): void {
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
    totalClotures,
    totalEnCours,
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
