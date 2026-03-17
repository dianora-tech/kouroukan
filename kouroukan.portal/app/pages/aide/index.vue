<template>
  <div class="pt-24">
    <section class="py-16">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        <SharedSectionTitle
          :title="$t('help.title')"
          :subtitle="$t('help.subtitle')"
          centered
        />

        <!-- Search -->
        <div class="max-w-xl mx-auto mb-12">
          <UInput
            v-model="searchQuery"
            :placeholder="$t('help.search')"
            size="lg"
            icon="i-heroicons-magnifying-glass"
            class="w-full"
            @input="search($event.target?.value || '')"
          />

          <div v-if="results.length" class="mt-4 space-y-2">
            <NuxtLink
              v-for="result in results"
              :key="result._path"
              :to="localePath(result._path)"
              class="block p-3 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-900 border border-gray-200 dark:border-gray-800"
            >
              <p class="font-medium text-gray-900 dark:text-white text-sm">{{ result.title }}</p>
              <p v-if="result.description" class="text-xs text-gray-500 mt-1">{{ result.description }}</p>
            </NuxtLink>
          </div>
        </div>

        <!-- Categories -->
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-6">
          <NuxtLink
            v-for="cat in categories"
            :key="cat.key"
            :to="localePath(`/aide/${cat.key}/index`)"
            class="p-6 rounded-xl border border-gray-200 dark:border-gray-800 hover:shadow-lg transition-shadow group"
          >
            <UIcon :name="cat.icon" class="w-8 h-8 text-green-600 mb-3" />
            <h3 class="font-semibold text-lg text-gray-900 dark:text-white group-hover:text-green-600 transition-colors">
              {{ $t(`help.categories.${cat.key}.title`) }}
            </h3>
            <p class="mt-2 text-sm text-gray-500">{{ $t(`help.categories.${cat.key}.description`) }}</p>
          </NuxtLink>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
useSeoMeta({ title: 'Aide' })

const localePath = useLocalePath()
const { query: searchQuery, results, search } = useHelpSearch()

const categories = [
  { key: 'demarrage', icon: 'i-heroicons-rocket-launch' },
  { key: 'utilisation', icon: 'i-heroicons-book-open' },
  { key: 'systeme-educatif', icon: 'i-heroicons-academic-cap' },
  { key: 'mobile', icon: 'i-heroicons-device-phone-mobile' }
]
</script>
