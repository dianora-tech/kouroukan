<template>
  <div class="pt-24">
    <section class="py-16">
      <div class="max-w-lg mx-auto px-4 sm:px-6 lg:px-8">
        <!-- Header -->
        <div class="text-center mb-8">
          <div class="w-16 h-16 rounded-full bg-green-100 dark:bg-green-900/30 flex items-center justify-center mx-auto mb-4">
            <UIcon name="i-heroicons-shield-check" class="w-8 h-8 text-green-600" />
          </div>
          <h1 class="text-3xl font-bold text-gray-900 dark:text-white">{{ $t('verification.title') }}</h1>
          <p class="mt-2 text-gray-500">{{ $t('verification.subtitle') }}</p>
        </div>

        <div class="bg-white dark:bg-gray-900 rounded-2xl border border-gray-200 dark:border-gray-800 p-6 sm:p-8 shadow-sm">
          <!-- Password form -->
          <template v-if="!result && !errorMsg">
            <p class="text-sm text-gray-600 dark:text-gray-400 mb-4">
              {{ $t('verification.enterPassword') }}
            </p>
            <div class="space-y-4">
              <UFormField :label="$t('verification.code')" name="code">
                <UInput :model-value="code" disabled class="w-full" />
              </UFormField>
              <UFormField :label="$t('verification.password')" name="password">
                <UInput
                  v-model="password"
                  type="password"
                  :placeholder="$t('verification.passwordPlaceholder')"
                  class="w-full"
                  @keyup.enter="verify"
                />
              </UFormField>
              <UButton
                color="primary"
                block
                :loading="loading"
                :disabled="!password"
                @click="verify"
              >
                {{ $t('verification.submit') }}
              </UButton>
            </div>
          </template>

          <!-- Success result -->
          <template v-if="result">
            <div class="text-center">
              <div class="w-12 h-12 rounded-full bg-green-100 dark:bg-green-900/30 flex items-center justify-center mx-auto mb-4">
                <UIcon name="i-heroicons-check-badge" class="w-6 h-6 text-green-600" />
              </div>
              <h2 class="text-xl font-bold text-green-600 mb-4">{{ $t('verification.valid') }}</h2>
            </div>

            <div class="space-y-3 mt-6">
              <div class="flex justify-between py-2 border-b border-gray-100 dark:border-gray-800">
                <span class="text-sm text-gray-500">{{ $t('verification.documentType') }}</span>
                <span class="text-sm font-medium text-gray-900 dark:text-white">{{ result.documentType }}</span>
              </div>
              <div class="flex justify-between py-2 border-b border-gray-100 dark:border-gray-800">
                <span class="text-sm text-gray-500">{{ $t('verification.date') }}</span>
                <span class="text-sm font-medium text-gray-900 dark:text-white">{{ formatDate(result.date) }}</span>
              </div>
              <div class="flex justify-between py-2 border-b border-gray-100 dark:border-gray-800">
                <span class="text-sm text-gray-500">{{ $t('verification.etablissement') }}</span>
                <span class="text-sm font-medium text-gray-900 dark:text-white">{{ result.etablissement }}</span>
              </div>
              <div class="flex justify-between py-2">
                <span class="text-sm text-gray-500">{{ $t('verification.authenticity') }}</span>
                <UBadge color="primary" variant="subtle" size="sm">{{ $t('verification.confirmed') }}</UBadge>
              </div>
            </div>

            <UButton
              variant="outline"
              block
              class="mt-6"
              @click="reset"
            >
              {{ $t('verification.verifyAnother') }}
            </UButton>
          </template>

          <!-- Error -->
          <template v-if="errorMsg">
            <div class="text-center">
              <div class="w-12 h-12 rounded-full bg-red-100 dark:bg-red-900/30 flex items-center justify-center mx-auto mb-4">
                <UIcon name="i-heroicons-x-circle" class="w-6 h-6 text-red-600" />
              </div>
              <h2 class="text-xl font-bold text-red-600 mb-2">{{ $t('verification.invalid') }}</h2>
              <p class="text-sm text-gray-500">{{ errorMsg }}</p>
            </div>

            <UButton
              variant="outline"
              block
              class="mt-6"
              @click="reset"
            >
              {{ $t('verification.tryAgain') }}
            </UButton>
          </template>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
definePageMeta({ layout: 'default' })

useSeoMeta({ title: 'Verification de document', robots: 'noindex' })

const route = useRoute()
const { t } = useI18n()
const { formatDate } = useFormatDate()

const code = route.params.code as string
const password = ref('')
const loading = ref(false)
const errorMsg = ref<string | null>(null)
const result = ref<{
  documentType: string
  date: string
  etablissement: string
} | null>(null)

async function verify() {
  if (!password.value) return
  loading.value = true
  errorMsg.value = null
  result.value = null

  try {
    const data = await $fetch<{
      documentType: string
      date: string
      etablissement: string
    }>(`/api/public/verification/${code}`, {
      params: { password: password.value },
    })
    result.value = data
  }
  catch {
    errorMsg.value = t('verification.errorMessage')
  }
  finally {
    loading.value = false
  }
}

function reset() {
  password.value = ''
  errorMsg.value = null
  result.value = null
}
</script>
