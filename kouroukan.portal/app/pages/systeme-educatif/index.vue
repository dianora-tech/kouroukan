<template>
  <div class="pt-24">
    <!-- Hero -->
    <section class="py-16 bg-gradient-to-br from-green-700 to-green-800">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
        <h1 class="text-4xl font-bold text-white">{{ $t('education.title') }}</h1>
        <p class="mt-4 text-green-100 text-lg">{{ $t('education.subtitle') }}</p>
      </div>
    </section>

    <!-- Visual diagram -->
    <section class="py-16">
      <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8">
        <SharedSectionTitle :title="$t('education.overview')" centered />

        <!-- Cycle flow diagram -->
        <div class="flex flex-col lg:flex-row gap-4 justify-center items-stretch">
          <div
            v-for="cycle in cycles"
            :key="cycle.key"
            class="flex-1 p-5 rounded-xl border-2 text-center hover:shadow-lg transition-shadow"
            :style="{ borderColor: cycle.color, backgroundColor: `${cycle.color}08` }"
          >
            <div class="w-10 h-10 rounded-full flex items-center justify-center mx-auto mb-3" :style="{ backgroundColor: `${cycle.color}20` }">
              <UIcon :name="cycle.icon" class="w-5 h-5" :style="{ color: cycle.color }" />
            </div>
            <h3 class="font-bold text-gray-900 dark:text-white text-sm">{{ cycle.label }}</h3>
            <p class="text-xs text-gray-500 mt-1">{{ cycle.age }}</p>
            <p class="text-xs mt-1 font-medium" :style="{ color: cycle.color }">{{ cycle.levels }}</p>
            <p v-if="cycle.diploma" class="text-xs text-gray-500 mt-2">{{ cycle.diploma }}</p>
          </div>
        </div>

        <!-- ETFP branch -->
        <div class="mt-8 p-6 rounded-xl border-2 border-orange-400 bg-orange-50/50 dark:bg-orange-900/10">
          <h3 class="font-bold text-orange-700 dark:text-orange-400 text-center mb-3">ETFP — Formation Technique et Professionnelle</h3>
          <div class="grid grid-cols-1 sm:grid-cols-3 gap-4 text-center text-sm">
            <div class="p-3 rounded-lg bg-white dark:bg-gray-900">
              <p class="font-semibold text-gray-900 dark:text-white">Post-primaire</p>
              <p class="text-xs text-gray-500">Apres CM2 — 9-18 mois</p>
              <p class="text-xs text-orange-600 font-medium">CQP</p>
            </div>
            <div class="p-3 rounded-lg bg-white dark:bg-gray-900">
              <p class="font-semibold text-gray-900 dark:text-white">Type A</p>
              <p class="text-xs text-gray-500">Apres BEPC — 2-3 ans</p>
              <p class="text-xs text-orange-600 font-medium">BEP / CAP</p>
            </div>
            <div class="p-3 rounded-lg bg-white dark:bg-gray-900">
              <p class="font-semibold text-gray-900 dark:text-white">Type B</p>
              <p class="text-xs text-gray-500">Apres Bac — 2-3 ans</p>
              <p class="text-xs text-orange-600 font-medium">BTS</p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Summary table -->
    <section class="py-16 bg-gray-50 dark:bg-gray-950">
      <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8">
        <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-8 text-center">Tableau recapitulatif</h2>
        <div class="overflow-x-auto">
          <table class="w-full text-sm">
            <thead>
              <tr class="border-b-2 border-gray-200 dark:border-gray-800">
                <th v-for="col in tableColumns" :key="col" class="py-3 px-4 text-left font-semibold text-gray-700 dark:text-gray-300">
                  {{ $t(`education.table.${col}`) }}
                </th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="row in EDUCATION_CYCLES" :key="row.key" class="border-b border-gray-100 dark:border-gray-800/50 hover:bg-gray-50 dark:hover:bg-gray-900">
                <td class="py-3 px-4 text-gray-600 dark:text-gray-400">{{ row.age }}</td>
                <td class="py-3 px-4 font-medium text-gray-900 dark:text-white">{{ $t(`education.cycles.${row.key}`) }}</td>
                <td class="py-3 px-4 text-gray-600 dark:text-gray-400">{{ row.levels }}</td>
                <td class="py-3 px-4 text-gray-600 dark:text-gray-400">{{ row.duration }}</td>
                <td class="py-3 px-4 font-medium text-green-700 dark:text-green-400">{{ row.diploma }}</td>
                <td class="py-3 px-4 text-gray-500">{{ row.ministry }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </section>

    <!-- Content links -->
    <section class="py-16">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-8 text-center">En savoir plus</h2>
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <NuxtLink
            v-for="link in contentLinks"
            :key="link.slug"
            :to="localePath(`/systeme-educatif/${link.slug}`)"
            class="p-5 rounded-xl border border-gray-200 dark:border-gray-800 hover:shadow-lg transition-shadow flex items-start gap-3"
          >
            <UIcon :name="link.icon" class="w-6 h-6 text-green-600 shrink-0" />
            <div>
              <h3 class="font-semibold text-gray-900 dark:text-white">{{ link.title }}</h3>
              <p class="text-sm text-gray-500 mt-1">{{ link.description }}</p>
            </div>
          </NuxtLink>
        </div>
      </div>
    </section>

    <LandingCtaSection />
  </div>
</template>

<script setup lang="ts">
import { EDUCATION_CYCLES } from '~/utils/constants'

const localePath = useLocalePath()

useSeoMeta({
  title: 'Systeme educatif guineen',
  description: 'Decouvrez la structure du systeme educatif guineen : cycles, niveaux, examens nationaux et passerelles.'
})

const tableColumns = ['age', 'cycle', 'levels', 'duration', 'diploma', 'ministry']

const cycles = [
  { key: 'preschool', label: 'Prescolaire', age: '3-5 ans', levels: 'PS, MS, GS', diploma: '', color: '#f59e0b', icon: 'i-heroicons-puzzle-piece' },
  { key: 'primary', label: 'Primaire', age: '6-11 ans', levels: 'CP1 → CM2', diploma: 'CEE', color: '#16a34a', icon: 'i-heroicons-book-open' },
  { key: 'college', label: 'College', age: '12-15 ans', levels: '7eme → 10eme', diploma: 'BEPC', color: '#2563eb', icon: 'i-heroicons-academic-cap' },
  { key: 'highschool', label: 'Lycee', age: '16-18 ans', levels: '11e, 12e, Tle', diploma: 'BU (SE/SM/SS)', color: '#7c3aed', icon: 'i-heroicons-building-library' },
  { key: 'university', label: 'Universite', age: '19+ ans', levels: 'L1 → M2', diploma: 'LMD', color: '#dc2626', icon: 'i-heroicons-trophy' }
]

const contentLinks = [
  { slug: 'structure-cycles', title: 'Structure des cycles', description: 'Du prescolaire a l\'universite en detail', icon: 'i-heroicons-list-bullet' },
  { slug: 'examens-nationaux', title: 'Examens nationaux', description: 'CEE, BEPC, Baccalaureat Unique', icon: 'i-heroicons-clipboard-document-check' },
  { slug: 'etfp', title: 'Formation technique (ETFP)', description: 'Post-primaire, Type A, Type B et passerelles', icon: 'i-heroicons-wrench-screwdriver' },
  { slug: 'hierarchie-admin', title: 'Hierarchie administrative', description: 'IRE, DPE, DCE, DSEE', icon: 'i-heroicons-building-office' }
]
</script>
