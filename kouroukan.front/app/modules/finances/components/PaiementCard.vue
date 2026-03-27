<script setup lang="ts">
import type { Paiement } from '../types/paiement.types'

defineProps<{
  paiement: Paiement
}>()

const emit = defineEmits<{
  (e: 'edit', paiement: Paiement): void
  (e: 'delete', paiement: Paiement): void
}>()

const { t } = useI18n()
const { formatDateShort } = useFormatDate()

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    EnAttente: 'warning',
    Confirme: 'success',
    Echec: 'error',
    Rembourse: 'info',
  }
  return colors[statut] ?? 'neutral'
}

function getMoyenColor(moyen: string): string {
  const colors: Record<string, string> = {
    OrangeMoney: 'warning',
    SoutraMoney: 'info',
    MTNMoMo: 'success',
    Especes: 'neutral',
  }
  return colors[moyen] ?? 'neutral'
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
          {{ paiement.numeroRecu }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ paiement.typeName }} - {{ $t('finances.paiement.facture') }} {{ paiement.factureNumero ?? `#${paiement.factureId}` }}
        </p>
      </div>
      <UBadge :color="getStatutColor(paiement.statutPaiement)" variant="subtle" size="sm">
        {{ $t(`finances.paiement.statut.${paiement.statutPaiement}`) }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon name="i-heroicons-banknotes" class="mr-1 inline h-4 w-4" />
        {{ formatMontant(paiement.montantPaye) }}
      </p>
      <p>
        <UIcon name="i-heroicons-device-phone-mobile" class="mr-1 inline h-4 w-4" />
        <UBadge :color="getMoyenColor(paiement.moyenPaiement)" variant="subtle" size="xs">
          {{ $t(`finances.paiement.moyen.${paiement.moyenPaiement}`) }}
        </UBadge>
      </p>
      <p v-if="paiement.referenceMobileMoney">
        <UIcon name="i-heroicons-hashtag" class="mr-1 inline h-4 w-4" />
        {{ paiement.referenceMobileMoney }}
      </p>
      <p>
        <UIcon name="i-heroicons-calendar" class="mr-1 inline h-4 w-4" />
        {{ formatDateShort(paiement.datePaiement) }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'finances:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', paiement)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'finances:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', paiement)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
