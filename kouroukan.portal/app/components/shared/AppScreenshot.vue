<template>
  <div class="relative" :class="device === 'mobile' ? 'max-w-xs mx-auto' : 'max-w-2xl mx-auto'">
    <div
      class="rounded-2xl overflow-hidden shadow-2xl border-4"
      :class="device === 'mobile'
        ? 'border-gray-800 rounded-[2rem]'
        : 'border-gray-300 dark:border-gray-700'"
    >
      <!-- Desktop top bar -->
      <div v-if="device === 'desktop'" class="bg-gray-200 dark:bg-gray-800 px-4 py-2 flex items-center gap-2">
        <div class="flex gap-1.5">
          <div class="w-3 h-3 rounded-full bg-red-400" />
          <div class="w-3 h-3 rounded-full bg-yellow-400" />
          <div class="w-3 h-3 rounded-full bg-green-400" />
        </div>
        <div class="flex-1 mx-8">
          <div class="bg-white dark:bg-gray-700 rounded-md px-3 py-1 text-xs text-gray-500 text-center">
            app.kouroukan.gn
          </div>
        </div>
      </div>

      <!-- Phone notch -->
      <div v-if="device === 'mobile'" class="bg-gray-800 px-4 pt-2 pb-1 flex justify-center">
        <div class="w-20 h-5 bg-gray-900 rounded-full" />
      </div>

      <!-- Screenshot -->
      <div class="aspect-video bg-gray-100 dark:bg-gray-800">
        <NuxtImg
          v-if="src"
          :src="src"
          :alt="alt"
          class="w-full h-full object-cover"
          loading="lazy"
          format="webp"
        />
        <div v-else class="w-full h-full flex items-center justify-center">
          <div class="text-center">
            <UIcon name="i-heroicons-computer-desktop" class="w-16 h-16 text-gray-300 mx-auto" />
            <p class="mt-2 text-sm text-gray-400">{{ alt || 'Apercu' }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  src?: string
  alt?: string
  device?: 'mobile' | 'desktop'
}>()
</script>
