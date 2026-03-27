<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import type { AnneeScolaire, CreateAnneeScolairePayload, UpdateAnneeScolairePayload } from '~/modules/inscriptions/types/annee-scolaire.types'
import { useAnneeScolaire } from '~/modules/inscriptions/composables/useAnneeScolaire'
import type { AnneeScolaireFilters } from '~/modules/inscriptions/types/annee-scolaire.types'
import AnneeScolaireForm from '~/modules/inscriptions/components/AnneeScolaireForm.vue'
import AnneeScolaireCard from '~/modules/inscriptions/components/AnneeScolaireCard.vue'
import AnneeScolaireFiltersComponent from '~/modules/inscriptions/components/AnneeScolaireFilters.vue'
import AnneeScolaireStats from '~/modules/inscriptions/components/AnneeScolaireStats.vue'
import { useAuthStore } from '~/core/stores/auth.store'

definePageMeta({ layout: 'default' })

const { formatDateShort } = useFormatDate()
const auth = useAuthStore()
const isSuperAdmin = computed(() => auth.roles.includes('super_admin'))

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
} = useAnneeScolaire()

const viewMode = ref<'table' | 'grid'>('table')
const showForm = ref(false)
const editingEntity = ref<AnneeScolaire | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<AnneeScolaire | null>(null)

const statutColorMap: Record<string, string> = {
  preparation: 'warning',
  active: 'success',
  cloturee: 'info',
  archivee: 'neutral',
}

const statutLabelMap: Record<string, string> = {
  preparation: 'En préparation',
  active: 'Active',
  cloturee: 'Clôturée',
  archivee: 'Archivée',
}

const columns: Column[] = [
  { key: 'libelle', label: t('inscriptions.anneeScolaire.libelle'), sortable: true },
  { key: 'dateDebut', label: t('inscriptions.anneeScolaire.dateDebut'), sortable: true },
  { key: 'dateFin', label: t('inscriptions.anneeScolaire.dateFin'), sortable: true },
  { key: 'statut', label: 'Statut', sortable: true },
  { key: 'periodes', label: 'Périodes', sortable: false },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(() => {
  fetchAll()
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: AnneeScolaire): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: AnneeScolaire): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleSubmit(payload: CreateAnneeScolairePayload | UpdateAnneeScolairePayload): Promise<void> {
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

function handleFilter(filters: AnneeScolaireFilters): void {
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
            { label: $t('nav.inscriptions'), to: '/inscriptions' },
            { label: $t('inscriptions.anneeScolaire.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('inscriptions.anneeScolaire.title') }}
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
          v-if="isSuperAdmin"
          color="primary"
          icon="i-heroicons-plus"
          @click="openCreate"
        >
          {{ $t('inscriptions.anneeScolaire.add') }}
        </UButton>
      </div>
    </div>

    <AnneeScolaireStats :items="items" :total-count="pagination.totalCount" />

    <AnneeScolaireFiltersComponent @filter="handleFilter" @reset="resetFilters" />

    <template v-if="viewMode === 'table'">
      <DataTable
        v-if="!isEmpty || loading"
        :columns="columns"
        :data="paginatedData"
        :loading="loading"
        @page-change="changePage"
        @sort="handleSort"
      >
        <template #cell-dateDebut="{ row }">
          {{ formatDateShort((row as AnneeScolaire).dateDebut) }}
        </template>
        <template #cell-dateFin="{ row }">
          {{ formatDateShort((row as AnneeScolaire).dateFin) }}
        </template>
        <template #cell-statut="{ row }">
          <UBadge
            :color="statutColorMap[(row as AnneeScolaire).statut] ?? 'neutral'"
            variant="subtle"
            size="sm"
          >
            {{ statutLabelMap[(row as AnneeScolaire).statut] ?? (row as AnneeScolaire).statut }}
          </UBadge>
        </template>
        <template #cell-periodes="{ row }">
          {{ (row as AnneeScolaire).nombrePeriodes }} {{ (row as AnneeScolaire).typePeriode === 'semestre' ? 'Semestres' : 'Trimestres' }}
        </template>
        <template #cell-actions="{ row }">
          <div v-if="isSuperAdmin" class="flex gap-1">
            <UButton
              variant="ghost"
              size="xs"
              icon="i-heroicons-pencil-square"
              @click="openEdit(row as AnneeScolaire)"
            />
            <UButton
              variant="ghost"
              size="xs"
              color="error"
              icon="i-heroicons-trash"
              @click="openDelete(row as AnneeScolaire)"
            />
          </div>
        </template>
      </DataTable>
      <EmptyState
        v-else
        icon="i-heroicons-calendar-days"
        :title="$t('inscriptions.anneeScolaire.emptyTitle')"
        :description="$t('inscriptions.anneeScolaire.emptyDescription')"
        :action-label="$t('inscriptions.anneeScolaire.add')"
        @action="openCreate"
      />
    </template>

    <template v-else>
      <div v-if="!isEmpty" class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <AnneeScolaireCard
          v-for="annee in items"
          :key="annee.id"
          :annee="annee"
          @edit="openEdit"
          @delete="openDelete"
        />
      </div>
      <EmptyState
        v-else
        icon="i-heroicons-calendar-days"
        :title="$t('inscriptions.anneeScolaire.emptyTitle')"
        :description="$t('inscriptions.anneeScolaire.emptyDescription')"
        :action-label="$t('inscriptions.anneeScolaire.add')"
        @action="openCreate"
      />
    </template>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('inscriptions.anneeScolaire.edit') : $t('inscriptions.anneeScolaire.add') }}
        </h3>
      </template>
      <template #body>
        <AnneeScolaireForm
          :entity="editingEntity"
          @submit="handleSubmit"
          @cancel="showForm = false"
        />
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('inscriptions.anneeScolaire.deleteTitle')"
      :description="$t('inscriptions.anneeScolaire.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
