import { useDossierAdmissionStore } from '../stores/dossier-admission.store'
import type {
  DossierAdmission,
  CreateDossierAdmissionPayload,
  UpdateDossierAdmissionPayload,
  DossierAdmissionFilters,
} from '../types/dossier-admission.types'

export function useDossierAdmission() {
  const store = useDossierAdmissionStore()
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

  async function fetchAll(params?: Partial<DossierAdmissionFilters & { page?: number, pageSize?: number }>): Promise<void> {
    try {
      await store.fetchAll(params)
    }
    catch {
      toast.add({ title: t('inscriptions.dossierAdmission.fetchError'), color: 'error' })
    }
  }

  async function fetchById(id: number): Promise<DossierAdmission | null> {
    try {
      return await store.fetchById(id)
    }
    catch {
      toast.add({ title: t('inscriptions.dossierAdmission.fetchError'), color: 'error' })
      return null
    }
  }

  async function fetchTypes(): Promise<void> {
    await store.fetchTypes()
  }

  async function create(payload: CreateDossierAdmissionPayload): Promise<DossierAdmission | null> {
    try {
      const result = await store.create(payload)
      if (result) {
        toast.add({ title: t('inscriptions.dossierAdmission.createSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('inscriptions.dossierAdmission.createError'), color: 'error' })
      return null
    }
  }

  async function update(id: number, payload: UpdateDossierAdmissionPayload): Promise<DossierAdmission | null> {
    try {
      const result = await store.update(id, payload)
      if (result) {
        toast.add({ title: t('inscriptions.dossierAdmission.updateSuccess'), color: 'success' })
      }
      return result
    }
    catch {
      toast.add({ title: t('inscriptions.dossierAdmission.updateError'), color: 'error' })
      return null
    }
  }

  async function remove(id: number): Promise<boolean> {
    try {
      const success = await store.remove(id)
      if (success) {
        toast.add({ title: t('inscriptions.dossierAdmission.deleteSuccess'), color: 'success' })
      }
      return success
    }
    catch {
      toast.add({ title: t('inscriptions.dossierAdmission.deleteError'), color: 'error' })
      return false
    }
  }

  function setFilters(filters: DossierAdmissionFilters): void {
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
