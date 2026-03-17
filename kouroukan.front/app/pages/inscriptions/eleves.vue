<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Eleve, CreateElevePayload, UpdateElevePayload } from '~/modules/inscriptions/types/eleve.types'
import { useEleve } from '~/modules/inscriptions/composables/useEleve'
import type { EleveFilters } from '~/modules/inscriptions/types/eleve.types'
import EleveForm from '~/modules/inscriptions/components/EleveForm.vue'
import EleveCard from '~/modules/inscriptions/components/EleveCard.vue'
import EleveFiltersComponent from '~/modules/inscriptions/components/EleveFilters.vue'
import EleveStats from '~/modules/inscriptions/components/EleveStats.vue'

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
} = useEleve()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Eleve | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Eleve | null>(null)

const columns: Column[] = [
  { key: 'numeroMatricule', label: t('inscriptions.eleve.numeroMatricule'), sortable: true },
  { key: 'lastName', label: t('inscriptions.eleve.lastName'), sortable: true },
  { key: 'firstName', label: t('inscriptions.eleve.firstName'), sortable: true },
  { key: 'dateNaissance', label: t('inscriptions.eleve.dateNaissance'), sortable: true },
  { key: 'genre', label: t('inscriptions.eleve.genre'), sortable: false },
  { key: 'statutInscription', label: t('inscriptions.eleve.statutInscription'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(() => {
  fetchAll()
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Eleve): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Eleve): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateElevePayload | UpdateElevePayload): Promise<void> {
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

function handleFilter(filters: EleveFilters): void {
  setFilters(filters)
}

function handleSort(key: string, direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    Prospect: 'info',
    PreInscrit: 'warning',
    Inscrit: 'success',
    Radie: 'error',
  }
  return colors[statut] ?? 'neutral'
}
</script>

<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('nav.inscriptions'), to: '/inscriptions' },
            { label: $t('inscriptions.eleve.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('inscriptions.eleve.title') }}
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
          v-permission="'inscriptions:create'"
          color="primary"
          icon="i-heroicons-plus"
          @click="openCreate"
        >
          {{ $t('inscriptions.eleve.add') }}
        </UButton>
      </div>
    </div>

    <!-- Stats -->
    <EleveStats :items="items" :total-count="pagination.totalCount" />

    <!-- Filters -->
    <EleveFiltersComponent @filter="handleFilter" @reset="resetFilters" />

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
        <template #cell-statutInscription="{ row }">
          <UBadge :color="getStatutColor((row as Eleve).statutInscription)" variant="subtle" size="sm">
            {{ $t(`inscriptions.eleve.statut.${(row as Eleve).statutInscription}`) }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'inscriptions:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Eleve)"
            />
            <UButton
              v-permission="'inscriptions:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Eleve)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-users"
        :title="$t('inscriptions.eleve.emptyTitle')"
        :description="$t('inscriptions.eleve.emptyDescription')"
        :action-label="$t('inscriptions.eleve.add')"
        @action="openCreate"
      />
    </template>

    <!-- Grid view -->
    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <EleveCard
          v-for="eleve in items"
          :key="eleve.id"
          :eleve="eleve"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-users"
        :title="$t('inscriptions.eleve.emptyTitle')"
        :description="$t('inscriptions.eleve.emptyDescription')"
        :action-label="$t('inscriptions.eleve.add')"
        @action="openCreate"
      />
    </template>

    <!-- Slideover Form -->
    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('inscriptions.eleve.edit') : $t('inscriptions.eleve.add') }}
        </h3>
      </template>
      <template #body>
        <EleveForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <!-- Delete Confirmation -->
    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('inscriptions.eleve.deleteTitle')"
      :description="$t('inscriptions.eleve.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
