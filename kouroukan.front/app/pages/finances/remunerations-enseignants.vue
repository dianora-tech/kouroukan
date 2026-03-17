<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { RemunerationEnseignant, CreateRemunerationPayload, UpdateRemunerationPayload } from '~/modules/finances/types/remuneration.types'
import { useRemuneration } from '~/modules/finances/composables/useRemuneration'
import type { RemunerationFilters } from '~/modules/finances/types/remuneration.types'
import RemunerationForm from '~/modules/finances/components/RemunerationForm.vue'
import RemunerationCard from '~/modules/finances/components/RemunerationCard.vue'
import RemunerationFiltersComponent from '~/modules/finances/components/RemunerationFilters.vue'
import RemunerationStats from '~/modules/finances/components/RemunerationStats.vue'

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
} = useRemuneration()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<RemunerationEnseignant | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<RemunerationEnseignant | null>(null)

function formatMontant(montant: number): string {
  return new Intl.NumberFormat('fr-GN', { style: 'decimal' }).format(montant) + ' GNF'
}

function getMoisLabel(mois: number): string {
  return t(`finances.remuneration.mois.${mois}`)
}

const columns: Column[] = [
  { key: 'enseignantNom', label: t('finances.remuneration.enseignantNom'), sortable: true },
  { key: 'mois', label: t('finances.remuneration.mois_label'), sortable: true },
  { key: 'annee', label: t('finances.remuneration.annee'), sortable: true },
  { key: 'modeRemuneration', label: t('finances.remuneration.modeRemuneration'), sortable: true },
  { key: 'montantTotal', label: t('finances.remuneration.montantTotal'), sortable: true },
  { key: 'statutPaiement', label: t('finances.remuneration.statutPaiement'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await fetchAll()
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: RemunerationEnseignant): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: RemunerationEnseignant): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateRemunerationPayload | UpdateRemunerationPayload): Promise<void> {
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

function handleFilter(filters: RemunerationFilters): void {
  setFilters(filters)
}

function handleSort(key: string, direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    EnAttente: 'warning',
    Valide: 'info',
    Paye: 'success',
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
            { label: $t('finances.remuneration.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('finances.remuneration.title') }}
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
          {{ $t('finances.remuneration.add') }}
        </UButton>
      </div>
    </div>

    <RemunerationStats :items="items" :total-count="pagination.totalCount" />

    <RemunerationFiltersComponent @filter="handleFilter" @reset="resetFilters" />

    <template v-if="viewMode === 'table'">
      <DataTable
        v-if="!isEmpty || loading"
        :columns="columns"
        :data="paginatedData"
        :loading="loading"
        @page-change="changePage"
        @sort="handleSort"
      >
        <template #cell-mois="{ row }">
          {{ getMoisLabel((row as RemunerationEnseignant).mois) }}
        </template>
        <template #cell-modeRemuneration="{ row }">
          <UBadge color="neutral" variant="subtle" size="sm">
            {{ $t(`finances.remuneration.mode.${(row as RemunerationEnseignant).modeRemuneration}`) }}
          </UBadge>
        </template>
        <template #cell-montantTotal="{ row }">
          {{ formatMontant((row as RemunerationEnseignant).montantTotal) }}
        </template>
        <template #cell-statutPaiement="{ row }">
          <UBadge :color="getStatutColor((row as RemunerationEnseignant).statutPaiement)" variant="subtle" size="sm">
            {{ $t(`finances.remuneration.statut.${(row as RemunerationEnseignant).statutPaiement}`) }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'finances:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as RemunerationEnseignant)"
            />
            <UButton
              v-permission="'finances:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as RemunerationEnseignant)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-users"
        :title="$t('finances.remuneration.emptyTitle')"
        :description="$t('finances.remuneration.emptyDescription')"
        :action-label="$t('finances.remuneration.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <RemunerationCard
          v-for="remuneration in items"
          :key="remuneration.id"
          :remuneration="remuneration"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-users"
        :title="$t('finances.remuneration.emptyTitle')"
        :description="$t('finances.remuneration.emptyDescription')"
        :action-label="$t('finances.remuneration.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('finances.remuneration.edit') : $t('finances.remuneration.add') }}
        </h3>
      </template>
      <template #body>
        <RemunerationForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('finances.remuneration.deleteTitle')"
      :description="$t('finances.remuneration.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
