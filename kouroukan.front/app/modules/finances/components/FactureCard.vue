<script setup lang="ts">
import type { Facture } from '../types/facture.types'

defineProps<{
  facture: Facture
}>()

const emit = defineEmits<{
  (e: 'edit', facture: Facture): void
  (e: 'delete', facture: Facture): void
}>()

const { t } = useI18n()
const { formatDate } = useFormatDate()

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    Emise: 'info',
    PartPaye: 'warning',
    Payee: 'success',
    Echue: 'error',
    Annulee: 'neutral',
  }
  return colors[statut] ?? 'neutral'
}

function formatMontant(montant: number): string {
  return new Intl.NumberFormat('fr-GN', { style: 'decimal' }).format(montant) + ' GNF'
}
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div>
        <h3 class="font-semibold text-gray-900 dark:text-white">
          {{ facture.numeroFacture }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ facture.typeName }} - {{ facture.eleveNom ?? `#${facture.eleveId}` }}
        </p>
      </div>
      <UBadge
        :color="getStatutColor(facture.statutFacture)"
        variant="subtle"
        size="sm"
      >
        {{ $t(`finances.facture.statut.${facture.statutFacture}`) }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon
          name="i-heroicons-banknotes"
          class="mr-1 inline h-4 w-4"
        />
        {{ formatMontant(facture.montantTotal) }}
      </p>
      <p>
        <UIcon
          name="i-heroicons-check-circle"
          class="mr-1 inline h-4 w-4"
        />
        {{ $t('finances.facture.paye') }}: {{ formatMontant(facture.montantPaye) }}
      </p>
      <p v-if="facture.solde > 0">
        <UIcon
          name="i-heroicons-exclamation-triangle"
          class="mr-1 inline h-4 w-4"
        />
        {{ $t('finances.facture.resteAPayer') }}: {{ formatMontant(facture.solde) }}
      </p>
      <p>
        <UIcon
          name="i-heroicons-calendar"
          class="mr-1 inline h-4 w-4"
        />
        {{ $t('finances.facture.echeance') }}: {{ formatDate(facture.dateEcheance) }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'finances:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', facture)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'finances:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', facture)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
