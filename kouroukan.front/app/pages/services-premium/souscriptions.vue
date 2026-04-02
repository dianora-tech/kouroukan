<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Souscription, CreateSouscriptionPayload, UpdateSouscriptionPayload, SouscriptionFilters } from '~/modules/services-premium/types/souscription.types'
import { useSouscription } from '~/modules/services-premium/composables/useSouscription'
import SouscriptionForm from '~/modules/services-premium/components/SouscriptionForm.vue'
import SouscriptionCard from '~/modules/services-premium/components/SouscriptionCard.vue'
import SouscriptionFiltersComponent from '~/modules/services-premium/components/SouscriptionFilters.vue'
import SouscriptionStats from '~/modules/services-premium/components/SouscriptionStats.vue'

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
} = useSouscription()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Souscription | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Souscription | null>(null)

function formatMontant(montant: number): string {
  return new Intl.NumberFormat('fr-GN', { style: 'decimal' }).format(montant) + ' GNF'
}

const columns: Column[] = [
  { key: 'serviceParentNom', label: t('servicesPremium.souscription.serviceParentId'), sortable: true },
  { key: 'parentNom', label: t('servicesPremium.souscription.parentId'), sortable: true },
  { key: 'dateDebut', label: t('servicesPremium.souscription.dateDebut'), sortable: true, render: (row: any) => formatDate(row.dateDebut) },
  { key: 'dateFin', label: t('servicesPremium.souscription.dateFin'), sortable: true, render: (row: any) => formatDate(row.dateFin) },
  { key: 'montantPaye', label: t('servicesPremium.souscription.montantPaye'), sortable: true },
  { key: 'statutSouscription', label: t('servicesPremium.souscription.statutSouscription'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await fetchAll()
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Souscription): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Souscription): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateSouscriptionPayload | UpdateSouscriptionPayload): Promise<void> {
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

function handleFilter(filters: SouscriptionFilters): void {
  setFilters(filters)
}

function handleSort(key: string, direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    Active: 'success',
    Expiree: 'neutral',
    Resiliee: 'error',
    Essai: 'info',
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
            { label: $t('nav.servicesPremium'), to: '/services-premium' },
            { label: $t('servicesPremium.souscription.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('servicesPremium.souscription.title') }}
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
          v-permission="'services-premium:create'"
          color="primary"
          icon="i-heroicons-plus"
          @click="openCreate"
        >
          {{ $t('servicesPremium.souscription.add') }}
        </UButton>
      </div>
    </div>

    <SouscriptionStats
      :items="items"
      :total-count="pagination.totalCount"
    />

    <SouscriptionFiltersComponent
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
        <template #cell-serviceParentNom="{ row }">
          {{ (row as Souscription).serviceParentNom ?? `#${(row as Souscription).serviceParentId}` }}
        </template>
        <template #cell-parentNom="{ row }">
          {{ (row as Souscription).parentNom ?? `#${(row as Souscription).parentId}` }}
        </template>
        <template #cell-dateFin="{ row }">
          <span v-if="(row as Souscription).dateFin">{{ formatDate((row as Souscription).dateFin) }}</span>
          <span
            v-else
            class="text-gray-400"
          >—</span>
        </template>
        <template #cell-montantPaye="{ row }">
          {{ formatMontant((row as Souscription).montantPaye) }}
        </template>
        <template #cell-statutSouscription="{ row }">
          <UBadge
            :color="getStatutColor((row as Souscription).statutSouscription)"
            variant="subtle"
            size="sm"
          >
            {{ $t(`servicesPremium.souscription.statut.${(row as Souscription).statutSouscription}`) }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'services-premium:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Souscription)"
            />
            <UButton
              v-permission="'services-premium:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Souscription)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-clipboard-document-list"
        :title="$t('servicesPremium.souscription.emptyTitle')"
        :description="$t('servicesPremium.souscription.emptyDescription')"
        :action-label="$t('servicesPremium.souscription.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div
        v-if="!isEmpty"
        class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3"
      >
        <SouscriptionCard
          v-for="souscription in items"
          :key="souscription.id"
          :souscription="souscription"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-clipboard-document-list"
        :title="$t('servicesPremium.souscription.emptyTitle')"
        :description="$t('servicesPremium.souscription.emptyDescription')"
        :action-label="$t('servicesPremium.souscription.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('servicesPremium.souscription.edit') : $t('servicesPremium.souscription.add') }}
        </h3>
      </template>
      <template #body>
        <SouscriptionForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('servicesPremium.souscription.deleteTitle')"
      :description="$t('servicesPremium.souscription.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
