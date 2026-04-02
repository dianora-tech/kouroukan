<script setup lang="ts">
</script>

<template>
  <div class="relative min-h-screen">
    <!-- Background gradient -->
    <div class="fixed inset-0 z-0 bg-gradient-to-br from-green-50 via-white to-green-100 dark:from-gray-900 dark:via-gray-800 dark:to-gray-900" />

    <!-- Content -->
    <div class="relative z-10 flex min-h-screen flex-col items-center justify-center p-4">
      <!-- Logo -->
      <div class="mb-6 text-center">
        <div class="mx-auto mb-3 flex h-14 w-14 items-center justify-center rounded-2xl bg-green-600 text-xl font-bold text-white shadow-lg">
          K
        </div>
        <h1 class="text-xl font-bold text-gray-900 dark:text-white">
          Kouroukan
        </h1>
      </div>

      <!-- Card -->
      <div class="w-full max-w-3xl">
        <slot />
      </div>
    </div>
  </div>
</template>
