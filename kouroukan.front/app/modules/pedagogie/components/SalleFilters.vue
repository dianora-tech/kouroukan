<script setup lang="ts">
import type { SalleFilters } from '../types/salle.types'
import { useSalleStore } from '../stores/salle.store'

const emit = defineEmits<{
  (e: 'filter', filters: SalleFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const salleStore = useSalleStore()

const filters = reactive<SalleFilters>({
  search: '',
  typeId: undefined,
  estDisponible: undefined,
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
  filters.estDisponible = undefined
  emit('reset')
}

const typeOptions = computed(() => [
  { label: t('pedagogie.salle.filters.allTypes'), value: '' },
  ...salleStore.types.map(tp => ({
    label: tp.name,
    value: tp.id,
  })),
])

const disponibiliteOptions = [
  { label: t('pedagogie.salle.filters.allDisponibilite'), value: '' },
  { label: t('pedagogie.salle.disponible'), value: 'true' },
  { label: t('pedagogie.salle.indisponible'), value: 'false' },
]

onMounted(() => {
  if (salleStore.types.length === 0) salleStore.fetchTypes()
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
      v-model="filters.estDisponible"
      :items="disponibiliteOptions"
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
      {{ $t('pedagogie.salle.filters.reset') }}
    </UButton>
  </div>
</template>
