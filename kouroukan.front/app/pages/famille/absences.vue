<script setup lang="ts">
import { useForfaitGating } from '~/composables/useForfaitGating'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { formatDate } = useFormatDate()
const { isFeatureLocked } = useForfaitGating()
const isLocked = computed(() => isFeatureLocked('absences'))

const enfants = ref([
  { label: 'Aissatou Diallo - 3eme', value: '1' },
  { label: 'Ibrahim Diallo - CM2', value: '2' },
])

const selectedEnfant = ref('1')

const absences = ref([
  { id: 1, date: '2025-03-10', motif: 'Maladie', justifie: true, duree: '1 jour' },
  { id: 2, date: '2025-02-25', motif: 'Rendez-vous medical', justifie: true, duree: '2 heures' },
  { id: 3, date: '2025-02-14', motif: 'Non communique', justifie: false, duree: '1 jour' },
  { id: 4, date: '2025-01-20', motif: 'Evenement familial', justifie: true, duree: '1 jour' },
])
</script>

<template>
  <ForfaitRequiredOverlay v-if="isLocked" user-type="famille" />
  <div v-else class="space-y-6">
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('famille.title'), to: '/famille' },
          { label: $t('famille.absences.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('famille.absences.title') }}
      </h1>
    </div>

    <USelect v-model="selectedEnfant" :items="enfants" class="w-72" />

    <div class="overflow-x-auto rounded-xl border border-gray-200 bg-white dark:border-gray-700 dark:bg-gray-800">
      <table class="w-full text-left text-sm">
        <thead class="border-b border-gray-200 dark:border-gray-700">
          <tr>
            <th class="px-4 py-3 font-medium text-gray-500">{{ $t('famille.absences.date') }}</th>
            <th class="px-4 py-3 font-medium text-gray-500">{{ $t('famille.absences.motif') }}</th>
            <th class="px-4 py-3 font-medium text-gray-500">{{ $t('famille.absences.duree') }}</th>
            <th class="px-4 py-3 font-medium text-gray-500">{{ $t('famille.absences.justifie') }}</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-100 dark:divide-gray-700">
          <tr v-for="a in absences" :key="a.id">
            <td class="px-4 py-3 text-gray-900 dark:text-white">{{ formatDate(a.date) }}</td>
            <td class="px-4 py-3 text-gray-600 dark:text-gray-400">{{ a.motif }}</td>
            <td class="px-4 py-3 text-gray-600 dark:text-gray-400">{{ a.duree }}</td>
            <td class="px-4 py-3">
              <UBadge
                :color="a.justifie ? 'success' : 'error'"
                variant="subtle"
                size="sm"
              >
                {{ a.justifie ? $t('famille.absences.oui') : $t('famille.absences.non') }}
              </UBadge>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
