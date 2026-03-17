<script setup lang="ts">
import type { RemunerationEnseignant } from '../types/remuneration.types'
import { MOIS_OPTIONS } from '../types/remuneration.types'

defineProps<{
  remuneration: RemunerationEnseignant
}>()

const emit = defineEmits<{
  (e: 'edit', remuneration: RemunerationEnseignant): void
  (e: 'delete', remuneration: RemunerationEnseignant): void
}>()

const { t } = useI18n()

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    EnAttente: 'warning',
    Valide: 'info',
    Paye: 'success',
  }
  return colors[statut] ?? 'neutral'
}

function formatMontant(montant: number): string {
  return new Intl.NumberFormat('fr-GN', { style: 'decimal' }).format(montant) + ' GNF'
}

function getMoisLabel(mois: number): string {
  return t(`finances.remuneration.mois.${mois}`)
}
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div>
        <h3 class="font-semibold text-gray-900 dark:text-white">
          {{ remuneration.enseignantNom ?? `#${remuneration.enseignantId}` }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ getMoisLabel(remuneration.mois) }} {{ remuneration.annee }}
        </p>
      </div>
      <UBadge :color="getStatutColor(remuneration.statutPaiement)" variant="subtle" size="sm">
        {{ $t(`finances.remuneration.statut.${remuneration.statutPaiement}`) }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon name="i-heroicons-banknotes" class="mr-1 inline h-4 w-4" />
        {{ formatMontant(remuneration.montantTotal) }}
      </p>
      <p>
        <UIcon name="i-heroicons-briefcase" class="mr-1 inline h-4 w-4" />
        {{ $t(`finances.remuneration.mode.${remuneration.modeRemuneration}`) }}
      </p>
      <p v-if="remuneration.nombreHeures">
        <UIcon name="i-heroicons-clock" class="mr-1 inline h-4 w-4" />
        {{ remuneration.nombreHeures }}h
        <span v-if="remuneration.tauxHoraire"> @ {{ formatMontant(remuneration.tauxHoraire) }}/h</span>
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'finances:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', remuneration)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'finances:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', remuneration)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
