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
    <LandingFaqAccordion />
    <LandingCtaSection />
  </div>
</template>

<script setup lang="ts">
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
