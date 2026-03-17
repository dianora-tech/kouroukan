import { ref, watch } from 'vue'
import { defaultTheme } from './tokens'
import type { ThemeTokens } from './tokens'

const STORAGE_KEY = 'kouroukan_theme'

function applyTokensToRoot(tokens: ThemeTokens): void {
  if (import.meta.server) return

  const root = document.documentElement
  root.style.setProperty('--color-primary', tokens.primary)
  root.style.setProperty('--color-secondary', tokens.secondary)
  root.style.setProperty('--color-accent', tokens.accent)

  for (const [slug, color] of Object.entries(tokens.moduleColors)) {
    root.style.setProperty(`--color-module-${slug}`, color)
  }
}

function loadPersistedTheme(): ThemeTokens {
  if (import.meta.server) return { ...defaultTheme }

  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    if (raw) return JSON.parse(raw) as ThemeTokens
  }
  catch {
    // Ignore parse errors
  }
  return { ...defaultTheme }
}

export function useTheme() {
  const currentTheme = ref<ThemeTokens>(loadPersistedTheme())

  function setTheme(tokens: Partial<ThemeTokens>): void {
    currentTheme.value = {
      ...currentTheme.value,
      ...tokens,
      moduleColors: {
        ...currentTheme.value.moduleColors,
        ...(tokens.moduleColors ?? {}),
      },
    }
  }

  function resetTheme(): void {
    currentTheme.value = { ...defaultTheme }
  }

  watch(
    currentTheme,
    (val) => {
      applyTokensToRoot(val)
      if (!import.meta.server) {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(val))
      }
    },
    { deep: true, immediate: true },
  )

  return {
    currentTheme: readonly(currentTheme),
    setTheme,
    resetTheme,
  }
}
