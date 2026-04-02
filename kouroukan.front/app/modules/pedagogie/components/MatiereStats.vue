<script setup lang="ts">
import type { Matiere } from '../types/matiere.types'

const props = defineProps<{
  items: Matiere[]
  totalCount: number
}>()

const { t } = useI18n()

const stats = computed(() => [
  {
    label: t('pedagogie.matiere.stats.total'),
    value: props.totalCount,
    icon: 'i-heroicons-book-open',
    color: 'text-blue-600 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-900/20',
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
