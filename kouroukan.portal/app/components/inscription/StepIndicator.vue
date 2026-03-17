<template>
  <div class="flex items-center justify-center mb-12">
    <div v-for="(step, index) in steps" :key="index" class="flex items-center">
      <!-- Step circle -->
      <div
        class="w-10 h-10 rounded-full flex items-center justify-center font-semibold text-sm transition-all"
        :class="stepClass(index + 1)"
      >
        <UIcon v-if="currentStep > index + 1" name="i-heroicons-check" class="w-5 h-5" />
        <span v-else>{{ index + 1 }}</span>
      </div>

      <!-- Label (hidden on mobile) -->
      <span class="hidden sm:inline-block ml-2 text-sm font-medium" :class="currentStep >= index + 1 ? 'text-gray-900 dark:text-white' : 'text-gray-400'">
        {{ step }}
      </span>

      <!-- Connector line -->
      <div v-if="index < steps.length - 1" class="w-8 sm:w-16 h-0.5 mx-2" :class="currentStep > index + 1 ? 'bg-green-600' : 'bg-gray-200 dark:bg-gray-700'" />
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  currentStep: number
  steps: string[]
}>()

function stepClass(step: number): string {
  const props = getCurrentInstance()?.props as { currentStep: number } | undefined
  const current = props?.currentStep || 1
  if (current > step) return 'bg-green-600 text-white'
  if (current === step) return 'bg-green-600 text-white ring-4 ring-green-100 dark:ring-green-900/30'
  return 'bg-gray-200 dark:bg-gray-700 text-gray-500'
}
</script>
