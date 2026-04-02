<script setup lang="ts">
import type { Enseignant } from '../types/enseignant.types'

defineProps<{
  enseignant: Enseignant
}>()

const emit = defineEmits<{
  (e: 'edit', enseignant: Enseignant): void
  (e: 'delete', enseignant: Enseignant): void
}>()

const { t } = useI18n()
const { formatDate } = useFormatDate()

function getStatutColor(statut: string): string {
  switch (statut) {
    case 'Actif': return 'success'
    case 'EnConge': return 'warning'
    case 'Suspendu': return 'error'
    case 'Inactif': return 'neutral'
    default: return 'neutral'
  }
}
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div>
        <h3 class="font-semibold text-gray-900 dark:text-white">
          {{ enseignant.matricule }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ enseignant.specialite }}
        </p>
      </div>
      <UBadge
        :color="getStatutColor(enseignant.statutEnseignant)"
        variant="subtle"
        size="sm"
      >
        {{ $t(`personnel.enseignant.statut.${enseignant.statutEnseignant}`) }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon
          name="i-heroicons-phone"
          class="mr-1 inline h-4 w-4"
        />
        {{ enseignant.telephone }}
      </p>
      <p>
        <UIcon
          name="i-heroicons-calendar"
          class="mr-1 inline h-4 w-4"
        />
        {{ $t('personnel.enseignant.dateEmbauche') }}: {{ formatDate(enseignant.dateEmbauche) }}
      </p>
      <p>
        <UIcon
          name="i-heroicons-banknotes"
          class="mr-1 inline h-4 w-4"
        />
        {{ $t(`personnel.enseignant.modeRemuneration.${enseignant.modeRemuneration}`) }}
        <template v-if="enseignant.montantForfait">
          — {{ enseignant.montantForfait.toLocaleString() }} GNF
        </template>
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'personnel:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', enseignant)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'personnel:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', enseignant)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
