<script setup lang="ts">
import type { NiveauClasse } from '../types/niveauClasse.types'

const props = defineProps<{
  items: NiveauClasse[]
  totalCount: number
}>()

const { t } = useI18n()

const totalPrescolaire = computed(() =>
  props.items.filter(n => n.cycleEtude === 'Prescolaire').length,
)

const totalPrimaire = computed(() =>
  props.items.filter(n => n.cycleEtude === 'Primaire').length,
)

const totalSecondaire = computed(() =>
  props.items.filter(n => ['College', 'Lycee'].includes(n.cycleEtude)).length,
)

const totalUniversite = computed(() =>
  props.items.filter(n => n.cycleEtude === 'Universite').length,
)

const stats = computed(() => [
  {
    label: t('pedagogie.niveauClasse.stats.total'),
    value: props.totalCount,
    icon: 'i-heroicons-academic-cap',
    color: 'text-blue-600 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-900/20',
  },
  {
    label: t('pedagogie.niveauClasse.stats.primaire'),
    value: totalPrimaire.value,
    icon: 'i-heroicons-book-open',
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-900/20',
  },
  {
    label: t('pedagogie.niveauClasse.stats.secondaire'),
    value: totalSecondaire.value,
    icon: 'i-heroicons-building-library',
    color: 'text-amber-600 dark:text-amber-400',
    bgColor: 'bg-amber-50 dark:bg-amber-900/20',
  },
  {
    label: t('pedagogie.niveauClasse.stats.universite'),
    value: totalUniversite.value,
    icon: 'i-heroicons-trophy',
    color: 'text-purple-600 dark:text-purple-400',
    bgColor: 'bg-purple-50 dark:bg-purple-900/20',
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
