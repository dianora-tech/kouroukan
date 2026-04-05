<script setup lang="ts">
import { useDashboardStore } from '~/modules/admin/stores/dashboard.store'

definePageMeta({ layout: 'default' })

const { t } = useI18n()

const store = useDashboardStore()
const loading = computed(() => store.loading)

const kpiCards = computed(() => {
  const k = store.kpis
  if (!k) return []
  return [
    { label: t('admin.kpi.etablissements'), value: k.totalEtablissements, icon: 'i-heroicons-building-office-2', color: 'text-blue-600', bg: 'bg-blue-50 dark:bg-blue-900/20' },
    { label: t('admin.kpi.enseignants'), value: k.totalEnseignants, icon: 'i-heroicons-academic-cap', color: 'text-green-600', bg: 'bg-green-50 dark:bg-green-900/20' },
    { label: t('admin.kpi.parents'), value: k.totalParents, icon: 'i-heroicons-users', color: 'text-purple-600', bg: 'bg-purple-50 dark:bg-purple-900/20' },
    { label: t('admin.kpi.eleves'), value: k.totalEleves, icon: 'i-heroicons-user-group', color: 'text-orange-600', bg: 'bg-orange-50 dark:bg-orange-900/20' },
  ]
})

const revenus = computed(() => store.revenus)
const maxRevenu = computed(() => Math.max(...revenus.value.map(r => r.montant), 1))
const regions = computed(() => store.regions)
const usage = computed(() => store.usage)

const forfaitStats = computed(() => store.forfaitStats)
const forfaitStatsCards = computed(() => {
  if (!forfaitStats.value) return []
  const s = forfaitStats.value
  return [
    {
      label: t('admin.kpi.etablissements'),
      total: s.totalEtablissements,
      avecForfait: s.etablissementsAvecForfait,
      taux: s.tauxEtablissements,
      icon: 'i-heroicons-building-office-2',
      color: 'text-blue-600',
      bg: 'bg-blue-50 dark:bg-blue-900/20',
      barColor: 'bg-blue-500',
    },
    {
      label: t('admin.kpi.enseignants'),
      total: s.totalEnseignants,
      avecForfait: s.enseignantsAvecForfait,
      taux: s.tauxEnseignants,
      icon: 'i-heroicons-academic-cap',
      color: 'text-green-600',
      bg: 'bg-green-50 dark:bg-green-900/20',
      barColor: 'bg-green-500',
    },
    {
      label: t('admin.kpi.parents'),
      total: s.totalParents,
      avecForfait: s.parentsAvecForfait,
      taux: s.tauxParents,
      icon: 'i-heroicons-users',
      color: 'text-purple-600',
      bg: 'bg-purple-50 dark:bg-purple-900/20',
      barColor: 'bg-purple-500',
    },
  ]
})

onMounted(() => {
  store.fetchAll()
})
</script>

<template>
  <div class="space-y-6">
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('admin.title') },
          { label: $t('admin.dashboard') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('admin.dashboard') }}
      </h1>
    </div>

    <!-- Loading -->
    <div
      v-if="loading && kpiCards.length === 0"
      class="flex justify-center py-12"
    >
      <UIcon
        name="i-heroicons-arrow-path"
        class="h-8 w-8 animate-spin text-gray-400"
      />
    </div>

    <!-- KPI Cards -->
    <div
      v-if="kpiCards.length > 0"
      class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4"
    >
      <div
        v-for="kpi in kpiCards"
        :key="kpi.label"
        class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800"
      >
        <div class="flex items-center gap-3">
          <div
            class="flex h-10 w-10 items-center justify-center rounded-lg"
            :class="kpi.bg"
          >
            <UIcon
              :name="kpi.icon"
              class="h-5 w-5"
              :class="kpi.color"
            />
          </div>
          <div>
            <p class="text-sm text-gray-500 dark:text-gray-400">
              {{ kpi.label }}
            </p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">
              {{ kpi.value.toLocaleString() }}
            </p>
          </div>
        </div>
      </div>
    </div>

    <div class="grid grid-cols-1 gap-6 lg:grid-cols-2">
      <!-- Revenus du mois -->
      <div
        v-if="revenus.length > 0"
        class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800"
      >
        <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          {{ $t('admin.revenus.title') }}
        </h2>
        <div class="space-y-3">
          <div
            v-for="r in revenus"
            :key="r.mois"
            class="flex items-center gap-3"
          >
            <span class="w-10 text-sm text-gray-500">{{ r.mois }}</span>
            <div class="flex-1 rounded-full bg-gray-100 dark:bg-gray-700">
              <div
                class="h-6 rounded-full bg-primary-500"
                :style="{ width: `${(r.montant / maxRevenu) * 100}%` }"
              />
            </div>
            <span class="w-24 text-right text-sm font-medium text-gray-900 dark:text-white">
              {{ (r.montant / 1_000_000).toFixed(1) }}M GNF
            </span>
          </div>
        </div>
      </div>

      <!-- Repartition par region -->
      <div
        v-if="regions.length > 0"
        class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800"
      >
        <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          {{ $t('admin.demographie.title') }}
        </h2>
        <div class="space-y-3">
          <div
            v-for="region in regions"
            :key="region.nom"
            class="flex items-center gap-3"
          >
            <span class="w-20 text-sm text-gray-700 dark:text-gray-300">{{ region.nom }}</span>
            <div class="flex-1 rounded-full bg-gray-100 dark:bg-gray-700">
              <div
                class="h-4 rounded-full bg-green-500"
                :style="{ width: `${region.pct}%` }"
              />
            </div>
            <span class="text-sm text-gray-500">{{ region.count }} ({{ region.pct }}%)</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Taux de forfaits actifs -->
    <div
      v-if="forfaitStats"
      class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800"
    >
      <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
        {{ $t('admin.forfaitStats.title') }}
      </h2>
      <div class="grid grid-cols-1 gap-4 sm:grid-cols-3">
        <div
          v-for="card in forfaitStatsCards"
          :key="card.label"
          class="rounded-lg border border-gray-100 p-4 dark:border-gray-700"
        >
          <div class="flex items-center gap-2 mb-3">
            <div
              class="flex h-8 w-8 items-center justify-center rounded-lg"
              :class="card.bg"
            >
              <UIcon
                :name="card.icon"
                class="h-4 w-4"
                :class="card.color"
              />
            </div>
            <span class="text-sm font-medium text-gray-700 dark:text-gray-300">{{ card.label }}</span>
          </div>
          <div class="flex items-baseline gap-2 mb-2">
            <span class="text-2xl font-bold text-gray-900 dark:text-white">{{ card.taux }}%</span>
            <span class="text-sm text-gray-500">
              {{ card.avecForfait }} / {{ card.total }}
            </span>
          </div>
          <div class="w-full rounded-full bg-gray-100 dark:bg-gray-700 h-2">
            <div
              class="h-2 rounded-full transition-all"
              :class="card.barColor"
              :style="{ width: `${card.taux}%` }"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- Axes d'amelioration -->
    <div
      v-if="usage.length > 0"
      class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800"
    >
      <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
        {{ $t('admin.ameliorations.title') }}
      </h2>
      <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <div
          v-for="item in usage"
          :key="item.label"
          class="rounded-lg border border-gray-100 p-4 dark:border-gray-700"
        >
          <p class="text-sm text-gray-500 dark:text-gray-400">
            {{ item.label }}
          </p>
          <div class="mt-1 flex items-baseline gap-2">
            <span class="text-xl font-bold text-gray-900 dark:text-white">{{ item.value }}</span>
            <UBadge
              v-if="item.trend"
              color="success"
              variant="subtle"
              size="xs"
            >
              {{ item.trend }}
            </UBadge>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
