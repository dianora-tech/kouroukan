<template>
  <div class="pt-24">
    <!-- Live stats bar -->
    <LandingLiveStatsBar />

    <!-- Pricing cards -->
    <LandingPricingCards />

    <!-- Comparison table -->
    <section class="py-16 bg-gray-50 dark:bg-gray-950">
      <div class="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
        <SharedSectionTitle :title="$t('pricing.compare')" centered />

        <div class="overflow-x-auto">
          <table class="w-full text-sm">
            <thead>
              <tr class="border-b border-gray-200 dark:border-gray-800">
                <th class="py-4 px-4 text-left font-medium text-gray-500">{{ $t('nav.features') }}</th>
                <th class="py-4 px-4 text-center font-semibold text-xs">{{ $t('pricing.plans.gratuit.name') }}</th>
                <th class="py-4 px-4 text-center font-semibold text-green-600 text-xs">{{ $t('pricing.plans.standard.name') }}</th>
                <th class="py-4 px-4 text-center font-semibold text-xs">{{ $t('pricing.plans.premium.name') }}</th>
                <th class="py-4 px-4 text-center font-semibold text-xs">{{ $t('pricing.plans.enseignant.name') }}</th>
                <th class="py-4 px-4 text-center font-semibold text-xs">{{ $t('pricing.plans.parent.name') }}</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="feature in comparisonFeatures" :key="feature.name" class="border-b border-gray-100 dark:border-gray-800/50">
                <td class="py-3 px-4 text-gray-700 dark:text-gray-300">{{ feature.name }}</td>
                <td class="py-3 px-4 text-center">
                  <UIcon v-if="feature.gratuit" name="i-heroicons-check" class="w-5 h-5 text-green-600 mx-auto" />
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
                <td class="py-3 px-4 text-center">
                  <UIcon v-if="feature.enseignant" name="i-heroicons-check" class="w-5 h-5 text-green-600 mx-auto" />
                  <UIcon v-else name="i-heroicons-x-mark" class="w-5 h-5 text-gray-300 mx-auto" />
                </td>
                <td class="py-3 px-4 text-center">
                  <UIcon v-if="feature.parent" name="i-heroicons-check" class="w-5 h-5 text-green-600 mx-auto" />
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
  { name: 'Inscriptions et dossiers', gratuit: true, standard: true, premium: true, enseignant: false, parent: false },
  { name: 'Notes et bulletins', gratuit: true, standard: true, premium: true, enseignant: true, parent: true },
  { name: 'Appels et presences', gratuit: true, standard: true, premium: true, enseignant: true, parent: false },
  { name: 'Emploi du temps', gratuit: true, standard: true, premium: true, enseignant: true, parent: false },
  { name: 'Communication interne', gratuit: true, standard: true, premium: true, enseignant: true, parent: true },
  { name: 'Eleves illimites', gratuit: false, standard: true, premium: true, enseignant: false, parent: false },
  { name: 'Paiements Mobile Money', gratuit: false, standard: true, premium: true, enseignant: false, parent: true },
  { name: 'Gestion financiere', gratuit: false, standard: true, premium: true, enseignant: false, parent: false },
  { name: 'Notifications SMS/Push', gratuit: false, standard: true, premium: true, enseignant: true, parent: true },
  { name: 'Suivi enfant', gratuit: false, standard: false, premium: false, enseignant: false, parent: true },
  { name: 'Espace personnel enseignant', gratuit: false, standard: false, premium: false, enseignant: true, parent: false },
  { name: 'Signature electronique', gratuit: false, standard: false, premium: true, enseignant: false, parent: false },
  { name: 'Documents personnalises', gratuit: false, standard: false, premium: true, enseignant: false, parent: false },
  { name: 'Module BDE', gratuit: false, standard: false, premium: true, enseignant: false, parent: false },
  { name: 'Multi-etablissements', gratuit: false, standard: false, premium: true, enseignant: false, parent: false },
  { name: 'Support prioritaire', gratuit: false, standard: false, premium: true, enseignant: false, parent: false }
]
</script>
