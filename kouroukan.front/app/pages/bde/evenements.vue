<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Evenement, CreateEvenementPayload, UpdateEvenementPayload, EvenementFilters } from '~/modules/bde/types/evenement.types'
import { useEvenement } from '~/modules/bde/composables/useEvenement'
import { useAssociation } from '~/modules/bde/composables/useAssociation'
import EvenementForm from '~/modules/bde/components/EvenementForm.vue'
import EvenementCard from '~/modules/bde/components/EvenementCard.vue'
import EvenementFiltersComponent from '~/modules/bde/components/EvenementFilters.vue'
import EvenementStats from '~/modules/bde/components/EvenementStats.vue'

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
  fetchTypes,
  create,
  update,
  remove,
  setFilters,
  resetFilters,
  changePage,
} = useEvenement()

const { fetchAll: fetchAssociations } = useAssociation()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Evenement | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Evenement | null>(null)

function formatMontant(montant: number): string {
  return new Intl.NumberFormat('fr-GN', { style: 'decimal' }).format(montant) + ' GNF'
}

const columns: Column[] = [
  { key: 'name', label: t('bde.evenement.name'), sortable: true },
  { key: 'typeName', label: t('bde.evenement.type'), sortable: true },
  { key: 'associationNom', label: t('bde.evenement.associationId'), sortable: true },
  { key: 'dateEvenement', label: t('bde.evenement.dateEvenement'), sortable: true, render: (row: any) => formatDate(row.dateEvenement) },
  { key: 'lieu', label: t('bde.evenement.lieu'), sortable: true },
  { key: 'nombreInscrits', label: t('bde.evenement.nombreInscrits'), sortable: true },
  { key: 'statutEvenement', label: t('bde.evenement.statutEvenement'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await Promise.all([fetchAll(), fetchTypes(), fetchAssociations()])
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Evenement): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Evenement): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateEvenementPayload | UpdateEvenementPayload): Promise<void> {
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

function handleFilter(filters: EvenementFilters): void {
  setFilters(filters)
}

function handleSort(_key: string, _direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    Planifie: 'info',
    Valide: 'warning',
    EnCours: 'success',
    Termine: 'neutral',
    Annule: 'error',
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
            { label: $t('nav.bde'), to: '/bde' },
            { label: $t('bde.evenement.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('bde.evenement.title') }}
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
          v-permission="'bde:create'"
          color="primary"
          icon="i-heroicons-plus"
          @click="openCreate"
        >
          {{ $t('bde.evenement.add') }}
        </UButton>
      </div>
    </div>

    <EvenementStats
      :items="items"
      :total-count="pagination.totalCount"
    />

    <EvenementFiltersComponent
      @filter="handleFilter"
      @reset="resetFilters"
    />

    <template v-if="viewMode === 'table'">
      <DataTable
        v-if="!isEmpty || loading"
        :columns="columns"
        :data="paginatedData"
        :loading="loading"
        @page-change="changePage"
        @sort="handleSort"
      >
        <template #cell-statutEvenement="{ row }">
          <UBadge
            :color="getStatutColor((row as Evenement).statutEvenement)"
            variant="subtle"
            size="sm"
          >
            {{ $t(`bde.evenement.statut.${(row as Evenement).statutEvenement}`) }}
          </UBadge>
        </template>
        <template #cell-nombreInscrits="{ row }">
          {{ (row as Evenement).nombreInscrits }}<template v-if="(row as Evenement).capacite">
            / {{ (row as Evenement).capacite }}
          </template>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'bde:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Evenement)"
            />
            <UButton
              v-permission="'bde:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Evenement)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-calendar-days"
        :title="$t('bde.evenement.emptyTitle')"
        :description="$t('bde.evenement.emptyDescription')"
        :action-label="$t('bde.evenement.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div
        v-if="!isEmpty"
        class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3"
      >
        <EvenementCard
          v-for="evenement in items"
          :key="evenement.id"
          :evenement="evenement"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-calendar-days"
        :title="$t('bde.evenement.emptyTitle')"
        :description="$t('bde.evenement.emptyDescription')"
        :action-label="$t('bde.evenement.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('bde.evenement.edit') : $t('bde.evenement.add') }}
        </h3>
      </template>
      <template #body>
        <EvenementForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('bde.evenement.deleteTitle')"
      :description="$t('bde.evenement.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
