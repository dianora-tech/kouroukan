<script setup lang="ts">
import type { AssociationFilters } from '../types/association.types'
import { STATUTS_ASSOCIATION } from '../types/association.types'
import { useAssociation } from '../composables/useAssociation'

const emit = defineEmits<{
  (e: 'filter', filters: AssociationFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const { typeOptions } = useAssociation()

const filters = reactive<AssociationFilters>({
  search: '',
  typeId: undefined,
  statut: undefined,
  anneeScolaire: undefined,
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
  filters.statut = undefined
  filters.anneeScolaire = undefined
  emit('reset')
}

const statutOptions = [
  { label: t('bde.association.filters.allStatuts'), value: '' },
  ...STATUTS_ASSOCIATION.map(s => ({
    label: t(`bde.association.statut.${s}`),
    value: s,
  })),
]

const allTypeOptions = computed(() => [
  { label: t('bde.association.filters.allTypes'), value: '' },
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
      v-model="filters.statut"
      :items="statutOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <UButton variant="ghost" size="sm" icon="i-heroicons-x-mark" @click="resetFilters">
      {{ $t('bde.association.filters.reset') }}
    </UButton>
  </div>
</template>
