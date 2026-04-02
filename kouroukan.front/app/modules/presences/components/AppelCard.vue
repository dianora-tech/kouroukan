<script setup lang="ts">
import type { Appel } from '../types/appel.types'

defineProps<{
  appel: Appel
}>()

const emit = defineEmits<{
  (e: 'edit', appel: Appel): void
  (e: 'delete', appel: Appel): void
}>()

const { t } = useI18n()
const { formatDate } = useFormatDate()
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div>
        <h3 class="font-semibold text-gray-900 dark:text-white">
          {{ appel.classeName ?? `#${appel.classeId}` }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ appel.enseignantNom ?? `#${appel.enseignantId}` }}
        </p>
      </div>
      <UBadge :color="appel.estCloture ? 'success' : 'warning'" variant="subtle" size="sm">
        {{ appel.estCloture ? t('presences.appel.cloture') : t('presences.appel.enCours') }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon name="i-heroicons-calendar" class="mr-1 inline h-4 w-4" />
        {{ formatDate(appel.dateAppel) }}
      </p>
      <p>
        <UIcon name="i-heroicons-clock" class="mr-1 inline h-4 w-4" />
        {{ appel.heureAppel }}
      </p>
      <p v-if="appel.seanceId">
        <UIcon name="i-heroicons-book-open" class="mr-1 inline h-4 w-4" />
        {{ appel.seanceInfo ?? `${t('presences.appel.seanceId')}: #${appel.seanceId}` }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'presences:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', appel)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'presences:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', appel)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
