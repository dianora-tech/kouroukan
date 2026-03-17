<script setup lang="ts">
import type { FactureFilters } from '../types/facture.types'
import { STATUTS_FACTURE } from '../types/facture.types'
import { useFacture } from '../composables/useFacture'

const emit = defineEmits<{
  (e: 'filter', filters: FactureFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const { typeOptions } = useFacture()

const filters = reactive<FactureFilters>({
  search: '',
  typeId: undefined,
  statutFacture: undefined,
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
  filters.statutFacture = undefined
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

const statutOptions = [
  { label: t('finances.facture.filters.allStatuts'), value: '' },
  ...STATUTS_FACTURE.map(s => ({
    label: t(`finances.facture.statut.${s}`),
    value: s,
  })),
]

const allTypeOptions = computed(() => [
  { label: t('finances.facture.filters.allTypes'), value: '' },
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
      v-model="filters.statutFacture"
      :items="statutOptions"
      value-key="value"
      class="w-36"
      @update:model-value="onFilterChange"
    />

    <UInput
      v-model="filters.dateFrom"
      type="date"
      :placeholder="t('finances.facture.filters.dateFrom')"
      class="w-40"
      @change="onFilterChange"
    />

    <UInput
      v-model="filters.dateTo"
      type="date"
      :placeholder="t('finances.facture.filters.dateTo')"
      class="w-40"
      @change="onFilterChange"
    />

    <UButton variant="ghost" size="sm" icon="i-heroicons-x-mark" @click="resetFilters">
      {{ $t('finances.facture.filters.reset') }}
    </UButton>
  </div>
</template>
