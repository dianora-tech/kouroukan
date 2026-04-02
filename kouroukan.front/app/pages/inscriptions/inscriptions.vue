<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Inscription, CreateInscriptionPayload, UpdateInscriptionPayload, InscriptionFilters } from '~/modules/inscriptions/types/inscription.types'
import { useInscription } from '~/modules/inscriptions/composables/useInscription'
import InscriptionForm from '~/modules/inscriptions/components/InscriptionForm.vue'
import InscriptionCard from '~/modules/inscriptions/components/InscriptionCard.vue'
import InscriptionFiltersComponent from '~/modules/inscriptions/components/InscriptionFilters.vue'
import InscriptionStats from '~/modules/inscriptions/components/InscriptionStats.vue'

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
} = useInscription()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Inscription | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Inscription | null>(null)

function formatMontant(montant: number): string {
  return new Intl.NumberFormat('fr-GN', { style: 'decimal' }).format(montant) + ' GNF'
}

const columns: Column[] = [
  { key: 'eleveNom', label: t('inscriptions.inscription.eleveNom'), sortable: true },
  { key: 'typeName', label: t('inscriptions.inscription.type'), sortable: true },
  { key: 'classeName', label: t('inscriptions.inscription.classe'), sortable: true },
  { key: 'dateInscription', label: t('inscriptions.inscription.dateInscription'), sortable: true, render: (row: any) => formatDate(row.dateInscription) },
  { key: 'montantInscription', label: t('inscriptions.inscription.montantInscription'), sortable: true },
  { key: 'statutInscription', label: t('inscriptions.inscription.statutInscription'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await Promise.all([fetchAll(), fetchTypes()])
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Inscription): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Inscription): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateInscriptionPayload | UpdateInscriptionPayload): Promise<void> {
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

function handleFilter(filters: InscriptionFilters): void {
  setFilters(filters)
}

function handleSort(key: string, direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    EnAttente: 'warning',
    Validee: 'success',
    Annulee: 'error',
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
            { label: $t('nav.inscriptions'), to: '/inscriptions' },
            { label: $t('inscriptions.inscription.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('inscriptions.inscription.title') }}
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
          {{ $t('inscriptions.inscription.add') }}
        </UButton>
      </div>
    </div>

    <InscriptionStats
      :items="items"
      :total-count="pagination.totalCount"
    />

    <InscriptionFiltersComponent
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
        <template #cell-montantInscription="{ row }">
          {{ formatMontant((row as Inscription).montantInscription) }}
        </template>
        <template #cell-statutInscription="{ row }">
          <UBadge
            :color="getStatutColor((row as Inscription).statutInscription)"
            variant="subtle"
            size="sm"
          >
            {{ $t(`inscriptions.inscription.statut.${(row as Inscription).statutInscription}`) }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'inscriptions:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Inscription)"
            />
            <UButton
              v-permission="'inscriptions:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Inscription)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-clipboard-document-list"
        :title="$t('inscriptions.inscription.emptyTitle')"
        :description="$t('inscriptions.inscription.emptyDescription')"
        :action-label="$t('inscriptions.inscription.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div
        v-if="!isEmpty"
        class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3"
      >
        <InscriptionCard
          v-for="inscription in items"
          :key="inscription.id"
          :inscription="inscription"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-clipboard-document-list"
        :title="$t('inscriptions.inscription.emptyTitle')"
        :description="$t('inscriptions.inscription.emptyDescription')"
        :action-label="$t('inscriptions.inscription.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('inscriptions.inscription.edit') : $t('inscriptions.inscription.add') }}
        </h3>
      </template>
      <template #body>
        <InscriptionForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('inscriptions.inscription.deleteTitle')"
      :description="$t('inscriptions.inscription.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
