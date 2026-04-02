<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { DossierAdmission, CreateDossierAdmissionPayload, UpdateDossierAdmissionPayload, DossierAdmissionFilters } from '~/modules/inscriptions/types/dossier-admission.types'
import { useDossierAdmission } from '~/modules/inscriptions/composables/useDossierAdmission'
import DossierAdmissionForm from '~/modules/inscriptions/components/DossierAdmissionForm.vue'
import DossierAdmissionCard from '~/modules/inscriptions/components/DossierAdmissionCard.vue'
import DossierAdmissionFiltersComponent from '~/modules/inscriptions/components/DossierAdmissionFilters.vue'
import DossierAdmissionStats from '~/modules/inscriptions/components/DossierAdmissionStats.vue'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { formatDate } = useFormatDate()
const {
  items,
  loading,
  saving,
  isEmpty,
  paginatedData,
  pagination,
  fetchAll,
  fetchTypes,
  create,
  update,
  remove,
  setFilters,
  resetFilters,
  changePage,
} = useDossierAdmission()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<DossierAdmission | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<DossierAdmission | null>(null)

const columns: Column[] = [
  { key: 'eleveNom', label: t('inscriptions.dossierAdmission.eleveNom'), sortable: true },
  { key: 'typeName', label: t('inscriptions.dossierAdmission.type'), sortable: true },
  { key: 'statutDossier', label: t('inscriptions.dossierAdmission.statutDossier'), sortable: true },
  { key: 'etapeActuelle', label: t('inscriptions.dossierAdmission.etapeActuelle'), sortable: true },
  { key: 'dateDemande', label: t('inscriptions.dossierAdmission.dateDemande'), sortable: true, render: (row: any) => formatDate(row.dateDemande) },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await Promise.all([fetchAll(), fetchTypes()])
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: DossierAdmission): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: DossierAdmission): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateDossierAdmissionPayload | UpdateDossierAdmissionPayload): Promise<void> {
  if ('id' in payload) {
    const result = await update(payload.id, payload)
    if (result) showForm.value = false
  }
  else {
    const result = await create(payload)
    if (result) showForm.value = false
  }
}

async function handleDelete(): Promise<void> {
  if (!deletingEntity.value) return
  const success = await remove(deletingEntity.value.id)
  if (success) {
    showDeleteDialog.value = false
    deletingEntity.value = null
  }
}

function handleFilter(filters: DossierAdmissionFilters): void {
  setFilters(filters)
}

function handleSort(key: string, direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    Prospect: 'neutral',
    PreInscrit: 'info',
    EnEtude: 'warning',
    Convoque: 'info',
    Admis: 'success',
    Refuse: 'error',
    ListeAttente: 'warning',
  }
  return colors[statut] ?? 'neutral'
}
</script>

<template>
  <div class="space-y-6">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('nav.inscriptions'), to: '/inscriptions' },
            { label: $t('inscriptions.dossierAdmission.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('inscriptions.dossierAdmission.title') }}
        </h1>
      </div>
      <div class="flex items-center gap-2">
        <UButton
          :variant="viewMode === 'table' ? 'solid' : 'outline'"
          size="sm"
          icon="i-heroicons-table-cells"
          @click="viewMode = 'table'"
        />
        <UButton
          :variant="viewMode === 'grid' ? 'solid' : 'outline'"
          size="sm"
          icon="i-heroicons-squares-2x2"
          @click="viewMode = 'grid'"
        />
        <UButton
          v-permission="'inscriptions:create'"
          color="primary"
          icon="i-heroicons-plus"
          @click="openCreate"
        >
          {{ $t('inscriptions.dossierAdmission.add') }}
        </UButton>
      </div>
    </div>

    <DossierAdmissionStats
      :items="items"
      :total-count="pagination.totalCount"
    />

    <DossierAdmissionFiltersComponent
      @filter="handleFilter"
      @reset="resetFilters"
    />

    <template v-if="viewMode === 'table'">
      <DataTable
        v-if="!isEmpty || loading"
        :columns="columns"
        :data="paginatedData"
        :loading="loading"
        @page-change="changePage"
        @sort="handleSort"
      >
        <template #cell-statutDossier="{ row }">
          <UBadge
            :color="getStatutColor((row as DossierAdmission).statutDossier)"
            variant="subtle"
            size="sm"
          >
            {{ $t(`inscriptions.dossierAdmission.statut.${(row as DossierAdmission).statutDossier}`) }}
          </UBadge>
        </template>
        <template #cell-etapeActuelle="{ row }">
          {{ $t(`inscriptions.dossierAdmission.etape.${(row as DossierAdmission).etapeActuelle}`) }}
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'inscriptions:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as DossierAdmission)"
            />
            <UButton
              v-permission="'inscriptions:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as DossierAdmission)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-document-text"
        :title="$t('inscriptions.dossierAdmission.emptyTitle')"
        :description="$t('inscriptions.dossierAdmission.emptyDescription')"
        :action-label="$t('inscriptions.dossierAdmission.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div
        v-if="!isEmpty"
        class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3"
      >
        <DossierAdmissionCard
          v-for="dossier in items"
          :key="dossier.id"
          :dossier="dossier"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-document-text"
        :title="$t('inscriptions.dossierAdmission.emptyTitle')"
        :description="$t('inscriptions.dossierAdmission.emptyDescription')"
        :action-label="$t('inscriptions.dossierAdmission.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('inscriptions.dossierAdmission.edit') : $t('inscriptions.dossierAdmission.add') }}
        </h3>
      </template>
      <template #body>
        <DossierAdmissionForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('inscriptions.dossierAdmission.deleteTitle')"
      :description="$t('inscriptions.dossierAdmission.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
