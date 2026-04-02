<script setup lang="ts">
import { useForfaitGating } from '~/composables/useForfaitGating'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { formatDate } = useFormatDate()
const { isFeatureLocked } = useForfaitGating()
const isLocked = computed(() => isFeatureLocked('heures'))

const heures = ref([
  { id: 1, date: '2025-03-03', etablissement: 'Lycee Kwame Nkrumah', classe: 'TS', matiere: 'Mathematiques', duree: 2 },
  { id: 2, date: '2025-03-03', etablissement: 'College Soundjata Keita', classe: '3eme', matiere: 'Physique', duree: 2 },
  { id: 3, date: '2025-03-04', etablissement: 'Lycee Kwame Nkrumah', classe: '1S', matiere: 'Mathematiques', duree: 2 },
  { id: 4, date: '2025-03-05', etablissement: 'Lycee Kwame Nkrumah', classe: '2nde', matiere: 'Mathematiques', duree: 2 },
  { id: 5, date: '2025-03-06', etablissement: 'College Soundjata Keita', classe: '4eme', matiere: 'Physique', duree: 2 },
  { id: 6, date: '2025-03-07', etablissement: 'Lycee Kwame Nkrumah', classe: 'TS', matiere: 'Mathematiques', duree: 2 },
  { id: 7, date: '2025-03-10', etablissement: 'Lycee Kwame Nkrumah', classe: 'TS', matiere: 'Mathematiques', duree: 2 },
  { id: 8, date: '2025-03-10', etablissement: 'College Soundjata Keita', classe: '3eme', matiere: 'Physique', duree: 2 },
])

const totalMois = computed(() => heures.value.reduce((sum, h) => sum + h.duree, 0))
</script>

<template>
  <ForfaitRequiredOverlay v-if="isLocked" user-type="enseignant" />
  <div v-else class="space-y-6">
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('enseignant.title'), to: '/enseignant' },
          { label: $t('enseignant.heures.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('enseignant.heures.title') }}
      </h1>
    </div>

    <!-- Total -->
    <div class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800">
      <div class="flex items-center gap-3">
        <UIcon name="i-heroicons-clock" class="h-8 w-8 text-green-600" />
        <div>
          <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ totalMois }}h</p>
          <p class="text-sm text-gray-500">{{ $t('enseignant.heures.totalMois') }}</p>
        </div>
      </div>
    </div>

    <!-- Table -->
    <div class="overflow-x-auto rounded-xl border border-gray-200 bg-white dark:border-gray-700 dark:bg-gray-800">
      <table class="w-full text-left text-sm">
        <thead class="border-b border-gray-200 dark:border-gray-700">
          <tr>
            <th class="px-4 py-3 font-medium text-gray-500">{{ $t('enseignant.heures.date') }}</th>
            <th class="px-4 py-3 font-medium text-gray-500">{{ $t('enseignant.heures.etablissement') }}</th>
            <th class="px-4 py-3 font-medium text-gray-500">{{ $t('enseignant.heures.classe') }}</th>
            <th class="px-4 py-3 font-medium text-gray-500">{{ $t('enseignant.heures.matiere') }}</th>
            <th class="px-4 py-3 font-medium text-gray-500">{{ $t('enseignant.heures.duree') }}</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-100 dark:divide-gray-700">
          <tr v-for="h in heures" :key="h.id">
            <td class="px-4 py-3 text-gray-900 dark:text-white">{{ formatDate(h.date) }}</td>
            <td class="px-4 py-3 text-gray-600 dark:text-gray-400">{{ h.etablissement }}</td>
            <td class="px-4 py-3 text-gray-600 dark:text-gray-400">{{ h.classe }}</td>
            <td class="px-4 py-3 text-gray-900 dark:text-white">{{ h.matiere }}</td>
            <td class="px-4 py-3 text-gray-900 dark:text-white">{{ h.duree }}h</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
