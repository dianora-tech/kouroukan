<script setup lang="ts">
import { useForfaitGating } from '~/composables/useForfaitGating'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { formatDate } = useFormatDate()
const { isFeatureLocked } = useForfaitGating()
const isLocked = computed(() => isFeatureLocked('bulletins'))

const bulletins = ref([
  { id: 1, etablissement: 'Lycee Kwame Nkrumah', classe: 'TS', trimestre: 'Trimestre 1', date: '2024-12-15', statut: 'genere' },
  { id: 2, etablissement: 'Lycee Kwame Nkrumah', classe: '1S', trimestre: 'Trimestre 1', date: '2024-12-15', statut: 'genere' },
  { id: 3, etablissement: 'College Soundjata Keita', classe: '3eme', trimestre: 'Trimestre 1', date: '2024-12-20', statut: 'en_attente' },
  { id: 4, etablissement: 'Lycee Kwame Nkrumah', classe: 'TS', trimestre: 'Trimestre 2', date: '2025-03-15', statut: 'en_attente' },
])

const statutColor: Record<string, 'success' | 'warning'> = {
  genere: 'success',
  en_attente: 'warning',
}
</script>

<template>
  <ForfaitRequiredOverlay
    v-if="isLocked"
    user-type="enseignant"
  />
  <div
    v-else
    class="space-y-6"
  >
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('enseignant.title'), to: '/enseignant' },
          { label: $t('enseignant.bulletins.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('enseignant.bulletins.title') }}
      </h1>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
        {{ $t('enseignant.bulletins.subtitle') }}
      </p>
    </div>

    <div class="overflow-x-auto rounded-xl border border-gray-200 bg-white dark:border-gray-700 dark:bg-gray-800">
      <table class="w-full text-left text-sm">
        <thead class="border-b border-gray-200 dark:border-gray-700">
          <tr>
            <th class="px-4 py-3 font-medium text-gray-500">
              {{ $t('enseignant.bulletins.etablissement') }}
            </th>
            <th class="px-4 py-3 font-medium text-gray-500">
              {{ $t('enseignant.bulletins.classe') }}
            </th>
            <th class="px-4 py-3 font-medium text-gray-500">
              {{ $t('enseignant.bulletins.trimestre') }}
            </th>
            <th class="px-4 py-3 font-medium text-gray-500">
              {{ $t('enseignant.bulletins.date') }}
            </th>
            <th class="px-4 py-3 font-medium text-gray-500">
              {{ $t('enseignant.bulletins.statut') }}
            </th>
            <th class="px-4 py-3 font-medium text-gray-500 w-24" />
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-100 dark:divide-gray-700">
          <tr
            v-for="b in bulletins"
            :key="b.id"
          >
            <td class="px-4 py-3 text-gray-900 dark:text-white">
              {{ b.etablissement }}
            </td>
            <td class="px-4 py-3 text-gray-600 dark:text-gray-400">
              {{ b.classe }}
            </td>
            <td class="px-4 py-3 text-gray-600 dark:text-gray-400">
              {{ b.trimestre }}
            </td>
            <td class="px-4 py-3 text-gray-600 dark:text-gray-400">
              {{ formatDate(b.date) }}
            </td>
            <td class="px-4 py-3">
              <UBadge
                :color="statutColor[b.statut]"
                variant="subtle"
                size="sm"
              >
                {{ $t(`enseignant.bulletins.${b.statut}`) }}
              </UBadge>
            </td>
            <td class="px-4 py-3">
              <UButton
                v-if="b.statut === 'genere'"
                variant="ghost"
                size="xs"
                icon="i-heroicons-arrow-down-tray"
              />
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
