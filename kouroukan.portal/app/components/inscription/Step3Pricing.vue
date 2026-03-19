<template>
  <div>
    <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-2">{{ $t('inscription.stepPricing.title') }}</h2>
    <p class="text-gray-500 mb-6">{{ $t('inscription.stepPricing.subtitle') }}</p>

    <div class="grid grid-cols-1 gap-4">
      <button
        v-for="plan in PRICING_PLANS"
        :key="plan.key"
        type="button"
        class="relative rounded-xl border-2 p-5 text-left transition-all"
        :class="data.plan === plan.key
          ? 'border-green-500 bg-green-50 dark:bg-green-900/20'
          : 'border-gray-200 dark:border-gray-700 hover:border-gray-300 dark:hover:border-gray-600'"
        @click="data.plan = plan.key"
      >
        <div v-if="plan.recommended" class="absolute -top-3 right-4">
          <UBadge color="primary" variant="solid" size="sm">
            {{ $t('inscription.stepPricing.recommended') }}
          </UBadge>
        </div>

        <div class="flex items-start justify-between">
          <div>
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">
              {{ $t(plan.name) }}
            </h3>
            <p class="text-sm text-gray-500 mt-1">{{ $t(plan.description) }}</p>
          </div>
          <span class="text-lg font-bold text-green-600 dark:text-green-400 shrink-0 ml-4">
            {{ $t(plan.price) }}
          </span>
        </div>

        <ul class="mt-3 grid grid-cols-2 gap-x-4 gap-y-1">
          <li
            v-for="(feature, i) in plan.features"
            :key="i"
            class="flex items-center gap-1.5 text-sm text-gray-600 dark:text-gray-400"
          >
            <UIcon name="i-heroicons-check" class="h-3.5 w-3.5 text-green-500 shrink-0" />
            {{ $t(feature) }}
          </li>
        </ul>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { PRICING_PLANS } from '~/utils/constants'

defineProps<{
  data: {
    plan: string
  }
}>()
</script>
