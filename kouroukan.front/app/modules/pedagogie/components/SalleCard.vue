<script setup lang="ts">
import type { Salle } from '../types/salle.types'

defineProps<{
  salle: Salle
}>()

const emit = defineEmits<{
  (e: 'edit', salle: Salle): void
  (e: 'delete', salle: Salle): void
}>()

const { t } = useI18n()
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div class="flex items-center gap-3">
        <div class="flex h-10 w-10 items-center justify-center rounded-full bg-cyan-100 text-cyan-700 dark:bg-cyan-900 dark:text-cyan-300">
          <UIcon
            name="i-heroicons-building-office-2"
            class="h-5 w-5"
          />
        </div>
        <div>
          <h3 class="font-semibold text-gray-900 dark:text-white">
            {{ salle.name }}
          </h3>
          <p
            v-if="salle.typeName"
            class="text-sm text-gray-500 dark:text-gray-400"
          >
            {{ salle.typeName }}
          </p>
        </div>
      </div>
      <UBadge
        :color="salle.estDisponible ? 'success' : 'error'"
        variant="subtle"
        size="sm"
      >
        {{ salle.estDisponible ? $t('pedagogie.salle.disponible') : $t('pedagogie.salle.indisponible') }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon
          name="i-heroicons-users"
          class="mr-1 inline h-4 w-4"
        />
        {{ $t('pedagogie.salle.capaciteLabel', { capacite: salle.capacite }) }}
      </p>
      <p v-if="salle.batiment">
        <UIcon
          name="i-heroicons-building-library"
          class="mr-1 inline h-4 w-4"
        />
        {{ salle.batiment }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'pedagogie:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', salle)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'pedagogie:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', salle)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
