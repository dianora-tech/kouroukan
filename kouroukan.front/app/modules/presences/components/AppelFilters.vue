<script setup lang="ts">
import type { AppelFilters } from '../types/appel.types'

const emit = defineEmits<{
  (e: 'filter', filters: AppelFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()

const filters = reactive<AppelFilters>({
  search: '',
  estCloture: undefined,
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
  filters.estCloture = undefined
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

const statutOptions = [
  { label: t('presences.appel.filters.allStatuts'), value: '' },
  { label: t('presences.appel.cloture'), value: true },
  { label: t('presences.appel.enCours'), value: false },
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
      v-model="filters.estCloture"
      :items="statutOptions"
      value-key="value"
      class="w-48"
      @update:model-value="onFilterChange"
    />

    <UInput
      v-model="filters.dateFrom"
      type="date"
      :placeholder="t('presences.appel.filters.dateFrom')"
      class="w-40"
      @change="onFilterChange"
    />

    <UInput
      v-model="filters.dateTo"
      type="date"
      :placeholder="t('presences.appel.filters.dateTo')"
      class="w-40"
      @change="onFilterChange"
    />

    <UButton
      variant="ghost"
      size="sm"
      icon="i-heroicons-x-mark"
      @click="resetFilters"
    >
      {{ $t('presences.appel.filters.reset') }}
    </UButton>
  </div>
</template>
