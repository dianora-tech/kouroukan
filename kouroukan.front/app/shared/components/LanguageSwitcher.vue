<script setup lang="ts">
import { useUiStore } from '~/core/stores/ui.store'
import { useAuthStore } from '~/core/stores/auth.store'

const { locale, locales, setLocale } = useI18n()
const ui = useUiStore()
const auth = useAuthStore()
const colorMode = useColorMode()

const dropdownItems = computed(() =>
  (locales.value as Array<{ code: string, name: string }>).map(l => ({
    label: l.name,
    disabled: l.code === locale.value,
    onSelect: () => switchLocale(l.code),
  })),
)

async function switchLocale(code: string): Promise<void> {
  await setLocale(code)
  ui.setLocale(code)
  const langCookie = useCookie('kouroukan_lang', { maxAge: 365 * 24 * 60 * 60 })
  langCookie.value = code
  // Persister en BDD si connecte
  if (auth.isAuthenticated) {
    try {
      await auth.updatePreferences(code, colorMode.preference)
    }
    catch {
      // Erreur non bloquante
    }
  }
}
</script>

<template>
  <UDropdownMenu :items="dropdownItems">
    <UButton
      variant="ghost"
      size="sm"
      icon="i-heroicons-language"
    />
  </UDropdownMenu>
</template>
