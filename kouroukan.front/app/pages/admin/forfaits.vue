<script setup lang="ts">
import { z } from 'zod'
import type { Column } from '~/shared/components/DataTable.vue'
import { useAdminForfait } from '~/modules/admin/composables/useAdminForfait'
import { useAdminGesteCommercial } from '~/modules/admin/composables/useAdminGesteCommercial'
import type { Forfait, GesteCommercial } from '~/modules/admin/types/admin.types'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { formatDate } = useFormatDate()

const {
  items: forfaits,
  loading: forfaitLoading,
  saving: forfaitSaving,
  paginatedData: forfaitPaginatedData,
  fetchAll: fetchForfaits,
  update: updateForfait,
  updateTarif,
  remove: removeForfait,
  changePage: changeForfaitPage,
} = useAdminForfait()

const {
  items: gestes,
  loading: gesteLoading,
  paginatedData: gestePaginatedData,
  fetchAll: fetchGestes,
  changePage: changeGestePage,
} = useAdminGesteCommercial()

const showForm = ref(false)
const editingEntity = ref<Forfait | null>(null)
const activeTab = ref('forfaits')

const tarifSchema = z.object({
  montantMensuel: z.number().min(0, t('validation.required')),
  montantAnnuel: z.number().min(0, t('validation.required')),
  dateEffet: z.string().min(1, t('validation.required')),
})

const tarifState = reactive({
  montantMensuel: 0,
  montantAnnuel: 0,
  dateEffet: '',
})

const formSaving = ref(false)

watch(() => editingEntity.value, (entity) => {
  if (entity) {
    tarifState.montantMensuel = entity.montantMensuel ?? 0
    tarifState.montantAnnuel = entity.montantAnnuel ?? 0
    tarifState.dateEffet = entity.dateEffet ?? ''
  }
  else {
    tarifState.montantMensuel = 0
    tarifState.montantAnnuel = 0
    tarifState.dateEffet = ''
  }
}, { immediate: true })

async function handleTarifSubmit(): Promise<void> {
  if (!editingEntity.value) return
  formSaving.value = true
  try {
    const result = await updateTarif({
      forfaitId: editingEntity.value.id,
      montantMensuel: tarifState.montantMensuel,
      montantAnnuel: tarifState.montantAnnuel,
    })
    if (result) {
      showForm.value = false
    }
  }
  finally {
    formSaving.value = false
  }
}

const forfaitColumns: Column[] = [
  { key: 'nom', label: t('admin.forfait.nom'), sortable: true },
  { key: 'type', label: t('admin.forfait.type'), sortable: true },
  { key: 'montantMensuel', label: t('admin.forfait.montantMensuel'), sortable: true },
  { key: 'montantAnnuel', label: t('admin.forfait.montantAnnuel'), sortable: true },
  { key: 'dateEffet', label: t('admin.forfait.dateEffet'), sortable: true, render: (row: any) => formatDate(row.dateEffet) },
  { key: 'statut', label: t('admin.forfait.statut'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

const gesteColumns: Column[] = [
  { key: 'beneficiaire', label: t('admin.forfait.beneficiaire'), sortable: true },
  { key: 'type', label: t('admin.forfait.typeGeste'), sortable: true },
  { key: 'montant', label: t('admin.forfait.montant'), sortable: true },
  { key: 'raison', label: t('admin.forfait.raison'), sortable: false },
  { key: 'dateCreation', label: t('admin.forfait.dateCreation'), sortable: true, render: (row: any) => formatDate(row.dateCreation) },
]

onMounted(() => {
  fetchForfaits()
  fetchGestes()
})

function openEdit(entity: Forfait): void {
  editingEntity.value = entity
  showForm.value = true
}

function formatMontant(montant: number): string {
  return montant.toLocaleString('fr-GN') + ' GNF'
}
</script>

<template>
  <div class="space-y-6">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('admin.title'), to: '/admin' },
            { label: $t('admin.forfait.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('admin.forfait.title') }}
        </h1>
      </div>
    </div>

    <!-- Tabs -->
    <div class="flex gap-2 border-b border-gray-200 dark:border-gray-700">
      <button
        class="px-4 py-2 text-sm font-medium"
        :class="activeTab === 'forfaits' ? 'border-b-2 border-primary-500 text-primary-600' : 'text-gray-500'"
        @click="activeTab = 'forfaits'"
      >
        {{ $t('admin.forfait.tabForfaits') }}
      </button>
      <button
        class="px-4 py-2 text-sm font-medium"
        :class="activeTab === 'gestes' ? 'border-b-2 border-primary-500 text-primary-600' : 'text-gray-500'"
        @click="activeTab = 'gestes'"
      >
        {{ $t('admin.forfait.tabGestes') }}
      </button>
    </div>

    <!-- Forfaits Table -->
    <DataTable
      v-if="activeTab === 'forfaits'"
      :columns="forfaitColumns"
      :data="forfaitPaginatedData"
      :loading="forfaitLoading"
      @page-change="changeForfaitPage"
    >
      <template #cell-montantMensuel="{ row }">
        {{ formatMontant((row as Forfait).montantMensuel) }}
      </template>
      <template #cell-montantAnnuel="{ row }">
        {{ formatMontant((row as Forfait).montantAnnuel) }}
      </template>
      <template #cell-type="{ row }">
        <UBadge variant="subtle" size="sm">{{ (row as Forfait).type }}</UBadge>
      </template>
      <template #cell-statut="{ row }">
        <UBadge :color="(row as Forfait).statut === 'Actif' ? 'success' : 'neutral'" variant="subtle" size="sm">
          {{ (row as Forfait).statut }}
        </UBadge>
      </template>
      <template #cell-actions="{ row }">
        <UButton variant="ghost" size="xs" icon="i-heroicons-pencil-square" @click="openEdit(row as Forfait)" />
      </template>
    </DataTable>

    <!-- Gestes Commerciaux Table -->
    <DataTable
      v-if="activeTab === 'gestes'"
      :columns="gesteColumns"
      :data="gestePaginatedData"
      :loading="gesteLoading"
      @page-change="changeGestePage"
    >
      <template #cell-montant="{ row }">
        {{ formatMontant((row as GesteCommercial).montant) }}
      </template>
    </DataTable>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">{{ $t('admin.forfait.edit') }}</h3>
      </template>
      <template #body>
        <UForm :schema="tarifSchema" :state="tarifState" class="space-y-4 p-4" @submit="handleTarifSubmit">
          <UFormField :label="$t('admin.forfait.montantMensuel')" name="montantMensuel" required>
            <UInput v-model.number="tarifState.montantMensuel" type="number" class="w-full" />
          </UFormField>
          <UFormField :label="$t('admin.forfait.montantAnnuel')" name="montantAnnuel" required>
            <UInput v-model.number="tarifState.montantAnnuel" type="number" class="w-full" />
          </UFormField>
          <UFormField :label="$t('admin.forfait.dateEffet')" name="dateEffet" required>
            <UInput v-model="tarifState.dateEffet" type="date" class="w-full" />
          </UFormField>
          <div class="flex justify-end gap-3 pt-4">
            <UButton variant="outline" @click="showForm = false">
              {{ $t('actions.cancel') }}
            </UButton>
            <UButton type="submit" color="primary" :loading="formSaving">
              {{ $t('actions.save') }}
            </UButton>
          </div>
        </UForm>
      </template>
    </USlideover>
  </div>
</template>
