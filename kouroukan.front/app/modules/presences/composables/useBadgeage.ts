import { useBadgeageStore } from '../stores/badgeage.store'
import type {
  Badgeage,
  CreateBadgeagePayload,
  UpdateBadgeagePayload,
  BadgeageFilters,
} from '../types/badgeage.types'

export function useBadgeage() {
  const store = useBadgeageStore()
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

  const totalParMethode = computed(() => {
    const counts: Record<string, number> = {}
    store.items.forEach((b) => {
      counts[b.methodeBadgeage] = (counts[b.methodeBadgeage] || 0) + 1
    })
    return counts
  })

  const totalParPointAcces = computed(() => {
    const counts: Record<string, number> = {}
    store.items.forEach((b) => {
      counts[b.pointAcces] = (counts[b.pointAcces] || 0) + 1
    })
    return counts
  })

  async function fetchAll(params?: Partial<BadgeageFilters & { page?: number, pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('presences.badgeage.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<Badgeage | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('presences.badgeage.fetchError'), color: 'error' })
      return null
    }
  }

  async function fetchTypes(): Promise<void> {
    await store.fetchTypes()
  }

  async function create(payload: CreateBadgeagePayload): Promise<Badgeage | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('presences.badgeage.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('presences.badgeage.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateBadgeagePayload): Promise<Badgeage | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('presences.badgeage.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('presences.badgeage.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('presences.badgeage.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('presences.badgeage.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: BadgeageFilters): void {
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
    totalParMethode,
    totalParPointAcces,
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
