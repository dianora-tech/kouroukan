<script setup lang="ts">
import { z } from 'zod'
import type { Paiement, CreatePaiementPayload, UpdatePaiementPayload } from '../types/paiement.types'
import { STATUTS_PAIEMENT, MOYENS_PAIEMENT } from '../types/paiement.types'
import { usePaiement } from '../composables/usePaiement'

const props = defineProps<{
  entity?: Paiement | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreatePaiementPayload | UpdatePaiementPayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const { typeOptions } = usePaiement()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  factureId: z.number({ required_error: t('validation.required') }).int().positive(),
  montantPaye: z.number({ required_error: t('validation.required') }).min(0),
  datePaiement: z.string().min(1, t('validation.required')),
  moyenPaiement: z.string().min(1, t('validation.required')),
  referenceMobileMoney: z.string().optional().nullable().or(z.literal('')),
  statutPaiement: z.string().min(1, t('validation.required')),
  caissierId: z.number().int().positive().optional().nullable(),
  numeroRecu: z.string().min(1, t('validation.required')),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  factureId: props.entity?.factureId ?? undefined,
  montantPaye: props.entity?.montantPaye ?? 0,
  datePaiement: props.entity?.datePaiement ? props.entity.datePaiement.split('T')[0] : new Date().toISOString().split('T')[0],
  moyenPaiement: props.entity?.moyenPaiement ?? '',
  referenceMobileMoney: props.entity?.referenceMobileMoney ?? '',
  statutPaiement: props.entity?.statutPaiement ?? 'EnAttente',
  caissierId: props.entity?.caissierId ?? undefined,
  numeroRecu: props.entity?.numeroRecu ?? '',
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

const isMobileMoney = computed(() =>
  ['OrangeMoney', 'SoutraMoney', 'MTNMoMo'].includes(state.moyenPaiement ?? ''),
)

const statutOptions = STATUTS_PAIEMENT.map(s => ({
  label: t(`finances.paiement.statut.${s}`),
  value: s,
}))

const moyenOptions = MOYENS_PAIEMENT.map(m => ({
  label: t(`finances.paiement.moyen.${m}`),
  value: m,
}))

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdatePaiementPayload)
    }
    else {
      emit('submit', state as CreatePaiementPayload)
    }
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <UForm :schema="schema" :state="state" class="space-y-4" @submit="onSubmit">
    <UFormField :label="t('finances.paiement.typeId')" name="typeId" required>
      <USelect v-model="state.typeId" :items="typeOptions" value-key="value" :placeholder="t('finances.paiement.typePlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('finances.paiement.numeroRecu')" name="numeroRecu" required>
      <UInput v-model="state.numeroRecu" :placeholder="t('finances.paiement.numeroRecuPlaceholder')" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('finances.paiement.montantPaye')" name="montantPaye" required>
        <UInput v-model.number="state.montantPaye" type="number" min="0" step="1" class="w-full" />
      </UFormField>

      <UFormField :label="t('finances.paiement.datePaiement')" name="datePaiement" required>
        <UInput v-model="state.datePaiement" type="date" class="w-full" />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('finances.paiement.moyenPaiement')" name="moyenPaiement" required>
        <USelect v-model="state.moyenPaiement" :items="moyenOptions" value-key="value" class="w-full" />
      </UFormField>

      <UFormField :label="t('finances.paiement.statutPaiement')" name="statutPaiement" required>
        <USelect v-model="state.statutPaiement" :items="statutOptions" value-key="value" class="w-full" />
      </UFormField>
    </div>

    <UFormField v-if="isMobileMoney" :label="t('finances.paiement.referenceMobileMoney')" name="referenceMobileMoney">
      <UInput v-model="state.referenceMobileMoney" :placeholder="t('finances.paiement.referencePlaceholder')" class="w-full" />
    </UFormField>

    <div class="flex justify-end gap-3 pt-4">
      <UButton variant="outline" @click="emit('cancel')">
        {{ $t('actions.cancel') }}
      </UButton>
      <UButton type="submit" color="primary" :loading="saving">
        {{ isEdit ? $t('actions.save') : $t('actions.create') }}
      </UButton>
    </div>
  </UForm>
</template>
