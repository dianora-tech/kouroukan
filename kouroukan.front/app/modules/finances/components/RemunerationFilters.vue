<script setup lang="ts">
import type { RemunerationFilters } from '../types/remuneration.types'
import { STATUTS_REMUNERATION, MODES_REMUNERATION, MOIS_OPTIONS } from '../types/remuneration.types'

const emit = defineEmits<{
  (e: 'filter', filters: RemunerationFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()

const currentYear = new Date().getFullYear()

const filters = reactive<RemunerationFilters>({
  search: '',
  mois: undefined,
  annee: undefined,
  modeRemuneration: undefined,
  statutPaiement: undefined,
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
  filters.mois = undefined
  filters.annee = undefined
  filters.modeRemuneration = undefined
  filters.statutPaiement = undefined
  emit('reset')
}

const statutOptions = [
  { label: t('finances.remuneration.filters.allStatuts'), value: '' },
  ...STATUTS_REMUNERATION.map(s => ({
    label: t(`finances.remuneration.statut.${s}`),
    value: s,
  })),
]

const modeOptions = [
  { label: t('finances.remuneration.filters.allModes'), value: '' },
  ...MODES_REMUNERATION.map(m => ({
    label: t(`finances.remuneration.mode.${m}`),
    value: m,
  })),
]

const moisFilterOptions = [
  { label: t('finances.remuneration.filters.allMois'), value: '' },
  ...MOIS_OPTIONS.map(m => ({
    label: t(`finances.remuneration.mois.${m.value}`),
    value: m.value,
  })),
]

const anneeOptions = [
  { label: t('finances.remuneration.filters.allAnnees'), value: '' },
  ...Array.from({ length: 4 }, (_, i) => ({
    label: String(currentYear - 1 + i),
    value: currentYear - 1 + i,
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
      v-model="filters.mois"
      :items="moisFilterOptions"
      value-key="value"
      class="w-36"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.annee"
      :items="anneeOptions"
      value-key="value"
      class="w-28"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.modeRemuneration"
      :items="modeOptions"
      value-key="value"
      class="w-36"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.statutPaiement"
      :items="statutOptions"
      value-key="value"
      class="w-36"
      @update:model-value="onFilterChange"
    />

    <UButton variant="ghost" size="sm" icon="i-heroicons-x-mark" @click="resetFilters">
      {{ $t('finances.remuneration.filters.reset') }}
    </UButton>
  </div>
</template>
