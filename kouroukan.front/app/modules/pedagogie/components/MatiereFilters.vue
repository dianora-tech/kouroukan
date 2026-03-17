<script setup lang="ts">
import type { MatiereFilters } from '../types/matiere.types'
import { useMatiereStore } from '../stores/matiere.store'
import { useNiveauClasseStore } from '../stores/niveauClasse.store'

const emit = defineEmits<{
  (e: 'filter', filters: MatiereFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const matiereStore = useMatiereStore()
const niveauClasseStore = useNiveauClasseStore()

const filters = reactive<MatiereFilters>({
  search: '',
  typeId: undefined,
  niveauClasseId: undefined,
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
  filters.niveauClasseId = undefined
  emit('reset')
}

const typeOptions = computed(() => [
  { label: t('pedagogie.matiere.filters.allTypes'), value: '' },
  ...matiereStore.types.map(tp => ({
    label: tp.name,
    value: tp.id,
  })),
])

const niveauClasseOptions = computed(() => [
  { label: t('pedagogie.matiere.filters.allNiveaux'), value: '' },
  ...niveauClasseStore.items.map(n => ({
    label: `${n.code} - ${n.name}`,
    value: n.id,
  })),
])

onMounted(() => {
  if (matiereStore.types.length === 0) matiereStore.fetchTypes()
  if (niveauClasseStore.items.length === 0) niveauClasseStore.fetchAll({ pageSize: 100 })
})
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
      :items="typeOptions"
      value-key="value"
      class="w-44"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.niveauClasseId"
      :items="niveauClasseOptions"
      value-key="value"
      class="w-48"
      @update:model-value="onFilterChange"
    />

    <UButton variant="ghost" size="sm" icon="i-heroicons-x-mark" @click="resetFilters">
      {{ $t('pedagogie.matiere.filters.reset') }}
    </UButton>
  </div>
</template>
