<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Seance, CreateSeancePayload, UpdateSeancePayload, SeanceFilters } from '~/modules/pedagogie/types/seance.types'
import { useSeance } from '~/modules/pedagogie/composables/useSeance'
import SeanceForm from '~/modules/pedagogie/components/SeanceForm.vue'
import SeanceCard from '~/modules/pedagogie/components/SeanceCard.vue'
import SeanceFiltersComponent from '~/modules/pedagogie/components/SeanceFilters.vue'
import SeanceStats from '~/modules/pedagogie/components/SeanceStats.vue'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const {
  items,
  loading,
  saving,
  isEmpty,
  paginatedData,
  pagination,
  fetchAll,
  create,
  update,
  remove,
  setFilters,
  resetFilters,
  changePage,
} = useSeance()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Seance | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Seance | null>(null)

const columns: Column[] = [
  { key: 'matiereName', label: t('pedagogie.seance.matiere'), sortable: true },
  { key: 'classeName', label: t('pedagogie.seance.classe'), sortable: true },
  { key: 'enseignantNom', label: t('pedagogie.seance.enseignant'), sortable: false },
  { key: 'salleName', label: t('pedagogie.seance.salle'), sortable: false },
  { key: 'jourSemaine', label: t('pedagogie.seance.jourSemaine'), sortable: true },
  { key: 'heureDebut', label: t('pedagogie.seance.heureDebut'), sortable: true },
  { key: 'heureFin', label: t('pedagogie.seance.heureFin'), sortable: false },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(() => {
  fetchAll()
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Seance): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Seance): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateSeancePayload | UpdateSeancePayload): Promise<void> {
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

function handleFilter(filters: SeanceFilters): void {
  setFilters(filters)
}

function handleSort(_key: string, _direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getJourColor(jour: number): string {
  const colors: Record<number, string> = {
    1: 'primary',
    2: 'success',
    3: 'warning',
    4: 'error',
    5: 'info',
    6: 'neutral',
  }
  return colors[jour] ?? 'neutral'
}
</script>

<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('nav.pedagogie'), to: '/pedagogie' },
            { label: $t('pedagogie.seance.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('pedagogie.seance.title') }}
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
          v-permission="'pedagogie:create'"
          color="primary"
          icon="i-heroicons-plus"
          @click="openCreate"
        >
          {{ $t('pedagogie.seance.add') }}
        </UButton>
      </div>
    </div>

    <!-- Stats -->
    <SeanceStats
      :items="items"
      :total-count="pagination.totalCount"
    />

    <!-- Filters -->
    <SeanceFiltersComponent
      @filter="handleFilter"
      @reset="resetFilters"
    />

    <!-- Table view -->
    <template v-if="viewMode === 'table'">
      <DataTable
        v-if="!isEmpty || loading"
        :columns="columns"
        :data="paginatedData"
        :loading="loading"
        @page-change="changePage"
        @sort="handleSort"
      >
        <template #cell-jourSemaine="{ row }">
          <UBadge
            :color="getJourColor((row as Seance).jourSemaine)"
            variant="subtle"
            size="sm"
          >
            {{ $t(`pedagogie.seance.jour.${(row as Seance).jourSemaine}`) }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'pedagogie:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Seance)"
            />
            <UButton
              v-permission="'pedagogie:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Seance)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-clock"
        :title="$t('pedagogie.seance.emptyTitle')"
        :description="$t('pedagogie.seance.emptyDescription')"
        :action-label="$t('pedagogie.seance.add')"
        @action="openCreate"
      />
    </template>

    <!-- Grid view -->
    <template v-else>
      <div
        v-if="!isEmpty"
        class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3"
      >
        <SeanceCard
          v-for="seance in items"
          :key="seance.id"
          :seance="seance"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-clock"
        :title="$t('pedagogie.seance.emptyTitle')"
        :description="$t('pedagogie.seance.emptyDescription')"
        :action-label="$t('pedagogie.seance.add')"
        @action="openCreate"
      />
    </template>

    <!-- Slideover Form -->
    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('pedagogie.seance.edit') : $t('pedagogie.seance.add') }}
        </h3>
      </template>
      <template #body>
        <SeanceForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <!-- Delete Confirmation -->
    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('pedagogie.seance.deleteTitle')"
      :description="$t('pedagogie.seance.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
