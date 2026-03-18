<script setup lang="ts">
definePageMeta({
  layout: 'default',
})

const searchQuery = ref('')
</script>

<template>
  <div class="space-y-6">
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('nav.support'), to: '/support' },
          { label: $t('nav.supportAide') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('supportPages.aide.title') }}
      </h1>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
        {{ $t('supportPages.aide.subtitle') }}
      </p>
    </div>

    <UInput
      v-model="searchQuery"
      :placeholder="$t('supportPages.aide.search')"
      icon="i-heroicons-magnifying-glass"
      size="lg"
    />

    <EmptyState
      icon="i-heroicons-book-open"
      :title="$t('supportPages.aide.empty')"
      description=""
    />
  </div>
</template>
