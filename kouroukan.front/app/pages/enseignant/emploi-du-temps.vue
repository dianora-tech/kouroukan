<script setup lang="ts">
import { useAffectationStore } from '~/modules/enseignant/stores/affectation.store'
import type { AffectationEnseignant } from '~/modules/enseignant/types/enseignant.types'
import { useForfaitGating } from '~/composables/useForfaitGating'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { isFeatureLocked } = useForfaitGating()
const isLocked = computed(() => isFeatureLocked('emploi-du-temps'))
const store = useAffectationStore()

const loading = computed(() => store.loading)
const affectations = computed(() => store.items)

const selectedEtablissement = ref('all')

const etablissements = computed(() => {
  const etabs = [...new Set(affectations.value.map(a => a.etablissement))]
  return [
    { label: 'Tous', value: 'all' },
    ...etabs.map(e => ({ label: e, value: e })),
  ]
})

const jours = ['Lundi', 'Mardi', 'Mercredi', 'Jeudi', 'Vendredi']
const joursMap: Record<string, number> = { Lundi: 1, Mardi: 2, Mercredi: 3, Jeudi: 4, Vendredi: 5 }
const creneaux = ['08:00', '10:00', '12:00', '14:00', '16:00']

const filteredAffectations = computed(() => {
  if (selectedEtablissement.value === 'all') return affectations.value
  return affectations.value.filter(e => e.etablissement === selectedEtablissement.value)
})

function getCours(jour: string, heure: string): AffectationEnseignant | undefined {
  const jourNum = joursMap[jour]
  return filteredAffectations.value.find(e => e.jourSemaine === jourNum && e.heureDebut === heure)
}

onMounted(() => {
  store.fetchAll()
})
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
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('enseignant.title'), to: '/enseignant' },
            { label: $t('enseignant.emploiDuTemps.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('enseignant.emploiDuTemps.title') }}
        </h1>
      </div>
      <USelect
        v-model="selectedEtablissement"
        :items="etablissements"
        class="w-64"
      />
    </div>

    <!-- Loading state -->
    <div
      v-if="loading"
      class="flex items-center justify-center py-12"
    >
      <UIcon
        name="i-heroicons-arrow-path"
        class="h-8 w-8 animate-spin text-gray-400"
      />
    </div>

    <!-- Calendar grid -->
    <div
      v-else
      class="overflow-x-auto rounded-xl border border-gray-200 bg-white dark:border-gray-700 dark:bg-gray-800"
    >
      <table class="w-full text-sm">
        <thead>
          <tr class="border-b border-gray-200 dark:border-gray-700">
            <th class="px-4 py-3 text-left font-medium text-gray-500" />
            <th
              v-for="jour in jours"
              :key="jour"
              class="px-4 py-3 text-center font-medium text-gray-500"
            >
              {{ jour }}
            </th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="heure in creneaux"
            :key="heure"
            class="border-b border-gray-100 dark:border-gray-700"
          >
            <td class="px-4 py-3 text-sm font-medium text-gray-500">
              {{ heure }}
            </td>
            <td
              v-for="jour in jours"
              :key="jour"
              class="px-2 py-2"
            >
              <div
                v-if="getCours(jour, heure)"
                class="rounded-lg bg-blue-50 p-2 text-center dark:bg-blue-900/20"
              >
                <p class="text-xs font-semibold text-blue-700 dark:text-blue-300">
                  {{ getCours(jour, heure)!.matiere }}
                </p>
                <p class="text-xs text-blue-600 dark:text-blue-400">
                  {{ getCours(jour, heure)!.classe }}
                </p>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
