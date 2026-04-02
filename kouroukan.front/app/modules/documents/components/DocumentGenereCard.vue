<script setup lang="ts">
import type { DocumentGenere } from '../types/document-genere.types'

const { formatDate } = useFormatDate()

defineProps<{
  documentGenere: DocumentGenere
}>()

const emit = defineEmits<{
  (e: 'edit', documentGenere: DocumentGenere): void
  (e: 'delete', documentGenere: DocumentGenere): void
}>()

const { t } = useI18n()

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    EnAttente: 'warning',
    EnCours: 'info',
    Signe: 'success',
    Refuse: 'error',
  }
  return colors[statut] ?? 'neutral'
}
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div>
        <h3 class="font-semibold text-gray-900 dark:text-white">
          {{ documentGenere.name }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ documentGenere.typeName }}
        </p>
      </div>
      <UBadge :color="getStatutColor(documentGenere.statutSignature)" variant="subtle" size="sm">
        {{ $t(`documents.documentGenere.statut.${documentGenere.statutSignature}`) }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p v-if="documentGenere.modeleDocumentNom">
        <UIcon name="i-heroicons-document-duplicate" class="mr-1 inline h-4 w-4" />
        {{ documentGenere.modeleDocumentNom }}
      </p>
      <p v-if="documentGenere.eleveNom">
        <UIcon name="i-heroicons-academic-cap" class="mr-1 inline h-4 w-4" />
        {{ documentGenere.eleveNom }}
      </p>
      <p>
        <UIcon name="i-heroicons-calendar" class="mr-1 inline h-4 w-4" />
        {{ formatDate(documentGenere.dateGeneration) }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'documents:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', documentGenere)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'documents:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', documentGenere)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
