<script setup lang="ts">
import type { Evaluation } from '../types/evaluation.types'

defineProps<{
  evaluation: Evaluation
}>()

const emit = defineEmits<{
  (e: 'edit', evaluation: Evaluation): void
  (e: 'delete', evaluation: Evaluation): void
}>()

const { t } = useI18n()
const { formatDate } = useFormatDate()
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div>
        <h3 class="font-semibold text-gray-900 dark:text-white">
          {{ evaluation.typeName }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ evaluation.matiereName ?? `#${evaluation.matiereId}` }} - {{ evaluation.classeName ?? `#${evaluation.classeId}` }}
        </p>
      </div>
      <UBadge
        color="primary"
        variant="subtle"
        size="sm"
      >
        {{ $t(`evaluations.evaluation.trimestre.${evaluation.trimestre}`) }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon
          name="i-heroicons-calendar"
          class="mr-1 inline h-4 w-4"
        />
        {{ formatDate(evaluation.dateEvaluation) }}
      </p>
      <p>
        <UIcon
          name="i-heroicons-academic-cap"
          class="mr-1 inline h-4 w-4"
        />
        {{ $t('evaluations.evaluation.coefficient') }}: {{ evaluation.coefficient }} | {{ $t('evaluations.evaluation.noteMaximale') }}: {{ evaluation.noteMaximale }}
      </p>
      <p>
        <UIcon
          name="i-heroicons-user"
          class="mr-1 inline h-4 w-4"
        />
        {{ evaluation.enseignantNom ?? `#${evaluation.enseignantId}` }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'evaluations:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', evaluation)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'evaluations:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', evaluation)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
