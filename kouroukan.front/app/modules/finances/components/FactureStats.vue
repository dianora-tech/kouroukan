<script setup lang="ts">
import type { Facture } from '../types/facture.types'

const props = defineProps<{
  items: Facture[]
  totalCount: number
}>()

const { t } = useI18n()

const totalMontant = computed(() =>
  props.items.reduce((sum, f) => sum + f.montantTotal, 0),
)

const totalPaye = computed(() =>
  props.items.reduce((sum, f) => sum + f.montantPaye, 0),
)

const totalSolde = computed(() =>
  props.items.reduce((sum, f) => sum + f.solde, 0),
)

const totalEchues = computed(() =>
  props.items.filter(f => f.statutFacture === 'Echue').length,
)

function formatMontant(montant: number): string {
  return new Intl.NumberFormat('fr-GN', { style: 'decimal' }).format(montant)
}

const stats = computed(() => [
  {
    label: t('finances.facture.stats.total'),
    value: String(props.totalCount),
    icon: 'i-heroicons-document-text',
    color: 'text-red-600 dark:text-red-400',
    bgColor: 'bg-red-50 dark:bg-red-900/20',
  },
  {
    label: t('finances.facture.stats.montantTotal'),
    value: formatMontant(totalMontant.value) + ' GNF',
    icon: 'i-heroicons-banknotes',
    color: 'text-blue-600 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-900/20',
  },
  {
    label: t('finances.facture.stats.montantPaye'),
    value: formatMontant(totalPaye.value) + ' GNF',
    icon: 'i-heroicons-check-badge',
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-900/20',
  },
  {
    label: t('finances.facture.stats.echues'),
    value: String(totalEchues.value),
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
