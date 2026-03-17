import { useSignatureStore } from '../stores/signature.store'
import type {
  Signature,
  CreateSignaturePayload,
  UpdateSignaturePayload,
  SignatureFilters,
} from '../types/signature.types'

export function useSignature() {
  const store = useSignatureStore()
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

  const totalSignees = computed(() =>
    store.items.filter(s => s.statutSignature === 'Signe').length,
  )

  const totalEnAttente = computed(() =>
    store.items.filter(s => s.statutSignature === 'EnAttente').length,
  )

  const totalRefusees = computed(() =>
    store.items.filter(s => s.statutSignature === 'Refuse').length,
  )

  async function fetchAll(params?: Partial<SignatureFilters & { page?: number; pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('documents.signature.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<Signature | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('documents.signature.fetchError'), color: 'error' })
      return null
    }
  }

  async function fetchTypes(): Promise<void> {
    await store.fetchTypes()
  }

  async function create(payload: CreateSignaturePayload): Promise<Signature | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('documents.signature.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('documents.signature.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateSignaturePayload): Promise<Signature | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('documents.signature.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('documents.signature.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('documents.signature.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('documents.signature.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: SignatureFilters): void {
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
    totalSignees,
    totalEnAttente,
    totalRefusees,
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
