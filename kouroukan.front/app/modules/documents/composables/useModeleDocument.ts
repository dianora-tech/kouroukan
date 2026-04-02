import { useModeleDocumentStore } from '../stores/modele-document.store'
import type {
  ModeleDocument,
  CreateModeleDocumentPayload,
  UpdateModeleDocumentPayload,
  ModeleDocumentFilters,
} from '../types/modele-document.types'

export function useModeleDocument() {
  const store = useModeleDocumentStore()
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
    store.items.filter(m => m.estActif).length,
  )

  const totalInactifs = computed(() =>
    store.items.filter(m => !m.estActif).length,
  )

  async function fetchAll(params?: Partial<ModeleDocumentFilters & { page?: number, pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('documents.modeleDocument.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<ModeleDocument | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('documents.modeleDocument.fetchError'), color: 'error' })
      return null
    }
  }

  async function fetchTypes(): Promise<void> {
    await store.fetchTypes()
  }

  async function create(payload: CreateModeleDocumentPayload): Promise<ModeleDocument | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('documents.modeleDocument.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('documents.modeleDocument.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateModeleDocumentPayload): Promise<ModeleDocument | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('documents.modeleDocument.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('documents.modeleDocument.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('documents.modeleDocument.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('documents.modeleDocument.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: ModeleDocumentFilters): void {
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
    totalInactifs,
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
