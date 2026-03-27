<script setup lang="ts">
interface Step {
  number: number
  key: string
  label: string
  icon: string
}

defineProps<{
  steps: Step[]
  currentStep: number
}>()
</script>

<template>
  <div class="mb-8">
    <div class="flex items-center justify-between">
      <div
        v-for="(step, index) in steps"
        :key="step.number"
        class="flex items-center"
        :class="{ 'flex-1': index < steps.length - 1 }"
      >
        <!-- Step circle -->
        <div class="flex flex-col items-center">
          <div
            class="flex h-10 w-10 items-center justify-center rounded-full border-2 transition-colors"
            :class="{
              'border-green-500 bg-green-500 text-white': step.number < currentStep,
              'border-green-500 bg-green-50 text-green-600 dark:bg-green-950': step.number === currentStep,
              'border-gray-300 bg-white text-gray-400 dark:border-gray-600 dark:bg-gray-800': step.number > currentStep,
            }"
          >
            <UIcon
              v-if="step.number < currentStep"
              name="i-heroicons-check"
              class="h-5 w-5"
            />
            <UIcon
              v-else
              :name="step.icon"
              class="h-5 w-5"
            />
          </div>
          <span
            class="mt-2 hidden text-xs font-medium sm:block"
            :class="{
              'text-green-600 dark:text-green-400': step.number <= currentStep,
              'text-gray-400 dark:text-gray-500': step.number > currentStep,
            }"
          >
            {{ step.label }}
          </span>
        </div>
        <!-- Connector line -->
        <div
          v-if="index < steps.length - 1"
          class="mx-2 h-0.5 flex-1"
          :class="{
            'bg-green-500': step.number < currentStep,
            'bg-gray-300 dark:bg-gray-600': step.number >= currentStep,
          }"
        />
      </div>
    </div>
    <!-- Mobile: show current step label -->
    <p class="mt-3 text-center text-sm font-medium text-green-600 sm:hidden dark:text-green-400">
      {{ $t('onboarding.stepOf', { current: currentStep, total: steps.length }) }}
      — {{ steps[currentStep - 1]?.label }}
    </p>
  </div>
</template>
