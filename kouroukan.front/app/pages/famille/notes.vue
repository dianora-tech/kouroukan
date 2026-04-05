<script setup lang="ts">
import { useForfaitGating } from '~/composables/useForfaitGating'
import { useLiaisonParentStore } from '~/modules/famille/stores/liaison-parent.store'
import { useNotesStore } from '~/modules/famille/stores/notes.store'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { isFeatureLocked } = useForfaitGating()
const isLocked = computed(() => isFeatureLocked('notes'))

const liaisonStore = useLiaisonParentStore()
const notesStore = useNotesStore()

const loading = computed(() => liaisonStore.loading || notesStore.loading)

const enfants = computed(() =>
  liaisonStore.items.map(l => ({
    label: `${l.enfantPrenom} ${l.enfantNom} - ${l.classe}`,
    value: String(l.enfantId),
  })),
)

const selectedEnfant = ref('')

const notes = computed(() => notesStore.items)

const moyenne = computed(() => {
  if (notes.value.length === 0) return '0.00'
  const totalPondere = notes.value.reduce((sum, n) => sum + n.note * n.coefficient, 0)
  const totalCoeff = notes.value.reduce((sum, n) => sum + n.coefficient, 0)
  if (totalCoeff === 0) return '0.00'
  return (totalPondere / totalCoeff).toFixed(2)
})

watch(selectedEnfant, async (val) => {
  if (val) {
    await notesStore.fetchByEnfant(Number(val))
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
          { label: $t('famille.notes.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('famille.notes.title') }}
      </h1>
    </div>

    <!-- Selection enfant -->
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

    <template v-else-if="notes.length > 0">
      <!-- Moyenne -->
      <div class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800">
        <div class="flex items-center gap-3">
          <UIcon
            name="i-heroicons-academic-cap"
            class="h-8 w-8 text-indigo-600"
          />
          <div>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">
              {{ moyenne }}/20
            </p>
            <p class="text-sm text-gray-500">
              {{ $t('famille.notes.moyenneGenerale') }}
            </p>
          </div>
        </div>
      </div>

      <!-- Table notes -->
      <div class="overflow-x-auto rounded-xl border border-gray-200 bg-white dark:border-gray-700 dark:bg-gray-800">
        <table class="w-full text-left text-sm">
          <thead class="border-b border-gray-200 dark:border-gray-700">
            <tr>
              <th class="px-4 py-3 font-medium text-gray-500">
                {{ $t('famille.notes.matiere') }}
              </th>
              <th class="px-4 py-3 font-medium text-gray-500">
                {{ $t('famille.notes.note') }}
              </th>
              <th class="px-4 py-3 font-medium text-gray-500">
                {{ $t('famille.notes.coefficient') }}
              </th>
              <th class="px-4 py-3 font-medium text-gray-500">
                {{ $t('famille.notes.appreciation') }}
              </th>
              <th class="px-4 py-3 font-medium text-gray-500">
                {{ $t('famille.notes.enseignant') }}
              </th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-100 dark:divide-gray-700">
            <tr
              v-for="n in notes"
              :key="n.id"
            >
              <td class="px-4 py-3 text-gray-900 dark:text-white">
                {{ n.matiere }}
              </td>
              <td class="px-4 py-3">
                <span
                  :class="n.note >= 14 ? 'text-green-600' : n.note >= 10 ? 'text-yellow-600' : 'text-red-600'"
                  class="font-semibold"
                >
                  {{ n.note }}/{{ n.noteMax || 20 }}
                </span>
              </td>
              <td class="px-4 py-3 text-gray-600 dark:text-gray-400">
                {{ n.coefficient }}
              </td>
              <td class="px-4 py-3 text-gray-600 dark:text-gray-400">
                {{ n.appreciation }}
              </td>
              <td class="px-4 py-3 text-gray-600 dark:text-gray-400">
                {{ n.enseignant }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </template>

    <!-- Empty state -->
    <div
      v-else-if="selectedEnfant && !loading"
      class="rounded-xl border border-gray-200 bg-white p-12 text-center dark:border-gray-700 dark:bg-gray-800"
    >
      <UIcon
        name="i-heroicons-academic-cap"
        class="mx-auto h-12 w-12 text-gray-300"
      />
      <p class="mt-4 text-sm text-gray-500">
        {{ $t('common.noData') }}
      </p>
    </div>
  </div>
</template>
