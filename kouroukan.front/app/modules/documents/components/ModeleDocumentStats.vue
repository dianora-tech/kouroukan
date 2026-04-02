<script setup lang="ts">
import type { ModeleDocument } from '../types/modele-document.types'

const props = defineProps<{
  items: ModeleDocument[]
  totalCount: number
}>()

const { t } = useI18n()

const totalActifs = computed(() =>
  props.items.filter(m => m.estActif).length,
)

const totalInactifs = computed(() =>
  props.items.filter(m => !m.estActif).length,
)

const stats = computed(() => [
  {
    label: t('documents.modeleDocument.stats.total'),
    value: String(props.totalCount),
    icon: 'i-heroicons-document-duplicate',
    color: 'text-gray-600 dark:text-gray-400',
    bgColor: 'bg-gray-50 dark:bg-gray-900/20',
  },
  {
    label: t('documents.modeleDocument.stats.actifs'),
    value: String(totalActifs.value),
    icon: 'i-heroicons-check-circle',
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-900/20',
  },
  {
    label: t('documents.modeleDocument.stats.inactifs'),
    value: String(totalInactifs.value),
    icon: 'i-heroicons-x-circle',
    color: 'text-red-600 dark:text-red-400',
    bgColor: 'bg-red-50 dark:bg-red-900/20',
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
