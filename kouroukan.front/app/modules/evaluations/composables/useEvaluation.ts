import { useEvaluationStore } from '../stores/evaluation.store'
import type {
  Evaluation,
  CreateEvaluationPayload,
  UpdateEvaluationPayload,
  EvaluationFilters,
} from '../types/evaluation.types'

export function useEvaluation() {
  const store = useEvaluationStore()
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

  const moyenneCoefficient = computed(() => {
    if (store.items.length === 0) return 0
    return store.items.reduce((sum, e) => sum + e.coefficient, 0) / store.items.length
  })

  const totalParTrimestre = computed(() => {
    const counts: Record<number, number> = { 1: 0, 2: 0, 3: 0 }
    store.items.forEach((e) => {
      if (counts[e.trimestre] !== undefined) counts[e.trimestre]++
    })
    return counts
  })

  async function fetchAll(params?: Partial<EvaluationFilters & { page?: number, pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('evaluations.evaluation.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<Evaluation | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('evaluations.evaluation.fetchError'), color: 'error' })
      return null
    }
  }

  async function fetchTypes(): Promise<void> {
    await store.fetchTypes()
  }

  async function create(payload: CreateEvaluationPayload): Promise<Evaluation | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('evaluations.evaluation.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('evaluations.evaluation.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateEvaluationPayload): Promise<Evaluation | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('evaluations.evaluation.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('evaluations.evaluation.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('evaluations.evaluation.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('evaluations.evaluation.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: EvaluationFilters): void {
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
    moyenneCoefficient,
    totalParTrimestre,
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
