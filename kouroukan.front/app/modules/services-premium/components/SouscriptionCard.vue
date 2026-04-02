<script setup lang="ts">
import type { Souscription } from '../types/souscription.types'

defineProps<{
  souscription: Souscription
}>()

const emit = defineEmits<{
  (e: 'edit', souscription: Souscription): void
  (e: 'delete', souscription: Souscription): void
}>()

const { t } = useI18n()
const { formatDate } = useFormatDate()

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    Active: 'success',
    Expiree: 'neutral',
    Resiliee: 'error',
    Essai: 'info',
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
          {{ souscription.serviceParentNom ?? `#${souscription.serviceParentId}` }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ souscription.parentNom ?? `Parent #${souscription.parentId}` }}
        </p>
      </div>
      <UBadge :color="getStatutColor(souscription.statutSouscription)" variant="subtle" size="sm">
        {{ $t(`servicesPremium.souscription.statut.${souscription.statutSouscription}`) }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon name="i-heroicons-banknotes" class="mr-1 inline h-4 w-4" />
        {{ formatMontant(souscription.montantPaye) }}
      </p>
      <p>
        <UIcon name="i-heroicons-calendar" class="mr-1 inline h-4 w-4" />
        {{ formatDate(souscription.dateDebut) }}
        <span v-if="souscription.dateFin"> — {{ formatDate(souscription.dateFin) }}</span>
      </p>
      <p v-if="souscription.renouvellementAuto">
        <UIcon name="i-heroicons-arrow-path" class="mr-1 inline h-4 w-4" />
        {{ $t('servicesPremium.souscription.autoRenew') }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'services-premium:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', souscription)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'services-premium:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', souscription)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
