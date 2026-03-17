<template>
  <div class="pt-24">
    <section class="py-16">
      <div class="max-w-3xl mx-auto px-4 sm:px-6 lg:px-8">
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">{{ $t('help.searchResults') }}</h1>

        <UInput
          v-model="searchQuery"
          :placeholder="$t('help.search')"
          size="lg"
          icon="i-heroicons-magnifying-glass"
          class="w-full mb-8"
          @input="search($event.target?.value || '')"
        />

        <div v-if="loading" class="text-center py-12">
          <p class="text-gray-500">{{ $t('common.loading') }}</p>
        </div>

        <div v-else-if="results.length" class="space-y-4">
          <NuxtLink
            v-for="result in results"
            :key="result._path"
            :to="localePath(result._path)"
            class="block p-4 rounded-xl border border-gray-200 dark:border-gray-800 hover:shadow-md transition-shadow"
          >
            <h3 class="font-semibold text-gray-900 dark:text-white">{{ result.title }}</h3>
            <p v-if="result.description" class="mt-1 text-sm text-gray-500">{{ result.description }}</p>
            <UBadge v-if="result.category" color="primary" variant="subtle" size="xs" class="mt-2">{{ result.category }}</UBadge>
          </NuxtLink>
        </div>

        <div v-else-if="searchQuery" class="text-center py-12">
          <UIcon name="i-heroicons-magnifying-glass" class="w-12 h-12 text-gray-300 mx-auto" />
          <p class="mt-4 text-gray-500">{{ $t('help.noResults') }}</p>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
useSeoMeta({ title: 'Recherche aide', robots: 'noindex' })

const localePath = useLocalePath()
const route = useRoute()
const { query: searchQuery, results, loading, search } = useHelpSearch()

onMounted(() => {
  const q = route.query.q as string
  if (q) {
    searchQuery.value = q
    search(q)
  }
})
</script>
