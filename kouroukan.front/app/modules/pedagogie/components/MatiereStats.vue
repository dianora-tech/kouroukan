<script setup lang="ts">
import type { Matiere } from '../types/matiere.types'

const props = defineProps<{
  items: Matiere[]
  totalCount: number
}>()

const { t } = useI18n()

const totalHeures = computed(() =>
  props.items.reduce((sum, m) => sum + m.nombreHeures, 0),
)

const moyenneCoeff = computed(() => {
  if (props.items.length === 0) return 0
  const sum = props.items.reduce((s, m) => s + m.coefficient, 0)
  return Math.round((sum / props.items.length) * 100) / 100
})

const stats = computed(() => [
  {
    label: t('pedagogie.matiere.stats.total'),
    value: props.totalCount,
    icon: 'i-heroicons-book-open',
    color: 'text-blue-600 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-900/20',
  },
  {
    label: t('pedagogie.matiere.stats.totalHeures'),
    value: `${totalHeures.value}h`,
    icon: 'i-heroicons-clock',
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-900/20',
  },
  {
    label: t('pedagogie.matiere.stats.moyenneCoeff'),
    value: moyenneCoeff.value,
    icon: 'i-heroicons-scale',
    color: 'text-amber-600 dark:text-amber-400',
    bgColor: 'bg-amber-50 dark:bg-amber-900/20',
  },
])
</script>

<template>
  <div class="grid grid-cols-2 gap-4 lg:grid-cols-3">
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
