<script setup lang="ts">
import type { Evaluation } from '../types/evaluation.types'

const props = defineProps<{
  items: Evaluation[]
  totalCount: number
}>()

const { t } = useI18n()

const totalParTrimestre = computed(() => {
  const counts: Record<number, number> = { 1: 0, 2: 0, 3: 0 }
  props.items.forEach((e) => {
    if (counts[e.trimestre] !== undefined) counts[e.trimestre]++
  })
  return counts
})

const moyenneCoefficient = computed(() => {
  if (props.items.length === 0) return '0'
  return (props.items.reduce((sum, e) => sum + e.coefficient, 0) / props.items.length).toFixed(1)
})

const stats = computed(() => [
  {
    label: t('evaluations.evaluation.stats.total'),
    value: String(props.totalCount),
    icon: 'i-heroicons-clipboard-document-list',
    color: 'text-purple-600 dark:text-purple-400',
    bgColor: 'bg-purple-50 dark:bg-purple-900/20',
  },
  {
    label: t('evaluations.evaluation.stats.trimestre1'),
    value: String(totalParTrimestre.value[1]),
    icon: 'i-heroicons-calendar-days',
    color: 'text-blue-600 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-900/20',
  },
  {
    label: t('evaluations.evaluation.stats.trimestre2'),
    value: String(totalParTrimestre.value[2]),
    icon: 'i-heroicons-calendar-days',
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-900/20',
  },
  {
    label: t('evaluations.evaluation.stats.moyenneCoeff'),
    value: moyenneCoefficient.value,
    icon: 'i-heroicons-chart-bar',
    color: 'text-amber-600 dark:text-amber-400',
    bgColor: 'bg-amber-50 dark:bg-amber-900/20',
  },
])
</script>

<template>
  <div class="grid grid-cols-2 gap-4 lg:grid-cols-4">
    <div
      v-for="stat in stats"
      :key="stat.label"
      class="rounded-lg border border-gray-200 bg-white p-4 dark:border-gray-700 dark:bg-gray-900"
    >
      <div class="flex items-center gap-3">
        <div
          class="rounded-lg p-2"
          :class="stat.bgColor"
        >
          <UIcon
            :name="stat.icon"
            class="h-5 w-5"
            :class="stat.color"
          />
        </div>
        <div>
          <p class="text-2xl font-bold text-gray-900 dark:text-white">
            {{ stat.value }}
          </p>
          <p class="text-sm text-gray-500 dark:text-gray-400">
            {{ stat.label }}
          </p>
        </div>
      </div>
    </div>
  </div>
</template>
