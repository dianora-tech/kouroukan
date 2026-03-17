<script setup lang="ts">
import type { Annonce } from '../types/annonce.types'

defineProps<{
  annonce: Annonce
}>()

const emit = defineEmits<{
  (e: 'edit', annonce: Annonce): void
  (e: 'delete', annonce: Annonce): void
}>()

const { t } = useI18n()

function getStatutColor(estActive: boolean): string {
  return estActive ? 'success' : 'neutral'
}

function getPrioriteColor(priorite: number): string {
  if (priorite <= 1) return 'error'
  if (priorite <= 2) return 'warning'
  return 'info'
}
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div class="flex items-center gap-3">
        <div class="flex h-10 w-10 items-center justify-center rounded-full bg-teal-100 text-sm font-semibold text-teal-700 dark:bg-teal-900 dark:text-teal-300">
          <UIcon name="i-heroicons-megaphone" class="h-5 w-5" />
        </div>
        <div>
          <h3 class="font-semibold text-gray-900 dark:text-white">
            {{ annonce.contenu.substring(0, 50) }}{{ annonce.contenu.length > 50 ? '...' : '' }}
          </h3>
          <p v-if="annonce.typeName" class="text-sm text-gray-500 dark:text-gray-400">
            {{ annonce.typeName }}
          </p>
        </div>
      </div>
      <div class="flex gap-1">
        <UBadge :color="getStatutColor(annonce.estActive)" variant="subtle" size="sm">
          {{ annonce.estActive ? $t('communication.annonce.active') : $t('communication.annonce.inactive') }}
        </UBadge>
        <UBadge :color="getPrioriteColor(annonce.priorite)" variant="subtle" size="sm">
          P{{ annonce.priorite }}
        </UBadge>
      </div>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon name="i-heroicons-calendar" class="mr-1 inline h-4 w-4" />
        {{ annonce.dateDebut }}
        <span v-if="annonce.dateFin"> - {{ annonce.dateFin }}</span>
      </p>
      <p>
        <UIcon name="i-heroicons-users" class="mr-1 inline h-4 w-4" />
        {{ $t(`communication.annonce.cible.${annonce.cibleAudience}`) }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'communication:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', annonce)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'communication:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', annonce)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
