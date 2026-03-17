<template>
  <div class="pt-24">
    <template v-if="mod">
      <!-- Hero -->
      <section class="py-16" :style="{ background: `linear-gradient(135deg, ${mod.color}15, ${mod.color}05)` }">
        <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
          <NuxtLink :to="localePath('/fonctionnalites')" class="inline-flex items-center gap-1 text-sm text-gray-500 hover:text-gray-700 mb-6">
            <UIcon name="i-heroicons-arrow-left" class="w-4 h-4" />
            {{ $t('features.overview') }}
          </NuxtLink>

          <div class="flex items-center gap-4 mb-6">
            <div class="w-14 h-14 rounded-xl flex items-center justify-center" :style="{ backgroundColor: `${mod.color}20` }">
              <UIcon :name="mod.icon" class="w-7 h-7" :style="{ color: mod.color }" />
            </div>
            <h1 class="text-3xl sm:text-4xl font-bold text-gray-900 dark:text-white">{{ $t(mod.name) }}</h1>
          </div>
          <p class="text-lg text-gray-600 dark:text-gray-400 leading-relaxed">{{ $t(mod.longDescription) }}</p>
        </div>
      </section>

      <!-- Key features for this module -->
      <section class="py-16">
        <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
          <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-8">Fonctionnalites cles</h2>
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div
              v-for="feature in moduleFeatures"
              :key="feature"
              class="flex items-start gap-3 p-4 rounded-lg bg-gray-50 dark:bg-gray-900"
            >
              <UIcon name="i-heroicons-check-circle" class="w-5 h-5 shrink-0 mt-0.5" :style="{ color: mod.color }" />
              <span class="text-gray-700 dark:text-gray-300">{{ feature }}</span>
            </div>
          </div>
        </div>
      </section>

      <!-- Screenshot -->
      <section class="py-16 bg-gray-50 dark:bg-gray-950">
        <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
          <SharedAppScreenshot :alt="$t(mod.name)" device="desktop" />
        </div>
      </section>

      <LandingCtaSection />
    </template>

    <!-- 404 -->
    <div v-else class="py-32 text-center">
      <h1 class="text-4xl font-bold text-gray-900 dark:text-white">Module non trouve</h1>
      <UButton :to="localePath('/fonctionnalites')" color="primary" class="mt-6">
        {{ $t('features.overview') }}
      </UButton>
    </div>
  </div>
</template>

<script setup lang="ts">
import { MODULE_LIST } from '~/utils/constants'

const localePath = useLocalePath()
const route = useRoute()

const mod = computed(() => MODULE_LIST.find(m => m.slug === route.params.slug))

if (mod.value) {
  useSeoMeta({ title: `${mod.value.slug.charAt(0).toUpperCase() + mod.value.slug.slice(1)} - Fonctionnalites` })
}

const moduleFeatureMap: Record<string, string[]> = {
  inscriptions: ['Gestion des dossiers d\'admission', 'Workflow d\'admission personnalisable', 'Suivi du statut des candidats', 'Import d\'eleves par fichier Excel', 'Gestion des annees scolaires', 'Matricules uniques automatiques'],
  pedagogie: ['Niveaux guineens (CP1-CM2, 7eme-10eme...)', 'Gestion des classes et capacites', 'Matieres avec coefficients', 'Emplois du temps automatises', 'Cahier de textes numerique', 'Gestion des salles et equipements'],
  evaluations: ['Saisie des notes par evaluation', 'Types : devoir, examen, CEE, BEPC, BU', 'Generation de bulletins PDF', 'Calcul automatique des moyennes et rangs', 'Appreciations par matiere et generales', 'Historique des resultats par eleve'],
  presences: ['Appel numerique par seance', 'Suivi des absences et retards', 'Justifications avec pieces jointes', 'Badgeage NFC / QR Code optionnel', 'Alertes automatiques aux parents', 'Statistiques de presences'],
  finances: ['Facturation par type de frais', 'Paiements Orange Money / Soutra / MTN', 'Caisse physique avec recus', 'Suivi des depenses par categorie', 'Remuneration des enseignants', 'Rapports financiers detailles'],
  personnel: ['Dossiers enseignants complets', 'Gestion des contrats et specialites', 'Demandes de conges en ligne', 'Suivi des heures effectuees', 'Modes : forfait, horaire ou mixte', 'Solde de conges automatique'],
  communication: ['Messagerie interne', 'Notifications push et SMS', 'Annonces sur tableau d\'affichage', 'Diffusion par groupe ou role', 'Alertes automatiques (absences, notes)', 'Integration NimbaSMS'],
  bde: ['Gestion des associations', 'Organisation d\'evenements', 'Cotisations et budgets BDE', 'Suivi des membres et roles', 'Validation des depenses', 'Bilan financier par association'],
  documents: ['Modeles personnalisables', 'Generation PDF automatique', 'Signature electronique (OpenSign)', 'Attestations et certificats officiels', 'Recus de paiement', 'Chaine de validation configurable'],
  'services-premium': ['Alertes SMS pour les parents', 'Graphiques de progression', 'Rapports hebdomadaires', 'Badge NFC pour enfants', 'Consultation enseignant en ligne', 'Archive complete des bulletins'],
  support: ['Centre d\'aide avec articles', 'Systeme de tickets', 'Suggestions d\'amelioration', 'Assistant IA integre', 'Base de connaissances', 'Votes sur les suggestions']
}

const moduleFeatures = computed(() => {
  const slug = route.params.slug as string
  return moduleFeatureMap[slug] || []
})
</script>
