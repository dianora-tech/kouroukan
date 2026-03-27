<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Absence, CreateAbsencePayload, UpdateAbsencePayload } from '~/modules/presences/types/absence.types'
import type { AbsenceFilters } from '~/modules/presences/types/absence.types'
import { useAbsence } from '~/modules/presences/composables/useAbsence'
import AbsenceForm from '~/modules/presences/components/AbsenceForm.vue'
import AbsenceCard from '~/modules/presences/components/AbsenceCard.vue'
import AbsenceFiltersComponent from '~/modules/presences/components/AbsenceFilters.vue'
import AbsenceStats from '~/modules/presences/components/AbsenceStats.vue'

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
} = useAbsence()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Absence | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Absence | null>(null)

const columns: Column[] = [
  { key: 'eleveNom', label: t('presences.absence.eleve'), sortable: true },
  { key: 'typeName', label: t('presences.absence.type'), sortable: true },
  { key: 'dateAbsence', label: t('presences.absence.dateAbsence'), sortable: true },
  { key: 'heureDebut', label: t('presences.absence.heureDebut'), sortable: true },
  { key: 'heureFin', label: t('presences.absence.heureFin'), sortable: true },
  { key: 'estJustifiee', label: t('presences.absence.justification'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await Promise.all([fetchAll(), fetchTypes()])
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Absence): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Absence): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateAbsencePayload | UpdateAbsencePayload): Promise<void> {
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

function handleFilter(filters: AbsenceFilters): void {
  setFilters(filters)
}

function handleSort(_key: string, _direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}
</script>

<template>
  <div class="space-y-6">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('nav.presences'), to: '/presences' },
            { label: $t('presences.absence.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('presences.absence.title') }}
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
          v-permission="'presences:create'"
          color="primary"
          icon="i-heroicons-plus"
          @click="openCreate"
        >
          {{ $t('presences.absence.add') }}
        </UButton>
      </div>
    </div>

    <AbsenceStats :items="items" :total-count="pagination.totalCount" />

    <AbsenceFiltersComponent @filter="handleFilter" @reset="resetFilters" />

    <template v-if="viewMode === 'table'">
      <DataTable
        v-if="!isEmpty || loading"
        :columns="columns"
        :data="paginatedData"
        :loading="loading"
        @page-change="changePage"
        @sort="handleSort"
      >
        <template #cell-eleveNom="{ row }">
          {{ (row as Absence).eleveNom ?? `#${(row as Absence).eleveId}` }}
        </template>
        <template #cell-dateAbsence="{ row }">
          {{ formatDateShort((row as Absence).dateAbsence) }}
        </template>
        <template #cell-estJustifiee="{ row }">
          <UBadge :color="(row as Absence).estJustifiee ? 'success' : 'error'" variant="subtle" size="sm">
            {{ (row as Absence).estJustifiee ? $t('presences.absence.justifiee') : $t('presences.absence.nonJustifiee') }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'presences:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Absence)"
            />
            <UButton
              v-permission="'presences:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Absence)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-exclamation-triangle"
        :title="$t('presences.absence.emptyTitle')"
        :description="$t('presences.absence.emptyDescription')"
        :action-label="$t('presences.absence.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <AbsenceCard
          v-for="absence in items"
          :key="absence.id"
          :absence="absence"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-exclamation-triangle"
        :title="$t('presences.absence.emptyTitle')"
        :description="$t('presences.absence.emptyDescription')"
        :action-label="$t('presences.absence.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('presences.absence.edit') : $t('presences.absence.add') }}
        </h3>
      </template>
      <template #body>
        <AbsenceForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('presences.absence.deleteTitle')"
      :description="$t('presences.absence.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
