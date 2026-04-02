<script setup lang="ts">
import type { NoteFilters } from '../types/note.types'
import { TRIMESTRES } from '../types/evaluation.types'

const emit = defineEmits<{
  (e: 'filter', filters: NoteFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()

const filters = reactive<NoteFilters>({
  search: '',
  evaluationId: undefined,
  classeId: undefined,
  matiereId: undefined,
  trimestre: undefined,
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
  filters.evaluationId = undefined
  filters.classeId = undefined
  filters.matiereId = undefined
  filters.trimestre = undefined
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

const trimestreOptions = [
  { label: t('evaluations.evaluation.filters.allTrimestres'), value: '' },
  ...TRIMESTRES.map(tr => ({
    label: t(`evaluations.evaluation.trimestre.${tr}`),
    value: tr,
  })),
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
      v-model="filters.trimestre"
      :items="trimestreOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <UInput
      v-model="filters.dateFrom"
      type="date"
      :placeholder="$t('evaluations.note.filters.dateFrom')"
      class="w-40"
      @change="onFilterChange"
    />

    <UInput
      v-model="filters.dateTo"
      type="date"
      :placeholder="$t('evaluations.note.filters.dateTo')"
      class="w-40"
      @change="onFilterChange"
    />

    <UButton
      variant="ghost"
      size="sm"
      icon="i-heroicons-x-mark"
      @click="resetFilters"
    >
      {{ $t('evaluations.note.filters.reset') }}
    </UButton>
  </div>
</template>
