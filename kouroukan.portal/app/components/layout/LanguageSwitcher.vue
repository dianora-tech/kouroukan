<template>
  <UDropdownMenu :items="localeItems">
    <UButton
      variant="ghost"
      size="sm"
      :class="scrolled ? 'text-gray-600 dark:text-gray-300' : 'text-white/80'"
    >
      <UIcon name="i-heroicons-language" class="w-4 h-4" />
      <span class="uppercase text-xs font-medium">{{ locale }}</span>
    </UButton>
  </UDropdownMenu>
</template>

<script setup lang="ts">
defineProps<{
  scrolled?: boolean
}>()

const { locale, locales } = useI18n()
const switchLocalePath = useSwitchLocalePath()

const localeItems = computed(() =>
  (locales.value as Array<{ code: string; name: string }>)
    .filter(l => l.code !== locale.value)
    .map(l => [{
      label: l.name,
      click: () => navigateTo(switchLocalePath(l.code))
    }])
)
</script>
