<template>
  <div>
    <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-8">{{ $t('legal.privacy.title') }}</h1>

    <div class="prose prose-green dark:prose-invert max-w-none">
      <h2>1. Responsable du traitement</h2>
      <p>Kouroukan SARL, dont le siege social est situe a Conakry, Republique de Guinee, est responsable du traitement des donnees personnelles collectees via la plateforme.</p>

      <h2>2. Donnees collectees</h2>
      <p>Nous collectons les donnees suivantes :</p>
      <ul>
        <li>Donnees d'identification : nom, prenom, telephone, email</li>
        <li>Donnees scolaires : notes, presences, inscriptions</li>
        <li>Donnees financieres : paiements, factures</li>
        <li>Donnees de connexion : adresse IP, navigateur, date et heure de connexion</li>
      </ul>

      <h2>3. Finalites du traitement</h2>
      <p>Les donnees sont traitees pour :</p>
      <ul>
        <li>La gestion des comptes utilisateurs</li>
        <li>La fourniture des services de gestion scolaire</li>
        <li>L'amelioration de la plateforme</li>
        <li>La communication avec les utilisateurs</li>
      </ul>

      <h2>4. Duree de conservation</h2>
      <p>Les donnees sont conservees pendant la duree de l'utilisation du service et supprimees dans un delai de 12 mois apres la cloture du compte, sauf obligation legale de conservation plus longue.</p>

      <h2>5. Partage des donnees</h2>
      <p>Les donnees personnelles ne sont jamais vendues a des tiers. Elles peuvent etre partagees avec des prestataires techniques (hebergement, envoi de SMS) dans le strict cadre de la fourniture du service.</p>

      <h2>6. Securite des donnees</h2>
      <p>Kouroukan met en oeuvre des mesures techniques et organisationnelles pour proteger les donnees : chiffrement, controle d'acces par roles, sauvegardes automatiques, journalisation des acces.</p>

      <h2>7. Droits des utilisateurs</h2>
      <p>Conformement a la reglementation applicable, vous disposez d'un droit d'acces, de rectification, de suppression et de portabilite de vos donnees. Pour exercer ces droits, contactez-nous a : contact@kouroukan.gn</p>

      <h2>8. Cookies</h2>
      <p>La plateforme utilise des cookies techniques necessaires au fonctionnement du service. Aucun cookie publicitaire n'est utilise. Vous pouvez configurer votre navigateur pour refuser les cookies.</p>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({ layout: 'minimal' })
useSeoMeta({ title: 'Politique de Confidentialite' })
</script>
