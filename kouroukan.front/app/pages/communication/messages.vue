<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { Message, CreateMessagePayload, UpdateMessagePayload } from '~/modules/communication/types/message.types'
import { useMessage } from '~/modules/communication/composables/useMessage'
import type { MessageFilters } from '~/modules/communication/types/message.types'
import MessageForm from '~/modules/communication/components/MessageForm.vue'
import MessageCard from '~/modules/communication/components/MessageCard.vue'
import MessageFiltersComponent from '~/modules/communication/components/MessageFilters.vue'
import MessageStats from '~/modules/communication/components/MessageStats.vue'

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
} = useMessage()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<Message | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Message | null>(null)

const columns: Column[] = [
  { key: 'sujet', label: t('communication.message.sujet'), sortable: true },
  { key: 'expediteurNom', label: t('communication.message.expediteur'), sortable: true },
  { key: 'destinataireNom', label: t('communication.message.destinataire'), sortable: true },
  { key: 'typeName', label: t('communication.message.type'), sortable: false },
  { key: 'estLu', label: t('communication.message.statut'), sortable: true },
  { key: 'createdAt', label: t('communication.message.date'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(() => {
  fetchAll()
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Message): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Message): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateMessagePayload | UpdateMessagePayload): Promise<void> {
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

function handleFilter(filters: MessageFilters): void {
  setFilters(filters)
}

function handleSort(key: string, direction: 'asc' | 'desc'): void {
  fetchAll({ page: 1 })
}

function getStatutLuColor(estLu: boolean): string {
  return estLu ? 'success' : 'warning'
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
            { label: $t('communication.message.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('communication.message.title') }}
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
          {{ $t('communication.message.add') }}
        </UButton>
      </div>
    </div>

    <!-- Stats -->
    <MessageStats :items="items" :total-count="pagination.totalCount" />

    <!-- Filters -->
    <MessageFiltersComponent @filter="handleFilter" @reset="resetFilters" />

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
        <template #cell-estLu="{ row }">
          <UBadge :color="getStatutLuColor((row as Message).estLu)" variant="subtle" size="sm">
            {{ (row as Message).estLu ? $t('communication.message.lu') : $t('communication.message.nonLu') }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'communication:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as Message)"
            />
            <UButton
              v-permission="'communication:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as Message)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-envelope"
        :title="$t('communication.message.emptyTitle')"
        :description="$t('communication.message.emptyDescription')"
        :action-label="$t('communication.message.add')"
        @action="openCreate"
      />
    </template>

    <!-- Grid view -->
    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <MessageCard
          v-for="message in items"
          :key="message.id"
          :message="message"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-envelope"
        :title="$t('communication.message.emptyTitle')"
        :description="$t('communication.message.emptyDescription')"
        :action-label="$t('communication.message.add')"
        @action="openCreate"
      />
    </template>

    <!-- Slideover Form -->
    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('communication.message.edit') : $t('communication.message.add') }}
        </h3>
      </template>
      <template #body>
        <MessageForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <!-- Delete Confirmation -->
    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('communication.message.deleteTitle')"
      :description="$t('communication.message.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
