<script setup lang="ts">
import { useFamilleLiaisonParent } from '~/modules/famille/composables/useFamilleLiaisonParent'
import { useQRCode } from '~/composables/useQRCode'

definePageMeta({ layout: 'default' })

const { t } = useI18n()

const { items: enfants, loading, fetchAll } = useFamilleLiaisonParent()
const { qrData, loading: qrLoading, getMyQR } = useQRCode()

const parentCode = computed(() => qrData.value?.code ?? '')
const qrCodeUrl = computed(() => qrData.value?.qrCodeUrl ?? '')

onMounted(async () => {
  await fetchAll()
  await getMyQR()
})
</script>

<template>
  <div class="space-y-6">
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('famille.title'), to: '/famille' },
          { label: $t('famille.enfants.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('famille.enfants.title') }}
      </h1>
    </div>

    <!-- QR Code parent -->
    <div class="rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-800">
      <div class="flex flex-col items-center gap-4 sm:flex-row">
        <div class="flex h-32 w-32 items-center justify-center rounded-lg border-2 border-dashed border-gray-300 bg-gray-50 dark:border-gray-600 dark:bg-gray-800">
          <div v-if="qrLoading">
            <UIcon name="i-heroicons-arrow-path" class="h-8 w-8 animate-spin text-gray-400" />
          </div>
          <img v-else-if="qrCodeUrl" :src="qrCodeUrl" alt="QR Code" class="h-full w-full object-contain" />
          <UIcon v-else name="i-heroicons-qr-code" class="h-12 w-12 text-gray-400" />
        </div>
        <div>
          <h2 class="text-lg font-semibold text-gray-900 dark:text-white">
            {{ $t('famille.enfants.monQRCode') }}
          </h2>
          <p class="text-sm text-gray-500">{{ $t('famille.enfants.qrDescription') }}</p>
          <p class="mt-2 font-mono text-sm font-bold text-gray-900 dark:text-white">{{ parentCode }}</p>
        </div>
      </div>
    </div>

    <!-- Loading state -->
    <div v-if="loading" class="flex items-center justify-center py-12">
      <UIcon name="i-heroicons-arrow-path" class="h-8 w-8 animate-spin text-gray-400" />
    </div>

    <!-- Liste enfants -->
    <div v-else class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <div
        v-for="enfant in enfants"
        :key="enfant.id"
        class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800"
      >
        <div class="flex items-center gap-3 mb-4">
          <div class="flex h-12 w-12 items-center justify-center rounded-full bg-purple-100 text-lg font-bold text-purple-600 dark:bg-purple-900/30 dark:text-purple-400">
            {{ enfant.enfantPrenom.charAt(0) }}
          </div>
          <div>
            <p class="font-semibold text-gray-900 dark:text-white">{{ enfant.enfantPrenom }} {{ enfant.enfantNom }}</p>
            <p class="text-sm text-gray-500">{{ enfant.matricule }}</p>
          </div>
        </div>
        <div class="space-y-2 text-sm">
          <div class="flex justify-between">
            <span class="text-gray-500">{{ $t('famille.enfants.classe') }}</span>
            <span class="text-gray-900 dark:text-white">{{ enfant.classe }}</span>
          </div>
          <div class="flex justify-between">
            <span class="text-gray-500">{{ $t('famille.enfants.etablissement') }}</span>
            <span class="text-gray-900 dark:text-white">{{ enfant.etablissement }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
