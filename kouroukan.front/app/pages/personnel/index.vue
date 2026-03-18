<script setup lang="ts">
import { useAuthStore } from '~/core/stores/auth.store'

definePageMeta({
  layout: 'default',
})

const { t } = useI18n()
const auth = useAuthStore()

const subModules = computed(() => [
  {
    title: t('personnel.enseignant.title'),
    description: t('personnel.enseignant.description'),
    icon: 'i-heroicons-user-group',
    to: '/personnel/enseignants',
    color: 'text-orange-600 dark:text-orange-400',
    bgColor: 'bg-orange-50 dark:bg-orange-900/20',
  },
  {
    title: t('personnel.demandeConge.title'),
    description: t('personnel.demandeConge.description'),
    icon: 'i-heroicons-calendar-days',
    to: '/personnel/demandes-conges',
    color: 'text-teal-600 dark:text-teal-400',
    bgColor: 'bg-teal-50 dark:bg-teal-900/20',
  },
].filter(() => auth.hasPermission('personnel:read')))
</script>

<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('nav.personnel') }}
      </h1>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
        {{ $t('personnel.subtitle') }}
      </p>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <NuxtLink
        v-for="sub in subModules"
        :key="sub.to"
        :to="sub.to"
        class="group rounded-lg border border-gray-200 bg-white p-5 transition-all hover:border-orange-300 hover:shadow-md dark:border-gray-700 dark:bg-gray-900 dark:hover:border-orange-600"
      >
        <div class="flex items-center gap-3">
          <div class="rounded-lg p-2" :class="sub.bgColor">
            <UIcon :name="sub.icon" class="h-6 w-6" :class="sub.color" />
          </div>
          <div>
            <h3 class="font-semibold text-gray-900 group-hover:text-orange-600 dark:text-white dark:group-hover:text-orange-400">
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
