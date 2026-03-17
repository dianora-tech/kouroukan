<template>
  <div class="pt-24">
    <section class="py-16">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <SharedSectionTitle
          :title="$t('blog.title')"
          :subtitle="$t('blog.subtitle')"
          centered
        />

        <div v-if="articles?.length" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <SharedBlogCard
            v-for="article in articles"
            :key="article._path"
            :title="article.title || ''"
            :slug="article._path?.replace('/blog/', '') || ''"
            :description="article.description"
            :date="article.date"
            :category="article.category"
          />
        </div>

        <div v-else class="text-center py-12">
          <UIcon name="i-heroicons-document-text" class="w-12 h-12 text-gray-300 mx-auto" />
          <p class="mt-4 text-gray-500">Aucun article pour le moment.</p>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
useSeoMeta({ title: 'Blog' })

const { data: articles } = await useAsyncData('blog', () =>
  queryCollection('blog').order('date', 'DESC').all()
)
</script>
