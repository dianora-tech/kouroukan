<script setup lang="ts">
import type { ForfaitPlanDto, AbonnementHistoryDto } from '~/composables/useForfaitSubscription'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { forfaitStatus, fetchForfaitStatus } = useForfaitGating()
const {
  availablePlans,
  subscriptionHistory,
  loading,
  saving,
  fetchPlans,
  fetchHistory,
  subscribe,
  cancel,
  formatMontant,
  formatDate,
} = useForfaitSubscription()

const showCancelDialog = ref(false)

const statusColorMap: Record<string, 'success' | 'warning' | 'error' | 'info'> = {
  actif: 'success',
  essai: 'info',
  resilie: 'warning',
  expire: 'error',
  annule: 'error',
}

const historyColumns = [
  { key: 'forfaitNom', label: computed(() => t('forfait.columns.forfait')) },
  { key: 'dateDebut', label: computed(() => t('forfait.columns.dateDebut')) },
  { key: 'dateFin', label: computed(() => t('forfait.columns.dateFin')) },
  { key: 'montant', label: computed(() => t('forfait.columns.montant')) },
  { key: 'statut', label: computed(() => t('forfait.columns.statut')) },
]

async function handleSubscribe(plan: ForfaitPlanDto): Promise<void> {
  const success = await subscribe(plan.id)
  if (success) {
    await fetchForfaitStatus()
    await fetchHistory()
  }
}

async function handleCancel(): Promise<void> {
  if (!forfaitStatus.value?.forfaitId) return
  const success = await cancel(forfaitStatus.value.forfaitId)
  if (success) {
    showCancelDialog.value = false
    await fetchForfaitStatus()
    await fetchHistory()
  }
}

onMounted(async () => {
  await fetchForfaitStatus()
  await Promise.all([
    fetchPlans('enseignant'),
    fetchHistory(),
  ])
})
</script>

<template>
  <div class="space-y-6">
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('enseignant.title'), to: '/enseignant' },
          { label: $t('enseignant.forfait.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('enseignant.forfait.title') }}
      </h1>
    </div>

    <!-- Section 1: Mon forfait actuel -->
    <div class="rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-800">
      <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
        {{ $t('forfait.currentPlan') }}
      </h2>

      <!-- No active plan -->
      <div
        v-if="!forfaitStatus || !forfaitStatus.statut || forfaitStatus.statut === 'expire' || forfaitStatus.statut === 'annule'"
        class="flex flex-col items-center gap-4 py-8 text-center"
      >
        <UIcon
          name="i-heroicons-credit-card"
          class="h-12 w-12 text-gray-400"
        />
        <div>
          <p class="text-lg font-medium text-gray-900 dark:text-white">
            {{ $t('forfait.noPlan') }}
          </p>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            {{ $t('forfait.noPlanDescription') }}
          </p>
        </div>
        <UButton
          color="primary"
          @click="$el?.querySelector('#plans')?.scrollIntoView({ behavior: 'smooth' })"
        >
          {{ $t('forfait.subscribe') }}
        </UButton>
      </div>

      <!-- Active plan -->
      <div v-else>
        <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
          <div>
            <p class="text-sm text-gray-500">
              {{ $t('enseignant.forfait.nom') }}
            </p>
            <p class="font-semibold text-gray-900 dark:text-white">
              {{ forfaitStatus.forfaitNom }}
            </p>
          </div>
          <div>
            <p class="text-sm text-gray-500">
              {{ $t('forfait.columns.statut') }}
            </p>
            <UBadge
              :color="statusColorMap[forfaitStatus.statut ?? ''] ?? 'neutral'"
              variant="subtle"
              size="sm"
            >
              {{ $t(`forfait.status.${forfaitStatus.statut}`) }}
            </UBadge>
          </div>
          <div>
            <p class="text-sm text-gray-500">
              {{ $t('forfait.columns.dateDebut') }}
            </p>
            <p class="font-semibold text-gray-900 dark:text-white">
              {{ forfaitStatus.dateDebut ? formatDate(forfaitStatus.dateDebut) : '-' }}
            </p>
          </div>
          <div>
            <p class="text-sm text-gray-500">
              {{ $t('forfait.columns.dateFin') }}
            </p>
            <p class="font-semibold text-gray-900 dark:text-white">
              <template v-if="forfaitStatus.dateFin">
                {{ $t('forfait.activeUntil', { date: formatDate(forfaitStatus.dateFin) }) }}
              </template>
              <template v-else>
                -
              </template>
            </p>
          </div>
        </div>
        <div
          v-if="forfaitStatus.type !== 'gratuit'"
          class="mt-4"
        >
          <UButton
            color="error"
            variant="outline"
            @click="showCancelDialog = true"
          >
            {{ $t('forfait.cancel') }}
          </UButton>
        </div>
      </div>
    </div>

    <!-- Section 2: Plans disponibles -->
    <div id="plans">
      <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
        {{ $t('forfait.availablePlans') }}
      </h2>
      <div
        v-if="loading"
        class="flex items-center justify-center py-12"
      >
        <UIcon
          name="i-heroicons-arrow-path"
          class="h-6 w-6 animate-spin text-gray-400"
        />
      </div>
      <div
        v-else
        class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3"
      >
        <UCard
          v-for="plan in availablePlans"
          :key="plan.id"
          class="relative"
        >
          <template #header>
            <div class="flex items-center justify-between">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">
                {{ plan.nom }}
              </h3>
              <UBadge
                v-if="plan.estGratuit"
                color="info"
                variant="subtle"
                size="sm"
              >
                {{ $t('forfait.status.essai') }}
              </UBadge>
            </div>
          </template>
          <div class="space-y-3">
            <p
              v-if="plan.description"
              class="text-sm text-gray-500 dark:text-gray-400"
            >
              {{ plan.description }}
            </p>
            <div class="flex items-baseline gap-1">
              <span class="text-2xl font-bold text-gray-900 dark:text-white">
                {{ formatMontant(plan.prixMensuel) }}
              </span>
              <span class="text-sm text-gray-500">/ {{ $t('forfait.perMonth') }}</span>
            </div>
            <p
              v-if="plan.periodeEssaiJours > 0"
              class="text-sm text-primary-600 dark:text-primary-400"
            >
              <UIcon
                name="i-heroicons-gift"
                class="mr-1 inline h-4 w-4"
              />
              {{ $t('forfait.trialDays', { days: plan.periodeEssaiJours }) }}
            </p>
          </div>
          <template #footer>
            <UButton
              block
              color="primary"
              :loading="saving"
              :disabled="forfaitStatus?.forfaitId === plan.id && (forfaitStatus?.statut === 'actif' || forfaitStatus?.statut === 'essai')"
              @click="handleSubscribe(plan)"
            >
              {{ $t('forfait.subscribe') }}
            </UButton>
          </template>
        </UCard>
      </div>
    </div>

    <!-- Section 3: Historique -->
    <div class="rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-800">
      <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
        {{ $t('forfait.history') }}
      </h2>
      <UTable
        :data="subscriptionHistory"
        :columns="historyColumns"
        :loading="loading"
      >
        <template #dateDebut-cell="{ row }">
          {{ formatDate(row.original.dateDebut) }}
        </template>
        <template #dateFin-cell="{ row }">
          {{ row.original.dateFin ? formatDate(row.original.dateFin) : '-' }}
        </template>
        <template #montant-cell="{ row }">
          {{ formatMontant(row.original.montant) }}
        </template>
        <template #statut-cell="{ row }">
          <UBadge
            :color="statusColorMap[row.original.statut] ?? 'neutral'"
            variant="subtle"
            size="sm"
          >
            {{ $t(`forfait.status.${row.original.statut}`) }}
          </UBadge>
        </template>
      </UTable>
      <div
        v-if="!loading && subscriptionHistory.length === 0"
        class="py-8 text-center text-sm text-gray-500"
      >
        {{ $t('forfait.noHistory') }}
      </div>
    </div>

    <!-- Cancel confirmation dialog -->
    <ConfirmDialog
      :open="showCancelDialog"
      :title="$t('forfait.cancel')"
      :description="$t('forfait.cancelConfirm')"
      :confirm-label="$t('forfait.cancel')"
      variant="warning"
      :loading="saving"
      @confirm="handleCancel"
      @cancel="showCancelDialog = false"
      @update:open="showCancelDialog = $event"
    />
  </div>
</template>
