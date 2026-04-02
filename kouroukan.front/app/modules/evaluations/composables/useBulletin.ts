import { useBulletinStore } from '../stores/bulletin.store'
import type {
  Bulletin,
  CreateBulletinPayload,
  UpdateBulletinPayload,
  BulletinFilters,
} from '../types/bulletin.types'

export function useBulletin() {
  const store = useBulletinStore()
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

  const moyenneGeneraleClasse = computed(() => {
    if (store.items.length === 0) return 0
    return store.items.reduce((sum, b) => sum + b.moyenneGenerale, 0) / store.items.length
  })

  const totalPublies = computed(() =>
    store.items.filter(b => b.estPublie).length,
  )

  const totalNonPublies = computed(() =>
    store.items.filter(b => !b.estPublie).length,
  )

  async function fetchAll(params?: Partial<BulletinFilters & { page?: number, pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('evaluations.bulletin.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<Bulletin | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('evaluations.bulletin.fetchError'), color: 'error' })
      return null
    }
  }

  async function create(payload: CreateBulletinPayload): Promise<Bulletin | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('evaluations.bulletin.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('evaluations.bulletin.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateBulletinPayload): Promise<Bulletin | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('evaluations.bulletin.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('evaluations.bulletin.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('evaluations.bulletin.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('evaluations.bulletin.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: BulletinFilters): void {
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
    moyenneGeneraleClasse,
    totalPublies,
    totalNonPublies,
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
