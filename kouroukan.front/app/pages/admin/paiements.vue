<script setup lang="ts">
import { z } from 'zod'
import type { Column } from '~/shared/components/DataTable.vue'
import { useCompteAdminStore } from '~/modules/admin/stores/compte-admin.store'
import type { CompteAdmin, TransactionAdmin } from '~/modules/admin/types/admin.types'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { formatDate } = useFormatDate()
const toast = useToast()
const store = useCompteAdminStore()

const loading = computed(() => store.loading)
const comptes = computed(() => store.items)
const transactions = computed(() => store.transactions)

const showForm = ref(false)

const accountSchema = z.object({
  operateur: z.string().min(1, t('validation.required')),
  numero: z.string().min(1, t('validation.required')),
})

const accountState = reactive({
  operateur: 'orange_money',
  numero: '',
})

const formSaving = ref(false)

async function handleAddAccount(): Promise<void> {
  formSaving.value = true
  try {
    const result = await store.create({
      operateur: accountState.operateur,
      numero: accountState.numero,
    })
    if (result) {
      toast.add({ title: t('admin.paiement.addSuccess'), color: 'success' })
      showForm.value = false
      accountState.operateur = 'orange_money'
      accountState.numero = ''
    }
  }
  catch {
    toast.add({ title: t('admin.paiement.addError'), color: 'error' })
  }
  finally {
    formSaving.value = false
  }
}

onMounted(async () => {
  await store.fetchAll()
  await store.fetchTransactions()
})

const transactionColumns: Column[] = [
  { key: 'operateur', label: t('admin.paiement.operateur'), sortable: true },
  { key: 'type', label: t('admin.paiement.type'), sortable: true },
  { key: 'montant', label: t('admin.paiement.montant'), sortable: true },
  { key: 'reference', label: t('admin.paiement.reference'), sortable: false },
  { key: 'date', label: t('admin.paiement.date'), sortable: true, render: (row: any) => formatDate(row.date) },
  { key: 'statut', label: t('admin.paiement.statut'), sortable: true },
]

function formatMontant(montant: number): string {
  return montant.toLocaleString('fr-GN') + ' GNF'
}
</script>

<template>
  <div class="space-y-6">
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('admin.title'), to: '/admin' },
          { label: $t('admin.paiement.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('admin.paiement.title') }}
      </h1>
    </div>

    <div class="flex justify-end">
      <UButton
        color="primary"
        icon="i-heroicons-plus"
        @click="showForm = true"
      >
        {{ $t('admin.paiement.addAccount') }}
      </UButton>
    </div>

    <!-- Comptes Mobile Money -->
    <div class="grid grid-cols-1 gap-4 sm:grid-cols-3">
      <div
        v-for="compte in comptes"
        :key="compte.id"
        class="rounded-xl border border-gray-200 bg-white p-5 dark:border-gray-700 dark:bg-gray-800"
      >
        <div class="flex items-center justify-between">
          <h3 class="text-sm font-semibold text-gray-900 dark:text-white">
            {{ compte.operateur }}
          </h3>
          <UBadge
            :color="compte.statut === 'Actif' ? 'success' : 'neutral'"
            variant="subtle"
            size="xs"
          >
            {{ compte.statut }}
          </UBadge>
        </div>
        <p class="mt-1 text-xs text-gray-500 dark:text-gray-400">
          {{ compte.numero }}
        </p>
        <p class="mt-2 text-xl font-bold text-gray-900 dark:text-white">
          {{ formatMontant(compte.solde) }}
        </p>
      </div>
    </div>

    <!-- Historique Transactions -->
    <div>
      <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
        {{ $t('admin.paiement.historique') }}
      </h2>
      <DataTable
        :columns="transactionColumns"
        :data="{ items: transactions, totalCount: transactions.length, page: 1, pageSize: 20 }"
        :loading="loading"
      >
        <template #cell-montant="{ row }">
          {{ formatMontant((row as TransactionAdmin).montant) }}
        </template>
        <template #cell-statut="{ row }">
          <UBadge
            :color="(row as TransactionAdmin).statut === 'Succès' ? 'success' : 'error'"
            variant="subtle"
            size="sm"
          >
            {{ (row as TransactionAdmin).statut }}
          </UBadge>
        </template>
        <template #cell-type="{ row }">
          <UBadge
            :color="(row as TransactionAdmin).type === 'Réception' ? 'info' : 'warning'"
            variant="subtle"
            size="sm"
          >
            {{ (row as TransactionAdmin).type }}
          </UBadge>
        </template>
      </DataTable>
    </div>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ $t('admin.paiement.addAccount') }}
        </h3>
      </template>
      <template #body>
        <UForm
          :schema="accountSchema"
          :state="accountState"
          class="space-y-4 p-4"
          @submit="handleAddAccount"
        >
          <UFormField
            :label="$t('admin.paiement.operateur')"
            name="operateur"
            required
          >
            <USelect
              v-model="accountState.operateur"
              class="w-full"
              :items="[
                { label: 'Orange Money', value: 'orange_money' },
                { label: 'MTN MoMo', value: 'mtn_momo' },
                { label: 'Soutra Money', value: 'soutra_money' },
              ]"
            />
          </UFormField>
          <UFormField
            :label="$t('admin.paiement.numero')"
            name="numero"
            required
          >
            <UInput
              v-model="accountState.numero"
              class="w-full"
              placeholder="+224 6XX XX XX XX"
            />
          </UFormField>
          <div class="flex justify-end gap-3 pt-4">
            <UButton
              variant="outline"
              @click="showForm = false"
            >
              {{ $t('actions.cancel') }}
            </UButton>
            <UButton
              type="submit"
              color="primary"
              :loading="formSaving"
            >
              {{ $t('actions.create') }}
            </UButton>
          </div>
        </UForm>
      </template>
    </USlideover>
  </div>
</template>
