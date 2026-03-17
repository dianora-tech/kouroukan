<script setup lang="ts">
withDefaults(defineProps<{
  icon?: string
  title: string
  description?: string
  actionLabel?: string
  actionTo?: string
}>(), {
  icon: 'i-heroicons-inbox',
})

const emit = defineEmits<{
  (e: 'action'): void
}>()
</script>

<template>
  <div class="flex flex-col items-center justify-center py-12 text-center">
    <UIcon :name="icon" class="mb-4 h-12 w-12 text-gray-300 dark:text-gray-600" />
    <h3 class="text-lg font-medium text-gray-900 dark:text-white">
      {{ title }}
    </h3>
    <p v-if="description" class="mt-1 text-sm text-gray-500 dark:text-gray-400">
      {{ description }}
    </p>
    <div v-if="actionLabel" class="mt-4">
      <UButton
        v-if="actionTo"
        :to="actionTo"
        color="primary"
      >
        {{ actionLabel }}
      </UButton>
      <UButton
        v-else
        color="primary"
        @click="emit('action')"
      >
        {{ actionLabel }}
      </UButton>
    </div>
  </div>
</template>
