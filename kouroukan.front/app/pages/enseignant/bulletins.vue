<script setup lang="ts">
import { useForfaitGating } from '~/composables/useForfaitGating'
import { useBulletinsStore } from '~/modules/enseignant/stores/bulletins.store'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { formatDate } = useFormatDate()
const { isFeatureLocked } = useForfaitGating()
const isLocked = computed(() => isFeatureLocked('bulletins'))

const store = useBulletinsStore()
const loading = computed(() => store.loading)
const bulletins = computed(() => store.items)

const statutColor: Record<string, 'success' | 'warning'> = {
  genere: 'success',
  en_attente: 'warning',
}

onMounted(async () => {
  await store.fetchAll()
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
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('enseignant.title'), to: '/enseignant' },
          { label: $t('enseignant.bulletins.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('enseignant.bulletins.title') }}
      </h1>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
        {{ $t('enseignant.bulletins.subtitle') }}
      </p>
    </div>

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
      v-else-if="bulletins.length === 0"
      class="rounded-xl border border-gray-200 bg-white p-12 text-center dark:border-gray-700 dark:bg-gray-800"
    >
      <UIcon
        name="i-heroicons-document-text"
        class="mx-auto h-12 w-12 text-gray-300"
      />
      <p class="mt-4 text-sm text-gray-500">
        {{ $t('common.noData') }}
      </p>
    </div>

    <div
      v-else
      class="overflow-x-auto rounded-xl border border-gray-200 bg-white dark:border-gray-700 dark:bg-gray-800"
    >
      <table class="w-full text-left text-sm">
        <thead class="border-b border-gray-200 dark:border-gray-700">
          <tr>
            <th class="px-4 py-3 font-medium text-gray-500">
              {{ $t('enseignant.bulletins.etablissement') }}
            </th>
            <th class="px-4 py-3 font-medium text-gray-500">
              {{ $t('enseignant.bulletins.classe') }}
            </th>
            <th class="px-4 py-3 font-medium text-gray-500">
              {{ $t('enseignant.bulletins.trimestre') }}
            </th>
            <th class="px-4 py-3 font-medium text-gray-500">
              {{ $t('enseignant.bulletins.date') }}
            </th>
            <th class="px-4 py-3 font-medium text-gray-500">
              {{ $t('enseignant.bulletins.statut') }}
            </th>
            <th class="px-4 py-3 font-medium text-gray-500 w-24" />
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-100 dark:divide-gray-700">
          <tr
            v-for="b in bulletins"
            :key="b.id"
          >
            <td class="px-4 py-3 text-gray-900 dark:text-white">
              {{ b.etablissement }}
            </td>
            <td class="px-4 py-3 text-gray-600 dark:text-gray-400">
              {{ b.classe }}
            </td>
            <td class="px-4 py-3 text-gray-600 dark:text-gray-400">
              {{ b.trimestre }}
            </td>
            <td class="px-4 py-3 text-gray-600 dark:text-gray-400">
              {{ formatDate(b.date) }}
            </td>
            <td class="px-4 py-3">
              <UBadge
                :color="statutColor[b.statut] ?? 'warning'"
                variant="subtle"
                size="sm"
              >
                {{ $t(`enseignant.bulletins.${b.statut}`) }}
              </UBadge>
            </td>
            <td class="px-4 py-3">
              <UButton
                v-if="b.statut === 'genere'"
                variant="ghost"
                size="xs"
                icon="i-heroicons-arrow-down-tray"
              />
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
