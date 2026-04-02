<script setup lang="ts">
import type { ClasseFilters } from '../types/classe.types'
import { useNiveauClasseStore } from '../stores/niveauClasse.store'

const emit = defineEmits<{
  (e: 'filter', filters: ClasseFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const niveauClasseStore = useNiveauClasseStore()

const filters = reactive<ClasseFilters>({
  search: '',
  niveauClasseId: undefined,
  anneeScolaireId: undefined,
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
  filters.niveauClasseId = undefined
  filters.anneeScolaireId = undefined
  emit('reset')
}

const niveauClasseOptions = computed(() => [
  { label: t('pedagogie.classe.filters.allNiveaux'), value: '' },
  ...niveauClasseStore.items.map(n => ({
    label: `${n.code} - ${n.name}`,
    value: n.id,
  })),
])

onMounted(() => {
  if (niveauClasseStore.items.length === 0) {
    niveauClasseStore.fetchAll({ pageSize: 100 })
  }
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
      v-model="filters.niveauClasseId"
      :items="niveauClasseOptions"
      value-key="value"
      class="w-48"
      @update:model-value="onFilterChange"
    />

    <UButton
      variant="ghost"
      size="sm"
      icon="i-heroicons-x-mark"
      @click="resetFilters"
    >
      {{ $t('pedagogie.classe.filters.reset') }}
    </UButton>
  </div>
</template>
