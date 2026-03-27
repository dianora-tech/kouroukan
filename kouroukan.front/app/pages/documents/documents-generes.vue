<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { DocumentGenere, CreateDocumentGenerePayload, UpdateDocumentGenerePayload } from '~/modules/documents/types/document-genere.types'
import { useDocumentGenere } from '~/modules/documents/composables/useDocumentGenere'
import type { DocumentGenereFilters } from '~/modules/documents/types/document-genere.types'
import DocumentGenereForm from '~/modules/documents/components/DocumentGenereForm.vue'
import DocumentGenereCard from '~/modules/documents/components/DocumentGenereCard.vue'
import DocumentGenereFiltersComponent from '~/modules/documents/components/DocumentGenereFilters.vue'
import DocumentGenereStats from '~/modules/documents/components/DocumentGenereStats.vue'

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
} = useDocumentGenere()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<DocumentGenere | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<DocumentGenere | null>(null)

const columns: Column[] = [
  { key: 'name', label: t('documents.documentGenere.name'), sortable: true },
  { key: 'typeName', label: t('documents.documentGenere.type'), sortable: true },
  { key: 'modeleDocumentNom', label: t('documents.documentGenere.modeleDocumentId'), sortable: true },
  { key: 'eleveNom', label: t('documents.documentGenere.eleveId'), sortable: true },
  { key: 'dateGeneration', label: t('documents.documentGenere.dateGeneration'), sortable: true },
  { key: 'statutSignature', label: t('documents.documentGenere.statutSignature'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await Promise.all([fetchAll(), fetchTypes()])
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: DocumentGenere): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: DocumentGenere): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateDocumentGenerePayload | UpdateDocumentGenerePayload): Promise<void> {
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

function handleFilter(filters: DocumentGenereFilters): void {
  setFilters(filters)
}

function handleSort(key: string, direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    EnAttente: 'warning',
    EnCours: 'info',
    Signe: 'success',
    Refuse: 'error',
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
            { label: $t('nav.documents'), to: '/documents' },
            { label: $t('documents.documentGenere.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('documents.documentGenere.title') }}
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
          {{ $t('documents.documentGenere.add') }}
        </UButton>
      </div>
    </div>

    <DocumentGenereStats :items="items" :total-count="pagination.totalCount" />

    <DocumentGenereFiltersComponent @filter="handleFilter" @reset="resetFilters" />

    <template v-if="viewMode === 'table'">
      <DataTable
        v-if="!isEmpty || loading"
        :columns="columns"
        :data="paginatedData"
        :loading="loading"
        @page-change="changePage"
        @sort="handleSort"
      >
        <template #cell-dateGeneration="{ row }">
          {{ formatDateShort((row as DocumentGenere).dateGeneration) }}
        </template>
        <template #cell-statutSignature="{ row }">
          <UBadge :color="getStatutColor((row as DocumentGenere).statutSignature)" variant="subtle" size="sm">
            {{ $t(`documents.documentGenere.statut.${(row as DocumentGenere).statutSignature}`) }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'documents:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as DocumentGenere)"
            />
            <UButton
              v-permission="'documents:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as DocumentGenere)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-document-text"
        :title="$t('documents.documentGenere.emptyTitle')"
        :description="$t('documents.documentGenere.emptyDescription')"
        :action-label="$t('documents.documentGenere.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <DocumentGenereCard
          v-for="doc in items"
          :key="doc.id"
          :document-genere="doc"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-document-text"
        :title="$t('documents.documentGenere.emptyTitle')"
        :description="$t('documents.documentGenere.emptyDescription')"
        :action-label="$t('documents.documentGenere.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('documents.documentGenere.edit') : $t('documents.documentGenere.add') }}
        </h3>
      </template>
      <template #body>
        <DocumentGenereForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('documents.documentGenere.deleteTitle')"
      :description="$t('documents.documentGenere.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
