<script setup lang="ts">
import type { DossierAdmission } from '../types/dossier-admission.types'

const props = defineProps<{
  items: DossierAdmission[]
  totalCount: number
}>()

const { t } = useI18n()

const totalAdmis = computed(() =>
  props.items.filter(d => d.statutDossier === 'Admis').length,
)

const totalEnEtude = computed(() =>
  props.items.filter(d => d.statutDossier === 'EnEtude').length,
)

const totalRefuses = computed(() =>
  props.items.filter(d => d.statutDossier === 'Refuse').length,
)

const stats = computed(() => [
  {
    label: t('inscriptions.dossierAdmission.stats.total'),
    value: props.totalCount,
    icon: 'i-heroicons-document-text',
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-900/20',
  },
  {
    label: t('inscriptions.dossierAdmission.stats.admis'),
    value: totalAdmis.value,
    icon: 'i-heroicons-check-circle',
    color: 'text-blue-600 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-900/20',
  },
  {
    label: t('inscriptions.dossierAdmission.stats.enEtude'),
    value: totalEnEtude.value,
    icon: 'i-heroicons-clock',
    color: 'text-amber-600 dark:text-amber-400',
    bgColor: 'bg-amber-50 dark:bg-amber-900/20',
  },
  {
    label: t('inscriptions.dossierAdmission.stats.refuses'),
    value: totalRefuses.value,
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
