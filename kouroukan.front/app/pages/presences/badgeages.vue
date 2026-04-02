<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Badgeage, CreateBadgeagePayload, UpdateBadgeagePayload } from '~/modules/presences/types/badgeage.types'
import type { BadgeageFilters } from '~/modules/presences/types/badgeage.types'
import { useBadgeage } from '~/modules/presences/composables/useBadgeage'
import BadgeageForm from '~/modules/presences/components/BadgeageForm.vue'
import BadgeageCard from '~/modules/presences/components/BadgeageCard.vue'
import BadgeageFiltersComponent from '~/modules/presences/components/BadgeageFilters.vue'
import BadgeageStats from '~/modules/presences/components/BadgeageStats.vue'

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
} = useBadgeage()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Badgeage | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Badgeage | null>(null)

const columns: Column[] = [
  { key: 'eleveNom', label: t('presences.badgeage.eleve'), sortable: true },
  { key: 'typeName', label: t('presences.badgeage.type'), sortable: true },
  { key: 'dateBadgeage', label: t('presences.badgeage.dateBadgeage'), sortable: true, render: (row: any) => formatDate(row.dateBadgeage) },
  { key: 'heureBadgeage', label: t('presences.badgeage.heureBadgeage'), sortable: true },
  { key: 'pointAcces', label: t('presences.badgeage.pointAcces'), sortable: true },
  { key: 'methodeBadgeage', label: t('presences.badgeage.methodeBadgeage'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await Promise.all([fetchAll(), fetchTypes()])
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Badgeage): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Badgeage): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateBadgeagePayload | UpdateBadgeagePayload): Promise<void> {
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

function handleFilter(filters: BadgeageFilters): void {
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
            { label: $t('presences.badgeage.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('presences.badgeage.title') }}
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
          {{ $t('presences.badgeage.add') }}
        </UButton>
      </div>
    </div>

    <BadgeageStats :items="items" :total-count="pagination.totalCount" />

    <BadgeageFiltersComponent @filter="handleFilter" @reset="resetFilters" />

    <template v-if="viewMode === 'table'">
      <DataTable
        v-if="!isEmpty || loading"
        :columns="columns"
        :data="paginatedData"
        :loading="loading"
        @page-change="changePage"
        @sort="handleSort"
      >
        <template #cell-eleveNom="{ row }">
          {{ (row as Badgeage).eleveNom ?? `#${(row as Badgeage).eleveId}` }}
        </template>
        <template #cell-pointAcces="{ row }">
          {{ $t(`presences.badgeage.pointAcces.${(row as Badgeage).pointAcces}`) }}
        </template>
        <template #cell-methodeBadgeage="{ row }">
          <UBadge color="primary" variant="subtle" size="sm">
            {{ (row as Badgeage).methodeBadgeage }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'presences:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Badgeage)"
            />
            <UButton
              v-permission="'presences:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Badgeage)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-identification"
        :title="$t('presences.badgeage.emptyTitle')"
        :description="$t('presences.badgeage.emptyDescription')"
        :action-label="$t('presences.badgeage.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <BadgeageCard
          v-for="badgeage in items"
          :key="badgeage.id"
          :badgeage="badgeage"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-identification"
        :title="$t('presences.badgeage.emptyTitle')"
        :description="$t('presences.badgeage.emptyDescription')"
        :action-label="$t('presences.badgeage.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('presences.badgeage.edit') : $t('presences.badgeage.add') }}
        </h3>
      </template>
      <template #body>
        <BadgeageForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('presences.badgeage.deleteTitle')"
      :description="$t('presences.badgeage.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
