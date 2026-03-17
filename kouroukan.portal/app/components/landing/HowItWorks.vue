<template>
  <section class="py-20">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <SharedSectionTitle
        :title="$t('howItWorks.title')"
        :subtitle="$t('howItWorks.subtitle')"
        centered
      />

      <div class="relative">
        <!-- Connecting line (desktop) -->
        <div class="hidden lg:block absolute top-16 left-1/6 right-1/6 h-0.5 bg-green-200" />

        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8 lg:gap-12">
          <div v-for="(step, index) in steps" :key="step.key" class="text-center relative">
            <!-- Step number -->
            <div class="relative z-10 w-14 h-14 rounded-full bg-green-600 text-white font-bold text-xl flex items-center justify-center mx-auto shadow-lg">
              {{ index + 1 }}
            </div>

            <!-- Icon -->
            <div class="mt-6 w-16 h-16 rounded-2xl bg-green-50 dark:bg-green-900/20 flex items-center justify-center mx-auto">
              <UIcon :name="step.icon" class="w-8 h-8 text-green-600" />
            </div>

            <!-- Text -->
            <h3 class="mt-4 text-xl font-bold text-gray-900 dark:text-white">
              {{ $t(`howItWorks.steps.${step.key}.title`) }}
            </h3>
            <p class="mt-2 text-gray-600 dark:text-gray-400 max-w-xs mx-auto">
              {{ $t(`howItWorks.steps.${step.key}.description`) }}
            </p>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
const steps = [
  { key: 'register', icon: 'i-heroicons-user-plus' },
  { key: 'configure', icon: 'i-heroicons-cog-6-tooth' },
  { key: 'optimize', icon: 'i-heroicons-chart-bar' }
]
</script>
