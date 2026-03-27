<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Evaluation, CreateEvaluationPayload, UpdateEvaluationPayload } from '~/modules/evaluations/types/evaluation.types'
import { useEvaluation } from '~/modules/evaluations/composables/useEvaluation'
import type { EvaluationFilters } from '~/modules/evaluations/types/evaluation.types'
import EvaluationForm from '~/modules/evaluations/components/EvaluationForm.vue'
import EvaluationCard from '~/modules/evaluations/components/EvaluationCard.vue'
import EvaluationFiltersComponent from '~/modules/evaluations/components/EvaluationFilters.vue'
import EvaluationStats from '~/modules/evaluations/components/EvaluationStats.vue'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { formatDateShort } = useFormatDate()
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
} = useEvaluation()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Evaluation | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Evaluation | null>(null)

const columns: Column[] = [
  { key: 'typeName', label: t('evaluations.evaluation.type'), sortable: true },
  { key: 'matiereName', label: t('evaluations.evaluation.matiere'), sortable: true },
  { key: 'classeName', label: t('evaluations.evaluation.classe'), sortable: true },
  { key: 'enseignantNom', label: t('evaluations.evaluation.enseignant'), sortable: true },
  { key: 'dateEvaluation', label: t('evaluations.evaluation.dateEvaluation'), sortable: true },
  { key: 'coefficient', label: t('evaluations.evaluation.coefficient'), sortable: true },
  { key: 'noteMaximale', label: t('evaluations.evaluation.noteMaximale'), sortable: true },
  { key: 'trimestre', label: t('evaluations.evaluation.trimestreLabel'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await Promise.all([fetchAll(), fetchTypes()])
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Evaluation): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Evaluation): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateEvaluationPayload | UpdateEvaluationPayload): Promise<void> {
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

function handleFilter(filters: EvaluationFilters): void {
  setFilters(filters)
}

function handleSort(key: string, direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}
</script>

<template>
  <div class="space-y-6">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('nav.evaluations'), to: '/evaluations' },
            { label: $t('evaluations.evaluation.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('evaluations.evaluation.title') }}
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
          v-permission="'evaluations:create'"
          color="primary"
          icon="i-heroicons-plus"
          @click="openCreate"
        >
          {{ $t('evaluations.evaluation.add') }}
        </UButton>
      </div>
    </div>

    <EvaluationStats :items="items" :total-count="pagination.totalCount" />

    <EvaluationFiltersComponent @filter="handleFilter" @reset="resetFilters" />

    <template v-if="viewMode === 'table'">
      <DataTable
        v-if="!isEmpty || loading"
        :columns="columns"
        :data="paginatedData"
        :loading="loading"
        @page-change="changePage"
        @sort="handleSort"
      >
        <template #cell-dateEvaluation="{ row }">
          {{ formatDateShort((row as Evaluation).dateEvaluation) }}
        </template>
        <template #cell-trimestre="{ row }">
          <UBadge color="primary" variant="subtle" size="sm">
            {{ $t(`evaluations.evaluation.trimestre.${(row as Evaluation).trimestre}`) }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'evaluations:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Evaluation)"
            />
            <UButton
              v-permission="'evaluations:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Evaluation)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-clipboard-document-list"
        :title="$t('evaluations.evaluation.emptyTitle')"
        :description="$t('evaluations.evaluation.emptyDescription')"
        :action-label="$t('evaluations.evaluation.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <EvaluationCard
          v-for="evaluation in items"
          :key="evaluation.id"
          :evaluation="evaluation"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-clipboard-document-list"
        :title="$t('evaluations.evaluation.emptyTitle')"
        :description="$t('evaluations.evaluation.emptyDescription')"
        :action-label="$t('evaluations.evaluation.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('evaluations.evaluation.edit') : $t('evaluations.evaluation.add') }}
        </h3>
      </template>
      <template #body>
        <EvaluationForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('evaluations.evaluation.deleteTitle')"
      :description="$t('evaluations.evaluation.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
