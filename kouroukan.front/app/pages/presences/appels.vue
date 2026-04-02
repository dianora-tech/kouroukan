<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Appel, CreateAppelPayload, UpdateAppelPayload, AppelFilters } from '~/modules/presences/types/appel.types'
import { useAppel } from '~/modules/presences/composables/useAppel'
import AppelForm from '~/modules/presences/components/AppelForm.vue'
import AppelCard from '~/modules/presences/components/AppelCard.vue'
import AppelFiltersComponent from '~/modules/presences/components/AppelFilters.vue'
import AppelStats from '~/modules/presences/components/AppelStats.vue'

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
} = useAppel()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Appel | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Appel | null>(null)

const columns: Column[] = [
  { key: 'classeName', label: t('presences.appel.classe'), sortable: true },
  { key: 'enseignantNom', label: t('presences.appel.enseignant'), sortable: true },
  { key: 'dateAppel', label: t('presences.appel.dateAppel'), sortable: true, render: (row: any) => formatDate(row.dateAppel) },
  { key: 'heureAppel', label: t('presences.appel.heureAppel'), sortable: true },
  { key: 'estCloture', label: t('presences.appel.statut'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await fetchAll()
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Appel): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Appel): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateAppelPayload | UpdateAppelPayload): Promise<void> {
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

function handleFilter(filters: AppelFilters): void {
  setFilters(filters)
}

function handleSort(_key: string, _direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}
</script>

<template>
  <div class="space-y-6">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('nav.presences'), to: '/presences' },
            { label: $t('presences.appel.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('presences.appel.title') }}
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
          v-permission="'presences:create'"
          color="primary"
          icon="i-heroicons-plus"
          @click="openCreate"
        >
          {{ $t('presences.appel.add') }}
        </UButton>
      </div>
    </div>

    <AppelStats
      :items="items"
      :total-count="pagination.totalCount"
    />

    <AppelFiltersComponent
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
        <template #cell-classeName="{ row }">
          {{ (row as Appel).classeName ?? `#${(row as Appel).classeId}` }}
        </template>
        <template #cell-enseignantNom="{ row }">
          {{ (row as Appel).enseignantNom ?? `#${(row as Appel).enseignantId}` }}
        </template>
        <template #cell-estCloture="{ row }">
          <UBadge
            :color="(row as Appel).estCloture ? 'success' : 'warning'"
            variant="subtle"
            size="sm"
          >
            {{ (row as Appel).estCloture ? $t('presences.appel.cloture') : $t('presences.appel.enCours') }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'presences:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Appel)"
            />
            <UButton
              v-permission="'presences:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Appel)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-clipboard-document-check"
        :title="$t('presences.appel.emptyTitle')"
        :description="$t('presences.appel.emptyDescription')"
        :action-label="$t('presences.appel.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div
        v-if="!isEmpty"
        class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3"
      >
        <AppelCard
          v-for="appel in items"
          :key="appel.id"
          :appel="appel"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-clipboard-document-check"
        :title="$t('presences.appel.emptyTitle')"
        :description="$t('presences.appel.emptyDescription')"
        :action-label="$t('presences.appel.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('presences.appel.edit') : $t('presences.appel.add') }}
        </h3>
      </template>
      <template #body>
        <AppelForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('presences.appel.deleteTitle')"
      :description="$t('presences.appel.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
