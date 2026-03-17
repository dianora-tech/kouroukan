<script setup lang="ts">
import type { DepenseBDE } from '../types/depense-bde.types'

defineProps<{
  depense: DepenseBDE
}>()

const emit = defineEmits<{
  (e: 'edit', depense: DepenseBDE): void
  (e: 'delete', depense: DepenseBDE): void
}>()

const { t } = useI18n()

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    Demandee: 'info',
    ValideTresorier: 'warning',
    ValideSuper: 'success',
    Refusee: 'error',
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
          {{ depense.name }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ depense.typeName }} - {{ depense.associationNom }}
        </p>
      </div>
      <UBadge :color="getStatutColor(depense.statutValidation)" variant="subtle" size="sm">
        {{ $t(`bde.depenseBde.statut.${depense.statutValidation}`) }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon name="i-heroicons-banknotes" class="mr-1 inline h-4 w-4" />
        {{ formatMontant(depense.montant) }}
      </p>
      <p>
        <UIcon name="i-heroicons-tag" class="mr-1 inline h-4 w-4" />
        {{ $t(`bde.depenseBde.categorie.${depense.categorie}`) }}
      </p>
      <p>
        <UIcon name="i-heroicons-document-text" class="mr-1 inline h-4 w-4" />
        {{ depense.motif }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'bde:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', depense)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'bde:delete'"
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
