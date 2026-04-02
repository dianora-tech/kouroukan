<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Salle, CreateSallePayload, UpdateSallePayload, SalleFilters } from '~/modules/pedagogie/types/salle.types'
import { useSalle } from '~/modules/pedagogie/composables/useSalle'
import SalleForm from '~/modules/pedagogie/components/SalleForm.vue'
import SalleCard from '~/modules/pedagogie/components/SalleCard.vue'
import SalleFiltersComponent from '~/modules/pedagogie/components/SalleFilters.vue'
import SalleStats from '~/modules/pedagogie/components/SalleStats.vue'

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
} = useSalle()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Salle | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Salle | null>(null)

const columns: Column[] = [
  { key: 'name', label: t('pedagogie.salle.name'), sortable: true },
  { key: 'typeName', label: t('pedagogie.salle.type'), sortable: true },
  { key: 'capacite', label: t('pedagogie.salle.capacite'), sortable: true },
  { key: 'batiment', label: t('pedagogie.salle.batiment'), sortable: true },
  { key: 'estDisponible', label: t('pedagogie.salle.estDisponible'), sortable: false },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(() => {
  fetchAll()
  fetchTypes()
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Salle): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Salle): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateSallePayload | UpdateSallePayload): Promise<void> {
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

function handleFilter(filters: SalleFilters): void {
  setFilters(filters)
}

function handleSort(_key: string, _direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
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
            { label: $t('pedagogie.salle.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('pedagogie.salle.title') }}
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
          {{ $t('pedagogie.salle.add') }}
        </UButton>
      </div>
    </div>

    <!-- Stats -->
    <SalleStats
      :items="items"
      :total-count="pagination.totalCount"
    />

    <!-- Filters -->
    <SalleFiltersComponent
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
        <template #cell-estDisponible="{ row }">
          <UBadge
            :color="(row as Salle).estDisponible ? 'success' : 'error'"
            variant="subtle"
            size="sm"
          >
            {{ (row as Salle).estDisponible ? $t('pedagogie.salle.disponible') : $t('pedagogie.salle.indisponible') }}
          </UBadge>
        </template>
        <template #cell-typeName="{ row }">
          <UBadge
            v-if="(row as Salle).typeName"
            color="primary"
            variant="subtle"
            size="sm"
          >
            {{ (row as Salle).typeName }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'pedagogie:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Salle)"
            />
            <UButton
              v-permission="'pedagogie:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Salle)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-building-office-2"
        :title="$t('pedagogie.salle.emptyTitle')"
        :description="$t('pedagogie.salle.emptyDescription')"
        :action-label="$t('pedagogie.salle.add')"
        @action="openCreate"
      />
    </template>

    <!-- Grid view -->
    <template v-else>
      <div
        v-if="!isEmpty"
        class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3"
      >
        <SalleCard
          v-for="salle in items"
          :key="salle.id"
          :salle="salle"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-building-office-2"
        :title="$t('pedagogie.salle.emptyTitle')"
        :description="$t('pedagogie.salle.emptyDescription')"
        :action-label="$t('pedagogie.salle.add')"
        @action="openCreate"
      />
    </template>

    <!-- Slideover Form -->
    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('pedagogie.salle.edit') : $t('pedagogie.salle.add') }}
        </h3>
      </template>
      <template #body>
        <SalleForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <!-- Delete Confirmation -->
    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('pedagogie.salle.deleteTitle')"
      :description="$t('pedagogie.salle.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
