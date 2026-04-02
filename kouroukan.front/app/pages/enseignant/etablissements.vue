<script setup lang="ts">
import { useEnseignantLiaison } from '~/modules/enseignant/composables/useEnseignantLiaison'
import { useForfaitGating } from '~/composables/useForfaitGating'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { formatDate } = useFormatDate()
const { isFeatureLocked } = useForfaitGating()
const isLocked = computed(() => isFeatureLocked('etablissements'))

const {
  actives: liaisons,
  historique,
  loading,
  saving,
  fetchAll,
  accept,
  reject,
} = useEnseignantLiaison()

const statutColor: Record<string, 'success' | 'warning' | 'error' | 'neutral'> = {
  accepted: 'success',
  pending: 'warning',
  terminated: 'neutral',
}

onMounted(() => {
  fetchAll()
})

async function accepter(id: number) {
  await accept(id)
}

async function refuser(id: number) {
  await reject(id)
}
</script>

<template>
  <ForfaitRequiredOverlay v-if="isLocked" user-type="enseignant" />
  <div v-else class="space-y-6">
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('enseignant.title'), to: '/enseignant' },
          { label: $t('enseignant.etablissements.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('enseignant.etablissements.title') }}
      </h1>
    </div>

    <!-- Loading state -->
    <div v-if="loading" class="flex items-center justify-center py-12">
      <UIcon name="i-heroicons-arrow-path" class="h-8 w-8 animate-spin text-gray-400" />
    </div>

    <template v-else>
      <!-- Liaisons actives -->
      <div class="rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-800">
        <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          {{ $t('enseignant.etablissements.liaisonsActives') }}
        </h2>
        <table class="w-full text-left text-sm">
          <thead class="border-b border-gray-200 dark:border-gray-700">
            <tr>
              <th class="px-4 py-3 font-medium text-gray-500">{{ $t('enseignant.etablissements.nom') }}</th>
              <th class="px-4 py-3 font-medium text-gray-500">{{ $t('enseignant.etablissements.statut') }}</th>
              <th class="px-4 py-3 font-medium text-gray-500">{{ $t('enseignant.etablissements.date') }}</th>
              <th class="px-4 py-3 font-medium text-gray-500 w-32" />
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-100 dark:divide-gray-700">
            <tr v-for="liaison in liaisons" :key="liaison.id">
              <td class="px-4 py-3 text-gray-900 dark:text-white">{{ liaison.etablissement }}</td>
              <td class="px-4 py-3">
                <UBadge :color="statutColor[liaison.statut]" variant="subtle" size="sm">
                  {{ $t(`enseignant.statut.${liaison.statut}`) }}
                </UBadge>
              </td>
              <td class="px-4 py-3 text-gray-600 dark:text-gray-400">{{ formatDate(liaison.date) }}</td>
              <td class="px-4 py-3">
                <div v-if="liaison.statut === 'pending'" class="flex gap-1">
                  <UButton size="xs" color="primary" icon="i-heroicons-check" :loading="saving" @click="accepter(liaison.id)">
                    {{ $t('enseignant.etablissements.accepter') }}
                  </UButton>
                  <UButton size="xs" color="error" variant="ghost" icon="i-heroicons-x-mark" :loading="saving" @click="refuser(liaison.id)">
                    {{ $t('enseignant.etablissements.refuser') }}
                  </UButton>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Historique -->
      <div class="rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-800">
        <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          {{ $t('enseignant.etablissements.historique') }}
        </h2>
        <table class="w-full text-left text-sm">
          <thead class="border-b border-gray-200 dark:border-gray-700">
            <tr>
              <th class="px-4 py-3 font-medium text-gray-500">{{ $t('enseignant.etablissements.nom') }}</th>
              <th class="px-4 py-3 font-medium text-gray-500">{{ $t('enseignant.etablissements.statut') }}</th>
              <th class="px-4 py-3 font-medium text-gray-500">{{ $t('enseignant.etablissements.date') }}</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-100 dark:divide-gray-700">
            <tr v-for="h in historique" :key="h.id">
              <td class="px-4 py-3 text-gray-900 dark:text-white">{{ h.etablissement }}</td>
              <td class="px-4 py-3">
                <UBadge :color="statutColor[h.statut]" variant="subtle" size="sm">
                  {{ $t(`enseignant.statut.${h.statut}`) }}
                </UBadge>
              </td>
              <td class="px-4 py-3 text-gray-600 dark:text-gray-400">{{ formatDate(h.date) }} - {{ formatDate(h.dateFin) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </template>
  </div>
</template>
