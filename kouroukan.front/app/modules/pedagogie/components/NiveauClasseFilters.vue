<script setup lang="ts">
import type { NiveauClasseFilters } from '../types/niveauClasse.types'
import { CYCLES_ETUDE, MINISTERES_TUTELLE } from '../types/niveauClasse.types'

const emit = defineEmits<{
  (e: 'filter', filters: NiveauClasseFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()

const filters = reactive<NiveauClasseFilters>({
  search: '',
  cycleEtude: undefined,
  ministereTutelle: undefined,
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
  filters.cycleEtude = undefined
  filters.ministereTutelle = undefined
  emit('reset')
}

const cycleOptions = [
  { label: t('pedagogie.niveauClasse.filters.allCycles'), value: '' },
  ...CYCLES_ETUDE.map(c => ({
    label: t(`pedagogie.niveauClasse.cycle.${c}`),
    value: c,
  })),
]

const ministereOptions = [
  { label: t('pedagogie.niveauClasse.filters.allMinisteres'), value: '' },
  ...MINISTERES_TUTELLE.map(m => ({
    label: m,
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
      v-model="filters.cycleEtude"
      :items="cycleOptions"
      value-key="value"
      class="w-48"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.ministereTutelle"
      :items="ministereOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <UButton
      variant="ghost"
      size="sm"
      icon="i-heroicons-x-mark"
      @click="resetFilters"
    >
      {{ $t('pedagogie.niveauClasse.filters.reset') }}
    </UButton>
  </div>
</template>
