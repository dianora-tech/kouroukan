<script setup lang="ts">
import { useAuthStore } from '~/core/stores/auth.store'

definePageMeta({
  layout: 'default',
})

const { t, locale, setLocale, availableLocales } = useI18n()
const colorMode = useColorMode()
const auth = useAuthStore()
const toast = useToast()

const notifEmail = ref(true)
const notifPush = ref(false)
const saving = ref(false)

const selectedLocale = ref(locale.value)

watch(selectedLocale, async (newLocale) => {
  if (newLocale !== locale.value) {
    await setLocale(newLocale)
    const langCookie = useCookie('kouroukan_lang', { maxAge: 365 * 24 * 60 * 60 })
    langCookie.value = newLocale
  }
})

const themeOptions = computed(() => [
  { label: t('parametres.themeLight'), value: 'light' },
  { label: t('parametres.themeDark'), value: 'dark' },
  { label: t('parametres.themeSystem'), value: 'system' },
])

const languageOptions = computed(() =>
  availableLocales.map(l => ({ label: l.toUpperCase(), value: l })),
)

async function handleSave() {
  saving.value = true
  try {
    await auth.updatePreferences(selectedLocale.value, colorMode.preference)
    toast.add({ title: t('profil.updateSuccess'), color: 'success', icon: 'i-heroicons-check-circle' })
  }
  catch {
    // Toast d'erreur affiche par le store
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('parametres.title') }}
      </h1>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
        {{ $t('parametres.subtitle') }}
      </p>
    </div>

    <div class="grid grid-cols-1 gap-6 lg:grid-cols-2">
      <!-- Appearance -->
      <div class="rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-900">
        <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          {{ $t('parametres.appearance') }}
        </h2>
        <div class="space-y-4">
          <UFormField :label="$t('parametres.theme')">
            <USelect
              v-model="colorMode.preference"
              :items="themeOptions"
              value-key="value"
              label-key="label"
              class="w-full"
            />
          </UFormField>
          <UFormField :label="$t('parametres.language')">
            <USelect
              v-model="selectedLocale"
              :items="languageOptions"
              value-key="value"
              label-key="label"
              class="w-full"
            />
          </UFormField>
        </div>
      </div>

      <!-- Notifications -->
      <div class="rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-900">
        <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          {{ $t('parametres.notifications') }}
        </h2>
        <div class="space-y-4">
          <div class="flex items-center justify-between">
            <span class="text-sm text-gray-700 dark:text-gray-300">{{ $t('parametres.notifEmail') }}</span>
            <UToggle v-model="notifEmail" />
          </div>
          <div class="flex items-center justify-between">
            <span class="text-sm text-gray-700 dark:text-gray-300">{{ $t('parametres.notifPush') }}</span>
            <UToggle v-model="notifPush" />
          </div>
        </div>
      </div>
    </div>

    <div class="flex justify-end">
      <UButton
        color="primary"
        icon="i-heroicons-check"
        :loading="saving"
        @click="handleSave"
      >
        {{ $t('parametres.saveChanges') }}
      </UButton>
    </div>
  </div>
</template>
