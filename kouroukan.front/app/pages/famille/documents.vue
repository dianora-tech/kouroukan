<script setup lang="ts">
import { useForfaitGating } from '~/composables/useForfaitGating'
import { useLiaisonParentStore } from '~/modules/famille/stores/liaison-parent.store'
import { useDocumentsStore } from '~/modules/famille/stores/documents.store'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { formatDate } = useFormatDate()
const { isFeatureLocked } = useForfaitGating()
const isLocked = computed(() => isFeatureLocked('documents'))

const liaisonStore = useLiaisonParentStore()
const documentsStore = useDocumentsStore()

const loading = computed(() => liaisonStore.loading || documentsStore.loading)

const enfants = computed(() =>
  liaisonStore.items.map(l => ({
    label: `${l.enfantPrenom} ${l.enfantNom} - ${l.classe}`,
    value: String(l.enfantId),
  })),
)

const selectedEnfant = ref('')

const documents = computed(() => documentsStore.items)

const typeIcon: Record<string, string> = {
  bulletin: 'i-heroicons-document-chart-bar',
  attestation: 'i-heroicons-document-check',
}

const typeColor: Record<string, string> = {
  bulletin: 'text-blue-600',
  attestation: 'text-green-600',
}

watch(selectedEnfant, async (val) => {
  if (val) {
    await documentsStore.fetchByEnfant(Number(val))
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
          { label: $t('famille.documents.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('famille.documents.title') }}
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
      v-else-if="documents.length === 0 && selectedEnfant"
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
      class="space-y-3"
    >
      <div
        v-for="doc in documents"
        :key="doc.id"
        class="flex items-center justify-between rounded-xl border border-gray-200 bg-white p-4 dark:border-gray-700 dark:bg-gray-800"
      >
        <div class="flex items-center gap-3">
          <UIcon
            :name="typeIcon[doc.type] ?? 'i-heroicons-document-text'"
            :class="['h-8 w-8', typeColor[doc.type] ?? 'text-gray-600']"
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
          v-if="doc.url"
          variant="ghost"
          size="sm"
          icon="i-heroicons-arrow-down-tray"
          :to="doc.url"
          target="_blank"
        >
          {{ $t('famille.documents.telecharger') }}
        </UButton>
      </div>
    </div>
  </div>
</template>
