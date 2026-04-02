<script setup lang="ts">
import type { Signature } from '../types/signature.types'

const { formatDate } = useFormatDate()

defineProps<{
  signature: Signature
}>()

const emit = defineEmits<{
  (e: 'edit', signature: Signature): void
  (e: 'delete', signature: Signature): void
}>()

const { t } = useI18n()

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    EnAttente: 'warning',
    Signe: 'success',
    Refuse: 'error',
    Delegue: 'info',
  }
  return colors[statut] ?? 'neutral'
}
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div>
        <h3 class="font-semibold text-gray-900 dark:text-white">
          {{ signature.name }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ signature.typeName }} - {{ $t(`documents.signature.niveau.${signature.niveauSignature}`) }}
        </p>
      </div>
      <UBadge
        :color="getStatutColor(signature.statutSignature)"
        variant="subtle"
        size="sm"
      >
        {{ $t(`documents.signature.statut.${signature.statutSignature}`) }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p v-if="signature.signataireNom">
        <UIcon
          name="i-heroicons-user"
          class="mr-1 inline h-4 w-4"
        />
        {{ signature.signataireNom }}
      </p>
      <p>
        <UIcon
          name="i-heroicons-queue-list"
          class="mr-1 inline h-4 w-4"
        />
        {{ $t('documents.signature.ordreLabel') }} {{ signature.ordreSignature }}
      </p>
      <p v-if="signature.dateSignature">
        <UIcon
          name="i-heroicons-calendar"
          class="mr-1 inline h-4 w-4"
        />
        {{ formatDate(signature.dateSignature) }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'documents:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', signature)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'documents:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', signature)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
