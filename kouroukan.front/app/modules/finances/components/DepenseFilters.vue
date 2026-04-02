<script setup lang="ts">
import type { DepenseFilters } from '../types/depense.types'
import { STATUTS_DEPENSE, CATEGORIES_DEPENSE } from '../types/depense.types'
import { useDepense } from '../composables/useDepense'

const emit = defineEmits<{
  (e: 'filter', filters: DepenseFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const { typeOptions } = useDepense()

const filters = reactive<DepenseFilters>({
  search: '',
  typeId: undefined,
  categorie: undefined,
  statutDepense: undefined,
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
  filters.categorie = undefined
  filters.statutDepense = undefined
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

const statutOptions = [
  { label: t('finances.depense.filters.allStatuts'), value: '' },
  ...STATUTS_DEPENSE.map(s => ({
    label: t(`finances.depense.statut.${s}`),
    value: s,
  })),
]

const categorieOptions = [
  { label: t('finances.depense.filters.allCategories'), value: '' },
  ...CATEGORIES_DEPENSE.map(c => ({
    label: t(`finances.depense.categorie.${c}`),
    value: c,
  })),
]

const allTypeOptions = computed(() => [
  { label: t('finances.depense.filters.allTypes'), value: '' },
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
      v-model="filters.categorie"
      :items="categorieOptions"
      value-key="value"
      class="w-44"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.statutDepense"
      :items="statutOptions"
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
      {{ $t('finances.depense.filters.reset') }}
    </UButton>
  </div>
</template>
