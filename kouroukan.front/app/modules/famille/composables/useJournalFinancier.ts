import { useJournalFinancierStore } from '../stores/journal-financier.store'
import type {
  JournalFinancier,
  CreateJournalFinancierPayload,
} from '../types/famille.types'

export function useJournalFinancier() {
  const store = useJournalFinancierStore()
  const toast = useToast()
  const { t } = useI18n()

  const items = computed(() => store.items)
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

  async function fetchAll(params?: { page?: number; pageSize?: number; enfantId?: number }): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('famille.paiements.fetchError'), color: 'error' })
    }
  }

  async function create(payload: CreateJournalFinancierPayload): Promise<JournalFinancier | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('famille.paiements.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('famille.paiements.createError'), color: 'error' })
      return null
    }
  }

  function changePage(page: number): void {
    fetchAll({ page })
  }

  return {
    items,
    loading,
    saving,
    isEmpty,
    pagination,
    paginatedData,
    fetchAll,
    create,
    changePage,
  }
}
