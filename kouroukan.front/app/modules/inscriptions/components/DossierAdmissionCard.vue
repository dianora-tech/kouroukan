<script setup lang="ts">
import type { DossierAdmission } from '../types/dossier-admission.types'

defineProps<{
  dossier: DossierAdmission
}>()

const emit = defineEmits<{
  (e: 'edit', dossier: DossierAdmission): void
  (e: 'delete', dossier: DossierAdmission): void
}>()

const { t } = useI18n()

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    Prospect: 'neutral',
    PreInscrit: 'info',
    EnEtude: 'warning',
    Convoque: 'info',
    Admis: 'success',
    Refuse: 'error',
    ListeAttente: 'warning',
  }
  return colors[statut] ?? 'neutral'
}
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div>
        <h3 class="font-semibold text-gray-900 dark:text-white">
          {{ dossier.eleveNom ?? `#${dossier.eleveId}` }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ dossier.typeName }}
        </p>
      </div>
      <UBadge :color="getStatutColor(dossier.statutDossier)" variant="subtle" size="sm">
        {{ $t(`inscriptions.dossierAdmission.statut.${dossier.statutDossier}`) }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon name="i-heroicons-calendar" class="mr-1 inline h-4 w-4" />
        {{ dossier.dateDemande }}
      </p>
      <p>
        <UIcon name="i-heroicons-arrow-path" class="mr-1 inline h-4 w-4" />
        {{ $t(`inscriptions.dossierAdmission.etape.${dossier.etapeActuelle}`) }}
      </p>
      <p v-if="dossier.scoringInterne !== null">
        <UIcon name="i-heroicons-chart-bar" class="mr-1 inline h-4 w-4" />
        {{ $t('inscriptions.dossierAdmission.score') }}: {{ dossier.scoringInterne }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'inscriptions:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', dossier)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'inscriptions:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', dossier)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
