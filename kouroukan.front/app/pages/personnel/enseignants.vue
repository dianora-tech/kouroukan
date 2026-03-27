<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Enseignant, CreateEnseignantPayload, UpdateEnseignantPayload } from '~/modules/personnel/types/enseignant.types'
import { useEnseignant } from '~/modules/personnel/composables/useEnseignant'
import type { EnseignantFilters } from '~/modules/personnel/types/enseignant.types'
import EnseignantForm from '~/modules/personnel/components/EnseignantForm.vue'
import EnseignantCard from '~/modules/personnel/components/EnseignantCard.vue'
import EnseignantFiltersComponent from '~/modules/personnel/components/EnseignantFilters.vue'
import EnseignantStats from '~/modules/personnel/components/EnseignantStats.vue'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { formatDateShort } = useFormatDate()
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
} = useEnseignant()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Enseignant | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Enseignant | null>(null)

const columns: Column[] = [
  { key: 'matricule', label: t('personnel.enseignant.matricule'), sortable: true },
  { key: 'typeName', label: t('personnel.enseignant.type'), sortable: true },
  { key: 'specialite', label: t('personnel.enseignant.specialite'), sortable: true },
  { key: 'telephone', label: t('personnel.enseignant.telephone'), sortable: false },
  { key: 'dateEmbauche', label: t('personnel.enseignant.dateEmbauche'), sortable: true },
  { key: 'modeRemuneration', label: t('personnel.enseignant.modeRemunerationLabel'), sortable: true },
  { key: 'statutEnseignant', label: t('personnel.enseignant.statutEnseignantLabel'), sortable: true },
  { key: 'soldeCongesAnnuel', label: t('personnel.enseignant.soldeCongesAnnuel'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await Promise.all([fetchAll(), fetchTypes()])
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Enseignant): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Enseignant): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateEnseignantPayload | UpdateEnseignantPayload): Promise<void> {
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

function handleFilter(filters: EnseignantFilters): void {
  setFilters(filters)
}

function handleSort(key: string, direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getStatutColor(statut: string): string {
  switch (statut) {
    case 'Actif': return 'success'
    case 'EnConge': return 'warning'
    case 'Suspendu': return 'error'
    case 'Inactif': return 'neutral'
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
            { label: $t('personnel.enseignant.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('personnel.enseignant.title') }}
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
          {{ $t('personnel.enseignant.add') }}
        </UButton>
      </div>
    </div>

    <EnseignantStats :items="items" :total-count="pagination.totalCount" />

    <EnseignantFiltersComponent @filter="handleFilter" @reset="resetFilters" />

    <template v-if="viewMode === 'table'">
      <DataTable
        v-if="!isEmpty || loading"
        :columns="columns"
        :data="paginatedData"
        :loading="loading"
        @page-change="changePage"
        @sort="handleSort"
      >
        <template #cell-dateEmbauche="{ row }">
          {{ formatDateShort((row as Enseignant).dateEmbauche) }}
        </template>
        <template #cell-statutEnseignant="{ row }">
          <UBadge :color="getStatutColor((row as Enseignant).statutEnseignant)" variant="subtle" size="sm">
            {{ $t(`personnel.enseignant.statut.${(row as Enseignant).statutEnseignant}`) }}
          </UBadge>
        </template>
        <template #cell-modeRemuneration="{ row }">
          {{ $t(`personnel.enseignant.modeRemuneration.${(row as Enseignant).modeRemuneration}`) }}
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'personnel:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Enseignant)"
            />
            <UButton
              v-permission="'personnel:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Enseignant)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-user-group"
        :title="$t('personnel.enseignant.emptyTitle')"
        :description="$t('personnel.enseignant.emptyDescription')"
        :action-label="$t('personnel.enseignant.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <EnseignantCard
          v-for="enseignant in items"
          :key="enseignant.id"
          :enseignant="enseignant"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-user-group"
        :title="$t('personnel.enseignant.emptyTitle')"
        :description="$t('personnel.enseignant.emptyDescription')"
        :action-label="$t('personnel.enseignant.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('personnel.enseignant.edit') : $t('personnel.enseignant.add') }}
        </h3>
      </template>
      <template #body>
        <EnseignantForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('personnel.enseignant.deleteTitle')"
      :description="$t('personnel.enseignant.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
