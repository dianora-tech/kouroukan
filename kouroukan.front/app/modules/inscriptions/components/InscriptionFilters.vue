<script setup lang="ts">
import type { InscriptionFilters } from '../types/inscription.types'
import { STATUTS_INSCRIPTION, TYPES_ETABLISSEMENT } from '../types/inscription.types'
import { useInscription } from '../composables/useInscription'

const emit = defineEmits<{
  (e: 'filter', filters: InscriptionFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const { typeOptions } = useInscription()

const filters = reactive<InscriptionFilters>({
  search: '',
  typeId: undefined,
  statutInscription: undefined,
  estPaye: undefined,
  typeEtablissement: undefined,
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
  filters.statutInscription = undefined
  filters.estPaye = undefined
  filters.typeEtablissement = undefined
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

const statutOptions = [
  { label: t('inscriptions.inscription.filters.allStatuts'), value: '' },
  ...STATUTS_INSCRIPTION.map(s => ({
    label: t(`inscriptions.inscription.statut.${s}`),
    value: s,
  })),
]

const typeEtabOptions = [
  { label: t('inscriptions.inscription.filters.allTypes'), value: '' },
  ...TYPES_ETABLISSEMENT.map(te => ({
    label: t(`inscriptions.inscription.typeEtab.${te}`),
    value: te,
  })),
]

const payeOptions = [
  { label: t('inscriptions.inscription.filters.allPaiements'), value: '' },
  { label: t('inscriptions.inscription.paye'), value: 'true' },
  { label: t('inscriptions.inscription.nonPaye'), value: 'false' },
]

const allTypeOptions = computed(() => [
  { label: t('inscriptions.inscription.filters.allTypeInscription'), value: '' },
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
      v-model="filters.statutInscription"
      :items="statutOptions"
      value-key="value"
      class="w-36"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.typeEtablissement"
      :items="typeEtabOptions"
      value-key="value"
      class="w-44"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.estPaye"
      :items="payeOptions"
      value-key="value"
      class="w-36"
      @update:model-value="onFilterChange"
    />

    <UButton
      variant="ghost"
      size="sm"
      icon="i-heroicons-x-mark"
      @click="resetFilters"
    >
      {{ $t('inscriptions.inscription.filters.reset') }}
    </UButton>
  </div>
</template>
