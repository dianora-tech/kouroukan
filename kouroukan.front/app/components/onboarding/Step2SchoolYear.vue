<script setup lang="ts">
import { apiClient } from '~/core/api/client'
import { useOnboardingDraft } from '~/composables/useOnboardingDraft'
import type { AnneeScolaire } from '~/modules/inscriptions/types/annee-scolaire.types'

const emit = defineEmits<{
  submit: [data: { anneeScolaireId: number }]
  prev: []
}>()

const { t } = useI18n()
const { saveDraft, loadDraft } = useOnboardingDraft('step2')

// ── Data ──
const loading = ref(true)
const anneesDisponibles = ref<AnneeScolaire[]>([])
const selectedId = ref<number | null>(null)

// Fetch available school years
onMounted(async () => {
  // Restore draft
  const draft = loadDraft<{ anneeScolaireId: number }>()
  if (draft?.anneeScolaireId) {
    selectedId.value = draft.anneeScolaireId
  }

  try {
    const res = await apiClient.get<{ items: AnneeScolaire[] }>('/api/inscriptions/annees-scolaires?pageSize=50')
    if (res?.data?.items) {
      // Show only active or preparation years
      anneesDisponibles.value = res.data.items.filter(
        a => a.statut === 'active' || a.statut === 'preparation',
      )
      // Auto-select if only one available, or if draft matches
      if (selectedId.value && anneesDisponibles.value.some(a => a.id === selectedId.value)) {
        // Keep the draft selection
      }
      else if (anneesDisponibles.value.length === 1) {
        selectedId.value = anneesDisponibles.value[0].id
      }
    }
  }
  catch {
    // Silently fail
  }
  finally {
    loading.value = false
  }
})

// Save draft on selection change
watch(selectedId, (id) => {
  if (id) saveDraft({ anneeScolaireId: id })
})

// Computed: selected year details
const selectedYear = computed(() =>
  anneesDisponibles.value.find(a => a.id === selectedId.value) ?? null,
)

// Select options
const selectOptions = computed(() =>
  anneesDisponibles.value.map(a => ({
    label: a.libelle + (a.code ? ` (${a.code})` : ''),
    value: a.id,
  })),
)

// Status label & color
function getStatutLabel(statut: string): string {
  const labels: Record<string, string> = {
    preparation: t('onboarding.schoolYear.statusPreparation'),
    active: t('onboarding.schoolYear.statusActive'),
    cloturee: t('onboarding.schoolYear.statusClosed'),
    archivee: t('onboarding.schoolYear.statusArchived'),
  }
  return labels[statut] || statut
}

function getStatutColor(statut: string): string {
  const colors: Record<string, string> = {
    preparation: 'info',
    active: 'success',
    cloturee: 'warning',
    archivee: 'neutral',
  }
  return colors[statut] || 'neutral'
}

const { formatDate } = useFormatDate()

function onSubmit() {
  if (!selectedId.value) return
  emit('submit', { anneeScolaireId: selectedId.value })
}
</script>

<template>
  <div>
    <h2 class="mb-2 text-xl font-bold text-gray-900 dark:text-white">
      {{ $t('onboarding.schoolYear.title') }}
    </h2>
    <p class="mb-6 text-sm text-gray-500 dark:text-gray-400">
      {{ $t('onboarding.schoolYear.description') }}
    </p>

    <!-- Loading -->
    <div v-if="loading" class="flex items-center justify-center py-8">
      <UIcon name="i-heroicons-arrow-path" class="h-6 w-6 animate-spin text-gray-400" />
    </div>

    <!-- No years available -->
    <div
      v-else-if="anneesDisponibles.length === 0"
      class="rounded-lg border border-amber-200 bg-amber-50 p-4 text-center dark:border-amber-700 dark:bg-amber-900/20"
    >
      <UIcon name="i-heroicons-exclamation-triangle" class="mx-auto mb-2 h-8 w-8 text-amber-500" />
      <p class="text-sm font-medium text-amber-800 dark:text-amber-300">
        {{ $t('onboarding.schoolYear.noYearsAvailable') }}
      </p>
      <p class="mt-1 text-xs text-amber-600 dark:text-amber-400">
        {{ $t('onboarding.schoolYear.contactAdmin') }}
      </p>
    </div>

    <!-- Year selector -->
    <template v-else>
      <div class="space-y-4">
        <div>
          <label class="mb-1.5 block text-sm font-medium text-gray-700 dark:text-gray-300">
            {{ $t('onboarding.schoolYear.selectLabel') }}
          </label>
          <USelect
            v-model="selectedId"
            :items="selectOptions"
            value-key="value"
            :placeholder="$t('onboarding.schoolYear.selectPlaceholder')"
            class="w-full"
          />
        </div>

        <!-- Selected year details -->
        <Transition
          enter-active-class="transition duration-200 ease-out"
          enter-from-class="translate-y-1 opacity-0"
          enter-to-class="translate-y-0 opacity-100"
          leave-active-class="transition duration-150 ease-in"
          leave-from-class="translate-y-0 opacity-100"
          leave-to-class="translate-y-1 opacity-0"
        >
          <div
            v-if="selectedYear"
            class="rounded-lg border border-gray-200 bg-gray-50 p-4 dark:border-gray-700 dark:bg-gray-800/50"
          >
            <div class="mb-3 flex items-center justify-between">
              <h3 class="text-base font-semibold text-gray-900 dark:text-white">
                {{ selectedYear.libelle }}
              </h3>
              <UBadge :color="getStatutColor(selectedYear.statut)" variant="subtle" size="sm">
                {{ getStatutLabel(selectedYear.statut) }}
              </UBadge>
            </div>

            <div class="grid grid-cols-2 gap-3 text-sm">
              <!-- Code -->
              <div v-if="selectedYear.code">
                <span class="text-gray-500 dark:text-gray-400">{{ $t('onboarding.schoolYear.codeLabel') }}</span>
                <p class="font-medium text-gray-900 dark:text-white">{{ selectedYear.code }}</p>
              </div>

              <!-- Dates -->
              <div>
                <span class="text-gray-500 dark:text-gray-400">{{ $t('onboarding.schoolYear.startDate') }}</span>
                <p class="font-medium text-gray-900 dark:text-white">{{ formatDate(selectedYear.dateDebut) }}</p>
              </div>
              <div>
                <span class="text-gray-500 dark:text-gray-400">{{ $t('onboarding.schoolYear.endDate') }}</span>
                <p class="font-medium text-gray-900 dark:text-white">{{ formatDate(selectedYear.dateFin) }}</p>
              </div>

              <!-- Date rentrée -->
              <div v-if="selectedYear.dateRentree">
                <span class="text-gray-500 dark:text-gray-400">{{ $t('onboarding.schoolYear.effectiveStart') }}</span>
                <p class="font-medium text-gray-900 dark:text-white">{{ formatDate(selectedYear.dateRentree) }}</p>
              </div>

              <!-- Périodes -->
              <div>
                <span class="text-gray-500 dark:text-gray-400">{{ $t('onboarding.schoolYear.periods') }}</span>
                <p class="font-medium text-gray-900 dark:text-white">
                  {{ selectedYear.nombrePeriodes }}
                  {{ selectedYear.typePeriode === 'semestre' ? $t('onboarding.schoolYear.semesters') : $t('onboarding.schoolYear.trimesters') }}
                </p>
              </div>
            </div>

            <!-- Description -->
            <div v-if="selectedYear.description" class="mt-3 border-t border-gray-200 pt-3 dark:border-gray-700">
              <p class="text-sm text-gray-600 dark:text-gray-400">{{ selectedYear.description }}</p>
            </div>
          </div>
        </Transition>
      </div>

      <!-- Actions -->
      <div class="flex justify-between pt-6">
        <UButton variant="ghost" color="neutral" @click="emit('prev')">
          {{ $t('onboarding.previous') }}
        </UButton>
        <UButton color="primary" :disabled="!selectedId" @click="onSubmit">
          {{ $t('onboarding.next') }}
        </UButton>
      </div>
    </template>
  </div>
</template>
