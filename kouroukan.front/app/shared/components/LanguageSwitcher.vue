<script setup lang="ts">
import { useUiStore } from '~/core/stores/ui.store'

const { locale, locales, setLocale } = useI18n()
const ui = useUiStore()

const availableLocales = computed(() =>
  (locales.value as Array<{ code: string; name: string }>).map(l => ({
    label: l.name,
    value: l.code,
  })),
)

async function switchLocale(code: string): Promise<void> {
  await setLocale(code)
  ui.setLocale(code)
}
</script>

<template>
  <UDropdownMenu
    :items="availableLocales.map(l => [{ label: l.label, click: () => switchLocale(l.value) }])"
  >
    <UButton variant="ghost" size="sm" icon="i-heroicons-language" />
  </UDropdownMenu>
</template>
