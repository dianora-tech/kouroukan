<template>
  <section class="py-20 bg-gradient-to-br from-green-700 to-green-800">
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
      <h2 class="text-3xl sm:text-4xl font-bold text-white">{{ $t('cta.title') }}</h2>
      <p class="mt-4 text-lg text-green-100 max-w-2xl mx-auto">{{ $t('cta.subtitle') }}</p>

      <div class="mt-10 flex flex-col sm:flex-row items-center justify-center gap-4">
        <UButton
          :to="localePath('/inscription')"
          size="xl"
          color="white"
          class="font-semibold"
        >
          <UIcon name="i-heroicons-rocket-launch" class="w-5 h-5" />
          {{ $t('cta.button') }}
        </UButton>
        <UButton
          :to="localePath('/contact')"
          size="xl"
          variant="outline"
          class="font-semibold text-white border-white/30 hover:bg-white/10"
        >
          {{ $t('cta.contact') }}
        </UButton>
      </div>

      <div class="mt-8 flex items-center justify-center gap-6 text-green-100">
        <a href="tel:+224XXXXXXXX" class="flex items-center gap-2 text-sm hover:text-white transition-colors">
          <UIcon name="i-heroicons-phone" class="w-4 h-4" />
          +224 XX XX XX XX
        </a>
        <span class="text-green-400">|</span>
        <a href="https://wa.me/224XXXXXXXX" target="_blank" rel="noopener" class="flex items-center gap-2 text-sm hover:text-white transition-colors">
          <UIcon name="i-heroicons-chat-bubble-left" class="w-4 h-4" />
          WhatsApp
        </a>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
const localePath = useLocalePath()
</script>
