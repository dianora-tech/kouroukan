<script setup lang="ts">
import { apiClient } from '~/core/api/client'

definePageMeta({ layout: 'default' })

const { t } = useI18n()

const kpis = [
  { label: t('admin.kpi.etablissements'), value: 47, icon: 'i-heroicons-building-office-2', color: 'text-blue-600', bg: 'bg-blue-50 dark:bg-blue-900/20' },
  { label: t('admin.kpi.enseignants'), value: 312, icon: 'i-heroicons-academic-cap', color: 'text-green-600', bg: 'bg-green-50 dark:bg-green-900/20' },
  { label: t('admin.kpi.parents'), value: 1_845, icon: 'i-heroicons-users', color: 'text-purple-600', bg: 'bg-purple-50 dark:bg-purple-900/20' },
  { label: t('admin.kpi.eleves'), value: 3_210, icon: 'i-heroicons-user-group', color: 'text-orange-600', bg: 'bg-orange-50 dark:bg-orange-900/20' },
]

const revenusMensuels = [
  { mois: 'Oct', montant: 2_400_000 },
  { mois: 'Nov', montant: 3_100_000 },
  { mois: 'Dec', montant: 2_800_000 },
  { mois: 'Jan', montant: 3_500_000 },
  { mois: 'Fev', montant: 3_900_000 },
  { mois: 'Mar', montant: 4_200_000 },
]

const regions = [
  { nom: 'Conakry', count: 22, pct: 47 },
  { nom: 'Kindia', count: 8, pct: 17 },
  { nom: 'Kankan', count: 6, pct: 13 },
  { nom: 'Labé', count: 5, pct: 11 },
  { nom: 'Autres', count: 6, pct: 12 },
]

const ameliorations = [
  { label: t('admin.usage.connexionMoyenne'), value: '68%', trend: '+5%' },
  { label: t('admin.usage.tauxUtilisationNotes'), value: '42%', trend: '+12%' },
  { label: t('admin.usage.tauxPaiementEnLigne'), value: '31%', trend: '+8%' },
  { label: t('admin.usage.smsEnvoyes'), value: '12 450', trend: '+22%' },
]

// --- Forfait Stats (données réelles depuis l'API) ---
interface ForfaitStats {
  totalEtablissements: number
  etablissementsAvecForfait: number
  tauxEtablissements: number
  totalEnseignants: number
  enseignantsAvecForfait: number
  tauxEnseignants: number
  totalParents: number
  parentsAvecForfait: number
  tauxParents: number
}

const forfaitStats = ref<ForfaitStats | null>(null)
const forfaitStatsLoading = ref(false)

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

async function fetchForfaitStats() {
  forfaitStatsLoading.value = true
  try {
    const res = await apiClient.get<{ data: ForfaitStats }>('/api/admin/stats/forfaits')
    forfaitStats.value = res.data
  } catch {
    // silently fail — stats section simply won't show
  } finally {
    forfaitStatsLoading.value = false
  }
}

onMounted(() => {
  fetchForfaitStats()
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

    <!-- KPI Cards -->
    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
      <div
        v-for="kpi in kpis"
        :key="kpi.label"
        class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800"
      >
        <div class="flex items-center gap-3">
          <div class="flex h-10 w-10 items-center justify-center rounded-lg" :class="kpi.bg">
            <UIcon :name="kpi.icon" class="h-5 w-5" :class="kpi.color" />
          </div>
          <div>
            <p class="text-sm text-gray-500 dark:text-gray-400">{{ kpi.label }}</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ kpi.value.toLocaleString() }}</p>
          </div>
        </div>
      </div>
    </div>

    <div class="grid grid-cols-1 gap-6 lg:grid-cols-2">
      <!-- Revenus du mois -->
      <div class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800">
        <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">{{ $t('admin.revenus.title') }}</h2>
        <div class="space-y-3">
          <div v-for="r in revenusMensuels" :key="r.mois" class="flex items-center gap-3">
            <span class="w-10 text-sm text-gray-500">{{ r.mois }}</span>
            <div class="flex-1 rounded-full bg-gray-100 dark:bg-gray-700">
              <div
                class="h-6 rounded-full bg-primary-500"
                :style="{ width: `${(r.montant / 4_200_000) * 100}%` }"
              />
            </div>
            <span class="w-24 text-right text-sm font-medium text-gray-900 dark:text-white">
              {{ (r.montant / 1_000_000).toFixed(1) }}M GNF
            </span>
          </div>
        </div>
      </div>

      <!-- Repartition par region -->
      <div class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800">
        <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">{{ $t('admin.demographie.title') }}</h2>
        <div class="space-y-3">
          <div v-for="region in regions" :key="region.nom" class="flex items-center gap-3">
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
    <div v-if="forfaitStats" class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800">
      <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">{{ $t('admin.forfaitStats.title') }}</h2>
      <div class="grid grid-cols-1 gap-4 sm:grid-cols-3">
        <div
          v-for="card in forfaitStatsCards"
          :key="card.label"
          class="rounded-lg border border-gray-100 p-4 dark:border-gray-700"
        >
          <div class="flex items-center gap-2 mb-3">
            <div class="flex h-8 w-8 items-center justify-center rounded-lg" :class="card.bg">
              <UIcon :name="card.icon" class="h-4 w-4" :class="card.color" />
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
    <div class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800">
      <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">{{ $t('admin.ameliorations.title') }}</h2>
      <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <div
          v-for="item in ameliorations"
          :key="item.label"
          class="rounded-lg border border-gray-100 p-4 dark:border-gray-700"
        >
          <p class="text-sm text-gray-500 dark:text-gray-400">{{ item.label }}</p>
          <div class="mt-1 flex items-baseline gap-2">
            <span class="text-xl font-bold text-gray-900 dark:text-white">{{ item.value }}</span>
            <UBadge color="success" variant="subtle" size="xs">{{ item.trend }}</UBadge>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
