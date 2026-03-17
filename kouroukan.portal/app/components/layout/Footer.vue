<template>
  <footer class="bg-gray-900 text-gray-300">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-8">
        <!-- Brand -->
        <div class="space-y-4">
          <div class="flex items-center gap-2">
            <div class="w-8 h-8 bg-green-600 rounded-lg flex items-center justify-center">
              <span class="text-white font-bold">K</span>
            </div>
            <span class="font-bold text-lg text-white">Kouroukan</span>
          </div>
          <p class="text-sm text-gray-400">{{ $t('footer.tagline') }}</p>
          <LayoutNewsletterForm />
        </div>

        <!-- Product -->
        <div>
          <h3 class="font-semibold text-white mb-4">{{ $t('footer.product') }}</h3>
          <ul class="space-y-2">
            <li><NuxtLink :to="localePath('/fonctionnalites')" class="text-sm hover:text-green-400 transition-colors">{{ $t('nav.features') }}</NuxtLink></li>
            <li><NuxtLink :to="localePath('/tarifs')" class="text-sm hover:text-green-400 transition-colors">{{ $t('nav.pricing') }}</NuxtLink></li>
            <li><NuxtLink :to="localePath('/demo')" class="text-sm hover:text-green-400 transition-colors">{{ $t('nav.demo') }}</NuxtLink></li>
            <li><NuxtLink :to="localePath('/temoignages')" class="text-sm hover:text-green-400 transition-colors">{{ $t('nav.testimonials') }}</NuxtLink></li>
          </ul>
        </div>

        <!-- Resources -->
        <div>
          <h3 class="font-semibold text-white mb-4">{{ $t('footer.resources') }}</h3>
          <ul class="space-y-2">
            <li><NuxtLink :to="localePath('/aide')" class="text-sm hover:text-green-400 transition-colors">{{ $t('nav.help') }}</NuxtLink></li>
            <li><NuxtLink :to="localePath('/blog')" class="text-sm hover:text-green-400 transition-colors">{{ $t('nav.blog') }}</NuxtLink></li>
            <li><NuxtLink :to="localePath('/faq')" class="text-sm hover:text-green-400 transition-colors">{{ $t('nav.faq') }}</NuxtLink></li>
            <li><NuxtLink :to="localePath('/systeme-educatif')" class="text-sm hover:text-green-400 transition-colors">{{ $t('nav.education') }}</NuxtLink></li>
            <li><NuxtLink :to="localePath('/a-propos')" class="text-sm hover:text-green-400 transition-colors">{{ $t('nav.about') }}</NuxtLink></li>
          </ul>
        </div>

        <!-- Legal & Contact -->
        <div>
          <h3 class="font-semibold text-white mb-4">{{ $t('footer.legalTitle') }}</h3>
          <ul class="space-y-2">
            <li><NuxtLink :to="localePath('/legal/cgu')" class="text-sm hover:text-green-400 transition-colors">{{ $t('legal.cgu.title') }}</NuxtLink></li>
            <li><NuxtLink :to="localePath('/legal/confidentialite')" class="text-sm hover:text-green-400 transition-colors">{{ $t('legal.privacy.title') }}</NuxtLink></li>
            <li><NuxtLink :to="localePath('/legal/mentions-legales')" class="text-sm hover:text-green-400 transition-colors">{{ $t('legal.mentions.title') }}</NuxtLink></li>
          </ul>
          <div class="mt-6 space-y-2">
            <p class="text-sm flex items-center gap-2">
              <UIcon name="i-heroicons-phone" class="w-4 h-4" />
              +224 XX XX XX XX
            </p>
            <p class="text-sm flex items-center gap-2">
              <UIcon name="i-heroicons-envelope" class="w-4 h-4" />
              contact@kouroukan.gn
            </p>
          </div>
        </div>
      </div>

      <!-- Bottom bar -->
      <div class="mt-12 pt-6 border-t border-gray-800 flex flex-col sm:flex-row items-center justify-between gap-4">
        <p class="text-sm text-gray-500">&copy; {{ new Date().getFullYear() }} {{ $t('footer.copyright') }}</p>
        <div class="flex items-center gap-2 text-sm text-gray-500">
          <span class="inline-block w-5 h-3.5 rounded-sm overflow-hidden">
            <div class="flex h-full">
              <div class="w-1/3 bg-red-600"></div>
              <div class="w-1/3 bg-yellow-400"></div>
              <div class="w-1/3 bg-green-600"></div>
            </div>
          </span>
          {{ $t('footer.madeIn') }}
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup lang="ts">
const localePath = useLocalePath()
</script>
