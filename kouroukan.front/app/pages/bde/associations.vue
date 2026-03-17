<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Association, CreateAssociationPayload, UpdateAssociationPayload } from '~/modules/bde/types/association.types'
import type { AssociationFilters } from '~/modules/bde/types/association.types'
import { useAssociation } from '~/modules/bde/composables/useAssociation'
import AssociationForm from '~/modules/bde/components/AssociationForm.vue'
import AssociationCard from '~/modules/bde/components/AssociationCard.vue'
import AssociationFiltersComponent from '~/modules/bde/components/AssociationFilters.vue'
import AssociationStats from '~/modules/bde/components/AssociationStats.vue'

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
} = useAssociation()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Association | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Association | null>(null)

function formatMontant(montant: number): string {
  return new Intl.NumberFormat('fr-GN', { style: 'decimal' }).format(montant) + ' GNF'
}

const columns: Column[] = [
  { key: 'name', label: t('bde.association.name'), sortable: true },
  { key: 'typeName', label: t('bde.association.type'), sortable: true },
  { key: 'sigle', label: t('bde.association.sigle'), sortable: true },
  { key: 'anneeScolaire', label: t('bde.association.anneeScolaire'), sortable: true },
  { key: 'budgetAnnuel', label: t('bde.association.budgetAnnuel'), sortable: true },
  { key: 'statut', label: t('bde.association.statut_label'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await Promise.all([fetchAll(), fetchTypes()])
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Association): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Association): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateAssociationPayload | UpdateAssociationPayload): Promise<void> {
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

function handleFilter(filters: AssociationFilters): void {
  setFilters(filters)
}

function handleSort(_key: string, _direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    Active: 'success',
    Suspendue: 'warning',
    Dissoute: 'error',
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
            { label: $t('bde.association.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('bde.association.title') }}
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
          {{ $t('bde.association.add') }}
        </UButton>
      </div>
    </div>

    <AssociationStats :items="items" :total-count="pagination.totalCount" />

    <AssociationFiltersComponent @filter="handleFilter" @reset="resetFilters" />

    <template v-if="viewMode === 'table'">
      <DataTable
        v-if="!isEmpty || loading"
        :columns="columns"
        :data="paginatedData"
        :loading="loading"
        @page-change="changePage"
        @sort="handleSort"
      >
        <template #cell-budgetAnnuel="{ row }">
          {{ formatMontant((row as Association).budgetAnnuel) }}
        </template>
        <template #cell-statut="{ row }">
          <UBadge :color="getStatutColor((row as Association).statut)" variant="subtle" size="sm">
            {{ $t(`bde.association.statut.${(row as Association).statut}`) }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'bde:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Association)"
            />
            <UButton
              v-permission="'bde:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Association)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-user-group"
        :title="$t('bde.association.emptyTitle')"
        :description="$t('bde.association.emptyDescription')"
        :action-label="$t('bde.association.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <AssociationCard
          v-for="association in items"
          :key="association.id"
          :association="association"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-user-group"
        :title="$t('bde.association.emptyTitle')"
        :description="$t('bde.association.emptyDescription')"
        :action-label="$t('bde.association.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('bde.association.edit') : $t('bde.association.add') }}
        </h3>
      </template>
      <template #body>
        <AssociationForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('bde.association.deleteTitle')"
      :description="$t('bde.association.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
