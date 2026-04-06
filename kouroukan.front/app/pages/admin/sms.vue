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
const testSending = computed(() => store.testSending)
const syncing = computed(() => store.syncing)
const config = computed(() => store.config)
const historique = computed(() => store.historique)

const serviceId = ref('')
const secretToken = ref('')
const senderName = ref('Kouroukan')
const testPhone = ref('')
const testMessage = ref('')
const showSecretToken = ref(false)

const solde = computed(() => config.value?.solde ?? 0)
const smsRestants = computed(() => config.value?.smsRestants ?? 0)
const coutUnitaire = computed(() => config.value?.coutUnitaire ?? 200)

onMounted(async () => {
  await store.fetch()
  await store.fetchHistorique()
  if (config.value) {
    serviceId.value = config.value.serviceId
    senderName.value = config.value.senderName || 'Kouroukan'
  }
})

async function saveConfig(): Promise<void> {
  const success = await store.update({
    serviceId: serviceId.value,
    secretToken: secretToken.value || undefined,
    senderName: senderName.value,
  })
  if (success) {
    secretToken.value = ''
    toast.add({ title: t('admin.sms.saveSuccess'), color: 'success' })
  }
  else {
    toast.add({ title: t('admin.sms.saveError'), color: 'error' })
  }
}

async function sendTestSms(): Promise<void> {
  if (!testPhone.value || !testMessage.value) return
  const success = await store.sendTest({
    to: testPhone.value,
    message: testMessage.value,
  })
  if (success) {
    toast.add({ title: 'SMS de test envoye avec succes !', color: 'success' })
    testMessage.value = ''
  }
  else {
    toast.add({ title: 'Echec de l\'envoi du SMS. Verifiez la configuration.', color: 'error' })
  }
}

async function syncBalance(): Promise<void> {
  const success = await store.syncBalance()
  if (success) {
    toast.add({ title: 'Solde synchronise avec NimbaSMS.', color: 'success' })
  }
  else {
    toast.add({ title: 'Impossible de synchroniser le solde.', color: 'error' })
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

    <!-- Bouton Sync Solde -->
    <div class="flex justify-end">
      <UButton
        color="neutral"
        variant="outline"
        :loading="syncing"
        @click="syncBalance"
      >
        Synchroniser le solde
      </UButton>
    </div>

    <!-- Configuration NimbaSMS -->
    <div class="rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-800">
      <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
        Configuration NimbaSMS
      </h2>
      <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <UFormField label="Service ID (Account SID)">
          <UInput
            v-model="serviceId"
            placeholder="Votre Service ID NimbaSMS"
          />
        </UFormField>
        <UFormField label="Secret Token (Auth Token)">
          <UInput
            v-model="secretToken"
            :type="showSecretToken ? 'text' : 'password'"
            placeholder="Laisser vide pour ne pas modifier"
          >
            <template #trailing>
              <UButton
                variant="ghost"
                size="xs"
                :icon="showSecretToken ? 'i-heroicons-eye-slash' : 'i-heroicons-eye'"
                @click="showSecretToken = !showSecretToken"
              />
            </template>
          </UInput>
        </UFormField>
        <UFormField label="Nom expediteur">
          <UInput
            v-model="senderName"
            placeholder="Kouroukan"
          />
        </UFormField>
      </div>
      <div class="mt-6">
        <UButton
          color="primary"
          :loading="saving"
          @click="saveConfig"
        >
          {{ $t('admin.sms.save') }}
        </UButton>
      </div>
    </div>

    <!-- Envoyer un SMS de test -->
    <div class="rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-800">
      <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
        Envoyer un SMS de test
      </h2>
      <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <UFormField label="Numero de telephone">
          <UInput
            v-model="testPhone"
            placeholder="+224 6XX XX XX XX"
          />
        </UFormField>
        <UFormField label="Message">
          <UInput
            v-model="testMessage"
            placeholder="Votre message de test"
          />
        </UFormField>
      </div>
      <div class="mt-4">
        <UButton
          color="primary"
          :loading="testSending"
          @click="sendTestSms"
        >
          Envoyer le test
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
            :color="(row as SmsEnvoi).statut === 'envoye' ? 'success' : (row as SmsEnvoi).statut === 'echoue' ? 'error' : 'warning'"
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
