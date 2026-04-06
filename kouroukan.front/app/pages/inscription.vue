<script setup lang="ts">
definePageMeta({
  layout: 'auth',
})

const { t } = useI18n()
const toast = useToast()

const form = reactive({
  firstName: '',
  lastName: '',
  email: '',
  phoneNumber: '',
  password: '',
  confirmPassword: '',
})

const loading = ref(false)
const showPassword = ref(false)
const showConfirmPassword = ref(false)

async function handleRegister(): Promise<void> {
  if (form.password !== form.confirmPassword) {
    toast.add({
      title: t('validation.passwordMismatch'),
      color: 'error',
    })
    return
  }

  loading.value = true
  try {
    await $fetch('/api/auth/register', {
      method: 'POST',
      body: {
        firstName: form.firstName,
        lastName: form.lastName,
        email: form.email,
        phoneNumber: form.phoneNumber,
        password: form.password,
        confirmPassword: form.confirmPassword,
      },
    })

    toast.add({
      title: t('auth.registerSuccess'),
      color: 'success',
    })
    await navigateTo('/connexion')
  }
  catch {
    toast.add({
      title: t('auth.registerError'),
      color: 'error',
    })
  }
  finally {
    loading.value = false
  }
}
</script>

<template>
  <div>
    <h2 class="mb-6 text-center text-xl font-bold text-gray-900 dark:text-white">
      {{ $t('auth.registerTitle') }}
    </h2>

    <form
      class="space-y-4"
      @submit.prevent="handleRegister"
    >
      <div class="grid grid-cols-2 gap-3">
        <UFormField :label="$t('auth.firstName')">
          <UInput
            v-model="form.firstName"
            :placeholder="$t('auth.firstNamePlaceholder')"
            required
          />
        </UFormField>
        <UFormField :label="$t('auth.lastName')">
          <UInput
            v-model="form.lastName"
            :placeholder="$t('auth.lastNamePlaceholder')"
            required
          />
        </UFormField>
      </div>

      <UFormField :label="$t('auth.email')">
        <UInput
          v-model="form.email"
          type="email"
          :placeholder="$t('auth.emailPlaceholder')"
          icon="i-heroicons-envelope"
          autocomplete="email"
          required
        />
      </UFormField>

      <UFormField :label="$t('auth.phone')">
        <PhoneInput v-model="form.phoneNumber" />
      </UFormField>

      <UFormField :label="$t('auth.password')">
        <UInput
          v-model="form.password"
          :type="showPassword ? 'text' : 'password'"
          :placeholder="$t('auth.passwordPlaceholder')"
          icon="i-heroicons-lock-closed"
          autocomplete="new-password"
          required
        >
          <template #trailing>
            <UButton
              variant="ghost"
              size="xs"
              :icon="showPassword ? 'i-heroicons-eye-slash' : 'i-heroicons-eye'"
              @click="showPassword = !showPassword"
            />
          </template>
        </UInput>
      </UFormField>

      <UFormField :label="$t('auth.confirmPassword')">
        <UInput
          v-model="form.confirmPassword"
          :type="showConfirmPassword ? 'text' : 'password'"
          :placeholder="$t('auth.confirmPasswordPlaceholder')"
          icon="i-heroicons-lock-closed"
          autocomplete="new-password"
          required
        >
          <template #trailing>
            <UButton
              variant="ghost"
              size="xs"
              :icon="showConfirmPassword ? 'i-heroicons-eye-slash' : 'i-heroicons-eye'"
              @click="showConfirmPassword = !showConfirmPassword"
            />
          </template>
        </UInput>
      </UFormField>

      <UButton
        type="submit"
        block
        :loading="loading"
        color="primary"
        size="lg"
      >
        {{ $t('auth.register') }}
      </UButton>
    </form>

    <div class="mt-4 text-center">
      <NuxtLink
        to="/connexion"
        class="text-sm text-green-600 hover:underline dark:text-green-400"
      >
        {{ $t('auth.hasAccount') }}
      </NuxtLink>
    </div>
  </div>
</template>
