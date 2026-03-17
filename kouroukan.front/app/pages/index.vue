<script setup lang="ts">
import { useAuthStore } from '~/core/stores/auth.store'
import { MODULE_COLORS } from '~/core/theme/tokens'
import type { PermissionKey } from '~/core/auth/rbac'

definePageMeta({
  layout: 'default',
})

const { t } = useI18n()
const auth = useAuthStore()

interface DashboardWidget {
  slug: string
  label: string
  icon: string
  color: string
  permission: PermissionKey
  stat: string
  description: string
}

const widgets: DashboardWidget[] = [
  { slug: 'inscriptions', label: 'nav.inscriptions', icon: 'i-heroicons-user-plus', color: MODULE_COLORS.inscriptions, permission: 'inscriptions:read', stat: '--', description: 'dashboard.inscriptionsDesc' },
  { slug: 'pedagogie', label: 'nav.pedagogie', icon: 'i-heroicons-academic-cap', color: MODULE_COLORS.pedagogie, permission: 'pedagogie:read', stat: '--', description: 'dashboard.pedagogieDesc' },
  { slug: 'evaluations', label: 'nav.evaluations', icon: 'i-heroicons-clipboard-document-check', color: MODULE_COLORS.evaluations, permission: 'evaluations:read', stat: '--', description: 'dashboard.evaluationsDesc' },
  { slug: 'presences', label: 'nav.presences', icon: 'i-heroicons-clock', color: MODULE_COLORS.presences, permission: 'presences:read', stat: '--', description: 'dashboard.presencesDesc' },
  { slug: 'finances', label: 'nav.finances', icon: 'i-heroicons-banknotes', color: MODULE_COLORS.finances, permission: 'finances:read', stat: '--', description: 'dashboard.financesDesc' },
  { slug: 'personnel', label: 'nav.personnel', icon: 'i-heroicons-user-group', color: MODULE_COLORS.personnel, permission: 'personnel:read', stat: '--', description: 'dashboard.personnelDesc' },
  { slug: 'communication', label: 'nav.communication', icon: 'i-heroicons-chat-bubble-left-right', color: MODULE_COLORS.communication, permission: 'communication:read', stat: '--', description: 'dashboard.communicationDesc' },
  { slug: 'bde', label: 'nav.bde', icon: 'i-heroicons-sparkles', color: MODULE_COLORS.bde, permission: 'bde:read', stat: '--', description: 'dashboard.bdeDesc' },
  { slug: 'documents', label: 'nav.documents', icon: 'i-heroicons-document-text', color: MODULE_COLORS.documents, permission: 'documents:read', stat: '--', description: 'dashboard.documentsDesc' },
  { slug: 'services-premium', label: 'nav.servicesPremium', icon: 'i-heroicons-star', color: MODULE_COLORS['services-premium'], permission: 'services-premium:read', stat: '--', description: 'dashboard.servicesPremiumDesc' },
  { slug: 'support', label: 'nav.support', icon: 'i-heroicons-lifebuoy', color: MODULE_COLORS.support, permission: 'support:read', stat: '--', description: 'dashboard.supportDesc' },
]

const visibleWidgets = computed(() =>
  widgets.filter(w => auth.hasPermission(w.permission)),
)
</script>

<template>
  <div>
    <!-- Welcome -->
    <div class="mb-6">
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('dashboard.welcome', { name: auth.user?.firstName || '' }) }}
      </h1>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
        {{ $t('dashboard.subtitle') }}
      </p>
    </div>

    <!-- Module widgets -->
    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
      <NuxtLink
        v-for="widget in visibleWidgets"
        :key="widget.slug"
        :to="`/${widget.slug}`"
        class="group rounded-xl border border-gray-200 bg-white p-5 transition-all hover:shadow-md dark:border-gray-700 dark:bg-gray-800"
      >
        <div class="flex items-start justify-between">
          <div
            class="flex h-10 w-10 items-center justify-center rounded-lg"
            :style="{ backgroundColor: widget.color + '15', color: widget.color }"
          >
            <UIcon :name="widget.icon" class="h-5 w-5" />
          </div>
          <span class="text-2xl font-bold text-gray-900 dark:text-white">
            {{ widget.stat }}
          </span>
        </div>
        <h3 class="mt-3 text-sm font-semibold text-gray-900 dark:text-white">
          {{ $t(widget.label) }}
        </h3>
        <p class="mt-1 text-xs text-gray-500 dark:text-gray-400">
          {{ $t(widget.description) }}
        </p>
      </NuxtLink>
    </div>
  </div>
</template>
