import { useServiceParentStore } from '../stores/service-parent.store'
import type {
  ServiceParent,
  CreateServiceParentPayload,
  UpdateServiceParentPayload,
  ServiceParentFilters,
} from '../types/service-parent.types'

export function useServiceParent() {
  const store = useServiceParentStore()
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
    store.items.filter(s => s.estActif).length,
  )

  const totalAvecEssai = computed(() =>
    store.items.filter(s => s.periodeEssaiJours && s.periodeEssaiJours > 0).length,
  )

  async function fetchAll(params?: Partial<ServiceParentFilters & { page?: number, pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('servicesPremium.serviceParent.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<ServiceParent | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('servicesPremium.serviceParent.fetchError'), color: 'error' })
      return null
    }
  }

  async function fetchTypes(): Promise<void> {
    await store.fetchTypes()
  }

  async function create(payload: CreateServiceParentPayload): Promise<ServiceParent | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('servicesPremium.serviceParent.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('servicesPremium.serviceParent.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateServiceParentPayload): Promise<ServiceParent | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('servicesPremium.serviceParent.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('servicesPremium.serviceParent.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('servicesPremium.serviceParent.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('servicesPremium.serviceParent.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: ServiceParentFilters): void {
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
    totalAvecEssai,
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
