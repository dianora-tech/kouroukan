<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { ModeleDocument, CreateModeleDocumentPayload, UpdateModeleDocumentPayload } from '~/modules/documents/types/modele-document.types'
import { useModeleDocument } from '~/modules/documents/composables/useModeleDocument'
import type { ModeleDocumentFilters } from '~/modules/documents/types/modele-document.types'
import ModeleDocumentForm from '~/modules/documents/components/ModeleDocumentForm.vue'
import ModeleDocumentCard from '~/modules/documents/components/ModeleDocumentCard.vue'
import ModeleDocumentFiltersComponent from '~/modules/documents/components/ModeleDocumentFilters.vue'
import ModeleDocumentStats from '~/modules/documents/components/ModeleDocumentStats.vue'

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
} = useModeleDocument()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<ModeleDocument | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<ModeleDocument | null>(null)

const columns: Column[] = [
  { key: 'code', label: t('documents.modeleDocument.code'), sortable: true },
  { key: 'name', label: t('documents.modeleDocument.name'), sortable: true },
  { key: 'typeName', label: t('documents.modeleDocument.type'), sortable: true },
  { key: 'estActif', label: t('documents.modeleDocument.estActif'), sortable: true },
  { key: 'createdAt', label: t('documents.modeleDocument.createdAt'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await Promise.all([fetchAll(), fetchTypes()])
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: ModeleDocument): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: ModeleDocument): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateModeleDocumentPayload | UpdateModeleDocumentPayload): Promise<void> {
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

function handleFilter(filters: ModeleDocumentFilters): void {
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
            { label: $t('nav.documents'), to: '/documents' },
            { label: $t('documents.modeleDocument.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('documents.modeleDocument.title') }}
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
          v-permission="'documents:create'"
          color="primary"
          icon="i-heroicons-plus"
          @click="openCreate"
        >
          {{ $t('documents.modeleDocument.add') }}
        </UButton>
      </div>
    </div>

    <ModeleDocumentStats :items="items" :total-count="pagination.totalCount" />

    <ModeleDocumentFiltersComponent @filter="handleFilter" @reset="resetFilters" />

    <template v-if="viewMode === 'table'">
      <DataTable
        v-if="!isEmpty || loading"
        :columns="columns"
        :data="paginatedData"
        :loading="loading"
        @page-change="changePage"
        @sort="handleSort"
      >
        <template #cell-estActif="{ row }">
          <UBadge :color="(row as ModeleDocument).estActif ? 'success' : 'neutral'" variant="subtle" size="sm">
            {{ (row as ModeleDocument).estActif ? $t('documents.modeleDocument.actif') : $t('documents.modeleDocument.inactif') }}
          </UBadge>
        </template>
        <template #cell-createdAt="{ row }">
          {{ (row as ModeleDocument).createdAt?.split('T')[0] }}
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'documents:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as ModeleDocument)"
            />
            <UButton
              v-permission="'documents:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as ModeleDocument)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-document-duplicate"
        :title="$t('documents.modeleDocument.emptyTitle')"
        :description="$t('documents.modeleDocument.emptyDescription')"
        :action-label="$t('documents.modeleDocument.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <ModeleDocumentCard
          v-for="modele in items"
          :key="modele.id"
          :modele-document="modele"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-document-duplicate"
        :title="$t('documents.modeleDocument.emptyTitle')"
        :description="$t('documents.modeleDocument.emptyDescription')"
        :action-label="$t('documents.modeleDocument.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('documents.modeleDocument.edit') : $t('documents.modeleDocument.add') }}
        </h3>
      </template>
      <template #body>
        <ModeleDocumentForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('documents.modeleDocument.deleteTitle')"
      :description="$t('documents.modeleDocument.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
