<template>
  <div class="pt-24">
    <section class="py-16">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        <NuxtLink :to="localePath('/aide')" class="inline-flex items-center gap-1 text-sm text-gray-500 hover:text-gray-700 mb-6">
          <UIcon name="i-heroicons-arrow-left" class="w-4 h-4" />
          {{ $t('help.title') }}
        </NuxtLink>

        <article v-if="doc">
          <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-8">{{ doc.title }}</h1>
          <div class="prose prose-green dark:prose-invert max-w-none">
            <ContentRenderer :value="doc" />
          </div>

          <div class="mt-12 pt-6 border-t border-gray-200 dark:border-gray-800 flex items-center gap-4">
            <span class="text-sm text-gray-600 dark:text-gray-400">{{ $t('help.helpful') }}</span>
            <UButton v-if="!voted" size="sm" variant="outline" color="primary" @click="vote(true)">
              {{ $t('help.helpfulYes') }}
            </UButton>
            <UButton v-if="!voted" size="sm" variant="outline" @click="vote(false)">
              {{ $t('help.helpfulNo') }}
            </UButton>
            <span v-if="voted" class="text-sm text-green-600">Merci !</span>
          </div>
        </article>

        <div v-else class="text-center py-16">
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Article non trouve</h1>
          <UButton :to="localePath('/aide')" color="primary" class="mt-6">{{ $t('common.back') }}</UButton>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
const localePath = useLocalePath()
const route = useRoute()
const voted = ref(false)

const { data: doc } = await useAsyncData(`aide-${route.params.category}-${route.params.slug}`, () =>
  queryCollection('aide').path(`/aide/${route.params.category}/${route.params.slug}`).first()
)

if (doc.value) {
  useSeoMeta({ title: doc.value.title, description: doc.value.description })
}

function vote(_helpful: boolean) {
  voted.value = true
}
</script>
