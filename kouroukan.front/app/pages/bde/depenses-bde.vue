<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { DepenseBDE, CreateDepenseBDEPayload, UpdateDepenseBDEPayload, DepenseBDEFilters } from '~/modules/bde/types/depense-bde.types'
import { useDepenseBDE } from '~/modules/bde/composables/useDepenseBDE'
import { useAssociation } from '~/modules/bde/composables/useAssociation'
import DepenseBDEForm from '~/modules/bde/components/DepenseBDEForm.vue'
import DepenseBDECard from '~/modules/bde/components/DepenseBDECard.vue'
import DepenseBDEFiltersComponent from '~/modules/bde/components/DepenseBDEFilters.vue'
import DepenseBDEStats from '~/modules/bde/components/DepenseBDEStats.vue'

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
} = useDepenseBDE()

const { fetchAll: fetchAssociations } = useAssociation()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<DepenseBDE | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<DepenseBDE | null>(null)

function formatMontant(montant: number): string {
  return new Intl.NumberFormat('fr-GN', { style: 'decimal' }).format(montant) + ' GNF'
}

const columns: Column[] = [
  { key: 'name', label: t('bde.depenseBde.name'), sortable: true },
  { key: 'typeName', label: t('bde.depenseBde.type'), sortable: true },
  { key: 'associationNom', label: t('bde.depenseBde.associationId'), sortable: true },
  { key: 'montant', label: t('bde.depenseBde.montant'), sortable: true },
  { key: 'categorie', label: t('bde.depenseBde.categorie_label'), sortable: true },
  { key: 'statutValidation', label: t('bde.depenseBde.statutValidation'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await Promise.all([fetchAll(), fetchTypes(), fetchAssociations()])
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: DepenseBDE): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: DepenseBDE): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateDepenseBDEPayload | UpdateDepenseBDEPayload): Promise<void> {
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

function handleFilter(filters: DepenseBDEFilters): void {
  setFilters(filters)
}

function handleSort(_key: string, _direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    Demandee: 'info',
    ValideTresorier: 'warning',
    ValideSuper: 'success',
    Refusee: 'error',
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
            { label: $t('bde.depenseBde.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('bde.depenseBde.title') }}
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
          {{ $t('bde.depenseBde.add') }}
        </UButton>
      </div>
    </div>

    <DepenseBDEStats
      :items="items"
      :total-count="pagination.totalCount"
    />

    <DepenseBDEFiltersComponent
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
        <template #cell-montant="{ row }">
          {{ formatMontant((row as DepenseBDE).montant) }}
        </template>
        <template #cell-categorie="{ row }">
          <UBadge
            color="neutral"
            variant="subtle"
            size="sm"
          >
            {{ $t(`bde.depenseBde.categorie.${(row as DepenseBDE).categorie}`) }}
          </UBadge>
        </template>
        <template #cell-statutValidation="{ row }">
          <UBadge
            :color="getStatutColor((row as DepenseBDE).statutValidation)"
            variant="subtle"
            size="sm"
          >
            {{ $t(`bde.depenseBde.statut.${(row as DepenseBDE).statutValidation}`) }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'bde:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as DepenseBDE)"
            />
            <UButton
              v-permission="'bde:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as DepenseBDE)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-receipt-percent"
        :title="$t('bde.depenseBde.emptyTitle')"
        :description="$t('bde.depenseBde.emptyDescription')"
        :action-label="$t('bde.depenseBde.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div
        v-if="!isEmpty"
        class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3"
      >
        <DepenseBDECard
          v-for="depense in items"
          :key="depense.id"
          :depense="depense"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-receipt-percent"
        :title="$t('bde.depenseBde.emptyTitle')"
        :description="$t('bde.depenseBde.emptyDescription')"
        :action-label="$t('bde.depenseBde.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('bde.depenseBde.edit') : $t('bde.depenseBde.add') }}
        </h3>
      </template>
      <template #body>
        <DepenseBDEForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('bde.depenseBde.deleteTitle')"
      :description="$t('bde.depenseBde.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
