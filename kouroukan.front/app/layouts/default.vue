<script setup lang="ts">
import { useAuthStore } from '~/core/stores/auth.store'
import { useUiStore } from '~/core/stores/ui.store'
import { MODULE_COLORS } from '~/core/theme/tokens'
import type { PermissionKey } from '~/core/auth/rbac'

const { t } = useI18n()
const auth = useAuthStore()
const ui = useUiStore()
const route = useRoute()

interface NavModule {
  slug: string
  label: string
  icon: string
  permission: PermissionKey
  color: string
}

const modules: NavModule[] = [
  { slug: 'inscriptions', label: 'nav.inscriptions', icon: 'i-heroicons-user-plus', permission: 'inscriptions:read', color: MODULE_COLORS.inscriptions },
  { slug: 'pedagogie', label: 'nav.pedagogie', icon: 'i-heroicons-academic-cap', permission: 'pedagogie:read', color: MODULE_COLORS.pedagogie },
  { slug: 'evaluations', label: 'nav.evaluations', icon: 'i-heroicons-clipboard-document-check', permission: 'evaluations:read', color: MODULE_COLORS.evaluations },
  { slug: 'presences', label: 'nav.presences', icon: 'i-heroicons-clock', permission: 'presences:read', color: MODULE_COLORS.presences },
  { slug: 'finances', label: 'nav.finances', icon: 'i-heroicons-banknotes', permission: 'finances:read', color: MODULE_COLORS.finances },
  { slug: 'personnel', label: 'nav.personnel', icon: 'i-heroicons-user-group', permission: 'personnel:read', color: MODULE_COLORS.personnel },
  { slug: 'communication', label: 'nav.communication', icon: 'i-heroicons-chat-bubble-left-right', permission: 'communication:read', color: MODULE_COLORS.communication },
  { slug: 'bde', label: 'nav.bde', icon: 'i-heroicons-sparkles', permission: 'bde:read', color: MODULE_COLORS.bde },
  { slug: 'documents', label: 'nav.documents', icon: 'i-heroicons-document-text', permission: 'documents:read', color: MODULE_COLORS.documents },
  { slug: 'services-premium', label: 'nav.servicesPremium', icon: 'i-heroicons-star', permission: 'services-premium:read', color: MODULE_COLORS['services-premium'] },
]

const supportModule: NavModule = {
  slug: 'support',
  label: 'nav.support',
  icon: 'i-heroicons-lifebuoy',
  permission: 'support:read',
  color: MODULE_COLORS.support,
}

const supportSubItems = computed(() => [
  { label: t('nav.supportTickets'), to: '/support/tickets', icon: 'i-heroicons-ticket' },
  { label: t('nav.supportSuggestions'), to: '/support/suggestions', icon: 'i-heroicons-light-bulb' },
  { label: t('nav.supportAide'), to: '/support/aide', icon: 'i-heroicons-book-open' },
  { label: t('nav.supportAideIA'), to: '/support/aide-ia', icon: 'i-heroicons-cpu-chip' },
])

const visibleModules = computed(() =>
  modules.filter(m => auth.hasPermission(m.permission)),
)

const showSupport = computed(() => auth.hasPermission(supportModule.permission))

const isActiveModule = (slug: string): boolean => {
  return route.path.startsWith(`/${slug}`)
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
          Kouroukan
        </span>
      </div>

      <!-- Navigation -->
      <nav class="flex-1 space-y-1 overflow-y-auto p-2">
        <!-- Dashboard -->
        <NuxtLink
          to="/"
          class="flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors"
          :class="route.path === '/' ? 'bg-green-50 text-green-700 dark:bg-green-900/20 dark:text-green-400' : 'text-gray-600 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-gray-700'"
        >
          <UIcon name="i-heroicons-home" class="h-5 w-5 shrink-0" />
          <span v-if="!ui.sidebarCollapsed">{{ $t('nav.dashboard') }}</span>
        </NuxtLink>

        <div class="my-2 border-t border-gray-200 dark:border-gray-700" />

        <!-- Module links -->
        <template v-for="mod in visibleModules" :key="mod.slug">
          <NuxtLink
            :to="`/${mod.slug}`"
            class="flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors"
            :class="isActiveModule(mod.slug)
              ? 'bg-gray-100 text-gray-900 dark:bg-gray-700 dark:text-white'
              : 'text-gray-600 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-gray-700'"
          >
            <UIcon :name="mod.icon" class="h-5 w-5 shrink-0" :style="{ color: mod.color }" />
            <span v-if="!ui.sidebarCollapsed">{{ $t(mod.label) }}</span>
          </NuxtLink>
        </template>

        <!-- Support section -->
        <template v-if="showSupport">
          <div class="my-2 border-t border-gray-200 dark:border-gray-700" />
          <NuxtLink
            to="/support"
            class="flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors"
            :class="isActiveModule('support')
              ? 'bg-gray-100 text-gray-900 dark:bg-gray-700 dark:text-white'
              : 'text-gray-600 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-gray-700'"
          >
            <UIcon :name="supportModule.icon" class="h-5 w-5 shrink-0" :style="{ color: supportModule.color }" />
            <span v-if="!ui.sidebarCollapsed">{{ $t(supportModule.label) }}</span>
          </NuxtLink>

          <!-- Support sub-items -->
          <template v-if="!ui.sidebarCollapsed && isActiveModule('support')">
            <NuxtLink
              v-for="sub in supportSubItems"
              :key="sub.to"
              :to="sub.to"
              class="ml-6 flex items-center gap-2 rounded-lg px-3 py-1.5 text-sm transition-colors"
              :class="route.path === sub.to
                ? 'text-indigo-600 dark:text-indigo-400'
                : 'text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200'"
            >
              <UIcon :name="sub.icon" class="h-4 w-4 shrink-0" />
              <span>{{ sub.label }}</span>
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
          <!-- Company Switcher -->
          <CompanySwitcher />
          <!-- Language Switcher -->
          <LanguageSwitcher />

          <!-- User Menu -->
          <UDropdownMenu
            :items="[
              [
                { label: $t('user.profile'), icon: 'i-heroicons-user-circle', to: '/profil' },
                { label: $t('user.settings'), icon: 'i-heroicons-cog-6-tooth', to: '/parametres' },
              ],
              [
                { label: $t('user.logout'), icon: 'i-heroicons-arrow-right-on-rectangle', onSelect: handleLogout },
              ],
            ]"
          >
            <UButton variant="ghost" class="gap-2">
              <UAvatar :text="userInitials" size="sm" />
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
