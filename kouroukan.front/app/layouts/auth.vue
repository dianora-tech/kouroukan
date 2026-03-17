<script setup lang="ts">
</script>

<template>
  <div class="flex min-h-screen items-center justify-center bg-gradient-to-br from-green-600 to-green-800 p-4">
    <div class="w-full max-w-md">
      <!-- Logo -->
      <div class="mb-8 text-center">
        <div class="mx-auto mb-3 flex h-16 w-16 items-center justify-center rounded-2xl bg-white text-2xl font-bold text-green-600 shadow-lg">
          K
        </div>
        <h1 class="text-2xl font-bold text-white">Kouroukan</h1>
        <p class="mt-1 text-sm text-green-100">
          {{ $t('portal.tagline') }}
        </p>
      </div>

      <!-- Content -->
      <div class="rounded-xl bg-white p-6 shadow-2xl dark:bg-gray-800">
        <slot />
      </div>
    </div>
  </div>
</template>
