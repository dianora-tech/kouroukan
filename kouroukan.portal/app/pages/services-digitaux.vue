<template>
  <div class="pt-24">
    <!-- Hero -->
    <section class="py-16 bg-gradient-to-b from-green-50 to-white dark:from-gray-900 dark:to-gray-950">
      <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
        <UBadge color="primary" variant="subtle" class="mb-4">{{ $t('servicesDigitaux.badge') }}</UBadge>
        <h1 class="text-4xl sm:text-5xl font-bold text-gray-900 dark:text-white">
          {{ $t('servicesDigitaux.title') }}
        </h1>
        <p class="mt-4 text-lg text-gray-600 dark:text-gray-400 max-w-2xl mx-auto">
          {{ $t('servicesDigitaux.subtitle') }}
        </p>
      </div>
    </section>

    <!-- Service 1: Communication Etablissement -->
    <section class="py-20">
      <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-12 items-center">
          <div>
            <UBadge color="primary" variant="subtle" class="mb-4">
              {{ $t('servicesDigitaux.communication.badge') }}
            </UBadge>
            <h2 class="text-3xl font-bold text-gray-900 dark:text-white">
              {{ $t('servicesDigitaux.communication.title') }}
            </h2>
            <p class="mt-4 text-gray-600 dark:text-gray-400">
              {{ $t('servicesDigitaux.communication.description') }}
            </p>

            <div class="mt-4 p-4 rounded-xl bg-green-50 dark:bg-green-900/20 border border-green-200 dark:border-green-800">
              <p class="text-2xl font-bold text-green-600">{{ $t('servicesDigitaux.communication.price') }}</p>
            </div>

            <ul class="mt-6 space-y-3">
              <li v-for="i in 4" :key="i" class="flex items-start gap-3">
                <UIcon name="i-heroicons-check-circle" class="w-5 h-5 text-green-600 shrink-0 mt-0.5" />
                <span class="text-gray-700 dark:text-gray-300">
                  {{ $t(`servicesDigitaux.communication.features[${i - 1}]`) }}
                </span>
              </li>
            </ul>

            <UButton
              :to="localePath('/inscription')"
              color="primary"
              size="lg"
              class="mt-8"
            >
              {{ $t('servicesDigitaux.communication.cta') }}
            </UButton>
          </div>

          <div class="flex items-center justify-center">
            <div class="w-full max-w-sm bg-white dark:bg-gray-900 rounded-2xl border border-gray-200 dark:border-gray-800 p-8 shadow-lg">
              <div class="w-16 h-16 rounded-2xl bg-green-100 dark:bg-green-900/30 flex items-center justify-center mx-auto mb-6">
                <UIcon name="i-heroicons-chat-bubble-left-right" class="w-8 h-8 text-green-600" />
              </div>
              <h3 class="text-center text-lg font-semibold text-gray-900 dark:text-white mb-4">
                {{ $t('servicesDigitaux.communication.cardTitle') }}
              </h3>
              <div class="space-y-3">
                <div v-for="i in 3" :key="i" class="flex items-center gap-3 p-3 rounded-lg bg-gray-50 dark:bg-gray-800">
                  <UIcon name="i-heroicons-bell-alert" class="w-5 h-5 text-green-600 shrink-0" />
                  <span class="text-sm text-gray-700 dark:text-gray-300">
                    {{ $t(`servicesDigitaux.communication.notifications[${i - 1}]`) }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Service 2: GPS Tracker -->
    <section class="py-20 bg-gray-50 dark:bg-gray-950">
      <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-12 items-center">
          <div class="order-2 lg:order-1 flex items-center justify-center">
            <div class="w-full max-w-sm bg-white dark:bg-gray-900 rounded-2xl border border-gray-200 dark:border-gray-800 p-8 shadow-lg opacity-75">
              <div class="w-16 h-16 rounded-2xl bg-blue-100 dark:bg-blue-900/30 flex items-center justify-center mx-auto mb-6">
                <UIcon name="i-heroicons-map-pin" class="w-8 h-8 text-blue-600" />
              </div>
              <h3 class="text-center text-lg font-semibold text-gray-900 dark:text-white mb-4">
                {{ $t('servicesDigitaux.gpsTracker.cardTitle') }}
              </h3>
              <div class="space-y-3">
                <div v-for="i in 3" :key="i" class="flex items-center gap-3 p-3 rounded-lg bg-gray-50 dark:bg-gray-800">
                  <UIcon name="i-heroicons-map" class="w-5 h-5 text-blue-600 shrink-0" />
                  <span class="text-sm text-gray-700 dark:text-gray-300">
                    {{ $t(`servicesDigitaux.gpsTracker.preview[${i - 1}]`) }}
                  </span>
                </div>
              </div>
            </div>
          </div>

          <div class="order-1 lg:order-2">
            <div class="flex items-center gap-3 mb-4">
              <UBadge color="warning" variant="subtle">
                {{ $t('servicesDigitaux.gpsTracker.badge') }}
              </UBadge>
            </div>
            <h2 class="text-3xl font-bold text-gray-900 dark:text-white">
              {{ $t('servicesDigitaux.gpsTracker.title') }}
            </h2>
            <p class="mt-4 text-gray-600 dark:text-gray-400">
              {{ $t('servicesDigitaux.gpsTracker.description') }}
            </p>

            <ul class="mt-6 space-y-3">
              <li v-for="i in 3" :key="i" class="flex items-start gap-3">
                <UIcon name="i-heroicons-check-circle" class="w-5 h-5 text-blue-600 shrink-0 mt-0.5" />
                <span class="text-gray-700 dark:text-gray-300">
                  {{ $t(`servicesDigitaux.gpsTracker.features[${i - 1}]`) }}
                </span>
              </li>
            </ul>

            <UButton
              color="neutral"
              variant="outline"
              size="lg"
              class="mt-8"
              disabled
            >
              {{ $t('servicesDigitaux.gpsTracker.cta') }}
            </UButton>
          </div>
        </div>
      </div>
    </section>

    <LandingCtaSection />
  </div>
</template>

<script setup lang="ts">
definePageMeta({ layout: 'default' })

const localePath = useLocalePath()

useSeoMeta({
  title: 'Services Digitaux - Kouroukan',
  description: 'Services digitaux pour parents et etablissements scolaires en Guinee.'
})
</script>
