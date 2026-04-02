<script setup lang="ts">
import type { Message } from '../types/message.types'

const props = defineProps<{
  items: Message[]
  totalCount: number
}>()

const { t } = useI18n()

const totalLus = computed(() =>
  props.items.filter(m => m.estLu).length,
)

const totalNonLus = computed(() =>
  props.items.filter(m => !m.estLu).length,
)

const totalGroupes = computed(() =>
  props.items.filter(m => m.groupeDestinataire).length,
)

const stats = computed(() => [
  {
    label: t('communication.message.stats.total'),
    value: props.totalCount,
    icon: 'i-heroicons-envelope',
    color: 'text-teal-600 dark:text-teal-400',
    bgColor: 'bg-teal-50 dark:bg-teal-900/20',
  },
  {
    label: t('communication.message.stats.lus'),
    value: totalLus.value,
    icon: 'i-heroicons-envelope-open',
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-900/20',
  },
  {
    label: t('communication.message.stats.nonLus'),
    value: totalNonLus.value,
    icon: 'i-heroicons-envelope',
    color: 'text-amber-600 dark:text-amber-400',
    bgColor: 'bg-amber-50 dark:bg-amber-900/20',
  },
  {
    label: t('communication.message.stats.groupes'),
    value: totalGroupes.value,
    icon: 'i-heroicons-user-group',
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
