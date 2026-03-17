<script setup lang="ts">
import type { DepenseBDEFilters } from '../types/depense-bde.types'
import { STATUTS_VALIDATION_BDE, CATEGORIES_DEPENSE_BDE } from '../types/depense-bde.types'
import { useDepenseBDE } from '../composables/useDepenseBDE'
import { useAssociation } from '../composables/useAssociation'

const emit = defineEmits<{
  (e: 'filter', filters: DepenseBDEFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const { typeOptions } = useDepenseBDE()
const { items: associations } = useAssociation()

const filters = reactive<DepenseBDEFilters>({
  search: '',
  typeId: undefined,
  associationId: undefined,
  categorie: undefined,
  statutValidation: undefined,
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
  filters.categorie = undefined
  filters.statutValidation = undefined
  emit('reset')
}

const statutOptions = [
  { label: t('bde.depenseBde.filters.allStatuts'), value: '' },
  ...STATUTS_VALIDATION_BDE.map(s => ({
    label: t(`bde.depenseBde.statut.${s}`),
    value: s,
  })),
]

const categorieOptions = [
  { label: t('bde.depenseBde.filters.allCategories'), value: '' },
  ...CATEGORIES_DEPENSE_BDE.map(c => ({
    label: t(`bde.depenseBde.categorie.${c}`),
    value: c,
  })),
]

const allTypeOptions = computed(() => [
  { label: t('bde.depenseBde.filters.allTypes'), value: '' },
  ...typeOptions.value,
])

const associationOptions = computed(() => [
  { label: t('bde.depenseBde.filters.allAssociations'), value: '' },
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
      v-model="filters.categorie"
      :items="categorieOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.statutValidation"
      :items="statutOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <UButton variant="ghost" size="sm" icon="i-heroicons-x-mark" @click="resetFilters">
      {{ $t('bde.depenseBde.filters.reset') }}
    </UButton>
  </div>
</template>
