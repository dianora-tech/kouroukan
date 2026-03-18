<script setup lang="ts">
import { useAuthStore } from '~/core/stores/auth.store'

definePageMeta({
  layout: 'default',
})

const { t } = useI18n()
const auth = useAuthStore()

const subModules = computed(() => [
  {
    title: t('nav.supportTickets'),
    description: t('supportPages.tickets.subtitle'),
    icon: 'i-heroicons-ticket',
    to: '/support/tickets',
    color: 'text-red-600 dark:text-red-400',
    bgColor: 'bg-red-50 dark:bg-red-900/20',
  },
  {
    title: t('nav.supportSuggestions'),
    description: t('supportPages.suggestions.subtitle'),
    icon: 'i-heroicons-light-bulb',
    to: '/support/suggestions',
    color: 'text-amber-600 dark:text-amber-400',
    bgColor: 'bg-amber-50 dark:bg-amber-900/20',
  },
  {
    title: t('nav.supportAide'),
    description: t('supportPages.aide.subtitle'),
    icon: 'i-heroicons-book-open',
    to: '/support/aide',
    color: 'text-blue-600 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-900/20',
  },
  {
    title: t('nav.supportAideIA'),
    description: t('supportPages.aideIA.subtitle'),
    icon: 'i-heroicons-cpu-chip',
    to: '/support/aide-ia',
    color: 'text-indigo-600 dark:text-indigo-400',
    bgColor: 'bg-indigo-50 dark:bg-indigo-900/20',
  },
].filter(() => auth.hasPermission('support:read')))
</script>

<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('nav.support') }}
      </h1>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
        {{ $t('dashboard.supportDesc') }}
      </p>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
      <NuxtLink
        v-for="sub in subModules"
        :key="sub.to"
        :to="sub.to"
        class="group rounded-lg border border-gray-200 bg-white p-5 transition-all hover:border-indigo-300 hover:shadow-md dark:border-gray-700 dark:bg-gray-900 dark:hover:border-indigo-600"
      >
        <div class="flex items-center gap-3">
          <div class="rounded-lg p-2" :class="sub.bgColor">
            <UIcon :name="sub.icon" class="h-6 w-6" :class="sub.color" />
          </div>
          <div>
            <h3 class="font-semibold text-gray-900 group-hover:text-indigo-600 dark:text-white dark:group-hover:text-indigo-400">
              {{ sub.title }}
            </h3>
            <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
              {{ sub.description }}
            </p>
          </div>
        </div>
      </NuxtLink>
    </div>
  </div>
</template>
