<script setup lang="ts">
import type { DemandeConge } from '../types/demandeConge.types'

const props = defineProps<{
  items: DemandeConge[]
  totalCount: number
}>()

const { t } = useI18n()

const totalSoumises = computed(() =>
  props.items.filter(d => d.statutDemande === 'Soumise').length,
)

const totalApprouvees = computed(() =>
  props.items.filter(d => d.statutDemande === 'ApprouveeN1' || d.statutDemande === 'ApprouveeDirection').length,
)

const totalRefusees = computed(() =>
  props.items.filter(d => d.statutDemande === 'Refusee').length,
)

const totalImpactPaie = computed(() =>
  props.items.filter(d => d.impactPaie).length,
)

const stats = computed(() => [
  {
    label: t('personnel.demandeConge.stats.total'),
    value: String(props.totalCount),
    icon: 'i-heroicons-document-text',
    color: 'text-orange-600 dark:text-orange-400',
    bgColor: 'bg-orange-50 dark:bg-orange-900/20',
  },
  {
    label: t('personnel.demandeConge.stats.soumises'),
    value: String(totalSoumises.value),
    icon: 'i-heroicons-clock',
    color: 'text-blue-600 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-900/20',
  },
  {
    label: t('personnel.demandeConge.stats.approuvees'),
    value: String(totalApprouvees.value),
    icon: 'i-heroicons-check-circle',
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-900/20',
  },
  {
    label: t('personnel.demandeConge.stats.impactPaie'),
    value: String(totalImpactPaie.value),
    icon: 'i-heroicons-exclamation-triangle',
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
