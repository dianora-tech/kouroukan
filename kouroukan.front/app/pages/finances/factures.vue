<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Facture, CreateFacturePayload, UpdateFacturePayload } from '~/modules/finances/types/facture.types'
import { useFacture } from '~/modules/finances/composables/useFacture'
import type { FactureFilters } from '~/modules/finances/types/facture.types'
import FactureForm from '~/modules/finances/components/FactureForm.vue'
import FactureCard from '~/modules/finances/components/FactureCard.vue'
import FactureFiltersComponent from '~/modules/finances/components/FactureFilters.vue'
import FactureStats from '~/modules/finances/components/FactureStats.vue'

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
} = useFacture()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Facture | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Facture | null>(null)

function formatMontant(montant: number): string {
  return new Intl.NumberFormat('fr-GN', { style: 'decimal' }).format(montant) + ' GNF'
}

const columns: Column[] = [
  { key: 'numeroFacture', label: t('finances.facture.numeroFacture'), sortable: true },
  { key: 'typeName', label: t('finances.facture.type'), sortable: true },
  { key: 'eleveNom', label: t('finances.facture.eleveNom'), sortable: true },
  { key: 'montantTotal', label: t('finances.facture.montantTotal'), sortable: true },
  { key: 'solde', label: t('finances.facture.solde'), sortable: true },
  { key: 'dateEcheance', label: t('finances.facture.dateEcheance'), sortable: true },
  { key: 'statutFacture', label: t('finances.facture.statutFacture'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await Promise.all([fetchAll(), fetchTypes()])
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Facture): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Facture): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateFacturePayload | UpdateFacturePayload): Promise<void> {
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

function handleFilter(filters: FactureFilters): void {
  setFilters(filters)
}

function handleSort(key: string, direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    Emise: 'info',
    PartPaye: 'warning',
    Payee: 'success',
    Echue: 'error',
    Annulee: 'neutral',
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
            { label: $t('finances.facture.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('finances.facture.title') }}
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
          {{ $t('finances.facture.add') }}
        </UButton>
      </div>
    </div>

    <FactureStats :items="items" :total-count="pagination.totalCount" />

    <FactureFiltersComponent @filter="handleFilter" @reset="resetFilters" />

    <template v-if="viewMode === 'table'">
      <DataTable
        v-if="!isEmpty || loading"
        :columns="columns"
        :data="paginatedData"
        :loading="loading"
        @page-change="changePage"
        @sort="handleSort"
      >
        <template #cell-montantTotal="{ row }">
          {{ formatMontant((row as Facture).montantTotal) }}
        </template>
        <template #cell-solde="{ row }">
          <span :class="(row as Facture).solde > 0 ? 'text-red-600 dark:text-red-400' : 'text-green-600 dark:text-green-400'">
            {{ formatMontant((row as Facture).solde) }}
          </span>
        </template>
        <template #cell-statutFacture="{ row }">
          <UBadge :color="getStatutColor((row as Facture).statutFacture)" variant="subtle" size="sm">
            {{ $t(`finances.facture.statut.${(row as Facture).statutFacture}`) }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'finances:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Facture)"
            />
            <UButton
              v-permission="'finances:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Facture)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-document-text"
        :title="$t('finances.facture.emptyTitle')"
        :description="$t('finances.facture.emptyDescription')"
        :action-label="$t('finances.facture.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <FactureCard
          v-for="facture in items"
          :key="facture.id"
          :facture="facture"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-document-text"
        :title="$t('finances.facture.emptyTitle')"
        :description="$t('finances.facture.emptyDescription')"
        :action-label="$t('finances.facture.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('finances.facture.edit') : $t('finances.facture.add') }}
        </h3>
      </template>
      <template #body>
        <FactureForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('finances.facture.deleteTitle')"
      :description="$t('finances.facture.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
