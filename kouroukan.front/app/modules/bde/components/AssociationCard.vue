<script setup lang="ts">
import type { Association } from '../types/association.types'

defineProps<{
  association: Association
}>()

const emit = defineEmits<{
  (e: 'edit', association: Association): void
  (e: 'delete', association: Association): void
}>()

const { t } = useI18n()

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    Active: 'success',
    Suspendue: 'warning',
    Dissoute: 'error',
  }
  return colors[statut] ?? 'neutral'
}

function formatMontant(montant: number): string {
  return new Intl.NumberFormat('fr-GN', { style: 'decimal' }).format(montant) + ' GNF'
}
</script>

<template>
  <div class="rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-900">
    <div class="flex items-start justify-between">
      <div>
        <h3 class="font-semibold text-gray-900 dark:text-white">
          {{ association.name }}
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ association.typeName }}
          <template v-if="association.sigle">
            ({{ association.sigle }})
          </template>
        </p>
      </div>
      <UBadge
        :color="getStatutColor(association.statut)"
        variant="subtle"
        size="sm"
      >
        {{ $t(`bde.association.statut.${association.statut}`) }}
      </UBadge>
    </div>

    <div class="mt-3 space-y-1 text-sm text-gray-600 dark:text-gray-400">
      <p>
        <UIcon
          name="i-heroicons-banknotes"
          class="mr-1 inline h-4 w-4"
        />
        {{ formatMontant(association.budgetAnnuel) }}
      </p>
      <p>
        <UIcon
          name="i-heroicons-calendar"
          class="mr-1 inline h-4 w-4"
        />
        {{ association.anneeScolaire }}
      </p>
      <p v-if="association.superviseurNom">
        <UIcon
          name="i-heroicons-user"
          class="mr-1 inline h-4 w-4"
        />
        {{ association.superviseurNom }}
      </p>
    </div>

    <div class="mt-4 flex justify-end gap-2">
      <UButton
        v-permission="'bde:update'"
        variant="ghost"
        size="xs"
        icon="i-heroicons-pencil-square"
        @click="emit('edit', association)"
      >
        {{ $t('actions.edit') }}
      </UButton>
      <UButton
        v-permission="'bde:delete'"
        variant="ghost"
        size="xs"
        color="error"
        icon="i-heroicons-trash"
        @click="emit('delete', association)"
      >
        {{ $t('actions.delete') }}
      </UButton>
    </div>
  </div>
</template>
