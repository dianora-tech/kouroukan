<script setup lang="ts">
import { useForfaitGating } from '~/composables/useForfaitGating'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { isFeatureLocked } = useForfaitGating()
const isLocked = computed(() => isFeatureLocked('notes'))

const enfants = ref([
  { label: 'Aissatou Diallo - 3eme', value: '1' },
  { label: 'Ibrahim Diallo - CM2', value: '2' },
])

const selectedEnfant = ref('1')

const notes = ref([
  { id: 1, matiere: 'Mathematiques', note: 16, coefficient: 4, appreciation: 'Tres bien', enseignant: 'M. Barry' },
  { id: 2, matiere: 'Francais', note: 13, coefficient: 3, appreciation: 'Bien', enseignant: 'Mme Camara' },
  { id: 3, matiere: 'Physique-Chimie', note: 15, coefficient: 3, appreciation: 'Tres bien', enseignant: 'M. Sylla' },
  { id: 4, matiere: 'Histoire-Geo', note: 12, coefficient: 2, appreciation: 'Assez bien', enseignant: 'M. Bah' },
  { id: 5, matiere: 'Anglais', note: 14, coefficient: 2, appreciation: 'Bien', enseignant: 'Mme Sow' },
  { id: 6, matiere: 'SVT', note: 11, coefficient: 2, appreciation: 'Assez bien', enseignant: 'M. Diallo' },
])

const moyenne = computed(() => {
  const totalPondere = notes.value.reduce((sum, n) => sum + n.note * n.coefficient, 0)
  const totalCoeff = notes.value.reduce((sum, n) => sum + n.coefficient, 0)
  return (totalPondere / totalCoeff).toFixed(2)
})
</script>

<template>
  <ForfaitRequiredOverlay v-if="isLocked" user-type="famille" />
  <div v-else class="space-y-6">
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('famille.title'), to: '/famille' },
          { label: $t('famille.notes.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('famille.notes.title') }}
      </h1>
    </div>

    <!-- Selection enfant -->
    <USelect v-model="selectedEnfant" :items="enfants" class="w-72" />

    <!-- Moyenne -->
    <div class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800">
      <div class="flex items-center gap-3">
        <UIcon name="i-heroicons-academic-cap" class="h-8 w-8 text-indigo-600" />
        <div>
          <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ moyenne }}/20</p>
          <p class="text-sm text-gray-500">{{ $t('famille.notes.moyenneGenerale') }}</p>
        </div>
        <UBadge color="success" variant="subtle" size="sm" class="ml-auto">
          {{ $t('famille.notes.paye') }}
        </UBadge>
      </div>
    </div>

    <!-- Table notes -->
    <div class="overflow-x-auto rounded-xl border border-gray-200 bg-white dark:border-gray-700 dark:bg-gray-800">
      <table class="w-full text-left text-sm">
        <thead class="border-b border-gray-200 dark:border-gray-700">
          <tr>
            <th class="px-4 py-3 font-medium text-gray-500">{{ $t('famille.notes.matiere') }}</th>
            <th class="px-4 py-3 font-medium text-gray-500">{{ $t('famille.notes.note') }}</th>
            <th class="px-4 py-3 font-medium text-gray-500">{{ $t('famille.notes.coefficient') }}</th>
            <th class="px-4 py-3 font-medium text-gray-500">{{ $t('famille.notes.appreciation') }}</th>
            <th class="px-4 py-3 font-medium text-gray-500">{{ $t('famille.notes.enseignant') }}</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-100 dark:divide-gray-700">
          <tr v-for="n in notes" :key="n.id">
            <td class="px-4 py-3 text-gray-900 dark:text-white">{{ n.matiere }}</td>
            <td class="px-4 py-3">
              <span :class="n.note >= 14 ? 'text-green-600' : n.note >= 10 ? 'text-yellow-600' : 'text-red-600'" class="font-semibold">
                {{ n.note }}/20
              </span>
            </td>
            <td class="px-4 py-3 text-gray-600 dark:text-gray-400">{{ n.coefficient }}</td>
            <td class="px-4 py-3 text-gray-600 dark:text-gray-400">{{ n.appreciation }}</td>
            <td class="px-4 py-3 text-gray-600 dark:text-gray-400">{{ n.enseignant }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
