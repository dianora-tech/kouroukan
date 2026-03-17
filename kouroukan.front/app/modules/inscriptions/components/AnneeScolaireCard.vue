<script setup lang="ts">
import type { AnneeScolaire } from '../types/annee-scolaire.types'

defineProps<{
  annee: AnneeScolaire
}>()

const emit = defineEmits<{
  (e: 'edit', annee: AnneeScolaire): void
  (e: 'delete', annee: AnneeScolaire): void
}>()

const { t } = useI18n()
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div>
        <h3 class="font-semibold text-gray-900 dark:text-white">
          {{ annee.libelle }}
        </h3>
      </div>
      <UBadge v-if="annee.estActive" color="success" variant="subtle" size="sm">
        {{ $t('inscriptions.anneeScolaire.active') }}
      </UBadge>
      <UBadge v-else color="neutral" variant="subtle" size="sm">
        {{ $t('inscriptions.anneeScolaire.inactive') }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon name="i-heroicons-calendar" class="mr-1 inline h-4 w-4" />
        {{ annee.dateDebut }} - {{ annee.dateFin }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'inscriptions:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', annee)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'inscriptions:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', annee)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
