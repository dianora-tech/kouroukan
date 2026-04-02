<script setup lang="ts">
import type { MembreBDE } from '../types/membre-bde.types'

defineProps<{
  membre: MembreBDE
}>()

const emit = defineEmits<{
  (e: 'edit', membre: MembreBDE): void
  (e: 'delete', membre: MembreBDE): void
}>()

const { t } = useI18n()
const { formatDate } = useFormatDate()

function formatMontant(montant: number): string {
  return new Intl.NumberFormat('fr-GN', { style: 'decimal' }).format(montant) + ' GNF'
}
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div>
        <h3 class="font-semibold text-gray-900 dark:text-white">
          {{ membre.name }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ membre.associationNom }}
        </p>
      </div>
      <UBadge
        :color="membre.estActif ? 'success' : 'neutral'"
        variant="subtle"
        size="sm"
      >
        {{ membre.estActif ? $t('bde.membreBde.actif') : $t('bde.membreBde.inactif') }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon
          name="i-heroicons-identification"
          class="mr-1 inline h-4 w-4"
        />
        {{ $t(`bde.membreBde.role.${membre.roleBDE}`) }}
      </p>
      <p>
        <UIcon
          name="i-heroicons-calendar"
          class="mr-1 inline h-4 w-4"
        />
        {{ formatDate(membre.dateAdhesion) }}
      </p>
      <p v-if="membre.montantCotisation">
        <UIcon
          name="i-heroicons-banknotes"
          class="mr-1 inline h-4 w-4"
        />
        {{ formatMontant(membre.montantCotisation) }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'bde:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', membre)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'bde:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', membre)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
