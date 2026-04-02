<script setup lang="ts">
const emit = defineEmits<{
  finish: []
}>()
</script>

<template>
  <div class="py-12 text-center">
    <div class="mx-auto flex h-20 w-20 items-center justify-center rounded-full bg-green-100 dark:bg-green-900">
      <UIcon
        name="i-heroicons-check-circle"
        class="h-12 w-12 text-green-600 dark:text-green-400"
      />
    </div>

    <h2 class="mt-6 text-2xl font-bold text-gray-900 dark:text-white">
      {{ $t('onboarding.complete.title') }}
    </h2>
    <p class="mx-auto mt-3 max-w-md text-sm text-gray-500 dark:text-gray-400">
      {{ $t('onboarding.complete.description') }}
    </p>

    <div class="mt-8">
      <UButton
        color="primary"
        size="lg"
        @click="emit('finish')"
      >
        {{ $t('onboarding.complete.goToDashboard') }}
      </UButton>
    </div>
  </div>
</template>
