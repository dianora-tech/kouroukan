<script setup lang="ts">
import type { PaiementFilters } from '../types/paiement.types'
import { STATUTS_PAIEMENT, MOYENS_PAIEMENT } from '../types/paiement.types'
import { usePaiement } from '../composables/usePaiement'

const emit = defineEmits<{
  (e: 'filter', filters: PaiementFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const { typeOptions } = usePaiement()

const filters = reactive<PaiementFilters>({
  search: '',
  typeId: undefined,
  moyenPaiement: undefined,
  statutPaiement: undefined,
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
  filters.moyenPaiement = undefined
  filters.statutPaiement = undefined
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

const statutOptions = [
  { label: t('finances.paiement.filters.allStatuts'), value: '' },
  ...STATUTS_PAIEMENT.map(s => ({
    label: t(`finances.paiement.statut.${s}`),
    value: s,
  })),
]

const moyenOptions = [
  { label: t('finances.paiement.filters.allMoyens'), value: '' },
  ...MOYENS_PAIEMENT.map(m => ({
    label: t(`finances.paiement.moyen.${m}`),
    value: m,
  })),
]

const allTypeOptions = computed(() => [
  { label: t('finances.paiement.filters.allTypes'), value: '' },
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
      v-model="filters.moyenPaiement"
      :items="moyenOptions"
      value-key="value"
      class="w-44"
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
      {{ $t('finances.paiement.filters.reset') }}
    </UButton>
  </div>
</template>
