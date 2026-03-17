<script setup lang="ts">
import type { EvaluationFilters } from '../types/evaluation.types'
import { TRIMESTRES } from '../types/evaluation.types'
import { useEvaluation } from '../composables/useEvaluation'

const emit = defineEmits<{
  (e: 'filter', filters: EvaluationFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const { typeOptions } = useEvaluation()

const filters = reactive<EvaluationFilters>({
  search: '',
  typeId: undefined,
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
  filters.typeId = undefined
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

const allTypeOptions = computed(() => [
  { label: t('evaluations.evaluation.filters.allTypes'), value: '' },
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
      v-model="filters.trimestre"
      :items="trimestreOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <UInput
      v-model="filters.dateFrom"
      type="date"
      :placeholder="$t('evaluations.evaluation.filters.dateFrom')"
      class="w-40"
      @change="onFilterChange"
    />

    <UInput
      v-model="filters.dateTo"
      type="date"
      :placeholder="$t('evaluations.evaluation.filters.dateTo')"
      class="w-40"
      @change="onFilterChange"
    />

    <UButton variant="ghost" size="sm" icon="i-heroicons-x-mark" @click="resetFilters">
      {{ $t('evaluations.evaluation.filters.reset') }}
    </UButton>
  </div>
</template>
