<script setup lang="ts">
import { useEnseignantLiaison } from '~/modules/enseignant/composables/useEnseignantLiaison'
import { useAffectationStore } from '~/modules/enseignant/stores/affectation.store'

definePageMeta({ layout: 'default' })

const { t } = useI18n()

const { actives: liaisons, loading: liaisonsLoading, fetchAll: fetchLiaisons } = useEnseignantLiaison()
const affectationStore = useAffectationStore()

const loading = computed(() => liaisonsLoading.value || affectationStore.loading)

const stats = computed(() => [
  { label: t('enseignant.dashboard.etablissements'), value: liaisons.value.filter(l => l.statut === 'accepted').length, icon: 'i-heroicons-building-office-2', color: 'text-blue-600' },
  { label: t('enseignant.dashboard.heuresMois'), value: affectationStore.items.length * 2, icon: 'i-heroicons-clock', color: 'text-green-600' },
  { label: t('enseignant.dashboard.prochainsCours'), value: affectationStore.items.length, icon: 'i-heroicons-calendar', color: 'text-purple-600' },
])

const etablissements = computed(() =>
  liaisons.value.map(l => ({
    id: l.id,
    nom: l.etablissement,
    statut: l.statut,
    heures: 0,
  })),
)

onMounted(async () => {
  await fetchLiaisons()
  await affectationStore.fetchAll()
})
</script>

<template>
  <div class="space-y-6">
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('enseignant.title') },
          { label: $t('enseignant.dashboard.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('enseignant.dashboard.title') }}
      </h1>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
        {{ $t('enseignant.dashboard.subtitle') }}
      </p>
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

    <template v-else>
      <!-- Stats cards -->
      <div class="grid grid-cols-1 gap-4 sm:grid-cols-3">
        <div
          v-for="stat in stats"
          :key="stat.label"
          class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800"
        >
          <div class="flex items-center gap-3">
            <UIcon
              :name="stat.icon"
              :class="['h-8 w-8', stat.color]"
            />
            <div>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">
                {{ stat.value }}
              </p>
              <p class="text-sm text-gray-500 dark:text-gray-400">
                {{ stat.label }}
              </p>
            </div>
          </div>
        </div>
      </div>

      <!-- Etablissements lies -->
      <div class="rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-800">
        <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          {{ $t('enseignant.dashboard.mesEtablissements') }}
        </h2>
        <div class="space-y-3">
          <div
            v-for="etab in etablissements"
            :key="etab.id"
            class="flex items-center justify-between rounded-lg border border-gray-100 p-4 dark:border-gray-700"
          >
            <div>
              <p class="font-medium text-gray-900 dark:text-white">
                {{ etab.nom }}
              </p>
              <p class="text-sm text-gray-500">
                {{ etab.heures }}h {{ $t('enseignant.dashboard.ceMois') }}
              </p>
            </div>
            <UBadge
              :color="etab.statut === 'accepted' ? 'success' : 'warning'"
              variant="subtle"
              size="sm"
            >
              {{ etab.statut === 'accepted' ? $t('enseignant.statut.active') : $t('enseignant.statut.pending') }}
            </UBadge>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>
