<script setup lang="ts">
import { useAuthStore } from '~/core/stores/auth.store'

definePageMeta({
  layout: 'auth',
})

const { t, setLocale } = useI18n()
const localePath = useLocalePath()
const auth = useAuthStore()
const toast = useToast()
const colorMode = useColorMode()

const form = reactive({
  email: '',
  password: '',
})

const loading = ref(false)
const showPassword = ref(false)

async function handleLogin(): Promise<void> {
  if (!form.email || !form.password) {
    toast.add({
      title: t('validation.required'),
      color: 'error',
    })
    return
  }

  loading.value = true
  try {
    await auth.login(form.email, form.password)

    // Appliquer les preferences utilisateur (langue et theme)
    if (auth.user?.preferredLocale) {
      await setLocale(auth.user.preferredLocale)
      const langCookie = useCookie('kouroukan_lang', { maxAge: 365 * 24 * 60 * 60 })
      langCookie.value = auth.user.preferredLocale
    }
    if (auth.user?.preferredTheme) {
      colorMode.preference = auth.user.preferredTheme
    }

    // Must change password (first login with temporary password)
    if (auth.mustChangePassword) {
      toast.add({
        title: t('changePassword.title'),
        description: t('changePassword.subtitle'),
        color: 'warning',
      })
      await navigateTo(localePath('/changer-mot-de-passe'))
      return
    }

    // Check CGU after login
    const cguUpToDate = await auth.checkCgu()
    if (!cguUpToDate) {
      toast.add({
        title: t('cgu.required'),
        description: t('cgu.mustAccept'),
        color: 'warning',
      })
      await navigateTo(localePath('/support/cgu'))
      return
    }

    toast.add({
      title: t('auth.loginSuccess'),
      color: 'success',
    })

    // Director with incomplete onboarding → redirect to onboarding wizard
    if (!auth.onboardingCompleted && auth.roles.includes('directeur')) {
      await navigateTo(localePath('/onboarding'))
      return
    }

    await navigateTo(localePath('/'))
  }
  catch {
    toast.add({
      title: t('auth.loginError'),
      description: t('auth.invalidCredentials'),
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
      {{ $t('auth.loginTitle') }}
    </h2>

    <form class="space-y-4" @submit.prevent="handleLogin">
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

      <UFormField :label="$t('auth.password')">
        <UInput
          v-model="form.password"
          :type="showPassword ? 'text' : 'password'"
          :placeholder="$t('auth.passwordPlaceholder')"
          icon="i-heroicons-lock-closed"
          autocomplete="current-password"
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

      <UButton
        type="submit"
        block
        :loading="loading"
        color="primary"
        size="lg"
      >
        {{ $t('auth.login') }}
      </UButton>
    </form>

    <div class="mt-4 text-center">
      <NuxtLink to="/inscription" class="text-sm text-green-600 hover:underline dark:text-green-400">
        {{ $t('auth.noAccount') }}
      </NuxtLink>
    </div>
  </div>
</template>
