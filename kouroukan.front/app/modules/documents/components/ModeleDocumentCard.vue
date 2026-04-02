<script setup lang="ts">
import type { ModeleDocument } from '../types/modele-document.types'

defineProps<{
  modeleDocument: ModeleDocument
}>()

const emit = defineEmits<{
  (e: 'edit', modeleDocument: ModeleDocument): void
  (e: 'delete', modeleDocument: ModeleDocument): void
}>()

const { t } = useI18n()
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div>
        <h3 class="font-semibold text-gray-900 dark:text-white">
          {{ modeleDocument.name }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ modeleDocument.typeName }}
        </p>
      </div>
      <UBadge
        :color="modeleDocument.estActif ? 'success' : 'neutral'"
        variant="subtle"
        size="sm"
      >
        {{ modeleDocument.estActif ? $t('documents.modeleDocument.actif') : $t('documents.modeleDocument.inactif') }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon
          name="i-heroicons-code-bracket"
          class="mr-1 inline h-4 w-4"
        />
        {{ modeleDocument.code }}
      </p>
      <p v-if="modeleDocument.description">
        <UIcon
          name="i-heroicons-document-text"
          class="mr-1 inline h-4 w-4"
        />
        {{ modeleDocument.description }}
      </p>
      <p v-if="modeleDocument.couleurPrimaire">
        <span
          class="mr-1 inline-block h-3 w-3 rounded-full"
          :style="{ backgroundColor: modeleDocument.couleurPrimaire }"
        />
        {{ modeleDocument.couleurPrimaire }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'documents:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', modeleDocument)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'documents:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', modeleDocument)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
