<script setup lang="ts">
import { useAuthStore } from '~/core/stores/auth.store'

definePageMeta({
  layout: 'default',
})

const { t } = useI18n()
const auth = useAuthStore()

const subModules = computed(() => [
  {
    title: t('pedagogie.niveauClasse.title'),
    description: t('pedagogie.niveauClasse.description'),
    icon: 'i-heroicons-academic-cap',
    to: '/pedagogie/niveaux-classes',
    color: 'text-indigo-600 dark:text-indigo-400',
    bgColor: 'bg-indigo-50 dark:bg-indigo-900/20',
  },
  {
    title: t('pedagogie.classe.title'),
    description: t('pedagogie.classe.description'),
    icon: 'i-heroicons-rectangle-group',
    to: '/pedagogie/classes',
    color: 'text-blue-600 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-900/20',
  },
  {
    title: t('pedagogie.matiere.title'),
    description: t('pedagogie.matiere.description'),
    icon: 'i-heroicons-book-open',
    to: '/pedagogie/matieres',
    color: 'text-purple-600 dark:text-purple-400',
    bgColor: 'bg-purple-50 dark:bg-purple-900/20',
  },
  {
    title: t('pedagogie.salle.title'),
    description: t('pedagogie.salle.description'),
    icon: 'i-heroicons-building-office',
    to: '/pedagogie/salles',
    color: 'text-teal-600 dark:text-teal-400',
    bgColor: 'bg-teal-50 dark:bg-teal-900/20',
  },
  {
    title: t('pedagogie.seance.title'),
    description: t('pedagogie.seance.description'),
    icon: 'i-heroicons-calendar-days',
    to: '/pedagogie/seances',
    color: 'text-orange-600 dark:text-orange-400',
    bgColor: 'bg-orange-50 dark:bg-orange-900/20',
  },
  {
    title: t('pedagogie.cahierTextes.title'),
    description: t('pedagogie.cahierTextes.description'),
    icon: 'i-heroicons-document-text',
    to: '/pedagogie/cahiers-textes',
    color: 'text-green-600 dark:text-green-400',
    bgColor: 'bg-green-50 dark:bg-green-900/20',
  },
].filter(() => auth.hasPermission('pedagogie:read')))
</script>

<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('nav.pedagogie') }}
      </h1>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
        {{ $t('pedagogie.subtitle') }}
      </p>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
      <NuxtLink
        v-for="sub in subModules"
        :key="sub.to"
        :to="sub.to"
        class="group rounded-lg border border-gray-200 bg-white p-5 transition-all hover:border-indigo-300 hover:shadow-md dark:border-gray-700 dark:bg-gray-900 dark:hover:border-indigo-600"
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
