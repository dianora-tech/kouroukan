<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Annonce, CreateAnnoncePayload, UpdateAnnoncePayload } from '~/modules/communication/types/annonce.types'
import { useAnnonce } from '~/modules/communication/composables/useAnnonce'
import type { AnnonceFilters } from '~/modules/communication/types/annonce.types'
import AnnonceForm from '~/modules/communication/components/AnnonceForm.vue'
import AnnonceCard from '~/modules/communication/components/AnnonceCard.vue'
import AnnonceFiltersComponent from '~/modules/communication/components/AnnonceFilters.vue'
import AnnonceStats from '~/modules/communication/components/AnnonceStats.vue'

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
} = useAnnonce()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Annonce | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Annonce | null>(null)

const columns: Column[] = [
  { key: 'contenu', label: t('communication.annonce.contenu'), sortable: true },
  { key: 'typeName', label: t('communication.annonce.type'), sortable: false },
  { key: 'cibleAudience', label: t('communication.annonce.cibleAudience'), sortable: true },
  { key: 'priorite', label: t('communication.annonce.prioriteLabel'), sortable: true },
  { key: 'estActive', label: t('communication.annonce.statut'), sortable: true },
  { key: 'dateDebut', label: t('communication.annonce.dateDebut'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(() => {
  fetchAll()
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Annonce): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Annonce): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateAnnoncePayload | UpdateAnnoncePayload): Promise<void> {
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

function handleFilter(filters: AnnonceFilters): void {
  setFilters(filters)
}

function handleSort(key: string, direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getStatutColor(estActive: boolean): string {
  return estActive ? 'success' : 'neutral'
}

function getPrioriteColor(priorite: number): string {
  if (priorite <= 1) return 'error'
  if (priorite <= 2) return 'warning'
  return 'info'
}
</script>

<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('nav.communication'), to: '/communication' },
            { label: $t('communication.annonce.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('communication.annonce.title') }}
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
          v-permission="'communication:create'"
          color="primary"
          icon="i-heroicons-plus"
          @click="openCreate"
        >
          {{ $t('communication.annonce.add') }}
        </UButton>
      </div>
    </div>

    <!-- Stats -->
    <AnnonceStats :items="items" :total-count="pagination.totalCount" />

    <!-- Filters -->
    <AnnonceFiltersComponent @filter="handleFilter" @reset="resetFilters" />

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
        <template #cell-contenu="{ row }">
          <span class="line-clamp-1">{{ (row as Annonce).contenu }}</span>
        </template>
        <template #cell-cibleAudience="{ row }">
          <UBadge color="info" variant="subtle" size="sm">
            {{ $t(`communication.annonce.cible.${(row as Annonce).cibleAudience}`) }}
          </UBadge>
        </template>
        <template #cell-priorite="{ row }">
          <UBadge :color="getPrioriteColor((row as Annonce).priorite)" variant="subtle" size="sm">
            P{{ (row as Annonce).priorite }}
          </UBadge>
        </template>
        <template #cell-estActive="{ row }">
          <UBadge :color="getStatutColor((row as Annonce).estActive)" variant="subtle" size="sm">
            {{ (row as Annonce).estActive ? $t('communication.annonce.active') : $t('communication.annonce.inactive') }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'communication:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Annonce)"
            />
            <UButton
              v-permission="'communication:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Annonce)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-megaphone"
        :title="$t('communication.annonce.emptyTitle')"
        :description="$t('communication.annonce.emptyDescription')"
        :action-label="$t('communication.annonce.add')"
        @action="openCreate"
      />
    </template>

    <!-- Grid view -->
    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <AnnonceCard
          v-for="annonce in items"
          :key="annonce.id"
          :annonce="annonce"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-megaphone"
        :title="$t('communication.annonce.emptyTitle')"
        :description="$t('communication.annonce.emptyDescription')"
        :action-label="$t('communication.annonce.add')"
        @action="openCreate"
      />
    </template>

    <!-- Slideover Form -->
    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('communication.annonce.edit') : $t('communication.annonce.add') }}
        </h3>
      </template>
      <template #body>
        <AnnonceForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <!-- Delete Confirmation -->
    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('communication.annonce.deleteTitle')"
      :description="$t('communication.annonce.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
