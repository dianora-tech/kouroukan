<template>
  <div class="min-h-screen flex items-center justify-center px-4">
    <div class="text-center max-w-md">
      <div class="w-16 h-16 mx-auto mb-6 rounded-full bg-amber-100 dark:bg-amber-900/30 flex items-center justify-center">
        <UIcon name="i-heroicons-wifi" class="w-8 h-8 text-amber-600 dark:text-amber-400" />
      </div>

      <h1 class="text-2xl font-bold text-gray-900 dark:text-white mb-3">
        {{ $t('offline.title') }}
      </h1>

      <p class="text-gray-600 dark:text-gray-400 mb-8">
        {{ $t('offline.description') }}
      </p>

      <UButton
        :label="$t('offline.retry')"
        color="primary"
        size="lg"
        @click="retry"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({ layout: 'minimal' })
useSeoMeta({ title: 'Hors ligne' })

function retry() {
  window.location.reload()
}
</script>
