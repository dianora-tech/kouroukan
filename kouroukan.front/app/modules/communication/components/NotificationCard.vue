<script setup lang="ts">
import type { Notification } from '../types/notification.types'

defineProps<{
  notification: Notification
}>()

const emit = defineEmits<{
  (e: 'edit', notification: Notification): void
  (e: 'delete', notification: Notification): void
}>()

const { t } = useI18n()
const { formatDate } = useFormatDate()

function getStatutEnvoiColor(estEnvoyee: boolean): string {
  return estEnvoyee ? 'success' : 'warning'
}

function getCanalIcon(canal: string): string {
  const icons: Record<string, string> = {
    Push: 'i-heroicons-device-phone-mobile',
    SMS: 'i-heroicons-chat-bubble-left-right',
    Email: 'i-heroicons-envelope',
    InApp: 'i-heroicons-bell',
  }
  return icons[canal] ?? 'i-heroicons-bell'
}
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div class="flex items-center gap-3">
        <div class="flex h-10 w-10 items-center justify-center rounded-full bg-teal-100 text-sm font-semibold text-teal-700 dark:bg-teal-900 dark:text-teal-300">
          <UIcon :name="getCanalIcon(notification.canal)" class="h-5 w-5" />
        </div>
        <div>
          <h3 class="font-semibold text-gray-900 dark:text-white">
            {{ notification.contenu.substring(0, 60) }}{{ notification.contenu.length > 60 ? '...' : '' }}
          </h3>
          <p class="text-sm text-gray-500 dark:text-gray-400">
            {{ $t(`communication.notification.canal.${notification.canal}`) }}
          </p>
        </div>
      </div>
      <UBadge :color="getStatutEnvoiColor(notification.estEnvoyee)" variant="subtle" size="sm">
        {{ notification.estEnvoyee ? $t('communication.notification.envoyee') : $t('communication.notification.enAttente') }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p v-if="notification.typeName">
        <UIcon name="i-heroicons-tag" class="mr-1 inline h-4 w-4" />
        {{ notification.typeName }}
      </p>
      <p v-if="notification.dateEnvoi">
        <UIcon name="i-heroicons-clock" class="mr-1 inline h-4 w-4" />
        {{ formatDate(notification.dateEnvoi) }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'communication:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', notification)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'communication:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', notification)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
