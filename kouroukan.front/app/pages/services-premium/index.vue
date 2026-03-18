<script setup lang="ts">
import { useAuthStore } from '~/core/stores/auth.store'

definePageMeta({
  layout: 'default',
})

const { t } = useI18n()
const auth = useAuthStore()

const subModules = computed(() => [
  {
    title: t('servicesPremium.serviceParent.title'),
    description: t('servicesPremium.serviceParent.add'),
    icon: 'i-heroicons-star',
    to: '/services-premium/services-parents',
    color: 'text-yellow-600 dark:text-yellow-400',
    bgColor: 'bg-yellow-50 dark:bg-yellow-900/20',
  },
  {
    title: t('servicesPremium.souscription.title'),
    description: t('servicesPremium.souscription.add'),
    icon: 'i-heroicons-credit-card',
    to: '/services-premium/souscriptions',
    color: 'text-indigo-600 dark:text-indigo-400',
    bgColor: 'bg-indigo-50 dark:bg-indigo-900/20',
  },
].filter(() => auth.hasPermission('services-premium:read')))
</script>

<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('nav.servicesPremium') }}
      </h1>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
        {{ $t('servicesPremium.subtitle') }}
      </p>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <NuxtLink
        v-for="sub in subModules"
        :key="sub.to"
        :to="sub.to"
        class="group rounded-lg border border-gray-200 bg-white p-5 transition-all hover:border-yellow-300 hover:shadow-md dark:border-gray-700 dark:bg-gray-900 dark:hover:border-yellow-600"
      >
        <div class="flex items-center gap-3">
          <div class="rounded-lg p-2" :class="sub.bgColor">
            <UIcon :name="sub.icon" class="h-6 w-6" :class="sub.color" />
          </div>
          <div>
            <h3 class="font-semibold text-gray-900 group-hover:text-yellow-600 dark:text-white dark:group-hover:text-yellow-400">
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
