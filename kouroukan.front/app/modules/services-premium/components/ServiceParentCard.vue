<script setup lang="ts">
import type { ServiceParent } from '../types/service-parent.types'

defineProps<{
  serviceParent: ServiceParent
}>()

const emit = defineEmits<{
  (e: 'edit', serviceParent: ServiceParent): void
  (e: 'delete', serviceParent: ServiceParent): void
}>()

const { t } = useI18n()

function formatMontant(montant: number): string {
  return new Intl.NumberFormat('fr-GN', { style: 'decimal' }).format(montant) + ' GNF'
}
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div>
        <h3 class="font-semibold text-gray-900 dark:text-white">
          {{ serviceParent.code }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ serviceParent.typeName }}
        </p>
      </div>
      <UBadge
        :color="serviceParent.estActif ? 'success' : 'neutral'"
        variant="subtle"
        size="sm"
      >
        {{ serviceParent.estActif ? $t('servicesPremium.serviceParent.actif') : $t('servicesPremium.serviceParent.inactif') }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon
          name="i-heroicons-banknotes"
          class="mr-1 inline h-4 w-4"
        />
        {{ formatMontant(serviceParent.tarif) }} / {{ $t(`servicesPremium.serviceParent.periodicite.${serviceParent.periodicite}`) }}
      </p>
      <p v-if="serviceParent.periodeEssaiJours">
        <UIcon
          name="i-heroicons-clock"
          class="mr-1 inline h-4 w-4"
        />
        {{ $t('servicesPremium.serviceParent.essai') }}: {{ serviceParent.periodeEssaiJours }} {{ $t('servicesPremium.serviceParent.jours') }}
      </p>
      <p v-if="serviceParent.tarifDegressif">
        <UIcon
          name="i-heroicons-arrow-trending-down"
          class="mr-1 inline h-4 w-4"
        />
        {{ $t('servicesPremium.serviceParent.degressif') }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'services-premium:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', serviceParent)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'services-premium:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', serviceParent)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
