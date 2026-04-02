<template>
  <div class="flex items-center justify-center mb-12">
    <div v-for="(step, index) in steps" :key="index" class="flex items-center">
      <!-- Step circle + label -->
      <div class="flex flex-col items-center">
        <div class="flex items-center">
          <div
            class="w-10 h-10 rounded-full flex items-center justify-center font-semibold text-sm transition-all"
            :class="stepClass(index)"
          >
            <UIcon v-if="currentStep > index" name="i-heroicons-check" class="w-5 h-5" />
            <span v-else>{{ index + 1 }}</span>
          </div>

          <!-- Label (hidden on mobile) -->
          <span class="hidden sm:inline-block ml-2 text-sm font-medium" :class="currentStep >= index ? 'text-gray-900 dark:text-white' : 'text-gray-400'">
            {{ step }}
          </span>
        </div>

        <!-- Subtitle under completed steps -->
        <span
          v-if="index === 0 && currentStep > 0 && stepSubtitles?.[0]"
          class="text-xs text-green-600 dark:text-green-400 font-medium mt-1"
        >
          {{ stepSubtitles[0] }}
        </span>
      </div>

      <!-- Connector line -->
      <div v-if="index < steps.length - 1" class="w-8 sm:w-16 h-0.5 mx-2" :class="currentStep > index ? 'bg-green-600' : 'bg-gray-200 dark:bg-gray-700'" />
    </div>
  </div>
</template>

<script setup lang="ts">
const props = defineProps<{
  currentStep: number
  steps: string[]
  stepSubtitle?: string
  stepSubtitles?: string[]
}>()

function stepClass(step: number): string {
  if (props.currentStep > step) return 'bg-green-600 text-white'
  if (props.currentStep === step) return 'bg-green-600 text-white ring-4 ring-green-100 dark:ring-green-900/30'
  return 'bg-gray-200 dark:bg-gray-700 text-gray-500'
}
</script>
