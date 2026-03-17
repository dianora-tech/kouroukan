<template>
  <div class="pt-24">
    <section class="py-16">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <SharedSectionTitle
          :title="$t('features.overview')"
          :subtitle="$t('features.subtitle')"
          centered
        />

        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          <SharedFeatureCard
            v-for="mod in MODULE_LIST"
            :key="mod.slug"
            :icon="mod.icon"
            :title="$t(mod.name)"
            :description="$t(mod.description)"
            :color="mod.color"
            :link="localePath(`/fonctionnalites/${mod.slug}`)"
          />
        </div>
      </div>
    </section>

    <LandingCtaSection />
  </div>
</template>

<script setup lang="ts">
import { MODULE_LIST } from '~/utils/constants'

const localePath = useLocalePath()

useSeoMeta({ title: 'Fonctionnalites' })
</script>
