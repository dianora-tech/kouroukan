import { useMessageStore } from '../stores/message.store'
import type { Message, CreateMessagePayload, UpdateMessagePayload, MessageFilters } from '../types/message.types'

export function useMessage() {
  const store = useMessageStore()
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

  async function fetchAll(params?: Partial<MessageFilters & { page?: number; pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('communication.message.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<Message | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('communication.message.fetchError'), color: 'error' })
      return null
    }
  }

  async function fetchTypes(): Promise<void> {
    try {
      await store.fetchTypes()
    }
    catch {
      toast.add({ title: t('communication.message.fetchError'), color: 'error' })
    }
  }

  async function create(payload: CreateMessagePayload): Promise<Message | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('communication.message.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('communication.message.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateMessagePayload): Promise<Message | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('communication.message.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('communication.message.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('communication.message.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('communication.message.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: MessageFilters): void {
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
