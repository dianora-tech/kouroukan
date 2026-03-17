<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Notification, CreateNotificationPayload, UpdateNotificationPayload } from '~/modules/communication/types/notification.types'
import { useNotification } from '~/modules/communication/composables/useNotification'
import type { NotificationFilters } from '~/modules/communication/types/notification.types'
import NotificationForm from '~/modules/communication/components/NotificationForm.vue'
import NotificationCard from '~/modules/communication/components/NotificationCard.vue'
import NotificationFiltersComponent from '~/modules/communication/components/NotificationFilters.vue'
import NotificationStats from '~/modules/communication/components/NotificationStats.vue'

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
} = useNotification()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Notification | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Notification | null>(null)

const columns: Column[] = [
  { key: 'contenu', label: t('communication.notification.contenu'), sortable: true },
  { key: 'canal', label: t('communication.notification.canal'), sortable: true },
  { key: 'typeName', label: t('communication.notification.type'), sortable: false },
  { key: 'estEnvoyee', label: t('communication.notification.statut'), sortable: true },
  { key: 'dateEnvoi', label: t('communication.notification.dateEnvoi'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(() => {
  fetchAll()
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Notification): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Notification): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateNotificationPayload | UpdateNotificationPayload): Promise<void> {
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

function handleFilter(filters: NotificationFilters): void {
  setFilters(filters)
}

function handleSort(key: string, direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getStatutEnvoiColor(estEnvoyee: boolean): string {
  return estEnvoyee ? 'success' : 'warning'
}
</script>

<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('nav.communication'), to: '/communication' },
            { label: $t('communication.notification.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('communication.notification.title') }}
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
          v-permission="'communication:create'"
          color="primary"
          icon="i-heroicons-plus"
          @click="openCreate"
        >
          {{ $t('communication.notification.add') }}
        </UButton>
      </div>
    </div>

    <!-- Stats -->
    <NotificationStats :items="items" :total-count="pagination.totalCount" />

    <!-- Filters -->
    <NotificationFiltersComponent @filter="handleFilter" @reset="resetFilters" />

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
        <template #cell-canal="{ row }">
          <UBadge color="info" variant="subtle" size="sm">
            {{ $t(`communication.notification.canal.${(row as Notification).canal}`) }}
          </UBadge>
        </template>
        <template #cell-estEnvoyee="{ row }">
          <UBadge :color="getStatutEnvoiColor((row as Notification).estEnvoyee)" variant="subtle" size="sm">
            {{ (row as Notification).estEnvoyee ? $t('communication.notification.envoyee') : $t('communication.notification.enAttente') }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'communication:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Notification)"
            />
            <UButton
              v-permission="'communication:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Notification)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-bell"
        :title="$t('communication.notification.emptyTitle')"
        :description="$t('communication.notification.emptyDescription')"
        :action-label="$t('communication.notification.add')"
        @action="openCreate"
      />
    </template>

    <!-- Grid view -->
    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <NotificationCard
          v-for="notification in items"
          :key="notification.id"
          :notification="notification"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-bell"
        :title="$t('communication.notification.emptyTitle')"
        :description="$t('communication.notification.emptyDescription')"
        :action-label="$t('communication.notification.add')"
        @action="openCreate"
      />
    </template>

    <!-- Slideover Form -->
    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('communication.notification.edit') : $t('communication.notification.add') }}
        </h3>
      </template>
      <template #body>
        <NotificationForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <!-- Delete Confirmation -->
    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('communication.notification.deleteTitle')"
      :description="$t('communication.notification.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
