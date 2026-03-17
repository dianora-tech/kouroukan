<template>
  <section class="py-20 bg-gray-50 dark:bg-gray-950">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <SharedSectionTitle
        :title="$t('problem.title')"
        :subtitle="$t('problem.subtitle')"
        centered
      />

      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
        <div
          v-for="pain in PAIN_POINTS"
          :key="pain"
          class="p-6 rounded-xl bg-white dark:bg-gray-900 border border-red-100 dark:border-red-900/30 hover:shadow-lg transition-shadow"
        >
          <div class="w-12 h-12 rounded-lg bg-red-50 dark:bg-red-900/20 flex items-center justify-center mb-4">
            <UIcon :name="icons[pain]" class="w-6 h-6 text-red-600" />
          </div>
          <h3 class="font-semibold text-gray-900 dark:text-white mb-2">
            {{ $t(`problem.items.${pain}.title`) }}
          </h3>
          <p class="text-sm text-gray-600 dark:text-gray-400 leading-relaxed">
            {{ $t(`problem.items.${pain}.description`) }}
          </p>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { PAIN_POINTS } from '~/utils/constants'

const icons: Record<string, string> = {
  paper: 'i-heroicons-document-minus',
  visibility: 'i-heroicons-eye-slash',
  payments: 'i-heroicons-currency-dollar',
  communication: 'i-heroicons-no-symbol'
}
</script>
