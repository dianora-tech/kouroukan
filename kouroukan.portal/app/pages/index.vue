<template>
  <div>
    <LandingHeroSection />
    <LandingProblemSection />
    <LandingSolutionSection />
    <LandingFeaturesGrid />
    <LandingHowItWorks />
    <LandingStatsCounter />
    <LandingLiveStatsBar />
    <LandingTestimonialsCarousel />
    <LandingPricingCards />

    <!-- Services Digitaux section -->
    <section class="py-20 bg-gray-50 dark:bg-gray-950">
      <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8">
        <SharedSectionTitle
          :title="$t('servicesDigitaux.sectionTitle')"
          :subtitle="$t('servicesDigitaux.sectionSubtitle')"
          centered
        />

        <div class="grid grid-cols-1 md:grid-cols-2 gap-8 max-w-3xl mx-auto">
          <!-- Communication -->
          <div class="bg-white dark:bg-gray-900 rounded-2xl border border-gray-200 dark:border-gray-800 p-8 flex flex-col">
            <div class="w-12 h-12 rounded-xl bg-green-100 dark:bg-green-900/30 flex items-center justify-center mb-4">
              <UIcon name="i-heroicons-chat-bubble-left-right" class="w-6 h-6 text-green-600" />
            </div>
            <h3 class="text-lg font-bold text-gray-900 dark:text-white">
              {{ $t('servicesDigitaux.communication.title') }}
            </h3>
            <p class="text-sm text-gray-500 mt-2 flex-1">
              {{ $t('servicesDigitaux.communication.shortDescription') }}
            </p>
            <p class="text-sm font-semibold text-green-600 mt-3">
              {{ $t('servicesDigitaux.communication.price') }}
            </p>
            <NuxtLink
              :to="localePath('/services-digitaux')"
              class="mt-4 text-sm font-medium text-green-600 hover:text-green-700 flex items-center gap-1"
            >
              {{ $t('common.learnMore') }}
              <UIcon name="i-heroicons-arrow-right" class="w-4 h-4" />
            </NuxtLink>
          </div>

          <!-- GPS Tracker -->
          <div class="bg-white dark:bg-gray-900 rounded-2xl border border-gray-200 dark:border-gray-800 p-8 flex flex-col opacity-80">
            <div class="flex items-start justify-between mb-4">
              <div class="w-12 h-12 rounded-xl bg-blue-100 dark:bg-blue-900/30 flex items-center justify-center">
                <UIcon name="i-heroicons-map-pin" class="w-6 h-6 text-blue-600" />
              </div>
              <UBadge color="warning" variant="subtle" size="xs">
                {{ $t('servicesDigitaux.gpsTracker.badge') }}
              </UBadge>
            </div>
            <h3 class="text-lg font-bold text-gray-900 dark:text-white">
              {{ $t('servicesDigitaux.gpsTracker.title') }}
            </h3>
            <p class="text-sm text-gray-500 mt-2 flex-1">
              {{ $t('servicesDigitaux.gpsTracker.shortDescription') }}
            </p>
            <NuxtLink
              :to="localePath('/services-digitaux')"
              class="mt-4 text-sm font-medium text-gray-400 flex items-center gap-1"
            >
              {{ $t('common.learnMore') }}
              <UIcon name="i-heroicons-arrow-right" class="w-4 h-4" />
            </NuxtLink>
          </div>
        </div>
      </div>
    </section>

    <LandingFaqAccordion />
    <LandingCtaSection />
  </div>
</template>

<script setup lang="ts">
const localePath = useLocalePath()

useSeoMeta({
  title: 'Kouroukan — Gerez votre etablissement scolaire, simplement.',
  ogTitle: 'Kouroukan — Gerez votre etablissement scolaire, simplement.',
  description: 'Plateforme de gestion scolaire pour la Guinee. Inscriptions, notes, paiements Mobile Money, bulletins, emplois du temps — tout en une plateforme.',
  ogDescription: 'Plateforme de gestion scolaire pour la Guinee. Inscriptions, notes, paiements Mobile Money, bulletins, emplois du temps — tout en une plateforme.',
  ogImage: '/icons/icon-512x512.png'
})

useSchemaOrg([
  defineWebSite({
    name: 'Kouroukan',
    description: 'Plateforme de gestion d\'etablissement scolaire pour la Guinee'
  })
])
</script>
