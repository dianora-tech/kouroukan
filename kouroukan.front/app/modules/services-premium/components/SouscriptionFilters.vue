<script setup lang="ts">
import type { SouscriptionFilters } from '../types/souscription.types'
import { STATUTS_SOUSCRIPTION } from '../types/souscription.types'

const emit = defineEmits<{
  (e: 'filter', filters: SouscriptionFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()

const filters = reactive<SouscriptionFilters>({
  search: '',
  serviceParentId: undefined,
  statutSouscription: undefined,
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
  filters.serviceParentId = undefined
  filters.statutSouscription = undefined
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

const statutOptions = [
  { label: t('servicesPremium.souscription.filters.allStatuts'), value: '' },
  ...STATUTS_SOUSCRIPTION.map(s => ({
    label: t(`servicesPremium.souscription.statut.${s}`),
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
      v-model="filters.statutSouscription"
      :items="statutOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <UInput
      v-model="filters.dateFrom"
      type="date"
      :placeholder="t('servicesPremium.souscription.filters.dateFrom')"
      class="w-40"
      @change="onFilterChange"
    />

    <UInput
      v-model="filters.dateTo"
      type="date"
      :placeholder="t('servicesPremium.souscription.filters.dateTo')"
      class="w-40"
      @change="onFilterChange"
    />

    <UButton
      variant="ghost"
      size="sm"
      icon="i-heroicons-x-mark"
      @click="resetFilters"
    >
      {{ $t('servicesPremium.souscription.filters.reset') }}
    </UButton>
  </div>
</template>
