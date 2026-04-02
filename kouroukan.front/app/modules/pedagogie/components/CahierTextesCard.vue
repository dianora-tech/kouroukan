<script setup lang="ts">
import type { CahierTextes } from '../types/cahierTextes.types'

defineProps<{
  cahier: CahierTextes
}>()

const emit = defineEmits<{
  (e: 'edit', cahier: CahierTextes): void
  (e: 'delete', cahier: CahierTextes): void
}>()

const { t } = useI18n()
const { formatDate } = useFormatDate()
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div class="flex items-center gap-3">
        <div class="flex h-10 w-10 items-center justify-center rounded-full bg-emerald-100 text-emerald-700 dark:bg-emerald-900 dark:text-emerald-300">
          <UIcon
            name="i-heroicons-document-text"
            class="h-5 w-5"
          />
        </div>
        <div>
          <h3 class="font-semibold text-gray-900 dark:text-white">
            {{ cahier.matiereName }}
          </h3>
          <p
            v-if="cahier.classeName"
            class="text-sm text-gray-500 dark:text-gray-400"
          >
            {{ cahier.classeName }}
          </p>
        </div>
      </div>
      <span class="text-sm text-gray-500 dark:text-gray-400">
        {{ formatDate(cahier.dateSeance) }}
      </span>
    </div>

    <div class="mt-3 space-y-2">
      <p class="line-clamp-3 text-sm text-gray-600 dark:text-gray-400">
        {{ cahier.contenu }}
      </p>
      <div
        v-if="cahier.travailAFaire"
        class="rounded-md bg-amber-50 p-2 dark:bg-amber-900/20"
      >
        <p class="text-xs font-medium text-amber-700 dark:text-amber-300">
          <UIcon
            name="i-heroicons-pencil"
            class="mr-1 inline h-3 w-3"
          />
          {{ $t('pedagogie.cahierTextes.travailAFaire') }}
        </p>
        <p class="mt-1 line-clamp-2 text-xs text-amber-600 dark:text-amber-400">
          {{ cahier.travailAFaire }}
        </p>
      </div>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'pedagogie:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', cahier)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'pedagogie:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', cahier)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
