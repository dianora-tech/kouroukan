<template>
  <section class="py-20">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <SharedSectionTitle
        :title="$t('pricing.title')"
        :subtitle="$t('pricing.subtitle')"
        centered
      />

      <!-- Pricing rules -->
      <div class="flex flex-wrap justify-center gap-4 mb-12">
        <div class="flex items-center gap-2 px-4 py-2 bg-green-50 dark:bg-green-900/20 rounded-full">
          <UIcon name="i-heroicons-clock" class="w-4 h-4 text-green-600" />
          <span class="text-sm text-gray-700 dark:text-gray-300">{{ $t('pricing.rules.trial') }}</span>
        </div>
        <div class="flex items-center gap-2 px-4 py-2 bg-blue-50 dark:bg-blue-900/20 rounded-full">
          <UIcon name="i-heroicons-sun" class="w-4 h-4 text-blue-600" />
          <span class="text-sm text-gray-700 dark:text-gray-300">{{ $t('pricing.rules.vacation') }}</span>
        </div>
        <div class="flex items-center gap-2 px-4 py-2 bg-purple-50 dark:bg-purple-900/20 rounded-full">
          <UIcon name="i-heroicons-calendar" class="w-4 h-4 text-purple-600" />
          <span class="text-sm text-gray-700 dark:text-gray-300">{{ $t('pricing.rules.billing') }}</span>
        </div>
      </div>

      <!-- Plans grid: 3 establishment plans on top, 2 individual plans below -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 max-w-5xl mx-auto mb-8">
        <div
          v-for="plan in establishmentPlans"
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

          <!-- Target badge -->
          <UBadge color="neutral" variant="subtle" size="xs" class="w-fit mb-3">
            {{ $t(plan.target) }}
          </UBadge>

          <!-- Plan name -->
          <h3 class="text-xl font-bold text-gray-900 dark:text-white">
            {{ $t(`pricing.plans.${plan.key}.name`) }}
          </h3>

          <!-- Price -->
          <div class="mt-4">
            <span class="text-3xl font-bold text-gray-900 dark:text-white">
              {{ $t(`pricing.plans.${plan.key}.price`) }}
            </span>
            <span v-if="plan.priceAmount > 0" class="text-sm text-gray-500 ml-1">/ {{ $t('pricing.monthly') }}</span>
          </div>

          <!-- Description -->
          <p class="mt-2 text-sm text-gray-500">{{ $t(`pricing.plans.${plan.key}.description`) }}</p>

          <!-- Features -->
          <ul class="mt-8 space-y-3 flex-1">
            <li
              v-for="(_, fIndex) in plan.features"
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
            :to="`${localePath('/inscription')}?type=etablissement`"
            :color="plan.recommended ? 'primary' : 'neutral'"
            :variant="plan.recommended ? 'solid' : 'outline'"
            class="mt-8"
            block
          >
            {{ plan.priceAmount === 0 ? $t('pricing.cta') : $t('pricing.ctaTrial') }}
          </UButton>
        </div>
      </div>

      <!-- Individual plans -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6 max-w-3xl mx-auto">
        <div
          v-for="plan in individualPlans"
          :key="plan.key"
          class="relative rounded-2xl border-2 p-8 flex flex-col border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900"
        >
          <!-- Target badge -->
          <UBadge color="neutral" variant="subtle" size="xs" class="w-fit mb-3">
            {{ $t(plan.target) }}
          </UBadge>

          <!-- Plan name -->
          <h3 class="text-xl font-bold text-gray-900 dark:text-white">
            {{ $t(`pricing.plans.${plan.key}.name`) }}
          </h3>

          <!-- Price -->
          <div class="mt-4">
            <span class="text-3xl font-bold text-gray-900 dark:text-white">
              {{ $t(`pricing.plans.${plan.key}.price`) }}
            </span>
            <span class="text-sm text-gray-500 ml-1">/ {{ $t('pricing.monthly') }}</span>
          </div>

          <!-- Description -->
          <p class="mt-2 text-sm text-gray-500">{{ $t(`pricing.plans.${plan.key}.description`) }}</p>

          <!-- Features -->
          <ul class="mt-6 space-y-3 flex-1">
            <li
              v-for="(_, fIndex) in plan.features"
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
            :to="`${localePath('/inscription')}?type=${plan.key}`"
            color="neutral"
            variant="outline"
            class="mt-8"
            block
          >
            {{ $t('pricing.ctaTrial') }}
          </UButton>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { PRICING_PLANS } from '~/utils/constants'

const localePath = useLocalePath()

const establishmentPlans = PRICING_PLANS.filter(p => ['gratuit', 'standard', 'premium'].includes(p.key))
const individualPlans = PRICING_PLANS.filter(p => ['enseignant', 'parent'].includes(p.key))
</script>
