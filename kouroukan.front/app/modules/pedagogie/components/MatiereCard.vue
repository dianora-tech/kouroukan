<script setup lang="ts">
import type { Matiere } from '../types/matiere.types'

defineProps<{
  matiere: Matiere
}>()

const emit = defineEmits<{
  (e: 'edit', matiere: Matiere): void
  (e: 'delete', matiere: Matiere): void
}>()

const { t } = useI18n()
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div class="flex items-center gap-3">
        <div class="flex h-10 w-10 items-center justify-center rounded-full bg-purple-100 text-sm font-semibold text-purple-700 dark:bg-purple-900 dark:text-purple-300">
          {{ matiere.code }}
        </div>
        <div>
          <h3 class="font-semibold text-gray-900 dark:text-white">
            {{ matiere.name }}
          </h3>
          <p v-if="matiere.niveauClasseName" class="text-sm text-gray-500 dark:text-gray-400">
            {{ matiere.niveauClasseName }}
          </p>
        </div>
      </div>
      <UBadge v-if="matiere.typeName" color="primary" variant="subtle" size="sm">
        {{ matiere.typeName }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon name="i-heroicons-scale" class="mr-1 inline h-4 w-4" />
        {{ $t('pedagogie.matiere.coeffLabel', { coeff: matiere.coefficient }) }}
      </p>
      <p>
        <UIcon name="i-heroicons-clock" class="mr-1 inline h-4 w-4" />
        {{ $t('pedagogie.matiere.heuresLabel', { heures: matiere.nombreHeures }) }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'pedagogie:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', matiere)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'pedagogie:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', matiere)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
