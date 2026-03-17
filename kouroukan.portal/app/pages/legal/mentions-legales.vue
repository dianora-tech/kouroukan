<template>
  <div>
    <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-8">{{ $t('legal.mentions.title') }}</h1>

    <div class="prose prose-green dark:prose-invert max-w-none">
      <h2>Editeur</h2>
      <p>
        Kouroukan SARL<br />
        Conakry, Republique de Guinee<br />
        Telephone : +224 XX XX XX XX<br />
        Email : contact@kouroukan.gn
      </p>

      <h2>Directeur de la publication</h2>
      <p>Le directeur de la publication est le representant legal de Kouroukan SARL.</p>

      <h2>Hebergement</h2>
      <p>
        Le site et l'application sont heberges sur des serveurs VPS Linux securises.<br />
        Les donnees sont stockees en conformite avec les reglementations en vigueur.
      </p>

      <h2>Propriete intellectuelle</h2>
      <p>L'ensemble du contenu du site www.kouroukan.gn (textes, images, logos, structure) est protege par le droit de la propriete intellectuelle. Toute reproduction, meme partielle, est interdite sans autorisation prealable.</p>

      <h2>Contact</h2>
      <p>
        Pour toute question relative a ces mentions legales :<br />
        Email : contact@kouroukan.gn<br />
        Telephone : +224 XX XX XX XX
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({ layout: 'minimal' })
useSeoMeta({ title: 'Mentions Legales' })
</script>
