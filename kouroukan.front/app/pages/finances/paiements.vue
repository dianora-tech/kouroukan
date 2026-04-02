<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Paiement, CreatePaiementPayload, UpdatePaiementPayload, PaiementFilters } from '~/modules/finances/types/paiement.types'
import { usePaiement } from '~/modules/finances/composables/usePaiement'
import PaiementForm from '~/modules/finances/components/PaiementForm.vue'
import PaiementCard from '~/modules/finances/components/PaiementCard.vue'
import PaiementFiltersComponent from '~/modules/finances/components/PaiementFilters.vue'
import PaiementStats from '~/modules/finances/components/PaiementStats.vue'

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
} = usePaiement()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Paiement | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Paiement | null>(null)

function formatMontant(montant: number): string {
  return new Intl.NumberFormat('fr-GN', { style: 'decimal' }).format(montant) + ' GNF'
}

const columns: Column[] = [
  { key: 'numeroRecu', label: t('finances.paiement.numeroRecu'), sortable: true },
  { key: 'typeName', label: t('finances.paiement.type'), sortable: true },
  { key: 'factureNumero', label: t('finances.paiement.facture'), sortable: true },
  { key: 'montantPaye', label: t('finances.paiement.montantPaye'), sortable: true },
  { key: 'moyenPaiement', label: t('finances.paiement.moyenPaiement'), sortable: true },
  { key: 'datePaiement', label: t('finances.paiement.datePaiement'), sortable: true, render: (row: any) => formatDate(row.datePaiement) },
  { key: 'statutPaiement', label: t('finances.paiement.statutPaiement'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await Promise.all([fetchAll(), fetchTypes()])
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Paiement): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Paiement): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreatePaiementPayload | UpdatePaiementPayload): Promise<void> {
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

function handleFilter(filters: PaiementFilters): void {
  setFilters(filters)
}

function handleSort(key: string, direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    EnAttente: 'warning',
    Confirme: 'success',
    Echec: 'error',
    Rembourse: 'info',
  }
  return colors[statut] ?? 'neutral'
}

function getMoyenColor(moyen: string): string {
  const colors: Record<string, string> = {
    OrangeMoney: 'warning',
    SoutraMoney: 'info',
    MTNMoMo: 'success',
    Especes: 'neutral',
  }
  return colors[moyen] ?? 'neutral'
}
</script>

<template>
  <div class="space-y-6">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('nav.finances'), to: '/finances' },
            { label: $t('finances.paiement.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('finances.paiement.title') }}
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
          {{ $t('finances.paiement.add') }}
        </UButton>
      </div>
    </div>

    <PaiementStats
      :items="items"
      :total-count="pagination.totalCount"
    />

    <PaiementFiltersComponent
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
        <template #cell-montantPaye="{ row }">
          {{ formatMontant((row as Paiement).montantPaye) }}
        </template>
        <template #cell-moyenPaiement="{ row }">
          <UBadge
            :color="getMoyenColor((row as Paiement).moyenPaiement)"
            variant="subtle"
            size="sm"
          >
            {{ $t(`finances.paiement.moyen.${(row as Paiement).moyenPaiement}`) }}
          </UBadge>
        </template>
        <template #cell-datePaiement="{ row }">
          {{ formatDate((row as Paiement).datePaiement) }}
        </template>
        <template #cell-statutPaiement="{ row }">
          <UBadge
            :color="getStatutColor((row as Paiement).statutPaiement)"
            variant="subtle"
            size="sm"
          >
            {{ $t(`finances.paiement.statut.${(row as Paiement).statutPaiement}`) }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'finances:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Paiement)"
            />
            <UButton
              v-permission="'finances:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Paiement)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-credit-card"
        :title="$t('finances.paiement.emptyTitle')"
        :description="$t('finances.paiement.emptyDescription')"
        :action-label="$t('finances.paiement.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div
        v-if="!isEmpty"
        class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3"
      >
        <PaiementCard
          v-for="paiement in items"
          :key="paiement.id"
          :paiement="paiement"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-credit-card"
        :title="$t('finances.paiement.emptyTitle')"
        :description="$t('finances.paiement.emptyDescription')"
        :action-label="$t('finances.paiement.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('finances.paiement.edit') : $t('finances.paiement.add') }}
        </h3>
      </template>
      <template #body>
        <PaiementForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('finances.paiement.deleteTitle')"
      :description="$t('finances.paiement.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
