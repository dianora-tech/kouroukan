import { useNoteStore } from '../stores/note.store'
import type {
  Note,
  CreateNotePayload,
  UpdateNotePayload,
  NoteFilters,
} from '../types/note.types'

export function useNote() {
  const store = useNoteStore()
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

  const moyenneNotes = computed(() => {
    if (store.items.length === 0) return 0
    return store.items.reduce((sum, n) => sum + n.valeur, 0) / store.items.length
  })

  const noteMax = computed(() => {
    if (store.items.length === 0) return 0
    return Math.max(...store.items.map(n => n.valeur))
  })

  const noteMin = computed(() => {
    if (store.items.length === 0) return 0
    return Math.min(...store.items.map(n => n.valeur))
  })

  async function fetchAll(params?: Partial<NoteFilters & { page?: number; pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('evaluations.note.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<Note | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('evaluations.note.fetchError'), color: 'error' })
      return null
    }
  }

  async function create(payload: CreateNotePayload): Promise<Note | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('evaluations.note.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('evaluations.note.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateNotePayload): Promise<Note | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('evaluations.note.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('evaluations.note.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('evaluations.note.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('evaluations.note.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: NoteFilters): void {
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
    moyenneNotes,
    noteMax,
    noteMin,
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
