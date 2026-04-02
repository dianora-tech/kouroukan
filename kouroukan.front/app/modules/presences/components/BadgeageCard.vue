<script setup lang="ts">
import type { Badgeage } from '../types/badgeage.types'

defineProps<{
  badgeage: Badgeage
}>()

const emit = defineEmits<{
  (e: 'edit', badgeage: Badgeage): void
  (e: 'delete', badgeage: Badgeage): void
}>()

const { t } = useI18n()
const { formatDate } = useFormatDate()
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div>
        <h3 class="font-semibold text-gray-900 dark:text-white">
          {{ badgeage.eleveNom ?? `#${badgeage.eleveId}` }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ badgeage.typeName }}
        </p>
      </div>
      <UBadge color="primary" variant="subtle" size="sm">
        {{ badgeage.methodeBadgeage }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon name="i-heroicons-calendar" class="mr-1 inline h-4 w-4" />
        {{ formatDate(badgeage.dateBadgeage) }}
      </p>
      <p>
        <UIcon name="i-heroicons-clock" class="mr-1 inline h-4 w-4" />
        {{ badgeage.heureBadgeage }}
      </p>
      <p>
        <UIcon name="i-heroicons-map-pin" class="mr-1 inline h-4 w-4" />
        {{ t(`presences.badgeage.pointAcces.${badgeage.pointAcces}`) }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'presences:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', badgeage)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'presences:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', badgeage)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
