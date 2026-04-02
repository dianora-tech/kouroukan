<script setup lang="ts">
import type { EvenementFilters } from '../types/evenement.types'
import { STATUTS_EVENEMENT } from '../types/evenement.types'
import { useEvenement } from '../composables/useEvenement'
import { useAssociation } from '../composables/useAssociation'

const emit = defineEmits<{
  (e: 'filter', filters: EvenementFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const { typeOptions } = useEvenement()
const { items: associations } = useAssociation()

const filters = reactive<EvenementFilters>({
  search: '',
  typeId: undefined,
  associationId: undefined,
  statutEvenement: undefined,
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
  filters.associationId = undefined
  filters.statutEvenement = undefined
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

const statutOptions = [
  { label: t('bde.evenement.filters.allStatuts'), value: '' },
  ...STATUTS_EVENEMENT.map(s => ({
    label: t(`bde.evenement.statut.${s}`),
    value: s,
  })),
]

const allTypeOptions = computed(() => [
  { label: t('bde.evenement.filters.allTypes'), value: '' },
  ...typeOptions.value,
])

const associationOptions = computed(() => [
  { label: t('bde.evenement.filters.allAssociations'), value: '' },
  ...associations.value.map(a => ({ label: a.name, value: a.id })),
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
      class="w-44"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.associationId"
      :items="associationOptions"
      value-key="value"
      class="w-48"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.statutEvenement"
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
      {{ $t('bde.evenement.filters.reset') }}
    </UButton>
  </div>
</template>
