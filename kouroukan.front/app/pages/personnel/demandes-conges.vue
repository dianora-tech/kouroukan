<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { DemandeConge, CreateDemandeCongePayload, UpdateDemandeCongePayload } from '~/modules/personnel/types/demandeConge.types'
import { useDemandeConge } from '~/modules/personnel/composables/useDemandeConge'
import type { DemandeCongeFilters } from '~/modules/personnel/types/demandeConge.types'
import DemandeCongeForm from '~/modules/personnel/components/DemandeCongeForm.vue'
import DemandeCongeCard from '~/modules/personnel/components/DemandeCongeCard.vue'
import DemandeCongeFiltersComponent from '~/modules/personnel/components/DemandeCongeFilters.vue'
import DemandeCongeStats from '~/modules/personnel/components/DemandeCongeStats.vue'

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
} = useDemandeConge()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<DemandeConge | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<DemandeConge | null>(null)

const columns: Column[] = [
  { key: 'typeName', label: t('personnel.demandeConge.type'), sortable: true },
  { key: 'enseignantNom', label: t('personnel.demandeConge.enseignant'), sortable: true },
  { key: 'dateDebut', label: t('personnel.demandeConge.dateDebut'), sortable: true, render: (row: any) => formatDate(row.dateDebut) },
  { key: 'dateFin', label: t('personnel.demandeConge.dateFin'), sortable: true, render: (row: any) => formatDate(row.dateFin) },
  { key: 'motif', label: t('personnel.demandeConge.motif'), sortable: false },
  { key: 'statutDemande', label: t('personnel.demandeConge.statutDemandeLabel'), sortable: true },
  { key: 'impactPaie', label: t('personnel.demandeConge.impactPaie'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await Promise.all([fetchAll(), fetchTypes()])
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: DemandeConge): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: DemandeConge): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateDemandeCongePayload | UpdateDemandeCongePayload): Promise<void> {
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

function handleFilter(filters: DemandeCongeFilters): void {
  setFilters(filters)
}

function handleSort(key: string, direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getStatutColor(statut: string): string {
  switch (statut) {
    case 'Soumise': return 'info'
    case 'ApprouveeN1': return 'warning'
    case 'ApprouveeDirection': return 'success'
    case 'Refusee': return 'error'
    default: return 'neutral'
  }
}
</script>

<template>
  <div class="space-y-6">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('nav.personnel'), to: '/personnel' },
            { label: $t('personnel.demandeConge.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('personnel.demandeConge.title') }}
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
          v-permission="'personnel:create'"
          color="primary"
          icon="i-heroicons-plus"
          @click="openCreate"
        >
          {{ $t('personnel.demandeConge.add') }}
        </UButton>
      </div>
    </div>

    <DemandeCongeStats :items="items" :total-count="pagination.totalCount" />

    <DemandeCongeFiltersComponent @filter="handleFilter" @reset="resetFilters" />

    <template v-if="viewMode === 'table'">
      <DataTable
        v-if="!isEmpty || loading"
        :columns="columns"
        :data="paginatedData"
        :loading="loading"
        @page-change="changePage"
        @sort="handleSort"
      >
        <template #cell-statutDemande="{ row }">
          <UBadge :color="getStatutColor((row as DemandeConge).statutDemande)" variant="subtle" size="sm">
            {{ $t(`personnel.demandeConge.statut.${(row as DemandeConge).statutDemande}`) }}
          </UBadge>
        </template>
        <template #cell-impactPaie="{ row }">
          <UIcon
            :name="(row as DemandeConge).impactPaie ? 'i-heroicons-check-circle' : 'i-heroicons-x-circle'"
            :class="(row as DemandeConge).impactPaie ? 'text-amber-500' : 'text-gray-400'"
            class="h-5 w-5"
          />
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'personnel:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as DemandeConge)"
            />
            <UButton
              v-permission="'personnel:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as DemandeConge)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-document-text"
        :title="$t('personnel.demandeConge.emptyTitle')"
        :description="$t('personnel.demandeConge.emptyDescription')"
        :action-label="$t('personnel.demandeConge.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <DemandeCongeCard
          v-for="demandeConge in items"
          :key="demandeConge.id"
          :demande-conge="demandeConge"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-document-text"
        :title="$t('personnel.demandeConge.emptyTitle')"
        :description="$t('personnel.demandeConge.emptyDescription')"
        :action-label="$t('personnel.demandeConge.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('personnel.demandeConge.edit') : $t('personnel.demandeConge.add') }}
        </h3>
      </template>
      <template #body>
        <DemandeCongeForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('personnel.demandeConge.deleteTitle')"
      :description="$t('personnel.demandeConge.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
