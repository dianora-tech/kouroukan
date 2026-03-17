<script setup lang="ts">
import type { Evenement } from '../types/evenement.types'

defineProps<{
  evenement: Evenement
}>()

const emit = defineEmits<{
  (e: 'edit', evenement: Evenement): void
  (e: 'delete', evenement: Evenement): void
}>()

const { t } = useI18n()

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    Planifie: 'info',
    Valide: 'warning',
    EnCours: 'success',
    Termine: 'neutral',
    Annule: 'error',
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
          {{ evenement.name }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ evenement.typeName }} - {{ evenement.associationNom }}
        </p>
      </div>
      <UBadge :color="getStatutColor(evenement.statutEvenement)" variant="subtle" size="sm">
        {{ $t(`bde.evenement.statut.${evenement.statutEvenement}`) }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon name="i-heroicons-calendar" class="mr-1 inline h-4 w-4" />
        {{ evenement.dateEvenement }}
      </p>
      <p>
        <UIcon name="i-heroicons-map-pin" class="mr-1 inline h-4 w-4" />
        {{ evenement.lieu }}
      </p>
      <p>
        <UIcon name="i-heroicons-users" class="mr-1 inline h-4 w-4" />
        {{ evenement.nombreInscrits }}<template v-if="evenement.capacite"> / {{ evenement.capacite }}</template>
      </p>
      <p v-if="evenement.tarifEntree">
        <UIcon name="i-heroicons-banknotes" class="mr-1 inline h-4 w-4" />
        {{ formatMontant(evenement.tarifEntree) }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'bde:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', evenement)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'bde:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', evenement)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
