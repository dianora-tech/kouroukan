<script setup lang="ts">
import type { DocumentGenere } from '../types/document-genere.types'

const props = defineProps<{
  items: DocumentGenere[]
  totalCount: number
}>()

const { t } = useI18n()

const totalSignes = computed(() =>
  props.items.filter(d => d.statutSignature === 'Signe').length,
)

const totalEnAttente = computed(() =>
  props.items.filter(d => d.statutSignature === 'EnAttente').length,
)

const totalRefuses = computed(() =>
  props.items.filter(d => d.statutSignature === 'Refuse').length,
)

const stats = computed(() => [
  {
    label: t('documents.documentGenere.stats.total'),
    value: String(props.totalCount),
    icon: 'i-heroicons-document-text',
    color: 'text-gray-600 dark:text-gray-400',
    bgColor: 'bg-gray-50 dark:bg-gray-900/20',
  },
  {
    label: t('documents.documentGenere.stats.signes'),
    value: String(totalSignes.value),
    icon: 'i-heroicons-check-badge',
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-900/20',
  },
  {
    label: t('documents.documentGenere.stats.enAttente'),
    value: String(totalEnAttente.value),
    icon: 'i-heroicons-clock',
    color: 'text-amber-600 dark:text-amber-400',
    bgColor: 'bg-amber-50 dark:bg-amber-900/20',
  },
  {
    label: t('documents.documentGenere.stats.refuses'),
    value: String(totalRefuses.value),
    icon: 'i-heroicons-x-circle',
    color: 'text-red-600 dark:text-red-400',
    bgColor: 'bg-red-50 dark:bg-red-900/20',
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
