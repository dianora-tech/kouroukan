<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Matiere, CreateMatierePayload, UpdateMatierePayload } from '~/modules/pedagogie/types/matiere.types'
import { useMatiere } from '~/modules/pedagogie/composables/useMatiere'
import type { MatiereFilters } from '~/modules/pedagogie/types/matiere.types'
import MatiereForm from '~/modules/pedagogie/components/MatiereForm.vue'
import MatiereCard from '~/modules/pedagogie/components/MatiereCard.vue'
import MatiereFiltersComponent from '~/modules/pedagogie/components/MatiereFilters.vue'
import MatiereStats from '~/modules/pedagogie/components/MatiereStats.vue'

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
  fetchTypes,
  create,
  update,
  remove,
  setFilters,
  resetFilters,
  changePage,
} = useMatiere()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Matiere | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Matiere | null>(null)

const columns: Column[] = [
  { key: 'code', label: t('pedagogie.matiere.code'), sortable: true },
  { key: 'name', label: t('pedagogie.matiere.name'), sortable: true },
  { key: 'typeName', label: t('pedagogie.matiere.type'), sortable: true },
  { key: 'coefficient', label: t('pedagogie.matiere.coefficient'), sortable: true },
  { key: 'nombreHeures', label: t('pedagogie.matiere.nombreHeures'), sortable: true },
  { key: 'niveauClasseName', label: t('pedagogie.matiere.niveauClasse'), sortable: false },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(() => {
  fetchAll()
  fetchTypes()
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Matiere): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Matiere): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateMatierePayload | UpdateMatierePayload): Promise<void> {
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

function handleFilter(filters: MatiereFilters): void {
  setFilters(filters)
}

function handleSort(_key: string, _direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
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
            { label: $t('pedagogie.matiere.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('pedagogie.matiere.title') }}
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
          {{ $t('pedagogie.matiere.add') }}
        </UButton>
      </div>
    </div>

    <!-- Stats -->
    <MatiereStats :items="items" :total-count="pagination.totalCount" />

    <!-- Filters -->
    <MatiereFiltersComponent @filter="handleFilter" @reset="resetFilters" />

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
        <template #cell-typeName="{ row }">
          <UBadge v-if="(row as Matiere).typeName" color="primary" variant="subtle" size="sm">
            {{ (row as Matiere).typeName }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'pedagogie:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Matiere)"
            />
            <UButton
              v-permission="'pedagogie:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Matiere)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-book-open"
        :title="$t('pedagogie.matiere.emptyTitle')"
        :description="$t('pedagogie.matiere.emptyDescription')"
        :action-label="$t('pedagogie.matiere.add')"
        @action="openCreate"
      />
    </template>

    <!-- Grid view -->
    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <MatiereCard
          v-for="matiere in items"
          :key="matiere.id"
          :matiere="matiere"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-book-open"
        :title="$t('pedagogie.matiere.emptyTitle')"
        :description="$t('pedagogie.matiere.emptyDescription')"
        :action-label="$t('pedagogie.matiere.add')"
        @action="openCreate"
      />
    </template>

    <!-- Slideover Form -->
    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('pedagogie.matiere.edit') : $t('pedagogie.matiere.add') }}
        </h3>
      </template>
      <template #body>
        <MatiereForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <!-- Delete Confirmation -->
    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('pedagogie.matiere.deleteTitle')"
      :description="$t('pedagogie.matiere.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
