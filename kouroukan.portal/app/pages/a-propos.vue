<template>
  <div class="pt-24">
    <!-- Hero -->
    <section class="py-16 bg-gradient-to-br from-green-700 to-green-800">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
        <h1 class="text-4xl font-bold text-white">{{ $t('about.title') }}</h1>
      </div>
    </section>

    <!-- Mission -->
    <section class="py-16">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        <SharedSectionTitle :title="$t('about.mission.title')" />
        <p class="text-lg text-gray-600 dark:text-gray-400 leading-relaxed">
          {{ $t('about.mission.description') }}
        </p>
      </div>
    </section>

    <!-- Values -->
    <section class="py-16 bg-gray-50 dark:bg-gray-950">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <SharedSectionTitle :title="$t('about.values.title')" centered />
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
          <SharedFeatureCard
            v-for="value in values"
            :key="value.key"
            :icon="value.icon"
            :title="$t(`about.values.${value.key}.title`)"
            :description="$t(`about.values.${value.key}.description`)"
            :color="value.color"
          />
        </div>
      </div>
    </section>

    <!-- Context -->
    <section class="py-16">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        <SharedSectionTitle :title="$t('about.context.title')" />
        <p class="text-lg text-gray-600 dark:text-gray-400 leading-relaxed">
          {{ $t('about.context.description') }}
        </p>
      </div>
    </section>

    <LandingCtaSection />
  </div>
</template>

<script setup lang="ts">
useSeoMeta({ title: 'A propos' })

const values = [
  { key: 'accessibility', icon: 'i-heroicons-device-phone-mobile', color: '#16a34a' },
  { key: 'simplicity', icon: 'i-heroicons-sparkles', color: '#2563eb' },
  { key: 'trust', icon: 'i-heroicons-shield-check', color: '#7c3aed' },
  { key: 'local', icon: 'i-heroicons-map-pin', color: '#dc2626' }
]
</script>
