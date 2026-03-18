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
            { label: $t('nav.supportTickets') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('supportPages.tickets.title') }}
        </h1>
        <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
          {{ $t('supportPages.tickets.subtitle') }}
        </p>
      </div>
      <UButton
        v-permission="'support:create'"
        color="primary"
        icon="i-heroicons-plus"
      >
        {{ $t('supportPages.tickets.new') }}
      </UButton>
    </div>

    <EmptyState
      icon="i-heroicons-ticket"
      :title="$t('supportPages.tickets.empty')"
      :description="$t('supportPages.tickets.emptyDesc')"
      :action-label="$t('supportPages.tickets.new')"
    />
  </div>
</template>
