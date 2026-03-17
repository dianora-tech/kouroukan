<template>
  <section class="py-20 bg-gray-50 dark:bg-gray-950">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <SharedSectionTitle
        :title="$t('features.title')"
        :subtitle="$t('features.subtitle')"
        centered
      />

      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
        <SharedFeatureCard
          v-for="vp in VALUE_PROPOSITIONS"
          :key="vp"
          :icon="icons[vp]"
          :title="$t(`features.items.${vp}.title`)"
          :description="$t(`features.items.${vp}.description`)"
          :color="colors[vp]"
        />
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { VALUE_PROPOSITIONS } from '~/utils/constants'

const icons: Record<string, string> = {
  academic: 'i-heroicons-academic-cap',
  payments: 'i-heroicons-banknotes',
  mobile: 'i-heroicons-device-phone-mobile',
  offline: 'i-heroicons-signal-slash',
  documents: 'i-heroicons-document-text',
  security: 'i-heroicons-shield-check'
}

const colors: Record<string, string> = {
  academic: '#2563eb',
  payments: '#dc2626',
  mobile: '#16a34a',
  offline: '#0891b2',
  documents: '#64748b',
  security: '#7c3aed'
}
</script>
