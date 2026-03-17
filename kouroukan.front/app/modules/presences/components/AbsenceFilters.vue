<script setup lang="ts">
import type { AbsenceFilters } from '../types/absence.types'
import { useAbsence } from '../composables/useAbsence'

const emit = defineEmits<{
  (e: 'filter', filters: AbsenceFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const { typeOptions } = useAbsence()

const filters = reactive<AbsenceFilters>({
  search: '',
  typeId: undefined,
  estJustifiee: undefined,
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
  filters.estJustifiee = undefined
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

const allTypeOptions = computed(() => [
  { label: t('presences.absence.filters.allTypes'), value: '' },
  ...typeOptions.value,
])

const justificationOptions = [
  { label: t('presences.absence.filters.allJustifications'), value: '' },
  { label: t('presences.absence.justifiee'), value: true },
  { label: t('presences.absence.nonJustifiee'), value: false },
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
      v-model="filters.typeId"
      :items="allTypeOptions"
      value-key="value"
      class="w-48"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.estJustifiee"
      :items="justificationOptions"
      value-key="value"
      class="w-44"
      @update:model-value="onFilterChange"
    />

    <UInput
      v-model="filters.dateFrom"
      type="date"
      :placeholder="t('presences.absence.filters.dateFrom')"
      class="w-40"
      @change="onFilterChange"
    />

    <UInput
      v-model="filters.dateTo"
      type="date"
      :placeholder="t('presences.absence.filters.dateTo')"
      class="w-40"
      @change="onFilterChange"
    />

    <UButton variant="ghost" size="sm" icon="i-heroicons-x-mark" @click="resetFilters">
      {{ $t('presences.absence.filters.reset') }}
    </UButton>
  </div>
</template>
