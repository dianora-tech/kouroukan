<template>
  <div class="pt-24">
    <!-- Live stats bar -->
    <LandingLiveStatsBar />

    <!-- Pricing cards -->
    <LandingPricingCards />

    <!-- Comparison table -->
    <section class="py-16 bg-gray-50 dark:bg-gray-950">
      <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8">
        <SharedSectionTitle :title="$t('pricing.compare')" centered />

        <div class="overflow-x-auto">
          <table class="w-full text-sm">
            <thead>
              <tr class="border-b border-gray-200 dark:border-gray-800">
                <th class="py-4 px-4 text-left font-medium text-gray-500">{{ $t('nav.features') }}</th>
                <th class="py-4 px-4 text-center font-semibold">STARTER</th>
                <th class="py-4 px-4 text-center font-semibold text-green-600">STANDARD</th>
                <th class="py-4 px-4 text-center font-semibold">PREMIUM</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="feature in comparisonFeatures" :key="feature.name" class="border-b border-gray-100 dark:border-gray-800/50">
                <td class="py-3 px-4 text-gray-700 dark:text-gray-300">{{ feature.name }}</td>
                <td class="py-3 px-4 text-center">
                  <UIcon v-if="feature.starter" name="i-heroicons-check" class="w-5 h-5 text-green-600 mx-auto" />
                  <UIcon v-else name="i-heroicons-x-mark" class="w-5 h-5 text-gray-300 mx-auto" />
                </td>
                <td class="py-3 px-4 text-center">
                  <UIcon v-if="feature.standard" name="i-heroicons-check" class="w-5 h-5 text-green-600 mx-auto" />
                  <UIcon v-else name="i-heroicons-x-mark" class="w-5 h-5 text-gray-300 mx-auto" />
                </td>
                <td class="py-3 px-4 text-center">
                  <UIcon v-if="feature.premium" name="i-heroicons-check" class="w-5 h-5 text-green-600 mx-auto" />
                  <UIcon v-else name="i-heroicons-x-mark" class="w-5 h-5 text-gray-300 mx-auto" />
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </section>

    <LandingFaqAccordion />
    <LandingCtaSection />
  </div>
</template>

<script setup lang="ts">
useSeoMeta({ title: 'Tarifs' })

const comparisonFeatures = [
  { name: 'Inscriptions et dossiers', starter: true, standard: true, premium: true },
  { name: 'Notes et bulletins', starter: true, standard: true, premium: true },
  { name: 'Appels et presences', starter: true, standard: true, premium: true },
  { name: 'Emploi du temps', starter: true, standard: true, premium: true },
  { name: 'Communication interne', starter: true, standard: true, premium: true },
  { name: 'Eleves illimites', starter: false, standard: true, premium: true },
  { name: 'Paiements Mobile Money', starter: false, standard: true, premium: true },
  { name: 'Gestion financiere', starter: false, standard: true, premium: true },
  { name: 'Notifications SMS/Push', starter: false, standard: true, premium: true },
  { name: 'Signature electronique', starter: false, standard: false, premium: true },
  { name: 'Documents personnalises', starter: false, standard: false, premium: true },
  { name: 'Module BDE', starter: false, standard: false, premium: true },
  { name: 'Multi-etablissements', starter: false, standard: false, premium: true },
  { name: 'Support prioritaire', starter: false, standard: false, premium: true }
]
</script>
