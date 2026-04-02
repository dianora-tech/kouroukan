<script setup lang="ts">
import { useForfaitGating } from '~/composables/useForfaitGating'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { formatDate } = useFormatDate()
const { isFeatureLocked } = useForfaitGating()
const isLocked = computed(() => isFeatureLocked('documents'))

const enfants = ref([
  { label: 'Aissatou Diallo - 3eme', value: '1' },
  { label: 'Ibrahim Diallo - CM2', value: '2' },
])

const selectedEnfant = ref('1')

const documents = ref([
  { id: 1, nom: 'Bulletin Trimestre 1 - 2024/2025', type: 'bulletin', date: '2024-12-20', taille: '245 Ko' },
  { id: 2, nom: 'Attestation de scolarite', type: 'attestation', date: '2024-10-01', taille: '120 Ko' },
  { id: 3, nom: 'Certificat de frequentation', type: 'attestation', date: '2024-10-15', taille: '98 Ko' },
  { id: 4, nom: 'Bulletin Trimestre 2 - 2024/2025', type: 'bulletin', date: '2025-03-20', taille: '260 Ko' },
])

const typeIcon: Record<string, string> = {
  bulletin: 'i-heroicons-document-chart-bar',
  attestation: 'i-heroicons-document-check',
}

const typeColor: Record<string, string> = {
  bulletin: 'text-blue-600',
  attestation: 'text-green-600',
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
          { label: $t('famille.documents.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('famille.documents.title') }}
      </h1>
    </div>

    <USelect
      v-model="selectedEnfant"
      :items="enfants"
      class="w-72"
    />

    <div class="space-y-3">
      <div
        v-for="doc in documents"
        :key="doc.id"
        class="flex items-center justify-between rounded-xl border border-gray-200 bg-white p-4 dark:border-gray-700 dark:bg-gray-800"
      >
        <div class="flex items-center gap-3">
          <UIcon
            :name="typeIcon[doc.type]"
            :class="['h-8 w-8', typeColor[doc.type]]"
          />
          <div>
            <p class="font-medium text-gray-900 dark:text-white">
              {{ doc.nom }}
            </p>
            <p class="text-xs text-gray-500">
              {{ formatDate(doc.date) }} - {{ doc.taille }}
            </p>
          </div>
        </div>
        <UButton
          variant="ghost"
          size="sm"
          icon="i-heroicons-arrow-down-tray"
        >
          {{ $t('famille.documents.telecharger') }}
        </UButton>
      </div>
    </div>
  </div>
</template>
