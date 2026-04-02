<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Classe, CreateClassePayload, UpdateClassePayload, ClasseFilters } from '~/modules/pedagogie/types/classe.types'
import { useClasse } from '~/modules/pedagogie/composables/useClasse'
import ClasseForm from '~/modules/pedagogie/components/ClasseForm.vue'
import ClasseCard from '~/modules/pedagogie/components/ClasseCard.vue'
import ClasseFiltersComponent from '~/modules/pedagogie/components/ClasseFilters.vue'
import ClasseStats from '~/modules/pedagogie/components/ClasseStats.vue'

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
} = useClasse()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Classe | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Classe | null>(null)

const columns: Column[] = [
  { key: 'name', label: t('pedagogie.classe.name'), sortable: true },
  { key: 'niveauClasseName', label: t('pedagogie.classe.niveauClasse'), sortable: true },
  { key: 'effectif', label: t('pedagogie.classe.effectif'), sortable: true },
  { key: 'capacite', label: t('pedagogie.classe.capacite'), sortable: true },
  { key: 'enseignantPrincipalNom', label: t('pedagogie.classe.enseignantPrincipal'), sortable: false },
  { key: 'anneeScolaireLibelle', label: t('pedagogie.classe.anneeScolaire'), sortable: false },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(() => {
  fetchAll()
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Classe): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Classe): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateClassePayload | UpdateClassePayload): Promise<void> {
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

function handleFilter(filters: ClasseFilters): void {
  setFilters(filters)
}

function handleSort(_key: string, _direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getOccupancyColor(effectif: number, capacite: number): string {
  const ratio = effectif / capacite
  if (ratio >= 0.9) return 'error'
  if (ratio >= 0.7) return 'warning'
  return 'success'
}
</script>

<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('nav.pedagogie'), to: '/pedagogie' },
            { label: $t('pedagogie.classe.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('pedagogie.classe.title') }}
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
          v-permission="'pedagogie:create'"
          color="primary"
          icon="i-heroicons-plus"
          @click="openCreate"
        >
          {{ $t('pedagogie.classe.add') }}
        </UButton>
      </div>
    </div>

    <!-- Stats -->
    <ClasseStats
      :items="items"
      :total-count="pagination.totalCount"
    />

    <!-- Filters -->
    <ClasseFiltersComponent
      @filter="handleFilter"
      @reset="resetFilters"
    />

    <!-- Table view -->
    <template v-if="viewMode === 'table'">
      <DataTable
        v-if="!isEmpty || loading"
        :columns="columns"
        :data="paginatedData"
        :loading="loading"
        @page-change="changePage"
        @sort="handleSort"
      >
        <template #cell-effectif="{ row }">
          <UBadge
            :color="getOccupancyColor((row as Classe).effectif, (row as Classe).capacite)"
            variant="subtle"
            size="sm"
          >
            {{ (row as Classe).effectif }}/{{ (row as Classe).capacite }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'pedagogie:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Classe)"
            />
            <UButton
              v-permission="'pedagogie:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Classe)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-rectangle-group"
        :title="$t('pedagogie.classe.emptyTitle')"
        :description="$t('pedagogie.classe.emptyDescription')"
        :action-label="$t('pedagogie.classe.add')"
        @action="openCreate"
      />
    </template>

    <!-- Grid view -->
    <template v-else>
      <div
        v-if="!isEmpty"
        class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3"
      >
        <ClasseCard
          v-for="classe in items"
          :key="classe.id"
          :classe="classe"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-rectangle-group"
        :title="$t('pedagogie.classe.emptyTitle')"
        :description="$t('pedagogie.classe.emptyDescription')"
        :action-label="$t('pedagogie.classe.add')"
        @action="openCreate"
      />
    </template>

    <!-- Slideover Form -->
    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('pedagogie.classe.edit') : $t('pedagogie.classe.add') }}
        </h3>
      </template>
      <template #body>
        <ClasseForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <!-- Delete Confirmation -->
    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('pedagogie.classe.deleteTitle')"
      :description="$t('pedagogie.classe.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
