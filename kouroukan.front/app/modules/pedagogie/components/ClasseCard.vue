<script setup lang="ts">
import type { Classe } from '../types/classe.types'

defineProps<{
  classe: Classe
}>()

const emit = defineEmits<{
  (e: 'edit', classe: Classe): void
  (e: 'delete', classe: Classe): void
}>()

const { t } = useI18n()

function getOccupancyColor(effectif: number, capacite: number): string {
  const ratio = effectif / capacite
  if (ratio >= 0.9) return 'error'
  if (ratio >= 0.7) return 'warning'
  return 'success'
}
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div class="flex items-center gap-3">
        <div class="flex h-10 w-10 items-center justify-center rounded-full bg-blue-100 text-sm font-semibold text-blue-700 dark:bg-blue-900 dark:text-blue-300">
          <UIcon
            name="i-heroicons-user-group"
            class="h-5 w-5"
          />
        </div>
        <div>
          <h3 class="font-semibold text-gray-900 dark:text-white">
            {{ classe.name }}
          </h3>
          <p
            v-if="classe.niveauClasseName"
            class="text-sm text-gray-500 dark:text-gray-400"
          >
            {{ classe.niveauClasseName }}
          </p>
        </div>
      </div>
      <UBadge
        :color="getOccupancyColor(classe.effectif, classe.capacite)"
        variant="subtle"
        size="sm"
      >
        {{ classe.effectif }}/{{ classe.capacite }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p v-if="classe.anneeScolaireLibelle">
        <UIcon
          name="i-heroicons-calendar"
          class="mr-1 inline h-4 w-4"
        />
        {{ classe.anneeScolaireLibelle }}
      </p>
      <p v-if="classe.enseignantPrincipalNom">
        <UIcon
          name="i-heroicons-user"
          class="mr-1 inline h-4 w-4"
        />
        {{ classe.enseignantPrincipalNom }}
      </p>
      <p>
        <UIcon
          name="i-heroicons-users"
          class="mr-1 inline h-4 w-4"
        />
        {{ $t('pedagogie.classe.effectifLabel', { effectif: classe.effectif, capacite: classe.capacite }) }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'pedagogie:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', classe)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'pedagogie:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', classe)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
