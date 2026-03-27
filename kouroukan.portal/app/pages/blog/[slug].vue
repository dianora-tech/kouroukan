<template>
  <div class="pt-24">
    <section class="py-16">
      <div class="max-w-3xl mx-auto px-4 sm:px-6 lg:px-8">
        <NuxtLink :to="localePath('/blog')" class="inline-flex items-center gap-1 text-sm text-gray-500 hover:text-gray-700 mb-6">
          <UIcon name="i-heroicons-arrow-left" class="w-4 h-4" />
          {{ $t('blog.title') }}
        </NuxtLink>

        <article v-if="article">
          <div class="mb-8">
            <UBadge v-if="article.category" color="primary" variant="subtle" class="mb-4">{{ article.category }}</UBadge>
            <h1 class="text-3xl sm:text-4xl font-bold text-gray-900 dark:text-white">{{ article.title }}</h1>
            <div class="mt-4 flex items-center gap-4 text-sm text-gray-500">
              <span v-if="article.date">{{ formatDate(article.date) }}</span>
              <span v-if="article.author">{{ article.author }}</span>
            </div>
          </div>

          <div class="prose prose-green dark:prose-invert max-w-none">
            <ContentRenderer v-if="article" :value="article" />
          </div>

          <div class="mt-12 pt-6 border-t border-gray-200 dark:border-gray-800">
            <p class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-3">{{ $t('blog.share') }}</p>
            <a
              :href="`https://wa.me/?text=${encodeURIComponent(article.title + ' - ' + currentUrl)}`"
              target="_blank"
              rel="noopener"
              class="inline-flex items-center gap-2 px-4 py-2 rounded-lg bg-green-600 text-white text-sm hover:bg-green-700 transition-colors"
            >
              {{ $t('blog.shareWhatsApp') }}
            </a>
          </div>
        </article>

        <div v-else class="text-center py-16">
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Article non trouve</h1>
          <UButton :to="localePath('/blog')" color="primary" class="mt-6">{{ $t('common.back') }}</UButton>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
const localePath = useLocalePath()
const route = useRoute()
const currentUrl = computed(() => `https://www.kouroukan.gn/blog/${route.params.slug}`)

const { data: article } = await useAsyncData(`blog-${route.params.slug}`, () =>
  queryCollection('blog').path(`/blog/${route.params.slug}`).first()
)

if (article.value) {
  useSeoMeta({ title: article.value.title, description: article.value.description })
}

const { formatDate } = useFormatDate()
</script>
