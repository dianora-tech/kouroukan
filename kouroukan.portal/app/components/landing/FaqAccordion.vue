<template>
  <section class="py-20">
    <div class="max-w-3xl mx-auto px-4 sm:px-6 lg:px-8">
      <SharedSectionTitle
        :title="$t('faqSection.title')"
        :subtitle="$t('faqSection.subtitle')"
        centered
      />

      <UAccordion :items="faqItems" />

      <div class="text-center mt-8">
        <UButton :to="localePath('/faq')" variant="outline" color="primary">
          {{ $t('faqSection.seeAll') }}
          <UIcon name="i-heroicons-arrow-right" class="w-4 h-4" />
        </UButton>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
const localePath = useLocalePath()
const { t } = useI18n()

const faqItems = [
  {
    label: "Qu'est-ce que Kouroukan ?",
    content: "Kouroukan est une plateforme numerique de gestion d'etablissement scolaire couvrant le prescolaire, primaire, secondaire et universitaire. Elle est concue specifiquement pour le systeme educatif guineen."
  },
  {
    label: "L'application fonctionne-t-elle sans Internet ?",
    content: "Oui, Kouroukan dispose d'un mode hors ligne. Vous pouvez continuer a travailler meme sans connexion. Les donnees se synchronisent automatiquement lorsque la connexion revient."
  },
  {
    label: 'Quels types d\'etablissements sont pris en charge ?',
    content: "Kouroukan prend en charge tous les types d'etablissements guineens : public, prive laique, prive franco-arabe, communautaire, catholique et protestant, du prescolaire a l'universite."
  },
  {
    label: 'Quels moyens de paiement sont disponibles ?',
    content: "Vous pouvez accepter les paiements par Orange Money, Soutra Money, MTN MoMo et en especes. La reconciliation est automatique avec generation de recus instantanes."
  },
  {
    label: 'Le plan STARTER est-il vraiment gratuit ?',
    content: "Oui, le plan STARTER est entierement gratuit et sans engagement. Il permet de gerer jusqu'a 100 eleves avec les fonctionnalites essentielles : inscriptions, notes, bulletins, presences et communication."
  },
  {
    label: 'Comment gerer les niveaux du systeme guineen (CP1, 7eme, etc.) ?',
    content: "Kouroukan integre nativement la nomenclature guineenne : CP1 a CM2 pour le primaire, 7eme a 10eme pour le college, 11eme-12eme-Terminale pour le lycee, et le systeme LMD pour l'universite."
  }
]
</script>
