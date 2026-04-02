<script setup lang="ts">
import type { DemandeCongeFilters } from '../types/demandeConge.types'
import { STATUTS_DEMANDE_CONGE } from '../types/demandeConge.types'
import { useDemandeConge } from '../composables/useDemandeConge'

const emit = defineEmits<{
  (e: 'filter', filters: DemandeCongeFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const { typeOptions } = useDemandeConge()

const filters = reactive<DemandeCongeFilters>({
  search: '',
  typeId: undefined,
  statutDemande: undefined,
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
  filters.statutDemande = undefined
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

const allTypeOptions = computed(() => [
  { label: t('personnel.demandeConge.filters.allTypes'), value: '' },
  ...typeOptions.value,
])

const statutOptions = [
  { label: t('personnel.demandeConge.filters.allStatuts'), value: '' },
  ...STATUTS_DEMANDE_CONGE.map(s => ({
    label: t(`personnel.demandeConge.statut.${s}`),
    value: s,
  })),
]
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
      v-model="filters.statutDemande"
      :items="statutOptions"
      value-key="value"
      class="w-44"
      @update:model-value="onFilterChange"
    />

    <UInput
      v-model="filters.dateFrom"
      type="date"
      :placeholder="$t('personnel.demandeConge.filters.dateFrom')"
      class="w-40"
      @change="onFilterChange"
    />

    <UInput
      v-model="filters.dateTo"
      type="date"
      :placeholder="$t('personnel.demandeConge.filters.dateTo')"
      class="w-40"
      @change="onFilterChange"
    />

    <UButton
      variant="ghost"
      size="sm"
      icon="i-heroicons-x-mark"
      @click="resetFilters"
    >
      {{ $t('personnel.demandeConge.filters.reset') }}
    </UButton>
  </div>
</template>
