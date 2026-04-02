<script setup lang="ts">
import type { Bulletin } from '../types/bulletin.types'

const props = defineProps<{
  items: Bulletin[]
  totalCount: number
}>()

const { t } = useI18n()

const moyenneGeneraleClasse = computed(() => {
  if (props.items.length === 0) return '0'
  return (props.items.reduce((sum, b) => sum + b.moyenneGenerale, 0) / props.items.length).toFixed(2)
})

const totalPublies = computed(() =>
  props.items.filter(b => b.estPublie).length,
)

const totalNonPublies = computed(() =>
  props.items.filter(b => !b.estPublie).length,
)

const stats = computed(() => [
  {
    label: t('evaluations.bulletin.stats.total'),
    value: String(props.totalCount),
    icon: 'i-heroicons-document-duplicate',
    color: 'text-purple-600 dark:text-purple-400',
    bgColor: 'bg-purple-50 dark:bg-purple-900/20',
  },
  {
    label: t('evaluations.bulletin.stats.moyenneClasse'),
    value: moyenneGeneraleClasse.value,
    icon: 'i-heroicons-chart-bar',
    color: 'text-blue-600 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-900/20',
  },
  {
    label: t('evaluations.bulletin.stats.publies'),
    value: String(totalPublies.value),
    icon: 'i-heroicons-check-circle',
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-900/20',
  },
  {
    label: t('evaluations.bulletin.stats.nonPublies'),
    value: String(totalNonPublies.value),
    icon: 'i-heroicons-clock',
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
