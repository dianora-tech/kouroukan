<script setup lang="ts">
import { useAuthStore } from '~/core/stores/auth.store'

definePageMeta({
  layout: 'default',
})

const { t } = useI18n()
const auth = useAuthStore()

const subModules = computed(() => [
  {
    title: t('bde.association.title'),
    description: t('bde.association.description'),
    icon: 'i-heroicons-user-group',
    to: '/bde/associations',
    color: 'text-fuchsia-600 dark:text-fuchsia-400',
    bgColor: 'bg-fuchsia-50 dark:bg-fuchsia-900/20',
  },
  {
    title: t('bde.evenement.title'),
    description: t('bde.evenement.description'),
    icon: 'i-heroicons-calendar-days',
    to: '/bde/evenements',
    color: 'text-blue-600 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-900/20',
  },
  {
    title: t('bde.membreBde.title'),
    description: t('bde.membreBde.description'),
    icon: 'i-heroicons-users',
    to: '/bde/membres-bde',
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-900/20',
  },
  {
    title: t('bde.depenseBde.title'),
    description: t('bde.depenseBde.description'),
    icon: 'i-heroicons-receipt-percent',
    to: '/bde/depenses-bde',
    color: 'text-amber-600 dark:text-amber-400',
    bgColor: 'bg-amber-50 dark:bg-amber-900/20',
  },
].filter(() => auth.hasPermission('bde:read')))
</script>

<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('nav.bde') }}
      </h1>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
        {{ $t('bde.subtitle') }}
      </p>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
      <NuxtLink
        v-for="sub in subModules"
        :key="sub.to"
        :to="sub.to"
        class="group rounded-lg border border-gray-200 bg-white p-5 transition-all hover:border-fuchsia-300 hover:shadow-md dark:border-gray-700 dark:bg-gray-900 dark:hover:border-fuchsia-600"
      >
        <div class="flex items-center gap-3">
          <div
            class="rounded-lg p-2"
            :class="sub.bgColor"
          >
            <UIcon
              :name="sub.icon"
              class="h-6 w-6"
              :class="sub.color"
            />
          </div>
          <div>
            <h3 class="font-semibold text-gray-900 group-hover:text-fuchsia-600 dark:text-white dark:group-hover:text-fuchsia-400">
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
