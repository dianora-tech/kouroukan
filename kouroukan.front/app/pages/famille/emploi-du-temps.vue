<script setup lang="ts">
import { useForfaitGating } from '~/composables/useForfaitGating'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { isFeatureLocked } = useForfaitGating()
const isLocked = computed(() => isFeatureLocked('emploi-du-temps'))

const enfants = ref([
  { label: 'Aissatou Diallo - 3eme', value: '1' },
  { label: 'Ibrahim Diallo - CM2', value: '2' },
])

const selectedEnfant = ref('1')

const jours = ['Lundi', 'Mardi', 'Mercredi', 'Jeudi', 'Vendredi']
const creneaux = ['08:00', '10:00', '12:00', '14:00', '16:00']

const planning = ref([
  { jour: 'Lundi', heure: '08:00', matiere: 'Mathematiques', salle: 'S101' },
  { jour: 'Lundi', heure: '10:00', matiere: 'Francais', salle: 'S102' },
  { jour: 'Lundi', heure: '14:00', matiere: 'Anglais', salle: 'S103' },
  { jour: 'Mardi', heure: '08:00', matiere: 'Physique-Chimie', salle: 'Labo' },
  { jour: 'Mardi', heure: '10:00', matiere: 'Histoire-Geo', salle: 'S104' },
  { jour: 'Mercredi', heure: '08:00', matiere: 'Mathematiques', salle: 'S101' },
  { jour: 'Mercredi', heure: '10:00', matiere: 'SVT', salle: 'Labo' },
  { jour: 'Jeudi', heure: '08:00', matiere: 'Francais', salle: 'S102' },
  { jour: 'Jeudi', heure: '14:00', matiere: 'EPS', salle: 'Terrain' },
  { jour: 'Vendredi', heure: '08:00', matiere: 'Anglais', salle: 'S103' },
  { jour: 'Vendredi', heure: '10:00', matiere: 'Mathematiques', salle: 'S101' },
])

function getCours(jour: string, heure: string) {
  return planning.value.find(p => p.jour === jour && p.heure === heure)
}
</script>

<template>
  <ForfaitRequiredOverlay
    v-if="isLocked"
    user-type="famille"
  />
  <div
    v-else
    class="space-y-6"
  >
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('famille.title'), to: '/famille' },
          { label: $t('famille.emploiDuTemps.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('famille.emploiDuTemps.title') }}
      </h1>
    </div>

    <USelect
      v-model="selectedEnfant"
      :items="enfants"
      class="w-72"
    />

    <div class="overflow-x-auto rounded-xl border border-gray-200 bg-white dark:border-gray-700 dark:bg-gray-800">
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
                class="rounded-lg bg-purple-50 p-2 text-center dark:bg-purple-900/20"
              >
                <p class="text-xs font-semibold text-purple-700 dark:text-purple-300">
                  {{ getCours(jour, heure)!.matiere }}
                </p>
                <p class="text-xs text-purple-600 dark:text-purple-400">
                  {{ getCours(jour, heure)!.salle }}
                </p>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
