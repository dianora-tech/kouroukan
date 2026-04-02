<script setup lang="ts">
import type { AnnonceFilters } from '../types/annonce.types'
import { CIBLES_AUDIENCE } from '../types/annonce.types'

const emit = defineEmits<{
  (e: 'filter', filters: AnnonceFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()

const filters = reactive<AnnonceFilters>({
  search: '',
  estActive: undefined,
  cibleAudience: undefined,
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
  filters.estActive = undefined
  filters.cibleAudience = undefined
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

const statutOptions = [
  { label: t('communication.annonce.filters.allStatuts'), value: '' },
  { label: t('communication.annonce.active'), value: 'true' },
  { label: t('communication.annonce.inactive'), value: 'false' },
]

const cibleOptions = [
  { label: t('communication.annonce.filters.allCibles'), value: '' },
  ...CIBLES_AUDIENCE.map(c => ({
    label: t(`communication.annonce.cible.${c}`),
    value: c,
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
      v-model="filters.estActive"
      :items="statutOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.cibleAudience"
      :items="cibleOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <UInput
      v-model="filters.dateFrom"
      type="date"
      :placeholder="$t('communication.annonce.filters.dateFrom')"
      class="w-40"
      @change="onFilterChange"
    />

    <UInput
      v-model="filters.dateTo"
      type="date"
      :placeholder="$t('communication.annonce.filters.dateTo')"
      class="w-40"
      @change="onFilterChange"
    />

    <UButton
      variant="ghost"
      size="sm"
      icon="i-heroicons-x-mark"
      @click="resetFilters"
    >
      {{ $t('communication.annonce.filters.reset') }}
    </UButton>
  </div>
</template>
