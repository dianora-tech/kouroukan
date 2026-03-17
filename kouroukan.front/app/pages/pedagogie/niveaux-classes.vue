<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { NiveauClasse, CreateNiveauClassePayload, UpdateNiveauClassePayload } from '~/modules/pedagogie/types/niveauClasse.types'
import { useNiveauClasse } from '~/modules/pedagogie/composables/useNiveauClasse'
import type { NiveauClasseFilters } from '~/modules/pedagogie/types/niveauClasse.types'
import NiveauClasseForm from '~/modules/pedagogie/components/NiveauClasseForm.vue'
import NiveauClasseCard from '~/modules/pedagogie/components/NiveauClasseCard.vue'
import NiveauClasseFiltersComponent from '~/modules/pedagogie/components/NiveauClasseFilters.vue'
import NiveauClasseStats from '~/modules/pedagogie/components/NiveauClasseStats.vue'

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
} = useNiveauClasse()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<NiveauClasse | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<NiveauClasse | null>(null)

const columns: Column[] = [
  { key: 'code', label: t('pedagogie.niveauClasse.code'), sortable: true },
  { key: 'name', label: t('pedagogie.niveauClasse.name'), sortable: true },
  { key: 'cycleEtude', label: t('pedagogie.niveauClasse.cycleEtude'), sortable: true },
  { key: 'ordre', label: t('pedagogie.niveauClasse.ordre'), sortable: true },
  { key: 'ministereTutelle', label: t('pedagogie.niveauClasse.ministereTutelle'), sortable: false },
  { key: 'examenSortie', label: t('pedagogie.niveauClasse.examenSortie'), sortable: false },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(() => {
  fetchAll()
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: NiveauClasse): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: NiveauClasse): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateNiveauClassePayload | UpdateNiveauClassePayload): Promise<void> {
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

function handleFilter(filters: NiveauClasseFilters): void {
  setFilters(filters)
}

function handleSort(_key: string, _direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getCycleColor(cycle: string): string {
  const colors: Record<string, string> = {
    Prescolaire: 'info',
    Primaire: 'success',
    College: 'warning',
    Lycee: 'error',
    Universite: 'primary',
  }
  return colors[cycle] ?? 'neutral'
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
            { label: $t('pedagogie.niveauClasse.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('pedagogie.niveauClasse.title') }}
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
          {{ $t('pedagogie.niveauClasse.add') }}
        </UButton>
      </div>
    </div>

    <!-- Stats -->
    <NiveauClasseStats :items="items" :total-count="pagination.totalCount" />

    <!-- Filters -->
    <NiveauClasseFiltersComponent @filter="handleFilter" @reset="resetFilters" />

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
        <template #cell-cycleEtude="{ row }">
          <UBadge :color="getCycleColor((row as NiveauClasse).cycleEtude)" variant="subtle" size="sm">
            {{ $t(`pedagogie.niveauClasse.cycle.${(row as NiveauClasse).cycleEtude}`) }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'pedagogie:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as NiveauClasse)"
            />
            <UButton
              v-permission="'pedagogie:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as NiveauClasse)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-academic-cap"
        :title="$t('pedagogie.niveauClasse.emptyTitle')"
        :description="$t('pedagogie.niveauClasse.emptyDescription')"
        :action-label="$t('pedagogie.niveauClasse.add')"
        @action="openCreate"
      />
    </template>

    <!-- Grid view -->
    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <NiveauClasseCard
          v-for="niveau in items"
          :key="niveau.id"
          :niveau-classe="niveau"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-academic-cap"
        :title="$t('pedagogie.niveauClasse.emptyTitle')"
        :description="$t('pedagogie.niveauClasse.emptyDescription')"
        :action-label="$t('pedagogie.niveauClasse.add')"
        @action="openCreate"
      />
    </template>

    <!-- Slideover Form -->
    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('pedagogie.niveauClasse.edit') : $t('pedagogie.niveauClasse.add') }}
        </h3>
      </template>
      <template #body>
        <NiveauClasseForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <!-- Delete Confirmation -->
    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('pedagogie.niveauClasse.deleteTitle')"
      :description="$t('pedagogie.niveauClasse.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
