<script setup lang="ts">
import type { SeanceFilters } from '../types/seance.types'
import { JOURS_SEMAINE } from '../types/seance.types'
import { useClasseStore } from '../stores/classe.store'

const emit = defineEmits<{
  (e: 'filter', filters: SeanceFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const classeStore = useClasseStore()

const filters = reactive<SeanceFilters>({
  search: '',
  classeId: undefined,
  jourSemaine: undefined,
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
  filters.classeId = undefined
  filters.jourSemaine = undefined
  emit('reset')
}

const classeOptions = computed(() => [
  { label: t('pedagogie.seance.filters.allClasses'), value: '' },
  ...classeStore.items.map(c => ({
    label: c.name,
    value: c.id,
  })),
])

const jourOptions = [
  { label: t('pedagogie.seance.filters.allJours'), value: '' },
  ...JOURS_SEMAINE.map(j => ({
    label: t(`pedagogie.seance.jour.${j.value}`),
    value: j.value,
  })),
]

onMounted(() => {
  if (classeStore.items.length === 0) classeStore.fetchAll({ pageSize: 100 })
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
      v-model="filters.classeId"
      :items="classeOptions"
      value-key="value"
      class="w-44"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.jourSemaine"
      :items="jourOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <UButton variant="ghost" size="sm" icon="i-heroicons-x-mark" @click="resetFilters">
      {{ $t('pedagogie.seance.filters.reset') }}
    </UButton>
  </div>
</template>
