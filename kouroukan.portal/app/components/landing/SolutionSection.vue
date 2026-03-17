<template>
  <section class="py-20">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <SharedSectionTitle
        :title="$t('solution.title')"
        :subtitle="$t('solution.subtitle')"
        centered
      />

      <div class="space-y-16">
        <div
          v-for="(mod, index) in MODULE_LIST"
          :key="mod.slug"
          class="flex flex-col gap-8"
          :class="index % 2 === 0 ? 'lg:flex-row' : 'lg:flex-row-reverse'"
        >
          <!-- Text -->
          <div class="flex-1 flex flex-col justify-center">
            <div
              class="w-12 h-12 rounded-lg flex items-center justify-center mb-4"
              :style="{ backgroundColor: `${mod.color}15` }"
            >
              <UIcon :name="mod.icon" class="w-6 h-6" :style="{ color: mod.color }" />
            </div>
            <h3 class="text-2xl font-bold text-gray-900 dark:text-white">{{ $t(mod.name) }}</h3>
            <p class="mt-3 text-gray-600 dark:text-gray-400 leading-relaxed">{{ $t(mod.longDescription) }}</p>
            <NuxtLink
              :to="localePath(`/fonctionnalites/${mod.slug}`)"
              class="mt-4 inline-flex items-center gap-1 text-sm font-medium transition-colors"
              :style="{ color: mod.color }"
            >
              {{ $t('common.learnMore') }}
              <UIcon name="i-heroicons-arrow-right" class="w-4 h-4" />
            </NuxtLink>
          </div>

          <!-- Screenshot placeholder -->
          <div class="flex-1">
            <SharedAppScreenshot :alt="$t(mod.name)" device="desktop" />
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { MODULE_LIST } from '~/utils/constants'

const localePath = useLocalePath()
</script>
