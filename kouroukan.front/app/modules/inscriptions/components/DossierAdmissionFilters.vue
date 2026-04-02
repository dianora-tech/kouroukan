<script setup lang="ts">
import type { DossierAdmissionFilters } from '../types/dossier-admission.types'
import { STATUTS_DOSSIER } from '../types/dossier-admission.types'
import { useDossierAdmission } from '../composables/useDossierAdmission'

const emit = defineEmits<{
  (e: 'filter', filters: DossierAdmissionFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const { typeOptions } = useDossierAdmission()

const filters = reactive<DossierAdmissionFilters>({
  search: '',
  typeId: undefined,
  statutDossier: undefined,
  dateFrom: undefined,
  dateTo: undefined,
})

let searchTimeout: ReturnType<typeof setTimeout> | null = null

function onSearchInput(): void {
  if (searchTimeout) clearTimeout(searchTimeout)
  searchTimeout = setTimeout(() => {
    emit('filter', { ...filters })
  }, 300)
}

function onFilterChange(): void {
  emit('filter', { ...filters })
}

function resetFilters(): void {
  filters.search = ''
  filters.typeId = undefined
  filters.statutDossier = undefined
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

const statutOptions = [
  { label: t('inscriptions.dossierAdmission.filters.allStatuts'), value: '' },
  ...STATUTS_DOSSIER.map(s => ({
    label: t(`inscriptions.dossierAdmission.statut.${s}`),
    value: s,
  })),
]

const allTypeOptions = computed(() => [
  { label: t('inscriptions.dossierAdmission.filters.allTypes'), value: '' },
  ...typeOptions.value,
])
</script>

<template>
  <div class="flex flex-wrap items-end gap-3">
    <div class="min-w-[200px] flex-1">
      <UInput
        v-model="filters.search"
        :placeholder="$t('actions.search')"
        icon="i-heroicons-magnifying-glass"
        class="w-full"
        @input="onSearchInput"
      />
    </div>

    <USelect
      v-model="filters.typeId"
      :items="allTypeOptions"
      value-key="value"
      class="w-48"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.statutDossier"
      :items="statutOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <UInput
      v-model="filters.dateFrom"
      type="date"
      class="w-40"
      @change="onFilterChange"
    />

    <UInput
      v-model="filters.dateTo"
      type="date"
      class="w-40"
      @change="onFilterChange"
    />

    <UButton
      variant="ghost"
      size="sm"
      icon="i-heroicons-x-mark"
      @click="resetFilters"
    >
      {{ $t('inscriptions.dossierAdmission.filters.reset') }}
    </UButton>
  </div>
</template>
