<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Bulletin, CreateBulletinPayload, UpdateBulletinPayload } from '~/modules/evaluations/types/bulletin.types'
import { useBulletin } from '~/modules/evaluations/composables/useBulletin'
import type { BulletinFilters } from '~/modules/evaluations/types/bulletin.types'
import BulletinForm from '~/modules/evaluations/components/BulletinForm.vue'
import BulletinCard from '~/modules/evaluations/components/BulletinCard.vue'
import BulletinFiltersComponent from '~/modules/evaluations/components/BulletinFilters.vue'
import BulletinStats from '~/modules/evaluations/components/BulletinStats.vue'

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
} = useBulletin()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Bulletin | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Bulletin | null>(null)

function getMoyenneColor(moyenne: number): string {
  if (moyenne >= 14) return 'success'
  if (moyenne >= 10) return 'warning'
  return 'error'
}

const columns: Column[] = [
  { key: 'eleveNom', label: t('evaluations.bulletin.eleve'), sortable: true },
  { key: 'classeName', label: t('evaluations.bulletin.classe'), sortable: true },
  { key: 'trimestre', label: t('evaluations.bulletin.trimestreLabel'), sortable: true },
  { key: 'moyenneGenerale', label: t('evaluations.bulletin.moyenneGenerale'), sortable: true },
  { key: 'rang', label: t('evaluations.bulletin.rang'), sortable: true },
  { key: 'estPublie', label: t('evaluations.bulletin.estPublie'), sortable: true },
  { key: 'dateGeneration', label: t('evaluations.bulletin.dateGeneration'), sortable: true, render: (row: any) => formatDate(row.dateGeneration) },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await fetchAll()
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Bulletin): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Bulletin): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateBulletinPayload | UpdateBulletinPayload): Promise<void> {
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

function handleFilter(filters: BulletinFilters): void {
  setFilters(filters)
}

function handleSort(key: string, direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}
</script>

<template>
  <div class="space-y-6">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('nav.evaluations'), to: '/evaluations' },
            { label: $t('evaluations.bulletin.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('evaluations.bulletin.title') }}
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
          v-permission="'evaluations:create'"
          color="primary"
          icon="i-heroicons-plus"
          @click="openCreate"
        >
          {{ $t('evaluations.bulletin.add') }}
        </UButton>
      </div>
    </div>

    <BulletinStats :items="items" :total-count="pagination.totalCount" />

    <BulletinFiltersComponent @filter="handleFilter" @reset="resetFilters" />

    <template v-if="viewMode === 'table'">
      <DataTable
        v-if="!isEmpty || loading"
        :columns="columns"
        :data="paginatedData"
        :loading="loading"
        @page-change="changePage"
        @sort="handleSort"
      >
        <template #cell-trimestre="{ row }">
          <UBadge color="primary" variant="subtle" size="sm">
            {{ $t(`evaluations.bulletin.trimestre.${(row as Bulletin).trimestre}`) }}
          </UBadge>
        </template>
        <template #cell-moyenneGenerale="{ row }">
          <UBadge :color="getMoyenneColor((row as Bulletin).moyenneGenerale)" variant="subtle" size="sm">
            {{ (row as Bulletin).moyenneGenerale.toFixed(2) }} / 20
          </UBadge>
        </template>
        <template #cell-rang="{ row }">
          {{ (row as Bulletin).rang ?? '-' }}
        </template>
        <template #cell-estPublie="{ row }">
          <UBadge v-if="(row as Bulletin).estPublie" color="success" variant="subtle" size="xs">
            {{ $t('evaluations.bulletin.publie') }}
          </UBadge>
          <UBadge v-else color="warning" variant="subtle" size="xs">
            {{ $t('evaluations.bulletin.nonPublie') }}
          </UBadge>
        </template>
        <template #cell-dateGeneration="{ row }">
          {{ formatDate((row as Bulletin).dateGeneration) }}
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'evaluations:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Bulletin)"
            />
            <UButton
              v-permission="'evaluations:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Bulletin)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-document-duplicate"
        :title="$t('evaluations.bulletin.emptyTitle')"
        :description="$t('evaluations.bulletin.emptyDescription')"
        :action-label="$t('evaluations.bulletin.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <BulletinCard
          v-for="bulletin in items"
          :key="bulletin.id"
          :bulletin="bulletin"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-document-duplicate"
        :title="$t('evaluations.bulletin.emptyTitle')"
        :description="$t('evaluations.bulletin.emptyDescription')"
        :action-label="$t('evaluations.bulletin.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('evaluations.bulletin.edit') : $t('evaluations.bulletin.add') }}
        </h3>
      </template>
      <template #body>
        <BulletinForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('evaluations.bulletin.deleteTitle')"
      :description="$t('evaluations.bulletin.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
