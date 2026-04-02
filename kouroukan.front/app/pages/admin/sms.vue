<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import { useSmsConfigStore } from '~/modules/admin/stores/sms-config.store'
import type { SmsEnvoi } from '~/modules/admin/types/admin.types'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { formatDate } = useFormatDate()
const toast = useToast()
const store = useSmsConfigStore()

const loading = computed(() => store.loading)
const saving = computed(() => store.saving)
const config = computed(() => store.config)
const historique = computed(() => store.historique)

const apiKey = ref('')
const solde = computed(() => config.value?.solde ?? 0)
const smsRestants = computed(() => config.value?.smsRestants ?? 0)
const coutUnitaire = computed(() => config.value?.coutUnitaire ?? 200)

onMounted(async () => {
  await store.fetch()
  await store.fetchHistorique()
  if (config.value) {
    apiKey.value = config.value.apiKey
  }
})

async function saveConfig(): Promise<void> {
  const success = await store.update({ apiKey: apiKey.value })
  if (success) {
    toast.add({ title: t('admin.sms.saveSuccess'), color: 'success' })
  }
  else {
    toast.add({ title: t('admin.sms.saveError'), color: 'error' })
  }
}

const columns: Column[] = [
  { key: 'destinataire', label: t('admin.sms.destinataire'), sortable: false },
  { key: 'message', label: t('admin.sms.message'), sortable: false },
  { key: 'statut', label: t('admin.sms.statut'), sortable: true },
  { key: 'date', label: t('admin.sms.date'), sortable: true, render: (row: any) => formatDate(row.date) },
  { key: 'cout', label: t('admin.sms.cout'), sortable: true },
]
</script>

<template>
  <div class="space-y-6">
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('admin.title'), to: '/admin' },
          { label: $t('admin.sms.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('admin.sms.title') }}
      </h1>
    </div>

    <!-- Solde & Stats -->
    <div class="grid grid-cols-1 gap-4 sm:grid-cols-3">
      <div class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800">
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ $t('admin.sms.solde') }}
        </p>
        <p class="mt-1 text-2xl font-bold text-gray-900 dark:text-white">
          {{ solde.toLocaleString() }} GNF
        </p>
      </div>
      <div class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800">
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ $t('admin.sms.smsRestants') }}
        </p>
        <p class="mt-1 text-2xl font-bold text-gray-900 dark:text-white">
          {{ smsRestants.toLocaleString() }}
        </p>
      </div>
      <div class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800">
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ $t('admin.sms.coutUnitaire') }}
        </p>
        <p class="mt-1 text-2xl font-bold text-gray-900 dark:text-white">
          {{ coutUnitaire }} GNF
        </p>
      </div>
    </div>

    <!-- API Config -->
    <div class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800">
      <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
        {{ $t('admin.sms.configApi') }}
      </h2>
      <div class="flex items-end gap-4">
        <UFormField
          :label="$t('admin.sms.apiKey')"
          class="flex-1"
        >
          <UInput
            v-model="apiKey"
            type="password"
          />
        </UFormField>
        <UButton
          color="primary"
          :loading="saving"
          @click="saveConfig"
        >
          {{ $t('admin.sms.save') }}
        </UButton>
      </div>
    </div>

    <!-- Historique -->
    <div>
      <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
        {{ $t('admin.sms.historique') }}
      </h2>
      <DataTable
        :columns="columns"
        :data="{ items: historique, totalCount: historique.length, page: 1, pageSize: 20 }"
        :loading="loading"
      >
        <template #cell-statut="{ row }">
          <UBadge
            :color="(row as SmsEnvoi).statut === 'Envoyé' ? 'success' : 'error'"
            variant="subtle"
            size="sm"
          >
            {{ (row as SmsEnvoi).statut }}
          </UBadge>
        </template>
        <template #cell-cout="{ row }">
          {{ (row as SmsEnvoi).cout > 0 ? (row as SmsEnvoi).cout + ' GNF' : '-' }}
        </template>
      </DataTable>
    </div>
  </div>
</template>
