<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Depense, CreateDepensePayload, UpdateDepensePayload } from '~/modules/finances/types/depense.types'
import { useDepense } from '~/modules/finances/composables/useDepense'
import type { DepenseFilters } from '~/modules/finances/types/depense.types'
import DepenseForm from '~/modules/finances/components/DepenseForm.vue'
import DepenseCard from '~/modules/finances/components/DepenseCard.vue'
import DepenseFiltersComponent from '~/modules/finances/components/DepenseFilters.vue'
import DepenseStats from '~/modules/finances/components/DepenseStats.vue'

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
} = useDepense()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Depense | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Depense | null>(null)

function formatMontant(montant: number): string {
  return new Intl.NumberFormat('fr-GN', { style: 'decimal' }).format(montant) + ' GNF'
}

const columns: Column[] = [
  { key: 'numeroJustificatif', label: t('finances.depense.numeroJustificatif'), sortable: true },
  { key: 'typeName', label: t('finances.depense.type'), sortable: true },
  { key: 'categorie', label: t('finances.depense.categorie_label'), sortable: true },
  { key: 'montant', label: t('finances.depense.montant'), sortable: true },
  { key: 'beneficiaireNom', label: t('finances.depense.beneficiaireNom'), sortable: true },
  { key: 'dateDemande', label: t('finances.depense.dateDemande'), sortable: true },
  { key: 'statutDepense', label: t('finances.depense.statutDepense'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await Promise.all([fetchAll(), fetchTypes()])
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Depense): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Depense): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateDepensePayload | UpdateDepensePayload): Promise<void> {
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

function handleFilter(filters: DepenseFilters): void {
  setFilters(filters)
}

function handleSort(key: string, direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    Demande: 'info',
    ValideN1: 'warning',
    ValideFinance: 'warning',
    ValideDirection: 'success',
    Executee: 'success',
    Archivee: 'neutral',
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
            { label: $t('nav.finances'), to: '/finances' },
            { label: $t('finances.depense.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('finances.depense.title') }}
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
          v-permission="'finances:create'"
          color="primary"
          icon="i-heroicons-plus"
          @click="openCreate"
        >
          {{ $t('finances.depense.add') }}
        </UButton>
      </div>
    </div>

    <DepenseStats :items="items" :total-count="pagination.totalCount" />

    <DepenseFiltersComponent @filter="handleFilter" @reset="resetFilters" />

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
          {{ formatMontant((row as Depense).montant) }}
        </template>
        <template #cell-categorie="{ row }">
          <UBadge color="neutral" variant="subtle" size="sm">
            {{ $t(`finances.depense.categorie.${(row as Depense).categorie}`) }}
          </UBadge>
        </template>
        <template #cell-statutDepense="{ row }">
          <UBadge :color="getStatutColor((row as Depense).statutDepense)" variant="subtle" size="sm">
            {{ $t(`finances.depense.statut.${(row as Depense).statutDepense}`) }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'finances:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Depense)"
            />
            <UButton
              v-permission="'finances:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Depense)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-receipt-percent"
        :title="$t('finances.depense.emptyTitle')"
        :description="$t('finances.depense.emptyDescription')"
        :action-label="$t('finances.depense.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <DepenseCard
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
        :title="$t('finances.depense.emptyTitle')"
        :description="$t('finances.depense.emptyDescription')"
        :action-label="$t('finances.depense.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('finances.depense.edit') : $t('finances.depense.add') }}
        </h3>
      </template>
      <template #body>
        <DepenseForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('finances.depense.deleteTitle')"
      :description="$t('finances.depense.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
