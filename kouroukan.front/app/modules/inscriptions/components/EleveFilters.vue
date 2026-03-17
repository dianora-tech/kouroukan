<script setup lang="ts">
import type { EleveFilters } from '../types/eleve.types'
import { STATUTS_INSCRIPTION_ELEVE, GENRES } from '../types/eleve.types'

const emit = defineEmits<{
  (e: 'filter', filters: EleveFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()

const filters = reactive<EleveFilters>({
  search: '',
  statutInscription: undefined,
  genre: undefined,
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
  filters.statutInscription = undefined
  filters.genre = undefined
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

const statutOptions = [
  { label: t('inscriptions.eleve.filters.allStatuts'), value: '' },
  ...STATUTS_INSCRIPTION_ELEVE.map(s => ({
    label: t(`inscriptions.eleve.statut.${s}`),
    value: s,
  })),
]

const genreOptions = [
  { label: t('inscriptions.eleve.filters.allGenres'), value: '' },
  ...GENRES.map(g => ({
    label: t(`inscriptions.eleve.genre${g}`),
    value: g,
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
      v-model="filters.statutInscription"
      :items="statutOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.genre"
      :items="genreOptions"
      value-key="value"
      class="w-32"
      @update:model-value="onFilterChange"
    />

    <UInput
      v-model="filters.dateFrom"
      type="date"
      :placeholder="$t('inscriptions.eleve.filters.dateFrom')"
      class="w-40"
      @change="onFilterChange"
    />

    <UInput
      v-model="filters.dateTo"
      type="date"
      :placeholder="$t('inscriptions.eleve.filters.dateTo')"
      class="w-40"
      @change="onFilterChange"
    />

    <UButton variant="ghost" size="sm" icon="i-heroicons-x-mark" @click="resetFilters">
      {{ $t('inscriptions.eleve.filters.reset') }}
    </UButton>
  </div>
</template>
