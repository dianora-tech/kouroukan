<script setup lang="ts">
import type { MembreBDEFilters } from '../types/membre-bde.types'
import { ROLES_BDE } from '../types/membre-bde.types'
import { useAssociation } from '../composables/useAssociation'

const emit = defineEmits<{
  (e: 'filter', filters: MembreBDEFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const { items: associations } = useAssociation()

const filters = reactive<MembreBDEFilters>({
  search: '',
  associationId: undefined,
  roleBDE: undefined,
  estActif: undefined,
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
  filters.associationId = undefined
  filters.roleBDE = undefined
  filters.estActif = undefined
  emit('reset')
}

const roleOptions = [
  { label: t('bde.membreBde.filters.allRoles'), value: '' },
  ...ROLES_BDE.map(r => ({
    label: t(`bde.membreBde.role.${r}`),
    value: r,
  })),
]

const associationOptions = computed(() => [
  { label: t('bde.membreBde.filters.allAssociations'), value: '' },
  ...associations.value.map(a => ({ label: a.name, value: a.id })),
])

const statutOptions = [
  { label: t('bde.membreBde.filters.allStatuts'), value: '' },
  { label: t('bde.membreBde.actif'), value: 'true' },
  { label: t('bde.membreBde.inactif'), value: 'false' },
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
      v-model="filters.associationId"
      :items="associationOptions"
      value-key="value"
      class="w-48"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.roleBDE"
      :items="roleOptions"
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
      {{ $t('bde.membreBde.filters.reset') }}
    </UButton>
  </div>
</template>
