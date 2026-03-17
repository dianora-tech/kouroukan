<script setup lang="ts">
import type { Eleve } from '../types/eleve.types'

defineProps<{
  eleve: Eleve
}>()

const emit = defineEmits<{
  (e: 'edit', eleve: Eleve): void
  (e: 'delete', eleve: Eleve): void
}>()

const { t } = useI18n()

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    Prospect: 'info',
    PreInscrit: 'warning',
    Inscrit: 'success',
    Radie: 'error',
  }
  return colors[statut] ?? 'neutral'
}
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div class="flex items-center gap-3">
        <UAvatar
          v-if="eleve.photoUrl"
          :src="eleve.photoUrl"
          :alt="`${eleve.firstName} ${eleve.lastName}`"
          size="lg"
        />
        <div
          v-else
          class="flex h-10 w-10 items-center justify-center rounded-full bg-green-100 text-sm font-semibold text-green-700 dark:bg-green-900 dark:text-green-300"
        >
          {{ eleve.firstName[0] }}{{ eleve.lastName[0] }}
        </div>
        <div>
          <h3 class="font-semibold text-gray-900 dark:text-white">
            {{ eleve.lastName }} {{ eleve.firstName }}
          </h3>
          <p class="text-sm text-gray-500 dark:text-gray-400">
            {{ eleve.numeroMatricule }}
          </p>
        </div>
      </div>
      <UBadge :color="getStatutColor(eleve.statutInscription)" variant="subtle" size="sm">
        {{ $t(`inscriptions.eleve.statut.${eleve.statutInscription}`) }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon name="i-heroicons-cake" class="mr-1 inline h-4 w-4" />
        {{ eleve.dateNaissance }} - {{ eleve.lieuNaissance }}
      </p>
      <p v-if="eleve.niveauClasseName">
        <UIcon name="i-heroicons-academic-cap" class="mr-1 inline h-4 w-4" />
        {{ eleve.niveauClasseName }}
        <span v-if="eleve.classeName"> - {{ eleve.classeName }}</span>
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'inscriptions:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', eleve)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'inscriptions:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', eleve)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
