<script setup lang="ts">
import type { CahierTextes } from '../types/cahierTextes.types'

const props = defineProps<{
  items: CahierTextes[]
  totalCount: number
}>()

const { t } = useI18n()

const withHomework = computed(() =>
  props.items.filter(c => c.travailAFaire && c.travailAFaire.trim().length > 0).length,
)

const stats = computed(() => [
  {
    label: t('pedagogie.cahierTextes.stats.total'),
    value: props.totalCount,
    icon: 'i-heroicons-document-text',
    color: 'text-blue-600 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-900/20',
  },
  {
    label: t('pedagogie.cahierTextes.stats.avecDevoirs'),
    value: withHomework.value,
    icon: 'i-heroicons-pencil',
    color: 'text-amber-600 dark:text-amber-400',
    bgColor: 'bg-amber-50 dark:bg-amber-900/20',
  },
])
</script>

<template>
  <div class="grid grid-cols-2 gap-4 lg:grid-cols-2">
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
