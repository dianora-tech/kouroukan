<template>
  <div :class="centered ? 'text-center' : ''" class="mb-12">
    <UBadge v-if="badge" color="primary" variant="subtle" class="mb-4">{{ badge }}</UBadge>
    <h2 class="text-3xl sm:text-4xl font-bold text-gray-900 dark:text-white">{{ title }}</h2>
    <p v-if="subtitle" class="mt-4 text-lg text-gray-600 dark:text-gray-400 max-w-3xl" :class="centered ? 'mx-auto' : ''">
      {{ subtitle }}
    </p>
    <div v-if="!noDivider" class="mt-4 h-1 w-16 bg-green-600 rounded" :class="centered ? 'mx-auto' : ''" />
  </div>
</template>

<script setup lang="ts">
defineProps<{
  title: string
  subtitle?: string
  badge?: string
  centered?: boolean
  noDivider?: boolean
}>()
</script>
