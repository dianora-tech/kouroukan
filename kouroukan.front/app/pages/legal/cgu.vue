<script setup lang="ts">
import { marked } from 'marked'

definePageMeta({
  layout: 'default',
  auth: false,
})

const config = useRuntimeConfig()

const cguContent = ref('')
const cguVersion = ref('')

const { data } = await useFetch<{
  success: boolean
  data: { contenu: string, version: string }
}>('/api/auth/cgu/active')

if (data.value?.success && data.value.data) {
  cguContent.value = marked(data.value.data.contenu) as string
  cguVersion.value = data.value.data.version
}
else {
  cguVersion.value = config.public.cguVersion as string
}
</script>

<template>
  <div class="mx-auto max-w-3xl space-y-6">
    <div>
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('cgu.title') }}
      </h1>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
        {{ $t('cgu.versionLabel', { version: cguVersion }) }}
      </p>
    </div>

    <div
      class="prose prose-sm dark:prose-invert rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-900"
      v-html="cguContent || $t('cgu.loadingContent')"
    />

    <div class="flex justify-end">
      <UButton
        variant="outline"
        to="/"
      >
        {{ $t('cgu.decline') }}
      </UButton>
    </div>
  </div>
</template>
