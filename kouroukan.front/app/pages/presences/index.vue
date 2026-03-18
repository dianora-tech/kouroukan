<script setup lang="ts">
import { useAuthStore } from '~/core/stores/auth.store'

definePageMeta({
  layout: 'default',
})

const { t } = useI18n()
const auth = useAuthStore()

const subModules = computed(() => [
  {
    title: t('presences.appel.title'),
    description: t('presences.appel.description'),
    icon: 'i-heroicons-clipboard-document-list',
    to: '/presences/appels',
    color: 'text-sky-600 dark:text-sky-400',
    bgColor: 'bg-sky-50 dark:bg-sky-900/20',
  },
  {
    title: t('presences.absence.title'),
    description: t('presences.absence.description'),
    icon: 'i-heroicons-user-minus',
    to: '/presences/absences',
    color: 'text-red-600 dark:text-red-400',
    bgColor: 'bg-red-50 dark:bg-red-900/20',
  },
  {
    title: t('presences.badgeage.title'),
    description: t('presences.badgeage.description'),
    icon: 'i-heroicons-identification',
    to: '/presences/badgeages',
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-900/20',
  },
].filter(() => auth.hasPermission('presences:read')))
</script>

<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('nav.presences') }}
      </h1>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
        {{ $t('presences.subtitle') }}
      </p>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
      <NuxtLink
        v-for="sub in subModules"
        :key="sub.to"
        :to="sub.to"
        class="group rounded-lg border border-gray-200 bg-white p-5 transition-all hover:border-sky-300 hover:shadow-md dark:border-gray-700 dark:bg-gray-900 dark:hover:border-sky-600"
      >
        <div class="flex items-center gap-3">
          <div class="rounded-lg p-2" :class="sub.bgColor">
            <UIcon :name="sub.icon" class="h-6 w-6" :class="sub.color" />
          </div>
          <div>
            <h3 class="font-semibold text-gray-900 group-hover:text-sky-600 dark:text-white dark:group-hover:text-sky-400">
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
