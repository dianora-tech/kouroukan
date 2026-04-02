<script setup lang="ts">
import type { Absence } from '../types/absence.types'

defineProps<{
  absence: Absence
}>()

const emit = defineEmits<{
  (e: 'edit', absence: Absence): void
  (e: 'delete', absence: Absence): void
}>()

const { t } = useI18n()
const { formatDate } = useFormatDate()
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div>
        <h3 class="font-semibold text-gray-900 dark:text-white">
          {{ absence.eleveNom ?? `#${absence.eleveId}` }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ absence.typeName }}
        </p>
      </div>
      <UBadge
        :color="absence.estJustifiee ? 'success' : 'error'"
        variant="subtle"
        size="sm"
      >
        {{ absence.estJustifiee ? t('presences.absence.justifiee') : t('presences.absence.nonJustifiee') }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon
          name="i-heroicons-calendar"
          class="mr-1 inline h-4 w-4"
        />
        {{ formatDate(absence.dateAbsence) }}
      </p>
      <p v-if="absence.heureDebut || absence.heureFin">
        <UIcon
          name="i-heroicons-clock"
          class="mr-1 inline h-4 w-4"
        />
        {{ absence.heureDebut ?? '--:--' }} - {{ absence.heureFin ?? '--:--' }}
      </p>
      <p v-if="absence.motifJustification">
        <UIcon
          name="i-heroicons-document-text"
          class="mr-1 inline h-4 w-4"
        />
        {{ absence.motifJustification }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'presences:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', absence)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'presences:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', absence)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
