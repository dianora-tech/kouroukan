<script setup lang="ts">
import { useForfaitGating } from '~/composables/useForfaitGating'
import { useLiaisonParentStore } from '~/modules/famille/stores/liaison-parent.store'
import { useAbsencesStore } from '~/modules/famille/stores/absences.store'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { formatDate } = useFormatDate()
const { isFeatureLocked } = useForfaitGating()
const isLocked = computed(() => isFeatureLocked('absences'))

const liaisonStore = useLiaisonParentStore()
const absencesStore = useAbsencesStore()

const loading = computed(() => liaisonStore.loading || absencesStore.loading)

const enfants = computed(() =>
  liaisonStore.items.map(l => ({
    label: `${l.enfantPrenom} ${l.enfantNom} - ${l.classe}`,
    value: String(l.enfantId),
  })),
)

const selectedEnfant = ref('')

const absences = computed(() => absencesStore.items)

watch(selectedEnfant, async (val) => {
  if (val) {
    await absencesStore.fetchByEnfant(Number(val))
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
          { label: $t('famille.absences.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('famille.absences.title') }}
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
      v-else-if="absences.length === 0 && selectedEnfant"
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
      v-else-if="absences.length > 0"
      class="overflow-x-auto rounded-xl border border-gray-200 bg-white dark:border-gray-700 dark:bg-gray-800"
    >
      <table class="w-full text-left text-sm">
        <thead class="border-b border-gray-200 dark:border-gray-700">
          <tr>
            <th class="px-4 py-3 font-medium text-gray-500">
              {{ $t('famille.absences.date') }}
            </th>
            <th class="px-4 py-3 font-medium text-gray-500">
              {{ $t('famille.absences.motif') }}
            </th>
            <th class="px-4 py-3 font-medium text-gray-500">
              {{ $t('famille.absences.duree') }}
            </th>
            <th class="px-4 py-3 font-medium text-gray-500">
              {{ $t('famille.absences.justifie') }}
            </th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-100 dark:divide-gray-700">
          <tr
            v-for="a in absences"
            :key="a.id"
          >
            <td class="px-4 py-3 text-gray-900 dark:text-white">
              {{ formatDate(a.date) }}
            </td>
            <td class="px-4 py-3 text-gray-600 dark:text-gray-400">
              {{ a.motif }}
            </td>
            <td class="px-4 py-3 text-gray-600 dark:text-gray-400">
              {{ a.duree }}
            </td>
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
