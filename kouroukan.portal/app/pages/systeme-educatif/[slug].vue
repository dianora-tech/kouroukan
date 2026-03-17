<template>
  <div class="pt-24">
    <section class="py-16">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        <NuxtLink :to="localePath('/systeme-educatif')" class="inline-flex items-center gap-1 text-sm text-gray-500 hover:text-gray-700 mb-6">
          <UIcon name="i-heroicons-arrow-left" class="w-4 h-4" />
          {{ $t('education.title') }}
        </NuxtLink>

        <template v-if="doc">
          <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-8">{{ doc.title }}</h1>
          <div class="prose prose-green dark:prose-invert max-w-none">
            <ContentRenderer :value="doc" />
          </div>
        </template>

        <div v-else class="text-center py-16">
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Page non trouvee</h1>
          <UButton :to="localePath('/systeme-educatif')" color="primary" class="mt-6">
            {{ $t('common.back') }}
          </UButton>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
const localePath = useLocalePath()
const route = useRoute()

const { data: doc } = await useAsyncData(`edu-${route.params.slug}`, () =>
  queryCollection('systemeEducatif').path(`/systeme-educatif/${route.params.slug}`).first()
)

useSeoMeta({ title: doc.value?.title || 'Systeme educatif' })
</script>
