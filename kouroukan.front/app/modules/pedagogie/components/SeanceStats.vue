<script setup lang="ts">
import type { Seance } from '../types/seance.types'

const props = defineProps<{
  items: Seance[]
  totalCount: number
}>()

const { t } = useI18n()

const uniqueClasses = computed(() =>
  new Set(props.items.map(s => s.classeId)).size,
)

const uniqueEnseignants = computed(() =>
  new Set(props.items.map(s => s.enseignantId)).size,
)

const stats = computed(() => [
  {
    label: t('pedagogie.seance.stats.total'),
    value: props.totalCount,
    icon: 'i-heroicons-clock',
    color: 'text-blue-600 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-900/20',
  },
  {
    label: t('pedagogie.seance.stats.classes'),
    value: uniqueClasses.value,
    icon: 'i-heroicons-rectangle-group',
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-900/20',
  },
  {
    label: t('pedagogie.seance.stats.enseignants'),
    value: uniqueEnseignants.value,
    icon: 'i-heroicons-user',
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
