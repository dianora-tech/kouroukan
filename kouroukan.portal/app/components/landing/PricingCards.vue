<template>
  <section class="py-20">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <SharedSectionTitle
        :title="$t('pricing.title')"
        :subtitle="$t('pricing.subtitle')"
        centered
      />

      <div class="grid grid-cols-1 md:grid-cols-3 gap-8 max-w-5xl mx-auto">
        <div
          v-for="plan in plans"
          :key="plan.key"
          class="relative rounded-2xl border-2 p-8 flex flex-col"
          :class="plan.recommended
            ? 'border-green-600 bg-green-50/50 dark:bg-green-900/10 shadow-xl scale-105'
            : 'border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900'"
        >
          <!-- Recommended badge -->
          <div v-if="plan.recommended" class="absolute -top-3 left-1/2 -translate-x-1/2">
            <UBadge color="primary" size="sm">{{ $t('pricing.recommended') }}</UBadge>
          </div>

          <!-- Plan name -->
          <h3 class="text-xl font-bold text-gray-900 dark:text-white">
            {{ $t(`pricing.plans.${plan.key}.name`) }}
          </h3>

          <!-- Price -->
          <div class="mt-4">
            <span class="text-3xl font-bold text-gray-900 dark:text-white">
              {{ plan.key === 'starter' ? $t('pricing.free') : $t('pricing.onQuote') }}
            </span>
          </div>

          <!-- Description -->
          <p class="mt-2 text-sm text-gray-500">{{ $t(`pricing.plans.${plan.key}.description`) }}</p>

          <!-- Features -->
          <ul class="mt-8 space-y-3 flex-1">
            <li
              v-for="(_, fIndex) in Array(6)"
              :key="fIndex"
              class="flex items-start gap-2"
            >
              <UIcon name="i-heroicons-check" class="w-5 h-5 text-green-600 shrink-0 mt-0.5" />
              <span class="text-sm text-gray-700 dark:text-gray-300">
                {{ $t(`pricing.plans.${plan.key}.features[${fIndex}]`) }}
              </span>
            </li>
          </ul>

          <!-- CTA -->
          <UButton
            :to="plan.key === 'starter' ? localePath('/inscription') : localePath('/contact')"
            :color="plan.recommended ? 'primary' : 'neutral'"
            :variant="plan.recommended ? 'solid' : 'outline'"
            class="mt-8"
            block
          >
            {{ plan.key === 'starter' ? $t('pricing.cta') : $t('pricing.ctaContact') }}
          </UButton>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
const localePath = useLocalePath()

const plans = [
  { key: 'starter', recommended: false },
  { key: 'standard', recommended: true },
  { key: 'premium', recommended: false }
]
</script>
