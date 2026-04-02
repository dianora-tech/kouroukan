<script setup lang="ts">
import type { Depense } from '../types/depense.types'

defineProps<{
  depense: Depense
}>()

const emit = defineEmits<{
  (e: 'edit', depense: Depense): void
  (e: 'delete', depense: Depense): void
}>()

const { t } = useI18n()
const { formatDate } = useFormatDate()

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    Demande: 'info',
    ValideN1: 'warning',
    ValideFinance: 'warning',
    ValideDirection: 'success',
    Executee: 'success',
    Archivee: 'neutral',
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
          {{ depense.numeroJustificatif }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ depense.typeName }} - {{ $t(`finances.depense.categorie.${depense.categorie}`) }}
        </p>
      </div>
      <UBadge
        :color="getStatutColor(depense.statutDepense)"
        variant="subtle"
        size="sm"
      >
        {{ $t(`finances.depense.statut.${depense.statutDepense}`) }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon
          name="i-heroicons-banknotes"
          class="mr-1 inline h-4 w-4"
        />
        {{ formatMontant(depense.montant) }}
      </p>
      <p>
        <UIcon
          name="i-heroicons-user"
          class="mr-1 inline h-4 w-4"
        />
        {{ depense.beneficiaireNom }}
      </p>
      <p>
        <UIcon
          name="i-heroicons-calendar"
          class="mr-1 inline h-4 w-4"
        />
        {{ formatDate(depense.dateDemande) }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'finances:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', depense)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'finances:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', depense)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
