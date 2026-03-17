<template>
  <NuxtLayout>
    <NuxtPage />
  </NuxtLayout>
</template>

<script setup lang="ts">
useHead({
  titleTemplate: (title) => title ? `${title} - Kouroukan` : 'Kouroukan — Gerez votre etablissement scolaire, simplement.'
})

useSchemaOrg([
  defineOrganization({
    name: 'Kouroukan',
    url: 'https://www.kouroukan.gn',
    logo: '/icons/icon-512x512.png',
    contactPoint: {
      '@type': 'ContactPoint',
      telephone: '+224 XX XX XX XX',
      contactType: 'customer service',
      availableLanguage: ['French', 'English']
    },
    address: {
      '@type': 'PostalAddress',
      addressLocality: 'Conakry',
      addressCountry: 'GN'
    }
  })
])
</script>
