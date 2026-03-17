<script setup lang="ts">
import type { NiveauClasse } from '../types/niveauClasse.types'

defineProps<{
  niveauClasse: NiveauClasse
}>()

const emit = defineEmits<{
  (e: 'edit', niveauClasse: NiveauClasse): void
  (e: 'delete', niveauClasse: NiveauClasse): void
}>()

const { t } = useI18n()

function getCycleColor(cycle: string): string {
  const colors: Record<string, string> = {
    Prescolaire: 'info',
    Primaire: 'success',
    College: 'warning',
    Lycee: 'error',
    ETFP_PostPrimaire: 'neutral',
    ETFP_TypeA: 'neutral',
    ETFP_TypeB: 'neutral',
    ENF: 'info',
    Universite: 'primary',
  }
  return colors[cycle] ?? 'neutral'
}
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div class="flex items-center gap-3">
        <div class="flex h-10 w-10 items-center justify-center rounded-full bg-blue-100 text-sm font-semibold text-blue-700 dark:bg-blue-900 dark:text-blue-300">
          {{ niveauClasse.code }}
        </div>
        <div>
          <h3 class="font-semibold text-gray-900 dark:text-white">
            {{ niveauClasse.name }}
          </h3>
          <p class="text-sm text-gray-500 dark:text-gray-400">
            {{ $t('pedagogie.niveauClasse.ordreLabel', { ordre: niveauClasse.ordre }) }}
          </p>
        </div>
      </div>
      <UBadge :color="getCycleColor(niveauClasse.cycleEtude)" variant="subtle" size="sm">
        {{ $t(`pedagogie.niveauClasse.cycle.${niveauClasse.cycleEtude}`) }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p v-if="niveauClasse.ministereTutelle">
        <UIcon name="i-heroicons-building-library" class="mr-1 inline h-4 w-4" />
        {{ niveauClasse.ministereTutelle }}
      </p>
      <p v-if="niveauClasse.ageOfficielEntree">
        <UIcon name="i-heroicons-user" class="mr-1 inline h-4 w-4" />
        {{ $t('pedagogie.niveauClasse.ageEntree', { age: niveauClasse.ageOfficielEntree }) }}
      </p>
      <p v-if="niveauClasse.examenSortie">
        <UIcon name="i-heroicons-document-check" class="mr-1 inline h-4 w-4" />
        {{ niveauClasse.examenSortie }}
      </p>
      <p v-if="niveauClasse.tauxHoraireEnseignant">
        <UIcon name="i-heroicons-banknotes" class="mr-1 inline h-4 w-4" />
        {{ niveauClasse.tauxHoraireEnseignant.toLocaleString() }} GNF/h
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'pedagogie:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', niveauClasse)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'pedagogie:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', niveauClasse)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
