<template>
  <div>
    <template v-if="success">
      <div class="text-center py-8">
        <div class="w-16 h-16 rounded-full bg-green-100 dark:bg-green-900/30 flex items-center justify-center mx-auto mb-6">
          <UIcon name="i-heroicons-check" class="w-8 h-8 text-green-600" />
        </div>
        <h2 class="text-2xl font-bold text-gray-900 dark:text-white">{{ $t('inscription.confirmation.success') }}</h2>
        <UButton
          href="https://app.kouroukan.gn"
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
        <div class="p-4 rounded-lg bg-gray-50 dark:bg-gray-900">
          <h3 class="text-sm font-semibold text-gray-500 mb-2">{{ $t('inscription.steps.profile') }}</h3>
          <p class="text-gray-900 dark:text-white">{{ step1.firstName }} {{ step1.lastName }}</p>
          <p class="text-gray-600 dark:text-gray-400 text-sm">{{ step1.phone }}</p>
          <p v-if="step1.email" class="text-gray-600 dark:text-gray-400 text-sm">{{ step1.email }}</p>
        </div>

        <div class="p-4 rounded-lg bg-gray-50 dark:bg-gray-900">
          <h3 class="text-sm font-semibold text-gray-500 mb-2">{{ $t('inscription.steps.activity') }}</h3>
          <div class="flex flex-wrap gap-2">
            <UBadge v-for="slug in step2.modules" :key="slug" color="primary" variant="subtle">
              {{ slug }}
            </UBadge>
          </div>
        </div>

        <div v-if="step3.region" class="p-4 rounded-lg bg-gray-50 dark:bg-gray-900">
          <h3 class="text-sm font-semibold text-gray-500 mb-2">{{ $t('inscription.steps.location') }}</h3>
          <p class="text-gray-900 dark:text-white">{{ regionName }}</p>
          <p v-if="step3.prefecture" class="text-gray-600 dark:text-gray-400 text-sm">{{ prefectureName }}</p>
          <p v-if="step3.address" class="text-gray-600 dark:text-gray-400 text-sm">{{ step3.address }}</p>
        </div>
      </div>

      <p v-if="error" class="mt-4 text-sm text-red-600">{{ error }}</p>
    </template>
  </div>
</template>

<script setup lang="ts">
import { GUINEA_REGIONS, GUINEA_PREFECTURES } from '~/utils/constants'

const props = defineProps<{
  step1: { firstName: string; lastName: string; phone: string; email: string }
  step2: { modules: string[] }
  step3: { region: string; prefecture: string; address: string }
  success: boolean
  error: string | null
}>()

const regionName = computed(() => {
  const region = GUINEA_REGIONS.find(r => r.code === props.step3.region)
  return region?.name || props.step3.region
})

const prefectureName = computed(() => {
  const prefectures = GUINEA_PREFECTURES[props.step3.region] ?? []
  const prefecture = prefectures.find(p => p.code === props.step3.prefecture)
  return prefecture?.name || props.step3.prefecture
})
</script>
