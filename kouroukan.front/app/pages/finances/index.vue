<script setup lang="ts">
import { useAuthStore } from '~/core/stores/auth.store'

definePageMeta({
  layout: 'default',
})

const { t } = useI18n()
const auth = useAuthStore()

const subModules = computed(() => [
  {
    title: t('finances.facture.title'),
    description: t('finances.facture.description'),
    icon: 'i-heroicons-document-text',
    to: '/finances/factures',
    color: 'text-blue-600 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-900/20',
  },
  {
    title: t('finances.paiement.title'),
    description: t('finances.paiement.description'),
    icon: 'i-heroicons-banknotes',
    to: '/finances/paiements',
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-900/20',
  },
  {
    title: t('finances.depense.title'),
    description: t('finances.depense.description'),
    icon: 'i-heroicons-receipt-percent',
    to: '/finances/depenses',
    color: 'text-red-600 dark:text-red-400',
    bgColor: 'bg-red-50 dark:bg-red-900/20',
  },
  {
    title: t('finances.remuneration.title'),
    description: t('finances.remuneration.description'),
    icon: 'i-heroicons-credit-card',
    to: '/finances/remunerations-enseignants',
    color: 'text-amber-600 dark:text-amber-400',
    bgColor: 'bg-amber-50 dark:bg-amber-900/20',
  },
].filter(() => auth.hasPermission('finances:read')))
</script>

<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('nav.finances') }}
      </h1>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
        {{ $t('finances.subtitle') }}
      </p>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
      <NuxtLink
        v-for="sub in subModules"
        :key="sub.to"
        :to="sub.to"
        class="group rounded-lg border border-gray-200 bg-white p-5 transition-all hover:border-green-300 hover:shadow-md dark:border-gray-700 dark:bg-gray-900 dark:hover:border-green-600"
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
            <h3 class="font-semibold text-gray-900 group-hover:text-green-600 dark:text-white dark:group-hover:text-green-400">
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
