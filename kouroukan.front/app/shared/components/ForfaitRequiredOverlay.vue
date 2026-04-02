<script setup lang="ts">
const props = defineProps<{
  userType: 'enseignant' | 'famille'
}>()

const { t } = useI18n()
const localePath = useLocalePath()

const forfaitRoute = computed(() => {
  return localePath(`/${props.userType}/forfait`)
})
</script>

<template>
  <div class="flex min-h-[60vh] items-center justify-center p-6">
    <div class="w-full max-w-md rounded-xl border border-gray-200 bg-white p-8 text-center dark:border-gray-700 dark:bg-gray-800">
      <div class="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-amber-100 dark:bg-amber-900/30">
        <UIcon
          name="i-heroicons-lock-closed"
          class="h-8 w-8 text-amber-600 dark:text-amber-400"
        />
      </div>

      <h2 class="mb-2 text-xl font-bold text-gray-900 dark:text-white">
        {{ $t('forfaitGating.title') }}
      </h2>

      <p class="mb-6 text-sm text-gray-500 dark:text-gray-400">
        {{ $t('forfaitGating.description') }}
      </p>

      <UButton
        color="primary"
        size="lg"
        icon="i-heroicons-credit-card"
        :to="forfaitRoute"
        class="w-full justify-center"
      >
        {{ $t('forfaitGating.cta') }}
      </UButton>
    </div>
  </div>
</template>
