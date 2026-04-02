<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { ServiceParent, CreateServiceParentPayload, UpdateServiceParentPayload, ServiceParentFilters } from '~/modules/services-premium/types/service-parent.types'
import { useServiceParent } from '~/modules/services-premium/composables/useServiceParent'
import ServiceParentForm from '~/modules/services-premium/components/ServiceParentForm.vue'
import ServiceParentCard from '~/modules/services-premium/components/ServiceParentCard.vue'
import ServiceParentFiltersComponent from '~/modules/services-premium/components/ServiceParentFilters.vue'
import ServiceParentStats from '~/modules/services-premium/components/ServiceParentStats.vue'

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
} = useServiceParent()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<ServiceParent | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<ServiceParent | null>(null)

function formatMontant(montant: number): string {
  return new Intl.NumberFormat('fr-GN', { style: 'decimal' }).format(montant) + ' GNF'
}

const columns: Column[] = [
  { key: 'code', label: t('servicesPremium.serviceParent.code'), sortable: true },
  { key: 'typeName', label: t('servicesPremium.serviceParent.type'), sortable: true },
  { key: 'tarif', label: t('servicesPremium.serviceParent.tarif'), sortable: true },
  { key: 'periodicite', label: t('servicesPremium.serviceParent.periodiciteLabel'), sortable: true },
  { key: 'periodeEssaiJours', label: t('servicesPremium.serviceParent.periodeEssaiJours'), sortable: true },
  { key: 'estActif', label: t('servicesPremium.serviceParent.estActif'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(async () => {
  await Promise.all([fetchAll(), fetchTypes()])
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: ServiceParent): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: ServiceParent): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateServiceParentPayload | UpdateServiceParentPayload): Promise<void> {
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

function handleFilter(filters: ServiceParentFilters): void {
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
            { label: $t('nav.servicesPremium'), to: '/services-premium' },
            { label: $t('servicesPremium.serviceParent.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('servicesPremium.serviceParent.title') }}
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
          {{ $t('servicesPremium.serviceParent.add') }}
        </UButton>
      </div>
    </div>

    <ServiceParentStats
      :items="items"
      :total-count="pagination.totalCount"
    />

    <ServiceParentFiltersComponent
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
        <template #cell-tarif="{ row }">
          {{ formatMontant((row as ServiceParent).tarif) }}
        </template>
        <template #cell-periodicite="{ row }">
          {{ $t(`servicesPremium.serviceParent.periodicite.${(row as ServiceParent).periodicite}`) }}
        </template>
        <template #cell-periodeEssaiJours="{ row }">
          <span v-if="(row as ServiceParent).periodeEssaiJours">
            {{ (row as ServiceParent).periodeEssaiJours }} {{ $t('servicesPremium.serviceParent.jours') }}
          </span>
          <span
            v-else
            class="text-gray-400"
          >—</span>
        </template>
        <template #cell-estActif="{ row }">
          <UBadge
            :color="(row as ServiceParent).estActif ? 'success' : 'neutral'"
            variant="subtle"
            size="sm"
          >
            {{ (row as ServiceParent).estActif ? $t('servicesPremium.serviceParent.actif') : $t('servicesPremium.serviceParent.inactif') }}
          </UBadge>
        </template>
        <template #cell-actions="{ row }">
          <div class="flex gap-1">
            <UButton
              v-permission="'services-premium:update'"
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as ServiceParent)"
            />
            <UButton
              v-permission="'services-premium:delete'"
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as ServiceParent)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-star"
        :title="$t('servicesPremium.serviceParent.emptyTitle')"
        :description="$t('servicesPremium.serviceParent.emptyDescription')"
        :action-label="$t('servicesPremium.serviceParent.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div
        v-if="!isEmpty"
        class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3"
      >
        <ServiceParentCard
          v-for="service in items"
          :key="service.id"
          :service-parent="service"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-star"
        :title="$t('servicesPremium.serviceParent.emptyTitle')"
        :description="$t('servicesPremium.serviceParent.emptyDescription')"
        :action-label="$t('servicesPremium.serviceParent.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('servicesPremium.serviceParent.edit') : $t('servicesPremium.serviceParent.add') }}
        </h3>
      </template>
      <template #body>
        <ServiceParentForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('servicesPremium.serviceParent.deleteTitle')"
      :description="$t('servicesPremium.serviceParent.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
