<template>
  <section class="relative min-h-screen flex items-center bg-gradient-to-br from-green-700 via-green-600 to-green-800 overflow-hidden">
    <!-- Background pattern -->
    <div class="absolute inset-0 opacity-10">
      <div class="absolute top-20 left-10 w-72 h-72 bg-white rounded-full blur-3xl" />
      <div class="absolute bottom-20 right-10 w-96 h-96 bg-yellow-400 rounded-full blur-3xl" />
    </div>

    <div class="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-32 sm:py-40">
      <div class="max-w-3xl">
        <!-- Badge -->
        <div class="inline-flex items-center gap-2 px-4 py-2 rounded-full bg-white/10 backdrop-blur-sm border border-white/20 mb-8">
          <span class="inline-block w-5 h-3.5 rounded-sm overflow-hidden">
            <div class="flex h-full">
              <div class="w-1/3 bg-red-600"></div>
              <div class="w-1/3 bg-yellow-400"></div>
              <div class="w-1/3 bg-green-600"></div>
            </div>
          </span>
          <span class="text-sm text-white font-medium">{{ $t('hero.badge') }}</span>
        </div>

        <!-- Title -->
        <h1 class="text-4xl sm:text-5xl lg:text-6xl font-bold text-white leading-tight">
          {{ $t('hero.title') }}
        </h1>

        <!-- Subtitle -->
        <p class="mt-6 text-lg sm:text-xl text-white/80 max-w-2xl leading-relaxed">
          {{ $t('hero.subtitle') }}
        </p>

        <!-- CTAs -->
        <div class="mt-10 flex flex-col sm:flex-row gap-4">
          <UButton
            :to="localePath('/inscription')"
            size="xl"
            color="white"
            class="font-semibold"
          >
            <UIcon name="i-heroicons-rocket-launch" class="w-5 h-5" />
            {{ $t('hero.cta') }}
          </UButton>
          <UButton
            :to="localePath('/demo')"
            size="xl"
            variant="outline"
            class="font-semibold text-white border-white/30 hover:bg-white/10"
          >
            <UIcon name="i-heroicons-play" class="w-5 h-5" />
            {{ $t('hero.ctaDemo') }}
          </UButton>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
const localePath = useLocalePath()
</script>
