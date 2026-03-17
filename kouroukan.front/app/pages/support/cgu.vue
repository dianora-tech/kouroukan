<script setup lang="ts">
import { useAuthStore } from '~/core/stores/auth.store'

definePageMeta({
  layout: 'default',
})

const { t } = useI18n()
const auth = useAuthStore()
const toast = useToast()
const config = useRuntimeConfig()

const loading = ref(false)
const cguContent = ref('')
const cguVersion = ref('')

// Fetch CGU content
const { data } = await useFetch<{
  success: boolean
  data: { content: string; version: string }
}>('/api/auth/cgu/active')

if (data.value?.success && data.value.data) {
  cguContent.value = data.value.data.content
  cguVersion.value = data.value.data.version
}
else {
  cguVersion.value = config.public.cguVersion as string
}

async function handleAccept(): Promise<void> {
  loading.value = true
  try {
    await auth.acceptCgu(cguVersion.value)
    toast.add({
      title: t('cgu.accepted'),
      color: 'success',
    })
    await navigateTo('/')
  }
  catch {
    toast.add({
      title: t('cgu.acceptError'),
      color: 'error',
    })
  }
  finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="mx-auto max-w-3xl">
    <div class="rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-800">
      <h1 class="mb-4 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('cgu.title') }}
      </h1>

      <p class="mb-4 text-sm text-gray-500 dark:text-gray-400">
        {{ $t('cgu.versionLabel', { version: cguVersion }) }}
      </p>

      <!-- CGU Content (rendered from markdown) -->
      <div
        class="prose prose-sm dark:prose-invert mb-6 max-h-96 overflow-y-auto rounded-lg border border-gray-100 bg-gray-50 p-4 dark:border-gray-600 dark:bg-gray-900"
        v-html="cguContent || $t('cgu.loadingContent')"
      />

      <div class="flex items-center justify-end gap-3">
        <UButton
          variant="outline"
          @click="auth.logout()"
        >
          {{ $t('cgu.decline') }}
        </UButton>
        <UButton
          color="primary"
          :loading="loading"
          @click="handleAccept"
        >
          {{ $t('cgu.accept') }}
        </UButton>
      </div>
    </div>
  </div>
</template>
