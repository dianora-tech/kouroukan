<script setup lang="ts">
import type { Seance } from '../types/seance.types'

defineProps<{
  seance: Seance
}>()

const emit = defineEmits<{
  (e: 'edit', seance: Seance): void
  (e: 'delete', seance: Seance): void
}>()

const { t } = useI18n()

function getJourColor(jour: number): string {
  const colors: Record<number, string> = {
    1: 'primary',
    2: 'success',
    3: 'warning',
    4: 'error',
    5: 'info',
    6: 'neutral',
  }
  return colors[jour] ?? 'neutral'
}
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div class="flex items-center gap-3">
        <div class="flex h-10 w-10 items-center justify-center rounded-full bg-indigo-100 text-indigo-700 dark:bg-indigo-900 dark:text-indigo-300">
          <UIcon
            name="i-heroicons-clock"
            class="h-5 w-5"
          />
        </div>
        <div>
          <h3 class="font-semibold text-gray-900 dark:text-white">
            {{ seance.matiereName ?? seance.matiereCode }}
          </h3>
          <p
            v-if="seance.classeName"
            class="text-sm text-gray-500 dark:text-gray-400"
          >
            {{ seance.classeName }}
          </p>
        </div>
      </div>
      <UBadge
        :color="getJourColor(seance.jourSemaine)"
        variant="subtle"
        size="sm"
      >
        {{ $t(`pedagogie.seance.jour.${seance.jourSemaine}`) }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon
          name="i-heroicons-clock"
          class="mr-1 inline h-4 w-4"
        />
        {{ seance.heureDebut }} - {{ seance.heureFin }}
      </p>
      <p v-if="seance.enseignantNom">
        <UIcon
          name="i-heroicons-user"
          class="mr-1 inline h-4 w-4"
        />
        {{ seance.enseignantNom }}
      </p>
      <p v-if="seance.salleName">
        <UIcon
          name="i-heroicons-building-office-2"
          class="mr-1 inline h-4 w-4"
        />
        {{ seance.salleName }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'pedagogie:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', seance)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'pedagogie:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', seance)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
