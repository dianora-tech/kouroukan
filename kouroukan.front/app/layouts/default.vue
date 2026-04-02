<script setup lang="ts">
import { useAuthStore } from '~/core/stores/auth.store'
import { useUiStore } from '~/core/stores/ui.store'
import { useNavigation } from '~/composables/useNavigation'
import { useForfaitGating } from '~/composables/useForfaitGating'

const { t, locale, locales, setLocale } = useI18n()
const localePath = useLocalePath()
const auth = useAuthStore()
const ui = useUiStore()
const route = useRoute()
const colorMode = useColorMode()
const toast = useToast()
const { userType } = useForfaitGating()

function handleLockedClick() {
  const forfaitPath = userType.value === 'enseignant'
    ? localePath('/enseignant/forfait')
    : userType.value === 'famille'
      ? localePath('/famille/forfait')
      : null

  toast.add({
    title: t('forfaitGating.locked'),
    description: t('forfaitGating.description'),
    color: 'warning',
    icon: 'i-heroicons-lock-closed',
  })

  if (forfaitPath) {
    navigateTo(forfaitPath)
  }
}

const isDark = computed(() => colorMode.value === 'dark')

function toggleTheme() {
  const newMode = isDark.value ? 'light' : 'dark'
  colorMode.preference = newMode
  if (auth.isAuthenticated) {
    auth.updatePreferences(locale.value, newMode).catch(() => {})
  }
}

async function switchLocale(code: string) {
  await setLocale(code)
  ui.setLocale(code)
  const langCookie = useCookie('kouroukan_lang', { maxAge: 365 * 24 * 60 * 60 })
  langCookie.value = code
  if (auth.isAuthenticated) {
    auth.updatePreferences(code, colorMode.preference).catch(() => {})
  }
}

const otherLocale = computed(() => {
  const all = locales.value as Array<{ code: string, name: string }>
  return all.find(l => l.code !== locale.value) || all[0]
})

const { navItems, spaceLabel, showCompanySwitcher } = useNavigation()

const isActiveItem = (item: { slug: string, to: string, children?: Array<{ to: string }> }): boolean => {
  const path = route.path.replace(/^\/[a-z]{2}\//, '/')
  // Exact match for dashboard items
  if (item.to === localePath('/') || item.to === localePath('/admin') || item.to === localePath('/enseignant') || item.to === localePath('/famille')) {
    return route.path === item.to
  }
  // Direct match
  if (route.path === item.to) return true
  // Children match
  if (item.children?.some(child => route.path === child.to)) return true
  // Prefix match for module items
  return path.startsWith(`/${item.slug.replace(/^(admin-|enseignant-|famille-)/, '')}`)
}

const userName = computed(() => {
  if (!auth.user) return ''
  return `${auth.user.firstName} ${auth.user.lastName}`
})

const userInitials = computed(() => {
  if (!auth.user) return ''
  return `${auth.user.firstName[0]}${auth.user.lastName[0]}`.toUpperCase()
})

async function handleLogout(): Promise<void> {
  await auth.logout()
}
</script>

<template>
  <div class="flex h-screen overflow-hidden bg-gray-50 dark:bg-gray-900">
    <!-- Sidebar -->
    <aside
      class="flex flex-col border-r border-gray-200 bg-white transition-all duration-300 dark:border-gray-700 dark:bg-gray-800"
      :class="ui.sidebarCollapsed ? 'w-16' : 'w-64'"
    >
      <!-- Logo -->
      <div class="flex h-16 items-center gap-2 border-b border-gray-200 px-4 dark:border-gray-700">
        <div class="flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-green-600 text-sm font-bold text-white">
          K
        </div>
        <span
          v-if="!ui.sidebarCollapsed"
          class="text-lg font-semibold text-gray-900 dark:text-white"
        >
          {{ spaceLabel }}
        </span>
      </div>

      <!-- Navigation -->
      <nav class="flex-1 space-y-1 overflow-y-auto p-2">
        <template
          v-for="item in navItems"
          :key="item.slug"
        >
          <!-- Locked nav item -->
          <div
            v-if="item.locked"
            class="flex cursor-not-allowed items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium opacity-50 transition-colors text-gray-400 dark:text-gray-500"
            @click="handleLockedClick"
          >
            <UIcon
              :name="item.icon"
              class="h-5 w-5 shrink-0"
            />
            <span
              v-if="!ui.sidebarCollapsed"
              class="flex-1"
            >{{ item.label }}</span>
            <UIcon
              v-if="!ui.sidebarCollapsed"
              name="i-heroicons-lock-closed"
              class="h-4 w-4 shrink-0 text-amber-500"
            />
          </div>

          <!-- Main nav item (unlocked) -->
          <NuxtLink
            v-else
            :to="item.to"
            class="flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors"
            :class="isActiveItem(item)
              ? 'bg-green-50 text-green-700 dark:bg-green-900/20 dark:text-green-400'
              : 'text-gray-600 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-gray-700'"
          >
            <UIcon
              :name="item.icon"
              class="h-5 w-5 shrink-0"
            />
            <span
              v-if="!ui.sidebarCollapsed"
              class="flex-1"
            >{{ item.label }}</span>
            <UIcon
              v-if="!ui.sidebarCollapsed && item.children?.length"
              name="i-heroicons-chevron-down"
              class="h-4 w-4 shrink-0 transition-transform"
              :class="isActiveItem(item) ? 'rotate-180' : ''"
            />
          </NuxtLink>

          <!-- Sub-items (children) -->
          <template v-if="!item.locked && !ui.sidebarCollapsed && item.children?.length && isActiveItem(item)">
            <NuxtLink
              v-for="child in item.children"
              :key="child.to"
              :to="child.to"
              class="ml-6 flex items-center gap-2 rounded-lg px-3 py-1.5 text-sm transition-colors"
              :class="route.path === child.to
                ? 'text-green-600 dark:text-green-400'
                : 'text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200'"
            >
              <UIcon
                :name="child.icon"
                class="h-4 w-4 shrink-0"
              />
              <span>{{ child.label }}</span>
            </NuxtLink>
          </template>
        </template>
      </nav>

      <!-- Footer -->
      <div class="border-t border-gray-200 p-2 dark:border-gray-700">
        <div class="flex items-center gap-2 px-2">
          <NetworkIndicator />
          <SyncStatus v-if="!ui.sidebarCollapsed" />
        </div>
      </div>
    </aside>

    <!-- Main content -->
    <div class="flex flex-1 flex-col overflow-hidden">
      <!-- Header -->
      <header class="flex h-16 items-center justify-between border-b border-gray-200 bg-white px-4 dark:border-gray-700 dark:bg-gray-800">
        <div class="flex items-center gap-3">
          <UButton
            variant="ghost"
            :icon="ui.sidebarCollapsed ? 'i-heroicons-bars-3' : 'i-heroicons-x-mark'"
            size="sm"
            @click="ui.toggleSidebar()"
          />
        </div>

        <div class="flex items-center gap-3">
          <!-- Company Switcher (establishment only) -->
          <CompanySwitcher v-if="showCompanySwitcher" />

          <!-- User Menu -->
          <UDropdownMenu
            :items="[
              [
                { label: $t('user.profile'), icon: 'i-heroicons-user-circle', to: localePath('/profil') },
                { label: $t('user.settings'), icon: 'i-heroicons-cog-6-tooth', to: localePath('/parametres') },
              ],
              [
                { label: `${$t('user.language')}: ${otherLocale.name}`, icon: 'i-heroicons-language', onSelect: () => switchLocale(otherLocale.code) },
                { label: isDark ? $t('user.themeLight') : $t('user.themeDark'), icon: isDark ? 'i-heroicons-sun' : 'i-heroicons-moon', onSelect: toggleTheme },
              ],
              [
                { label: $t('user.logout'), icon: 'i-heroicons-arrow-right-on-rectangle', onSelect: handleLogout },
              ],
            ]"
          >
            <UButton
              variant="ghost"
              class="gap-2"
            >
              <span
                v-if="auth.user?.avatarUrl"
                class="inline-flex h-8 w-8 shrink-0 items-center justify-center overflow-hidden rounded-full"
              >
                <img
                  :src="auth.user.avatarUrl"
                  alt="avatar"
                  class="h-full w-full object-cover"
                >
              </span>
              <UAvatar
                v-else
                :text="userInitials"
                size="sm"
              />
              <span class="hidden text-sm font-medium sm:inline">{{ userName }}</span>
            </UButton>
          </UDropdownMenu>
        </div>
      </header>

      <!-- Page content -->
      <main class="flex-1 overflow-y-auto p-4 md:p-6">
        <slot />
      </main>
    </div>

    <!-- Floating Help Button -->
    <FloatingHelpButton />
  </div>
</template>
