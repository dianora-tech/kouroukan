import { useEtablissementAdminStore } from '../stores/etablissement-admin.store'
import type {
  EtablissementAdmin,
  UpdateEtablissementAdminPayload,
  AdminFilters,
} from '../types/admin.types'

export function useAdminEtablissement() {
  const store = useEtablissementAdminStore()
  const toast = useToast()
  const { t } = useI18n()

  const items = computed(() => store.items)
  const currentItem = computed(() => store.currentItem)
  const loading = computed(() => store.loading)
  const saving = computed(() => store.saving)
  const isEmpty = computed(() => store.isEmpty)
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

  async function fetchAll(params?: Partial<AdminFilters & { page?: number, pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('admin.etablissement.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<EtablissementAdmin | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('admin.etablissement.fetchError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateEtablissementAdminPayload): Promise<boolean> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('admin.etablissement.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('admin.etablissement.updateError'), color: 'error' })
      return false
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const result = await store.remove(id)
      if (result) {
        toast.add({ title: t('admin.etablissement.deleteSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('admin.etablissement.deleteError'), color: 'error' })
      return false
    }
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
    pagination,
    paginatedData,
    fetchAll,
    fetchById,
    update,
    remove,
    changePage,
  }
}
