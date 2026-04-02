<script setup lang="ts">
import type { DemandeConge } from '../types/demandeConge.types'

defineProps<{
  demandeConge: DemandeConge
}>()

const emit = defineEmits<{
  (e: 'edit', demandeConge: DemandeConge): void
  (e: 'delete', demandeConge: DemandeConge): void
}>()

const { t } = useI18n()
const { formatDate } = useFormatDate()

function getStatutColor(statut: string): string {
  switch (statut) {
    case 'Soumise': return 'info'
    case 'ApprouveeN1': return 'warning'
    case 'ApprouveeDirection': return 'success'
    case 'Refusee': return 'error'
    default: return 'neutral'
  }
}
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div>
        <h3 class="font-semibold text-gray-900 dark:text-white">
          {{ demandeConge.typeName }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ demandeConge.enseignantNom ?? `#${demandeConge.enseignantId}` }}
        </p>
      </div>
      <UBadge :color="getStatutColor(demandeConge.statutDemande)" variant="subtle" size="sm">
        {{ $t(`personnel.demandeConge.statut.${demandeConge.statutDemande}`) }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon name="i-heroicons-calendar" class="mr-1 inline h-4 w-4" />
        {{ formatDate(demandeConge.dateDebut) }} — {{ formatDate(demandeConge.dateFin) }}
      </p>
      <p>
        <UIcon name="i-heroicons-document-text" class="mr-1 inline h-4 w-4" />
        {{ demandeConge.motif }}
      </p>
      <p v-if="demandeConge.impactPaie">
        <UIcon name="i-heroicons-exclamation-triangle" class="mr-1 inline h-4 w-4 text-amber-500" />
        {{ $t('personnel.demandeConge.impactPaieOui') }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'personnel:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', demandeConge)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'personnel:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', demandeConge)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
