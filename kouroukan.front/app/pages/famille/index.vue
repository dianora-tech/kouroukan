<script setup lang="ts">
import { useFamilleLiaisonParent } from '~/modules/famille/composables/useFamilleLiaisonParent'

definePageMeta({ layout: 'default' })

const { t } = useI18n()

const { items: liaisons, loading, fetchAll } = useFamilleLiaisonParent()

const enfants = computed(() =>
  liaisons.value.map(l => ({
    id: l.id,
    nom: `${l.enfantPrenom} ${l.enfantNom}`,
    classe: l.classe,
    etablissement: l.etablissement,
    moyenne: 0,
    rang: 0,
    tauxPresence: 0,
  })),
)

onMounted(() => {
  fetchAll()
})
</script>

<template>
  <div class="space-y-6">
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('famille.title') },
          { label: $t('famille.dashboard.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('famille.dashboard.title') }}
      </h1>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
        {{ $t('famille.dashboard.subtitle') }}
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
      <!-- Cards par enfant -->
      <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <div
          v-for="enfant in enfants"
          :key="enfant.id"
          class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800"
        >
          <div class="flex items-center gap-3 mb-4">
            <div class="flex h-10 w-10 items-center justify-center rounded-full bg-indigo-100 text-sm font-bold text-indigo-600 dark:bg-indigo-900/30 dark:text-indigo-400">
              {{ enfant.nom.charAt(0) }}
            </div>
            <div>
              <p class="font-semibold text-gray-900 dark:text-white">
                {{ enfant.nom }}
              </p>
              <p class="text-xs text-gray-500">
                {{ enfant.classe }} - {{ enfant.etablissement }}
              </p>
            </div>
          </div>
          <div class="grid grid-cols-3 gap-3 text-center">
            <div>
              <p class="text-lg font-bold text-gray-900 dark:text-white">
                {{ enfant.moyenne || '-' }}
              </p>
              <p class="text-xs text-gray-500">
                {{ $t('famille.dashboard.moyenne') }}
              </p>
            </div>
            <div>
              <p class="text-lg font-bold text-gray-900 dark:text-white">
                {{ enfant.rang || '-' }}
              </p>
              <p class="text-xs text-gray-500">
                {{ $t('famille.dashboard.rang') }}
              </p>
            </div>
            <div>
              <p class="text-lg font-bold text-gray-900 dark:text-white">
                {{ enfant.tauxPresence || '-' }}%
              </p>
              <p class="text-xs text-gray-500">
                {{ $t('famille.dashboard.presence') }}
              </p>
            </div>
          </div>
        </div>
      </div>

      <!-- Evolution notes placeholder -->
      <div class="rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-800">
        <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          {{ $t('famille.dashboard.evolutionNotes') }}
        </h2>
        <div class="flex h-48 items-center justify-center rounded-lg border-2 border-dashed border-gray-300 bg-gray-50 dark:border-gray-600 dark:bg-gray-800">
          <div class="text-center text-gray-400">
            <UIcon
              name="i-heroicons-chart-bar"
              class="mx-auto h-12 w-12"
            />
            <p class="mt-2 text-sm">
              {{ $t('famille.dashboard.graphiquePlaceholder') }}
            </p>
          </div>
        </div>
      </div>

      <!-- Prochains evenements -->
      <div class="rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-800">
        <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          {{ $t('famille.dashboard.prochainsEvenements') }}
        </h2>
        <div class="text-center py-8 text-gray-400 text-sm">
          {{ $t('famille.dashboard.graphiquePlaceholder') }}
        </div>
      </div>
    </template>
  </div>
</template>
