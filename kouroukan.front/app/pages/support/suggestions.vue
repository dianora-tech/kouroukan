<script setup lang="ts">
definePageMeta({
  layout: 'default',
})
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('nav.support'), to: '/support' },
            { label: $t('nav.supportSuggestions') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('supportPages.suggestions.title') }}
        </h1>
        <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
          {{ $t('supportPages.suggestions.subtitle') }}
        </p>
      </div>
      <UButton
        v-permission="'support:create'"
        color="primary"
        icon="i-heroicons-plus"
      >
        {{ $t('supportPages.suggestions.new') }}
      </UButton>
    </div>

    <EmptyState
      icon="i-heroicons-light-bulb"
      :title="$t('supportPages.suggestions.empty')"
      :description="$t('supportPages.suggestions.emptyDesc')"
      :action-label="$t('supportPages.suggestions.new')"
    />
  </div>
</template>
