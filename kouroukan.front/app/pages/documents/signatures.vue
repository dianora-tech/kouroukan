<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Signature, CreateSignaturePayload, UpdateSignaturePayload } from '~/modules/documents/types/signature.types'
import { useSignature } from '~/modules/documents/composables/useSignature'
import type { SignatureFilters } from '~/modules/documents/types/signature.types'
import SignatureForm from '~/modules/documents/components/SignatureForm.vue'
import SignatureCard from '~/modules/documents/components/SignatureCard.vue'
import SignatureFiltersComponent from '~/modules/documents/components/SignatureFilters.vue'
import SignatureStats from '~/modules/documents/components/SignatureStats.vue'

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
} = useSignature()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Signature | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Signature | null>(null)

const columns: Column[] = [
  { key: 'name', label: t('documents.signature.name'), sortable: true },
  { key: 'typeName', label: t('documents.signature.type'), sortable: true },
  { key: 'signataireNom', label: t('documents.signature.signataireId'), sortable: true },
  { key: 'ordreSignature', label: t('documents.signature.ordreSignature'), sortable: true },
  { key: 'niveauSignature', label: t('documents.signature.niveauSignature'), sortable: true },
  { key: 'statutSignature', label: t('documents.signature.statutSignature'), sortable: true },
  { key: 'dateSignature', label: t('documents.signature.dateSignature'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await Promise.all([fetchAll(), fetchTypes()])
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Signature): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Signature): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateSignaturePayload | UpdateSignaturePayload): Promise<void> {
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

function handleFilter(filters: SignatureFilters): void {
  setFilters(filters)
}

function handleSort(key: string, direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    EnAttente: 'warning',
    Signe: 'success',
    Refuse: 'error',
    Delegue: 'info',
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
            { label: $t('documents.signature.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('documents.signature.title') }}
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
          {{ $t('documents.signature.add') }}
        </UButton>
      </div>
    </div>

    <SignatureStats :items="items" :total-count="pagination.totalCount" />

    <SignatureFiltersComponent @filter="handleFilter" @reset="resetFilters" />

    <template v-if="viewMode === 'table'">
      <DataTable
        v-if="!isEmpty || loading"
        :columns="columns"
        :data="paginatedData"
        :loading="loading"
        @page-change="changePage"
        @sort="handleSort"
      >
        <template #cell-niveauSignature="{ row }">
          <UBadge color="neutral" variant="subtle" size="sm">
            {{ $t(`documents.signature.niveau.${(row as Signature).niveauSignature}`) }}
          </UBadge>
        </template>
        <template #cell-statutSignature="{ row }">
          <UBadge :color="getStatutColor((row as Signature).statutSignature)" variant="subtle" size="sm">
            {{ $t(`documents.signature.statut.${(row as Signature).statutSignature}`) }}
          </UBadge>
        </template>
        <template #cell-dateSignature="{ row }">
          {{ (row as Signature).dateSignature?.split('T')[0] ?? '-' }}
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'documents:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Signature)"
            />
            <UButton
              v-permission="'documents:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Signature)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-pencil"
        :title="$t('documents.signature.emptyTitle')"
        :description="$t('documents.signature.emptyDescription')"
        :action-label="$t('documents.signature.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <SignatureCard
          v-for="sig in items"
          :key="sig.id"
          :signature="sig"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-pencil"
        :title="$t('documents.signature.emptyTitle')"
        :description="$t('documents.signature.emptyDescription')"
        :action-label="$t('documents.signature.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('documents.signature.edit') : $t('documents.signature.add') }}
        </h3>
      </template>
      <template #body>
        <SignatureForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('documents.signature.deleteTitle')"
      :description="$t('documents.signature.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
