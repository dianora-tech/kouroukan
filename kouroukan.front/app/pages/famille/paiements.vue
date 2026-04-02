<script setup lang="ts">
import { z } from 'zod'
import { useFamilleLiaisonParent } from '~/modules/famille/composables/useFamilleLiaisonParent'
import { useJournalFinancier } from '~/modules/famille/composables/useJournalFinancier'
import { useForfaitGating } from '~/composables/useForfaitGating'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { formatDate } = useFormatDate()
const toast = useToast()
const { isFeatureLocked } = useForfaitGating()
const isLocked = computed(() => isFeatureLocked('paiements'))

const { items: liaisons, fetchAll: fetchLiaisons } = useFamilleLiaisonParent()
const { items: journal, loading, fetchAll: fetchJournal } = useJournalFinancier()

const showPaymentForm = ref(false)

const paymentSchema = z.object({
  montant: z.number().min(1, t('validation.required')),
  methode: z.string().min(1, t('validation.required')),
})

const paymentState = reactive({
  montant: 0,
  methode: 'orange_money',
})

const paymentSaving = ref(false)

async function handlePayment(): Promise<void> {
  paymentSaving.value = true
  try {
    // TODO: wire to real payment API
    await new Promise(resolve => setTimeout(resolve, 1000))
    toast.add({ title: t('famille.paiements.paymentSuccess'), color: 'success' })
    showPaymentForm.value = false
    paymentState.montant = 0
    paymentState.methode = 'orange_money'
    await fetchJournal()
  }
  catch {
    toast.add({ title: t('famille.paiements.paymentError'), color: 'error' })
  }
  finally {
    paymentSaving.value = false
  }
}

const enfantOptions = computed(() =>
  liaisons.value.map(l => ({
    label: `${l.enfantPrenom} ${l.enfantNom} - ${l.classe}`,
    value: String(l.enfantId),
  })),
)

const selectedEnfant = ref('')

const historique = computed(() => {
  if (!selectedEnfant.value) return journal.value
  return journal.value.filter(j => String(j.enfantId) === selectedEnfant.value)
})

watch(enfantOptions, (opts) => {
  if (opts.length > 0 && !selectedEnfant.value) {
    selectedEnfant.value = opts[0].value
  }
})

onMounted(async () => {
  await fetchLiaisons()
  await fetchJournal()
})
</script>

<template>
  <ForfaitRequiredOverlay
    v-if="isLocked"
    user-type="famille"
  />
  <div
    v-else
    class="space-y-6"
  >
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('famille.title'), to: '/famille' },
          { label: $t('famille.paiements.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('famille.paiements.title') }}
      </h1>
    </div>

    <USelect
      v-if="enfantOptions.length > 0"
      v-model="selectedEnfant"
      :items="enfantOptions"
      class="w-72"
    />

    <!-- Loading state -->
    <div
      v-if="loading"
      class="flex items-center justify-center py-12"
    >
      <UIcon
        name="i-heroicons-arrow-path"
        class="h-8 w-8 animate-spin text-gray-400"
      />
    </div>

    <template v-else>
      <UButton
        color="primary"
        icon="i-heroicons-credit-card"
        size="lg"
        @click="showPaymentForm = true"
      >
        {{ $t('famille.paiements.payer') }}
      </UButton>

      <!-- Historique -->
      <div class="rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-800">
        <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
          {{ $t('famille.paiements.historique') }}
        </h2>
        <table class="w-full text-left text-sm">
          <thead class="border-b border-gray-200 dark:border-gray-700">
            <tr>
              <th class="px-4 py-3 font-medium text-gray-500">
                {{ $t('famille.paiements.date') }}
              </th>
              <th class="px-4 py-3 font-medium text-gray-500">
                {{ $t('famille.paiements.montant') }}
              </th>
              <th class="px-4 py-3 font-medium text-gray-500">
                {{ $t('famille.paiements.methode') }}
              </th>
              <th class="px-4 py-3 font-medium text-gray-500">
                {{ $t('famille.paiements.statut') }}
              </th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-100 dark:divide-gray-700">
            <tr
              v-for="p in historique"
              :key="p.id"
            >
              <td class="px-4 py-3 text-gray-900 dark:text-white">
                {{ formatDate(p.date) }}
              </td>
              <td class="px-4 py-3 text-gray-600 dark:text-gray-400">
                {{ p.montant.toLocaleString('fr-GN') }} GNF
              </td>
              <td class="px-4 py-3 text-gray-600 dark:text-gray-400">
                {{ p.methode }}
              </td>
              <td class="px-4 py-3">
                <UBadge
                  :color="p.statut === 'confirme' ? 'success' : 'warning'"
                  variant="subtle"
                  size="sm"
                >
                  {{ p.statut === 'confirme' ? $t('famille.paiements.confirme') : p.statut }}
                </UBadge>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </template>

    <USlideover v-model:open="showPaymentForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ $t('famille.paiements.payer') }}
        </h3>
      </template>
      <template #body>
        <UForm
          :schema="paymentSchema"
          :state="paymentState"
          class="space-y-4 p-4"
          @submit="handlePayment"
        >
          <UFormField
            :label="$t('famille.paiements.montant')"
            name="montant"
            required
          >
            <UInput
              v-model.number="paymentState.montant"
              type="number"
              class="w-full"
              placeholder="0"
            />
          </UFormField>
          <UFormField
            :label="$t('famille.paiements.methode')"
            name="methode"
            required
          >
            <USelect
              v-model="paymentState.methode"
              class="w-full"
              :items="[
                { label: 'Orange Money', value: 'orange_money' },
                { label: 'MTN MoMo', value: 'mtn_momo' },
                { label: 'Soutra Money', value: 'soutra_money' },
              ]"
            />
          </UFormField>
          <div class="flex justify-end gap-3 pt-4">
            <UButton
              variant="outline"
              @click="showPaymentForm = false"
            >
              {{ $t('actions.cancel') }}
            </UButton>
            <UButton
              type="submit"
              color="primary"
              :loading="paymentSaving"
            >
              {{ $t('famille.paiements.payer') }}
            </UButton>
          </div>
        </UForm>
      </template>
    </USlideover>
  </div>
</template>
