<script setup lang="ts">
import type { Appel } from '../types/appel.types'

const props = defineProps<{
  items: Appel[]
  totalCount: number
}>()

const { t } = useI18n()

const totalClotures = computed(() =>
  props.items.filter(a => a.estCloture).length,
)

const totalEnCours = computed(() =>
  props.items.filter(a => !a.estCloture).length,
)

const dernierAppel = computed(() => {
  if (props.items.length === 0) return '-'
  const sorted = [...props.items].sort((a, b) => b.dateAppel.localeCompare(a.dateAppel))
  return sorted[0].dateAppel
})

const stats = computed(() => [
  {
    label: t('presences.appel.stats.total'),
    value: String(props.totalCount),
    icon: 'i-heroicons-clipboard-document-check',
    color: 'text-cyan-600 dark:text-cyan-400',
    bgColor: 'bg-cyan-50 dark:bg-cyan-900/20',
  },
  {
    label: t('presences.appel.stats.clotures'),
    value: String(totalClotures.value),
    icon: 'i-heroicons-check-circle',
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-900/20',
  },
  {
    label: t('presences.appel.stats.enCours'),
    value: String(totalEnCours.value),
    icon: 'i-heroicons-clock',
    color: 'text-amber-600 dark:text-amber-400',
    bgColor: 'bg-amber-50 dark:bg-amber-900/20',
  },
  {
    label: t('presences.appel.stats.dernierAppel'),
    value: dernierAppel.value,
    icon: 'i-heroicons-calendar-days',
    color: 'text-blue-600 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-900/20',
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
