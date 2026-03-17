<template>
  <component
    :is="link ? resolveComponent('NuxtLink') : 'div'"
    :to="link"
    class="block p-6 rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 hover:shadow-lg transition-all duration-300 group"
  >
    <div
      class="w-12 h-12 rounded-lg flex items-center justify-center mb-4"
      :style="{ backgroundColor: `${color}15` }"
    >
      <UIcon :name="icon" class="w-6 h-6" :style="{ color }" />
    </div>
    <h3 class="font-semibold text-lg text-gray-900 dark:text-white group-hover:text-green-600 transition-colors">
      {{ title }}
    </h3>
    <p class="mt-2 text-sm text-gray-600 dark:text-gray-400 leading-relaxed">
      {{ description }}
    </p>
  </component>
</template>

<script setup lang="ts">
defineProps<{
  icon: string
  title: string
  description: string
  color?: string
  link?: string
}>()
</script>
