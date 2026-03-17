<script setup lang="ts">
import type { Inscription } from '../types/inscription.types'

defineProps<{
  inscription: Inscription
}>()

const emit = defineEmits<{
  (e: 'edit', inscription: Inscription): void
  (e: 'delete', inscription: Inscription): void
}>()

const { t } = useI18n()

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    EnAttente: 'warning',
    Validee: 'success',
    Annulee: 'error',
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
          {{ inscription.eleveNom ?? `#${inscription.eleveId}` }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ inscription.typeName }} - {{ inscription.classeName ?? `#${inscription.classeId}` }}
        </p>
      </div>
      <UBadge :color="getStatutColor(inscription.statutInscription)" variant="subtle" size="sm">
        {{ $t(`inscriptions.inscription.statut.${inscription.statutInscription}`) }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon name="i-heroicons-calendar" class="mr-1 inline h-4 w-4" />
        {{ inscription.dateInscription }}
      </p>
      <p>
        <UIcon name="i-heroicons-banknotes" class="mr-1 inline h-4 w-4" />
        {{ formatMontant(inscription.montantInscription) }}
        <UBadge v-if="inscription.estPaye" color="success" variant="subtle" size="xs" class="ml-1">
          {{ $t('inscriptions.inscription.paye') }}
        </UBadge>
        <UBadge v-else color="warning" variant="subtle" size="xs" class="ml-1">
          {{ $t('inscriptions.inscription.nonPaye') }}
        </UBadge>
      </p>
      <p v-if="inscription.estRedoublant">
        <UIcon name="i-heroicons-arrow-path" class="mr-1 inline h-4 w-4" />
        {{ $t('inscriptions.inscription.redoublant') }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'inscriptions:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', inscription)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'inscriptions:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', inscription)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
