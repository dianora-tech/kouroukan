<script setup lang="ts">
import type { Message } from '../types/message.types'

defineProps<{
  message: Message
}>()

const emit = defineEmits<{
  (e: 'edit', message: Message): void
  (e: 'delete', message: Message): void
}>()

const { t } = useI18n()

function getStatutLuColor(estLu: boolean): string {
  return estLu ? 'success' : 'warning'
}
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div class="flex items-center gap-3">
        <div class="flex h-10 w-10 items-center justify-center rounded-full bg-teal-100 text-sm font-semibold text-teal-700 dark:bg-teal-900 dark:text-teal-300">
          <UIcon
            name="i-heroicons-envelope"
            class="h-5 w-5"
          />
        </div>
        <div>
          <h3 class="font-semibold text-gray-900 dark:text-white">
            {{ message.sujet }}
          </h3>
          <p class="text-sm text-gray-500 dark:text-gray-400">
            {{ message.expediteurNom ?? `#${message.expediteurId}` }}
          </p>
        </div>
      </div>
      <UBadge
        :color="getStatutLuColor(message.estLu)"
        variant="subtle"
        size="sm"
      >
        {{ message.estLu ? $t('communication.message.lu') : $t('communication.message.nonLu') }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p v-if="message.destinataireNom">
        <UIcon
          name="i-heroicons-user"
          class="mr-1 inline h-4 w-4"
        />
        {{ $t('communication.message.destinataire') }}: {{ message.destinataireNom }}
      </p>
      <p v-if="message.groupeDestinataire">
        <UIcon
          name="i-heroicons-user-group"
          class="mr-1 inline h-4 w-4"
        />
        {{ message.groupeDestinataire }}
      </p>
      <p v-if="message.typeName">
        <UIcon
          name="i-heroicons-tag"
          class="mr-1 inline h-4 w-4"
        />
        {{ message.typeName }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'communication:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', message)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'communication:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', message)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
