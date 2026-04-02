<template>
  <div>
    <template v-if="success">
      <div class="text-center py-8">
        <div class="w-16 h-16 rounded-full bg-green-100 dark:bg-green-900/30 flex items-center justify-center mx-auto mb-6">
          <UIcon name="i-heroicons-check" class="w-8 h-8 text-green-600" />
        </div>
        <h2 class="text-2xl font-bold text-gray-900 dark:text-white">{{ $t('inscription.confirmation.success') }}</h2>

        <!-- Forfait info -->
        <div class="mt-4 p-4 rounded-lg bg-green-50 dark:bg-green-900/20 max-w-sm mx-auto">
          <p class="text-sm text-gray-600 dark:text-gray-400">{{ $t('inscription.confirmation.planInfo') }}</p>
          <p class="font-semibold text-green-600 dark:text-green-400 mt-1">
            {{ $t(`inscription.step0.types.${accountType}.name`) }}
          </p>
        </div>

        <UButton
          :href="`${appUrl}/connexion`"
          color="primary"
          size="lg"
          class="mt-6"
          external
        >
          {{ $t('inscription.confirmation.goToApp') }}
        </UButton>
      </div>
    </template>

    <template v-else>
      <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">{{ $t('inscription.confirmation.title') }}</h2>
      <p class="text-gray-500 mb-8">{{ $t('inscription.confirmation.summary') }}</p>

      <div class="space-y-4">
        <!-- Account type -->
        <div class="p-4 rounded-lg bg-gray-50 dark:bg-gray-900">
          <h3 class="text-sm font-semibold text-gray-500 mb-2">{{ $t('inscription.steps.accountType') }}</h3>
          <p class="text-gray-900 dark:text-white">{{ $t(`inscription.step0.types.${accountType}.name`) }}</p>
          <p class="text-sm text-green-600 dark:text-green-400">{{ $t(`inscription.step0.types.${accountType}.price`) }}</p>
        </div>

        <!-- Profile -->
        <div class="p-4 rounded-lg bg-gray-50 dark:bg-gray-900">
          <h3 class="text-sm font-semibold text-gray-500 mb-2">{{ $t('inscription.steps.profile') }}</h3>
          <p class="text-gray-900 dark:text-white">{{ step1.firstName }} {{ step1.lastName }}</p>
          <p v-if="step1.schoolName && isEtablissement" class="text-gray-600 dark:text-gray-400 text-sm">{{ step1.schoolName }}</p>
          <p class="text-gray-600 dark:text-gray-400 text-sm">{{ step1.phone }}</p>
          <p v-if="step1.email" class="text-gray-600 dark:text-gray-400 text-sm">{{ step1.email }}</p>
        </div>

        <!-- Location -->
        <div v-if="step2.region" class="p-4 rounded-lg bg-gray-50 dark:bg-gray-900">
          <h3 class="text-sm font-semibold text-gray-500 mb-2">{{ $t('inscription.steps.location') }}</h3>
          <p class="text-gray-900 dark:text-white">{{ regionName }}</p>
          <p v-if="step2.prefecture" class="text-gray-600 dark:text-gray-400 text-sm">{{ prefectureName }}</p>
          <p v-if="step2.address" class="text-gray-600 dark:text-gray-400 text-sm">{{ step2.address }}</p>
        </div>
      </div>

      <p v-if="error" class="mt-4 text-sm text-red-600">{{ error }}</p>
    </template>
  </div>
</template>

<script setup lang="ts">
import type { AccountType } from '~/utils/types'
import { GUINEA_REGIONS, GUINEA_PREFECTURES } from '~/utils/constants'

const { appUrl } = useRuntimeConfig().public

const props = defineProps<{
  step1: { firstName: string; lastName: string; schoolName?: string; phone: string; email: string }
  step2: { region: string; prefecture: string; address: string }
  accountType: AccountType | ''
  isEtablissement: boolean
  success: boolean
  error: string | null
}>()

const regionName = computed(() => {
  const region = GUINEA_REGIONS.find(r => r.code === props.step2.region)
  return region?.name || props.step2.region
})

const prefectureName = computed(() => {
  const prefectures = GUINEA_PREFECTURES[props.step2.region] ?? []
  const prefecture = prefectures.find(p => p.code === props.step2.prefecture)
  return prefecture?.name || props.step2.prefecture
})
</script>
