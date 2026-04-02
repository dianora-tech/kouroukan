<script setup lang="ts">
import type { Salle } from '../types/salle.types'

const props = defineProps<{
  items: Salle[]
  totalCount: number
}>()

const { t } = useI18n()

const totalDisponibles = computed(() =>
  props.items.filter(s => s.estDisponible).length,
)

const totalIndisponibles = computed(() =>
  props.items.filter(s => !s.estDisponible).length,
)

const capaciteTotale = computed(() =>
  props.items.reduce((sum, s) => sum + s.capacite, 0),
)

const stats = computed(() => [
  {
    label: t('pedagogie.salle.stats.total'),
    value: props.totalCount,
    icon: 'i-heroicons-building-office-2',
    color: 'text-blue-600 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-900/20',
  },
  {
    label: t('pedagogie.salle.stats.disponibles'),
    value: totalDisponibles.value,
    icon: 'i-heroicons-check-circle',
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-900/20',
  },
  {
    label: t('pedagogie.salle.stats.indisponibles'),
    value: totalIndisponibles.value,
    icon: 'i-heroicons-x-circle',
    color: 'text-red-600 dark:text-red-400',
    bgColor: 'bg-red-50 dark:bg-red-900/20',
  },
  {
    label: t('pedagogie.salle.stats.capaciteTotale'),
    value: capaciteTotale.value,
    icon: 'i-heroicons-users',
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
