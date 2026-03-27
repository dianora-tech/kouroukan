<script setup lang="ts">
import type { AnneeScolaireFilters } from '../types/annee-scolaire.types'

const emit = defineEmits<{
  (e: 'filter', filters: AnneeScolaireFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()

const filters = reactive<AnneeScolaireFilters>({
  search: '',
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
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

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

    <UButton variant="ghost" size="sm" icon="i-heroicons-x-mark" @click="resetFilters">
      {{ $t('inscriptions.anneeScolaire.filters.reset') }}
    </UButton>
  </div>
</template>
