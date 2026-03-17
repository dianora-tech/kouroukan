<template>
  <div>
    <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">{{ $t('legal.cgu.title') }}</h1>
    <p class="text-sm text-gray-500 mb-8">{{ $t('legal.cgu.version') }} — {{ $t('legal.cgu.lastUpdate') }} : 1er mars 2026</p>

    <div class="prose prose-green dark:prose-invert max-w-none">
      <h2>1. Objet</h2>
      <p>Les presentes Conditions Generales d'Utilisation (CGU) regissent l'utilisation de la plateforme Kouroukan, un service de gestion d'etablissement scolaire edite par Kouroukan SARL, dont le siege social est situe a Conakry, Republique de Guinee.</p>

      <h2>2. Acceptation des CGU</h2>
      <p>L'utilisation de la plateforme Kouroukan implique l'acceptation pleine et entiere des presentes CGU. L'utilisateur reconnait avoir lu, compris et accepte l'ensemble des conditions.</p>

      <h2>3. Description du service</h2>
      <p>Kouroukan est une plateforme numerique de gestion d'etablissement scolaire couvrant le prescolaire, le primaire, le secondaire et l'universitaire. Elle propose des modules de gestion des inscriptions, de la pedagogie, des evaluations, des presences, des finances, du personnel, de la communication, du BDE, des documents et des services premium.</p>

      <h2>4. Inscription et compte utilisateur</h2>
      <p>L'inscription est gratuite pour le plan STARTER. L'utilisateur s'engage a fournir des informations exactes et a jour. Chaque compte est personnel et ne peut etre cede ou partage.</p>

      <h2>5. Protection des donnees personnelles</h2>
      <p>Les donnees collectees sont traitees conformement a notre Politique de Confidentialite. Kouroukan s'engage a proteger la vie privee de ses utilisateurs et a ne pas revendre les donnees personnelles a des tiers.</p>

      <h2>6. Responsabilites</h2>
      <p>Kouroukan met tout en oeuvre pour assurer la disponibilite et la fiabilite du service. Toutefois, le service est fourni "en l'etat" et Kouroukan ne peut garantir une disponibilite de 100%.</p>

      <h2>7. Propriete intellectuelle</h2>
      <p>L'ensemble des elements de la plateforme (design, code, contenu, marque) est protege par le droit de la propriete intellectuelle. Toute reproduction non autorisee est interdite.</p>

      <h2>8. Modification des CGU</h2>
      <p>Kouroukan se reserve le droit de modifier les presentes CGU a tout moment. Les utilisateurs seront informes de toute modification et devront accepter les nouvelles conditions pour continuer a utiliser le service.</p>

      <h2>9. Droit applicable</h2>
      <p>Les presentes CGU sont regies par le droit guineen. Tout litige sera soumis aux juridictions competentes de Conakry, Republique de Guinee.</p>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({ layout: 'minimal' })
useSeoMeta({ title: 'CGU' })
</script>
