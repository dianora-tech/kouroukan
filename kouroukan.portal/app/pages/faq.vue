<template>
  <div class="pt-24">
    <section class="py-16">
      <div class="max-w-3xl mx-auto px-4 sm:px-6 lg:px-8">
        <SharedSectionTitle
          :title="$t('faqSection.title')"
          :subtitle="$t('faqSection.subtitle')"
          centered
        />

        <!-- Category filter -->
        <div class="flex flex-wrap justify-center gap-2 mb-8">
          <UButton
            v-for="cat in faqCategories"
            :key="cat"
            size="sm"
            :variant="activeCategory === cat ? 'solid' : 'outline'"
            :color="activeCategory === cat ? 'primary' : 'neutral'"
            @click="activeCategory = cat"
          >
            {{ cat === 'all' ? 'Toutes' : cat.charAt(0).toUpperCase() + cat.slice(1) }}
          </UButton>
        </div>

        <UAccordion :items="filteredFaq" />
      </div>
    </section>

    <LandingCtaSection />
  </div>
</template>

<script setup lang="ts">
useSeoMeta({ title: 'FAQ' })

const activeCategory = ref('all')

const allFaq = [
  { label: "Qu'est-ce que Kouroukan ?", content: "Kouroukan est une plateforme numerique de gestion d'etablissement scolaire couvrant le prescolaire, le primaire, le secondaire et l'universitaire. Elle est specialement concue pour le systeme educatif guineen avec les nomenclatures officielles.", category: 'general' },
  { label: "Comment creer un compte ?", content: "Cliquez sur 'Commencer gratuitement', remplissez vos informations en 3 etapes (profil, activite, localisation) et votre compte est cree instantanement. Aucun engagement requis.", category: 'general' },
  { label: "L'application fonctionne-t-elle sans Internet ?", content: "Oui, Kouroukan dispose d'un mode hors ligne (PWA). Vous pouvez consulter les donnees et effectuer certaines operations meme sans connexion. La synchronisation se fait automatiquement.", category: 'general' },
  { label: "Quels sont les tarifs ?", content: "Le plan STARTER est gratuit (jusqu'a 100 eleves). Les plans STANDARD et PREMIUM sont sur devis selon la taille de votre etablissement. Contactez-nous pour un devis personnalise.", category: 'general' },
  { label: "Mes donnees sont-elles securisees ?", content: "Absolument. Les donnees sont chiffrees, sauvegardees automatiquement et accessibles uniquement via un systeme de controle d'acces par roles. Nous utilisons les meilleures pratiques de securite.", category: 'general' },
  { label: "Quels types d'etablissements sont pris en charge ?", content: "Tous les types d'etablissements guineens : public, prive laique, prive franco-arabe, communautaire, catholique et protestant, du prescolaire a l'universite, y compris les etablissements ETFP.", category: 'guinee' },
  { label: "Comment gerer les niveaux du systeme guineen (CP1, CP2, 7eme-10eme) ?", content: "Kouroukan integre nativement la nomenclature guineenne complete : PS/MS/GS pour le prescolaire, CP1-CM2 pour le primaire, 7eme a 10eme pour le college, 11eme-12eme-Terminale pour le lycee.", category: 'guinee' },
  { label: "Peut-on suivre les resultats du CEE, BEPC et Baccalaureat ?", content: "Oui, le module Evaluations permet de creer des evaluations de type examen national (CEE, BEPC, BU) et de suivre les resultats par eleve, par classe et par etablissement.", category: 'guinee' },
  { label: "Comment gerer les redoublements ?", content: "Le systeme de redoublement est gere nativement : chaque inscription indique si l'eleve est redoublant. Les statistiques de redoublement sont disponibles dans les rapports.", category: 'guinee' },
  { label: "L'application prend-elle en charge les etablissements ETFP ?", content: "Oui, les cycles ETFP (post-primaire, Type A, Type B) sont integres avec leurs niveaux, diplomes (CQP, BEP, CAP, BTS) et les passerelles vers l'enseignement general.", category: 'guinee' },
  { label: "Comment configurer les frais de scolarite selon le type d'etablissement ?", content: "Les frais sont parametrables par type d'etablissement, niveau et annee scolaire. Les etablissements publics peuvent configurer uniquement les frais annexes (cantine, transport).", category: 'guinee' },
  { label: "Peut-on generer des attestations de reussite aux examens nationaux ?", content: "Oui, le module Documents propose des modeles specifiques : attestation de reussite CEE, BEPC et BU, en plus des bulletins et certificats standards.", category: 'guinee' },
  { label: "L'application fonctionne-t-elle avec une connexion faible (zone rurale) ?", content: "Oui, Kouroukan est optimise pour les connexions 2G/3G. Le mode economie de donnees reduit les transferts et le mode hors ligne permet de travailler sans connexion.", category: 'guinee' },
  { label: "Comment sont geres les 3 ministeres (MENA, METFP-ET, MESRS) ?", content: "Chaque niveau est associe a son ministere de tutelle. Les nomenclatures et reglementations specifiques a chaque ministere sont integrees dans le parametrage.", category: 'guinee' },
  { label: "Quels moyens de paiement sont disponibles ?", content: "Orange Money, Soutra Money, MTN MoMo et especes. La reconciliation est automatique pour le Mobile Money avec generation de recus instantanes.", category: 'technique' },
  { label: "Peut-on importer des donnees existantes ?", content: "Oui, vous pouvez importer vos listes d'eleves, enseignants et notes depuis des fichiers Excel. Un assistant d'importation vous guide dans le processus.", category: 'technique' },
  { label: "L'application est-elle disponible sur mobile ?", content: "Oui, Kouroukan est une Progressive Web App (PWA) installable sur tout smartphone. L'interface est optimisee pour une utilisation mobile.", category: 'technique' },
  { label: "Combien d'utilisateurs peuvent utiliser l'application simultanement ?", content: "Il n'y a pas de limite d'utilisateurs simultanes. L'architecture est concue pour supporter des milliers de connexions en parallele.", category: 'technique' },
  { label: "Peut-on changer de plan a tout moment ?", content: "Oui, vous pouvez passer du plan STARTER au plan STANDARD ou PREMIUM a tout moment. La migration est transparente et sans perte de donnees.", category: 'tarifs' },
  { label: "Y a-t-il des frais caches ?", content: "Non, le plan STARTER est entierement gratuit. Pour les plans payants, le tarif est fixe par devis sans frais caches. Les mises a jour et le support de base sont inclus.", category: 'tarifs' },
  { label: "Proposez-vous des remises pour les ecoles publiques ?", content: "Oui, nous proposons des conditions speciales pour les etablissements publics et communautaires. Contactez-nous pour en savoir plus.", category: 'tarifs' }
]

const faqCategories = ['all', 'general', 'guinee', 'technique', 'tarifs']

const filteredFaq = computed(() => {
  const items = activeCategory.value === 'all'
    ? allFaq
    : allFaq.filter(f => f.category === activeCategory.value)
  return items.map(f => ({ label: f.label, content: f.content }))
})
</script>
