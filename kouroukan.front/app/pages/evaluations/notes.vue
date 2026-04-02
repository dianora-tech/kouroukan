<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Note, CreateNotePayload, UpdateNotePayload } from '~/modules/evaluations/types/note.types'
import { useNote } from '~/modules/evaluations/composables/useNote'
import type { NoteFilters } from '~/modules/evaluations/types/note.types'
import NoteForm from '~/modules/evaluations/components/NoteForm.vue'
import NoteCard from '~/modules/evaluations/components/NoteCard.vue'
import NoteFiltersComponent from '~/modules/evaluations/components/NoteFilters.vue'
import NoteStats from '~/modules/evaluations/components/NoteStats.vue'

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
  create,
  update,
  remove,
  setFilters,
  resetFilters,
  changePage,
} = useNote()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Note | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Note | null>(null)

function getNoteColor(valeur: number, noteMax: number | undefined): string {
  const max = noteMax ?? 20
  const ratio = valeur / max
  if (ratio >= 0.7) return 'success'
  if (ratio >= 0.5) return 'warning'
  return 'error'
}

const columns: Column[] = [
  { key: 'eleveNom', label: t('evaluations.note.eleve'), sortable: true },
  { key: 'matiereName', label: t('evaluations.note.matiere'), sortable: true },
  { key: 'evaluationTypeName', label: t('evaluations.note.evaluationType'), sortable: true },
  { key: 'valeur', label: t('evaluations.note.valeur'), sortable: true },
  { key: 'dateSaisie', label: t('evaluations.note.dateSaisie'), sortable: true, render: (row: any) => formatDate(row.dateSaisie) },
  { key: 'commentaire', label: t('evaluations.note.commentaire'), sortable: false },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await fetchAll()
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Note): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Note): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateNotePayload | UpdateNotePayload): Promise<void> {
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

function handleFilter(filters: NoteFilters): void {
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
            { label: $t('evaluations.note.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('evaluations.note.title') }}
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
          {{ $t('evaluations.note.add') }}
        </UButton>
      </div>
    </div>

    <NoteStats :items="items" :total-count="pagination.totalCount" />

    <NoteFiltersComponent @filter="handleFilter" @reset="resetFilters" />

    <template v-if="viewMode === 'table'">
      <DataTable
        v-if="!isEmpty || loading"
        :columns="columns"
        :data="paginatedData"
        :loading="loading"
        @page-change="changePage"
        @sort="handleSort"
      >
        <template #cell-valeur="{ row }">
          <UBadge :color="getNoteColor((row as Note).valeur, (row as Note).noteMaximale)" variant="subtle" size="sm">
            {{ (row as Note).valeur }} / {{ (row as Note).noteMaximale ?? 20 }}
          </UBadge>
        </template>
        <template #cell-dateSaisie="{ row }">
          {{ formatDate((row as Note).dateSaisie) }}
        </template>
        <template #cell-commentaire="{ row }">
          <span class="max-w-[200px] truncate">{{ (row as Note).commentaire ?? '-' }}</span>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'evaluations:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Note)"
            />
            <UButton
              v-permission="'evaluations:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Note)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-document-text"
        :title="$t('evaluations.note.emptyTitle')"
        :description="$t('evaluations.note.emptyDescription')"
        :action-label="$t('evaluations.note.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <NoteCard
          v-for="note in items"
          :key="note.id"
          :note="note"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-document-text"
        :title="$t('evaluations.note.emptyTitle')"
        :description="$t('evaluations.note.emptyDescription')"
        :action-label="$t('evaluations.note.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('evaluations.note.edit') : $t('evaluations.note.add') }}
        </h3>
      </template>
      <template #body>
        <NoteForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('evaluations.note.deleteTitle')"
      :description="$t('evaluations.note.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
