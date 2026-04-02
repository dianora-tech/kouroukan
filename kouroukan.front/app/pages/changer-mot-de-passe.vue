<script setup lang="ts">
import { useAuthStore } from '~/core/stores/auth.store'

definePageMeta({
  layout: 'cgu',
  middleware: 'auth',
})

const { t } = useI18n()
const toast = useToast()
const auth = useAuthStore()

const form = reactive({
  currentPassword: '',
  newPassword: '',
  confirmPassword: '',
})

const loading = ref(false)
const showCurrentPassword = ref(false)
const showNewPassword = ref(false)

const isValid = computed(() =>
  form.currentPassword.length > 0
  && form.newPassword.length >= 8
  && form.newPassword === form.confirmPassword,
)

async function handleSubmit() {
  if (!isValid.value) return

  loading.value = true
  try {
    await auth.changePassword(form.currentPassword, form.newPassword)
    toast.add({ title: t('changePassword.success'), color: 'success' })
    await navigateTo('/', { replace: true })
  }
  catch {
    // Le toast d'erreur est affiche par le auth store
  }
  finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="flex min-h-screen items-center justify-center bg-gradient-to-br from-green-50 to-emerald-100 px-4 dark:from-gray-900 dark:to-gray-800">
    <div class="w-full max-w-md rounded-2xl bg-white p-8 shadow-xl dark:bg-gray-800">
      <div class="mb-6 text-center">
        <div class="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-amber-100 dark:bg-amber-900/30">
          <UIcon
            name="i-heroicons-key"
            class="h-8 w-8 text-amber-600"
          />
        </div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('changePassword.title') }}
        </h1>
        <p class="mt-2 text-sm text-gray-500 dark:text-gray-400">
          {{ $t('changePassword.subtitle') }}
        </p>
      </div>

      <form
        class="space-y-5"
        @submit.prevent="handleSubmit"
      >
        <UFormField :label="$t('changePassword.currentPassword')">
          <UInput
            v-model="form.currentPassword"
            :type="showCurrentPassword ? 'text' : 'password'"
            :placeholder="$t('changePassword.currentPasswordPlaceholder')"
            class="w-full"
            required
          >
            <template #trailing>
              <UButton
                variant="ghost"
                size="xs"
                :icon="showCurrentPassword ? 'i-heroicons-eye-slash' : 'i-heroicons-eye'"
                @click="showCurrentPassword = !showCurrentPassword"
              />
            </template>
          </UInput>
        </UFormField>

        <UFormField :label="$t('changePassword.newPassword')">
          <UInput
            v-model="form.newPassword"
            :type="showNewPassword ? 'text' : 'password'"
            :placeholder="$t('changePassword.newPasswordPlaceholder')"
            class="w-full"
            required
          >
            <template #trailing>
              <UButton
                variant="ghost"
                size="xs"
                :icon="showNewPassword ? 'i-heroicons-eye-slash' : 'i-heroicons-eye'"
                @click="showNewPassword = !showNewPassword"
              />
            </template>
          </UInput>
          <p
            v-if="form.newPassword && form.newPassword.length < 8"
            class="mt-1 text-xs text-red-500"
          >
            {{ $t('changePassword.minLength') }}
          </p>
        </UFormField>

        <UFormField :label="$t('changePassword.confirmPassword')">
          <UInput
            v-model="form.confirmPassword"
            type="password"
            :placeholder="$t('changePassword.confirmPasswordPlaceholder')"
            class="w-full"
            required
          />
          <p
            v-if="form.confirmPassword && form.newPassword !== form.confirmPassword"
            class="mt-1 text-xs text-red-500"
          >
            {{ $t('changePassword.mismatch') }}
          </p>
        </UFormField>

        <UButton
          type="submit"
          color="primary"
          block
          size="lg"
          :loading="loading"
          :disabled="!isValid"
        >
          {{ $t('changePassword.submit') }}
        </UButton>
      </form>
    </div>
  </div>
</template>
