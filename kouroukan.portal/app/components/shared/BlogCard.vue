<template>
  <NuxtLink :to="localePath(`/blog/${slug}`)" class="block group">
    <div class="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 overflow-hidden hover:shadow-lg transition-all duration-300">
      <div v-if="image" class="aspect-video bg-gray-100 dark:bg-gray-800 overflow-hidden">
        <NuxtImg :src="image" :alt="title" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300" loading="lazy" />
      </div>
      <div v-else class="aspect-video bg-gradient-to-br from-green-50 to-green-100 dark:from-green-900/20 dark:to-green-800/20 flex items-center justify-center">
        <UIcon name="i-heroicons-document-text" class="w-12 h-12 text-green-300" />
      </div>
      <div class="p-5">
        <div class="flex items-center gap-2 mb-3">
          <UBadge v-if="category" color="primary" variant="subtle" size="xs">{{ category }}</UBadge>
          <span class="text-xs text-gray-500">{{ formattedDate }}</span>
        </div>
        <h3 class="font-semibold text-gray-900 dark:text-white group-hover:text-green-600 transition-colors line-clamp-2">
          {{ title }}
        </h3>
        <p v-if="description" class="mt-2 text-sm text-gray-600 dark:text-gray-400 line-clamp-2">
          {{ description }}
        </p>
      </div>
    </div>
  </NuxtLink>
</template>

<script setup lang="ts">
const localePath = useLocalePath()

const props = defineProps<{
  title: string
  slug: string
  description?: string
  date?: string
  image?: string
  category?: string
}>()

const { locale } = useI18n()
const localeMap: Record<string, string> = { fr: 'fr-GN', en: 'en-US' }

const formattedDate = computed(() => {
  if (!props.date) return ''
  return new Intl.DateTimeFormat(localeMap[locale.value] ?? locale.value, { day: 'numeric', month: 'long', year: 'numeric' }).format(new Date(props.date))
})
</script>
