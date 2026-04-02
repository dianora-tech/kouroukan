<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { MembreBDE, CreateMembreBDEPayload, UpdateMembreBDEPayload, MembreBDEFilters } from '~/modules/bde/types/membre-bde.types'
import { useMembreBDE } from '~/modules/bde/composables/useMembreBDE'
import { useAssociation } from '~/modules/bde/composables/useAssociation'
import MembreBDEForm from '~/modules/bde/components/MembreBDEForm.vue'
import MembreBDECard from '~/modules/bde/components/MembreBDECard.vue'
import MembreBDEFiltersComponent from '~/modules/bde/components/MembreBDEFilters.vue'
import MembreBDEStats from '~/modules/bde/components/MembreBDEStats.vue'

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
} = useMembreBDE()

const { fetchAll: fetchAssociations } = useAssociation()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<MembreBDE | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<MembreBDE | null>(null)

function formatMontant(montant: number): string {
  return new Intl.NumberFormat('fr-GN', { style: 'decimal' }).format(montant) + ' GNF'
}

const columns: Column[] = [
  { key: 'name', label: t('bde.membreBde.name'), sortable: true },
  { key: 'associationNom', label: t('bde.membreBde.associationId'), sortable: true },
  { key: 'eleveNom', label: t('bde.membreBde.eleveId'), sortable: true },
  { key: 'roleBDE', label: t('bde.membreBde.roleBDE'), sortable: true },
  { key: 'dateAdhesion', label: t('bde.membreBde.dateAdhesion'), sortable: true, render: (row: any) => formatDate(row.dateAdhesion) },
  { key: 'montantCotisation', label: t('bde.membreBde.montantCotisation'), sortable: true },
  { key: 'estActif', label: t('bde.membreBde.estActif'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await Promise.all([fetchAll(), fetchAssociations()])
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: MembreBDE): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: MembreBDE): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateMembreBDEPayload | UpdateMembreBDEPayload): Promise<void> {
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

function handleFilter(filters: MembreBDEFilters): void {
  setFilters(filters)
}

function handleSort(_key: string, _direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}
</script>

<template>
  <div class="space-y-6">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('nav.bde'), to: '/bde' },
            { label: $t('bde.membreBde.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('bde.membreBde.title') }}
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
          {{ $t('bde.membreBde.add') }}
        </UButton>
      </div>
    </div>

    <MembreBDEStats
      :items="items"
      :total-count="pagination.totalCount"
    />

    <MembreBDEFiltersComponent
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
        <template #cell-roleBDE="{ row }">
          <UBadge
            color="neutral"
            variant="subtle"
            size="sm"
          >
            {{ $t(`bde.membreBde.role.${(row as MembreBDE).roleBDE}`) }}
          </UBadge>
        </template>
        <template #cell-montantCotisation="{ row }">
          {{ (row as MembreBDE).montantCotisation ? formatMontant((row as MembreBDE).montantCotisation!) : '-' }}
        </template>
        <template #cell-estActif="{ row }">
          <UBadge
            :color="(row as MembreBDE).estActif ? 'success' : 'neutral'"
            variant="subtle"
            size="sm"
          >
            {{ (row as MembreBDE).estActif ? $t('bde.membreBde.actif') : $t('bde.membreBde.inactif') }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'bde:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as MembreBDE)"
            />
            <UButton
              v-permission="'bde:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as MembreBDE)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-users"
        :title="$t('bde.membreBde.emptyTitle')"
        :description="$t('bde.membreBde.emptyDescription')"
        :action-label="$t('bde.membreBde.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div
        v-if="!isEmpty"
        class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3"
      >
        <MembreBDECard
          v-for="membre in items"
          :key="membre.id"
          :membre="membre"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-users"
        :title="$t('bde.membreBde.emptyTitle')"
        :description="$t('bde.membreBde.emptyDescription')"
        :action-label="$t('bde.membreBde.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('bde.membreBde.edit') : $t('bde.membreBde.add') }}
        </h3>
      </template>
      <template #body>
        <MembreBDEForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('bde.membreBde.deleteTitle')"
      :description="$t('bde.membreBde.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
