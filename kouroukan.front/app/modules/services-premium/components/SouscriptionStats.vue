<script setup lang="ts">
import type { Souscription } from '../types/souscription.types'

const props = defineProps<{
  items: Souscription[]
  totalCount: number
}>()

const { t } = useI18n()

const totalActives = computed(() =>
  props.items.filter(s => s.statutSouscription === 'Active').length,
)

const totalEssai = computed(() =>
  props.items.filter(s => s.statutSouscription === 'Essai').length,
)

const totalMontantPaye = computed(() =>
  props.items.reduce((sum, s) => sum + s.montantPaye, 0),
)

function formatMontant(montant: number): string {
  return new Intl.NumberFormat('fr-GN', { style: 'decimal' }).format(montant)
}

const stats = computed(() => [
  {
    label: t('servicesPremium.souscription.stats.total'),
    value: String(props.totalCount),
    icon: 'i-heroicons-clipboard-document-list',
    color: 'text-amber-600 dark:text-amber-400',
    bgColor: 'bg-amber-50 dark:bg-amber-900/20',
  },
  {
    label: t('servicesPremium.souscription.stats.actives'),
    value: String(totalActives.value),
    icon: 'i-heroicons-check-circle',
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-900/20',
  },
  {
    label: t('servicesPremium.souscription.stats.essai'),
    value: String(totalEssai.value),
    icon: 'i-heroicons-beaker',
    color: 'text-blue-600 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-900/20',
  },
  {
    label: t('servicesPremium.souscription.stats.montantTotal'),
    value: formatMontant(totalMontantPaye.value) + ' GNF',
    icon: 'i-heroicons-banknotes',
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
