<script setup lang="ts">
import type { Absence } from '../types/absence.types'

const props = defineProps<{
  items: Absence[]
  totalCount: number
}>()

const { t } = useI18n()

const totalJustifiees = computed(() =>
  props.items.filter(a => a.estJustifiee).length,
)

const totalNonJustifiees = computed(() =>
  props.items.filter(a => !a.estJustifiee).length,
)

const tauxJustification = computed(() => {
  if (props.items.length === 0) return '0%'
  return `${Math.round((totalJustifiees.value / props.items.length) * 100)}%`
})

const stats = computed(() => [
  {
    label: t('presences.absence.stats.total'),
    value: String(props.totalCount),
    icon: 'i-heroicons-exclamation-triangle',
    color: 'text-cyan-600 dark:text-cyan-400',
    bgColor: 'bg-cyan-50 dark:bg-cyan-900/20',
  },
  {
    label: t('presences.absence.stats.justifiees'),
    value: String(totalJustifiees.value),
    icon: 'i-heroicons-check-circle',
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-900/20',
  },
  {
    label: t('presences.absence.stats.nonJustifiees'),
    value: String(totalNonJustifiees.value),
    icon: 'i-heroicons-x-circle',
    color: 'text-red-600 dark:text-red-400',
    bgColor: 'bg-red-50 dark:bg-red-900/20',
  },
  {
    label: t('presences.absence.stats.tauxJustification'),
    value: tauxJustification.value,
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
        <div class="rounded-lg p-2" :class="stat.bgColor">
          <UIcon :name="stat.icon" class="h-5 w-5" :class="stat.color" />
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
