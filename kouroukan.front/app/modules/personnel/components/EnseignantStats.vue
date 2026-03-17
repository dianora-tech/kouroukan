<script setup lang="ts">
import type { Enseignant } from '../types/enseignant.types'

const props = defineProps<{
  items: Enseignant[]
  totalCount: number
}>()

const { t } = useI18n()

const totalActifs = computed(() =>
  props.items.filter(e => e.statutEnseignant === 'Actif').length,
)

const totalEnConge = computed(() =>
  props.items.filter(e => e.statutEnseignant === 'EnConge').length,
)

const moyenneSoldeConges = computed(() => {
  if (props.items.length === 0) return '0'
  return Math.round(props.items.reduce((sum, e) => sum + e.soldeCongesAnnuel, 0) / props.items.length).toString()
})

const stats = computed(() => [
  {
    label: t('personnel.enseignant.stats.total'),
    value: String(props.totalCount),
    icon: 'i-heroicons-user-group',
    color: 'text-orange-600 dark:text-orange-400',
    bgColor: 'bg-orange-50 dark:bg-orange-900/20',
  },
  {
    label: t('personnel.enseignant.stats.actifs'),
    value: String(totalActifs.value),
    icon: 'i-heroicons-check-circle',
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-900/20',
  },
  {
    label: t('personnel.enseignant.stats.enConge'),
    value: String(totalEnConge.value),
    icon: 'i-heroicons-clock',
    color: 'text-amber-600 dark:text-amber-400',
    bgColor: 'bg-amber-50 dark:bg-amber-900/20',
  },
  {
    label: t('personnel.enseignant.stats.moyenneConges'),
    value: moyenneSoldeConges.value,
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
