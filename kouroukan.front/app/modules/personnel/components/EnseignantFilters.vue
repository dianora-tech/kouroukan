<script setup lang="ts">
import type { EnseignantFilters } from '../types/enseignant.types'
import { STATUTS_ENSEIGNANT, MODES_REMUNERATION } from '../types/enseignant.types'
import { useEnseignant } from '../composables/useEnseignant'

const emit = defineEmits<{
  (e: 'filter', filters: EnseignantFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const { typeOptions } = useEnseignant()

const filters = reactive<EnseignantFilters>({
  search: '',
  typeId: undefined,
  statutEnseignant: undefined,
  modeRemuneration: undefined,
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
  filters.statutEnseignant = undefined
  filters.modeRemuneration = undefined
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

const allTypeOptions = computed(() => [
  { label: t('personnel.enseignant.filters.allTypes'), value: '' },
  ...typeOptions.value,
])

const statutOptions = [
  { label: t('personnel.enseignant.filters.allStatuts'), value: '' },
  ...STATUTS_ENSEIGNANT.map(s => ({
    label: t(`personnel.enseignant.statut.${s}`),
    value: s,
  })),
]

const modeRemunerationFilterOptions = [
  { label: t('personnel.enseignant.filters.allModes'), value: '' },
  ...MODES_REMUNERATION.map(m => ({
    label: t(`personnel.enseignant.modeRemuneration.${m}`),
    value: m,
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
      v-model="filters.statutEnseignant"
      :items="statutOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.modeRemuneration"
      :items="modeRemunerationFilterOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <UInput
      v-model="filters.dateFrom"
      type="date"
      :placeholder="$t('personnel.enseignant.filters.dateFrom')"
      class="w-40"
      @change="onFilterChange"
    />

    <UInput
      v-model="filters.dateTo"
      type="date"
      :placeholder="$t('personnel.enseignant.filters.dateTo')"
      class="w-40"
      @change="onFilterChange"
    />

    <UButton variant="ghost" size="sm" icon="i-heroicons-x-mark" @click="resetFilters">
      {{ $t('personnel.enseignant.filters.reset') }}
    </UButton>
  </div>
</template>
