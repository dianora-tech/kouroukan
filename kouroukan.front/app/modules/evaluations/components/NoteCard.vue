<script setup lang="ts">
import type { Note } from '../types/note.types'

const { formatDate } = useFormatDate()

defineProps<{
  note: Note
}>()

const emit = defineEmits<{
  (e: 'edit', note: Note): void
  (e: 'delete', note: Note): void
}>()

const { t } = useI18n()

function getNoteColor(valeur: number, noteMax: number | undefined): string {
  const max = noteMax ?? 20
  const ratio = valeur / max
  if (ratio >= 0.7) return 'success'
  if (ratio >= 0.5) return 'warning'
  return 'error'
}
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div>
        <h3 class="font-semibold text-gray-900 dark:text-white">
          {{ note.eleveNom ?? `#${note.eleveId}` }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ note.matiereName ?? note.evaluationTypeName ?? `Eval #${note.evaluationId}` }}
        </p>
      </div>
      <UBadge
        :color="getNoteColor(note.valeur, note.noteMaximale)"
        variant="subtle"
        size="sm"
      >
        {{ note.valeur }} / {{ note.noteMaximale ?? 20 }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon
          name="i-heroicons-calendar"
          class="mr-1 inline h-4 w-4"
        />
        {{ formatDate(note.dateSaisie) }}
      </p>
      <p v-if="note.commentaire">
        <UIcon
          name="i-heroicons-chat-bubble-left"
          class="mr-1 inline h-4 w-4"
        />
        {{ note.commentaire }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'evaluations:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', note)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'evaluations:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', note)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
