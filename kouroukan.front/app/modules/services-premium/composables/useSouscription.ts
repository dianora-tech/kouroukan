import { useSouscriptionStore } from '../stores/souscription.store'
import type {
  Souscription,
  CreateSouscriptionPayload,
  UpdateSouscriptionPayload,
  SouscriptionFilters,
} from '../types/souscription.types'

export function useSouscription() {
  const store = useSouscriptionStore()
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

  const totalMontantPaye = computed(() =>
    store.items.reduce((sum, s) => sum + s.montantPaye, 0),
  )

  const totalActives = computed(() =>
    store.items.filter(s => s.statutSouscription === 'Active').length,
  )

  const totalEssai = computed(() =>
    store.items.filter(s => s.statutSouscription === 'Essai').length,
  )

  async function fetchAll(params?: Partial<SouscriptionFilters & { page?: number, pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('servicesPremium.souscription.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<Souscription | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('servicesPremium.souscription.fetchError'), color: 'error' })
      return null
    }
  }

  async function create(payload: CreateSouscriptionPayload): Promise<Souscription | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('servicesPremium.souscription.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('servicesPremium.souscription.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateSouscriptionPayload): Promise<Souscription | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('servicesPremium.souscription.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('servicesPremium.souscription.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('servicesPremium.souscription.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('servicesPremium.souscription.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: SouscriptionFilters): void {
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
    totalMontantPaye,
    totalActives,
    totalEssai,
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
