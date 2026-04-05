<script setup lang="ts">
import { useForfaitGating } from '~/composables/useForfaitGating'
import { useLiaisonParentStore } from '~/modules/famille/stores/liaison-parent.store'
import { useEmploiDuTempsStore } from '~/modules/famille/stores/emploi-du-temps.store'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { isFeatureLocked } = useForfaitGating()
const isLocked = computed(() => isFeatureLocked('emploi-du-temps'))

const liaisonStore = useLiaisonParentStore()
const emploiStore = useEmploiDuTempsStore()

const loading = computed(() => liaisonStore.loading || emploiStore.loading)

const enfants = computed(() =>
  liaisonStore.items.map(l => ({
    label: `${l.enfantPrenom} ${l.enfantNom} - ${l.classe}`,
    value: String(l.enfantId),
  })),
)

const selectedEnfant = ref('')

const jours = ['Lundi', 'Mardi', 'Mercredi', 'Jeudi', 'Vendredi']
const joursMap: Record<string, number> = { Lundi: 1, Mardi: 2, Mercredi: 3, Jeudi: 4, Vendredi: 5 }
const creneaux = ['08:00', '10:00', '12:00', '14:00', '16:00']

const planning = computed(() => emploiStore.items)

function getCours(jour: string, heure: string) {
  const jourNum = joursMap[jour]
  return planning.value.find(p => p.jourSemaine === jourNum && p.heureDebut.startsWith(heure))
}

watch(selectedEnfant, async (val) => {
  if (val) {
    await emploiStore.fetchByEnfant(Number(val))
  }
})

onMounted(async () => {
  await liaisonStore.fetchAll()
  if (enfants.value.length > 0) {
    selectedEnfant.value = enfants.value[0].value
  }
})
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
      v-if="enfants.length > 0"
      v-model="selectedEnfant"
      :items="enfants"
      class="w-72"
    />

    <!-- Loading -->
    <div
      v-if="loading"
      class="flex justify-center py-12"
    >
      <UIcon
        name="i-heroicons-arrow-path"
        class="h-8 w-8 animate-spin text-gray-400"
      />
    </div>

    <!-- Empty state -->
    <div
      v-else-if="planning.length === 0 && selectedEnfant"
      class="rounded-xl border border-gray-200 bg-white p-12 text-center dark:border-gray-700 dark:bg-gray-800"
    >
      <UIcon
        name="i-heroicons-calendar-days"
        class="mx-auto h-12 w-12 text-gray-300"
      />
      <p class="mt-4 text-sm text-gray-500">
        {{ $t('common.noData') }}
      </p>
    </div>

    <div
      v-else-if="planning.length > 0"
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
