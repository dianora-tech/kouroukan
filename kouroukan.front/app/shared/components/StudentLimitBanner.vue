<script setup lang="ts">
const props = defineProps<{
  current: number
  limit: number
}>()

const { t } = useI18n()
const localePath = useLocalePath()

const percentage = computed(() => {
  if (props.limit <= 0) return 100
  return Math.min(Math.round((props.current / props.limit) * 100), 100)
})

const isAtLimit = computed(() => props.current >= props.limit)

const progressColor = computed(() => {
  if (percentage.value >= 100) return 'error' as const
  if (percentage.value >= 80) return 'warning' as const
  return 'primary' as const
})
</script>

<template>
  <div
    class="rounded-xl border p-4"
    :class="isAtLimit
      ? 'border-red-200 bg-red-50 dark:border-red-800 dark:bg-red-900/20'
      : 'border-amber-200 bg-amber-50 dark:border-amber-800 dark:bg-amber-900/20'"
  >
    <div class="flex items-start gap-3">
      <UIcon
        :name="isAtLimit ? 'i-heroicons-exclamation-triangle' : 'i-heroicons-information-circle'"
        :class="isAtLimit ? 'text-red-600 dark:text-red-400' : 'text-amber-600 dark:text-amber-400'"
        class="mt-0.5 h-5 w-5 shrink-0"
      />
      <div class="flex-1">
        <h3
          class="text-sm font-semibold"
          :class="isAtLimit ? 'text-red-800 dark:text-red-300' : 'text-amber-800 dark:text-amber-300'"
        >
          {{ isAtLimit ? $t('forfaitGating.studentLimit.title') : $t('forfaitGating.studentLimit.progress', { current, limit }) }}
        </h3>

        <p v-if="isAtLimit" class="mt-1 text-sm text-red-700 dark:text-red-400">
          {{ $t('forfaitGating.studentLimit.description', { limit }) }}
        </p>

        <!-- Progress bar -->
        <div class="mt-3">
          <div class="mb-1 flex justify-between text-xs text-gray-600 dark:text-gray-400">
            <span>{{ $t('forfaitGating.studentLimit.progress', { current, limit }) }}</span>
            <span>{{ percentage }}%</span>
          </div>
          <UProgress :value="percentage" :color="progressColor" size="sm" />
        </div>

        <UButton
          v-if="isAtLimit"
          color="primary"
          variant="outline"
          size="sm"
          icon="i-heroicons-arrow-up-circle"
          :to="localePath('/forfait')"
          class="mt-3"
        >
          {{ $t('forfaitGating.cta') }}
        </UButton>
      </div>
    </div>
  </div>
</template>
