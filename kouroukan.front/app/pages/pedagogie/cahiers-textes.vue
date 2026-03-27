<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { CahierTextes, CreateCahierTextesPayload, UpdateCahierTextesPayload } from '~/modules/pedagogie/types/cahierTextes.types'
import { useCahierTextes } from '~/modules/pedagogie/composables/useCahierTextes'
import type { CahierTextesFilters } from '~/modules/pedagogie/types/cahierTextes.types'
import CahierTextesForm from '~/modules/pedagogie/components/CahierTextesForm.vue'
import CahierTextesCard from '~/modules/pedagogie/components/CahierTextesCard.vue'
import CahierTextesFiltersComponent from '~/modules/pedagogie/components/CahierTextesFilters.vue'
import CahierTextesStats from '~/modules/pedagogie/components/CahierTextesStats.vue'

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
  create,
  update,
  remove,
  setFilters,
  resetFilters,
  changePage,
} = useCahierTextes()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<CahierTextes | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<CahierTextes | null>(null)

const columns: Column[] = [
  { key: 'dateSeance', label: t('pedagogie.cahierTextes.dateSeance'), sortable: true },
  { key: 'matiereName', label: t('pedagogie.cahierTextes.matiere'), sortable: true },
  { key: 'classeName', label: t('pedagogie.cahierTextes.classe'), sortable: true },
  { key: 'contenu', label: t('pedagogie.cahierTextes.contenu'), sortable: false },
  { key: 'travailAFaire', label: t('pedagogie.cahierTextes.travailAFaire'), sortable: false },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(() => {
  fetchAll()
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: CahierTextes): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: CahierTextes): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateCahierTextesPayload | UpdateCahierTextesPayload): Promise<void> {
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

function handleFilter(filters: CahierTextesFilters): void {
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
            { label: $t('pedagogie.cahierTextes.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('pedagogie.cahierTextes.title') }}
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
          {{ $t('pedagogie.cahierTextes.add') }}
        </UButton>
      </div>
    </div>

    <!-- Stats -->
    <CahierTextesStats :items="items" :total-count="pagination.totalCount" />

    <!-- Filters -->
    <CahierTextesFiltersComponent @filter="handleFilter" @reset="resetFilters" />

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
        <template #cell-dateSeance="{ row }">
          {{ formatDateShort((row as CahierTextes).dateSeance) }}
        </template>
        <template #cell-contenu="{ row }">
          <span class="line-clamp-2 max-w-xs">{{ (row as CahierTextes).contenu }}</span>
        </template>
        <template #cell-travailAFaire="{ row }">
          <span v-if="(row as CahierTextes).travailAFaire" class="line-clamp-1 max-w-xs text-amber-600 dark:text-amber-400">
            {{ (row as CahierTextes).travailAFaire }}
          </span>
          <span v-else class="text-gray-400">-</span>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'pedagogie:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as CahierTextes)"
            />
            <UButton
              v-permission="'pedagogie:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as CahierTextes)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-document-text"
        :title="$t('pedagogie.cahierTextes.emptyTitle')"
        :description="$t('pedagogie.cahierTextes.emptyDescription')"
        :action-label="$t('pedagogie.cahierTextes.add')"
        @action="openCreate"
      />
    </template>

    <!-- Grid view -->
    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <CahierTextesCard
          v-for="cahier in items"
          :key="cahier.id"
          :cahier="cahier"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-document-text"
        :title="$t('pedagogie.cahierTextes.emptyTitle')"
        :description="$t('pedagogie.cahierTextes.emptyDescription')"
        :action-label="$t('pedagogie.cahierTextes.add')"
        @action="openCreate"
      />
    </template>

    <!-- Slideover Form -->
    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('pedagogie.cahierTextes.edit') : $t('pedagogie.cahierTextes.add') }}
        </h3>
      </template>
      <template #body>
        <CahierTextesForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <!-- Delete Confirmation -->
    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('pedagogie.cahierTextes.deleteTitle')"
      :description="$t('pedagogie.cahierTextes.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
