<script setup lang="ts">
import type { Bulletin } from '../types/bulletin.types'

const { formatDate } = useFormatDate()

defineProps<{
  bulletin: Bulletin
}>()

const emit = defineEmits<{
  (e: 'edit', bulletin: Bulletin): void
  (e: 'delete', bulletin: Bulletin): void
}>()

const { t } = useI18n()

function getMoyenneColor(moyenne: number): string {
  if (moyenne >= 14) return 'success'
  if (moyenne >= 10) return 'warning'
  return 'error'
}
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div>
        <h3 class="font-semibold text-gray-900 dark:text-white">
          {{ bulletin.eleveNom ?? `#${bulletin.eleveId}` }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ bulletin.classeName ?? `#${bulletin.classeId}` }} - {{ $t(`evaluations.bulletin.trimestre.${bulletin.trimestre}`) }}
        </p>
      </div>
      <UBadge :color="getMoyenneColor(bulletin.moyenneGenerale)" variant="subtle" size="sm">
        {{ bulletin.moyenneGenerale.toFixed(2) }} / 20
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p v-if="bulletin.rang">
        <UIcon name="i-heroicons-trophy" class="mr-1 inline h-4 w-4" />
        {{ $t('evaluations.bulletin.rangLabel') }}: {{ bulletin.rang }}
      </p>
      <p>
        <UIcon name="i-heroicons-calendar" class="mr-1 inline h-4 w-4" />
        {{ formatDate(bulletin.dateGeneration) }}
      </p>
      <p>
        <UBadge v-if="bulletin.estPublie" color="success" variant="subtle" size="xs">
          {{ $t('evaluations.bulletin.publie') }}
        </UBadge>
        <UBadge v-else color="warning" variant="subtle" size="xs">
          {{ $t('evaluations.bulletin.nonPublie') }}
        </UBadge>
      </p>
      <p v-if="bulletin.appreciation" class="truncate">
        <UIcon name="i-heroicons-chat-bubble-left" class="mr-1 inline h-4 w-4" />
        {{ bulletin.appreciation }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'evaluations:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', bulletin)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'evaluations:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', bulletin)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
