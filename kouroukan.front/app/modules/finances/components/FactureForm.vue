<script setup lang="ts">
import { z } from 'zod'
import type { Facture, CreateFacturePayload, UpdateFacturePayload } from '../types/facture.types'
import { STATUTS_FACTURE } from '../types/facture.types'
import { useFacture } from '../composables/useFacture'

const props = defineProps<{
  entity?: Facture | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateFacturePayload | UpdateFacturePayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const { typeOptions } = useFacture()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  eleveId: z.number({ required_error: t('validation.required') }).int().positive(),
  parentId: z.number().int().positive().optional().nullable(),
  anneeScolaireId: z.number({ required_error: t('validation.required') }).int().positive(),
  montantTotal: z.number({ required_error: t('validation.required') }).min(0),
  montantPaye: z.number().min(0).optional(),
  dateEmission: z.string().min(1, t('validation.required')),
  dateEcheance: z.string().min(1, t('validation.required')),
  statutFacture: z.string().min(1, t('validation.required')),
  numeroFacture: z.string().min(1, t('validation.required')),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  eleveId: props.entity?.eleveId ?? undefined,
  parentId: props.entity?.parentId ?? undefined,
  anneeScolaireId: props.entity?.anneeScolaireId ?? undefined,
  montantTotal: props.entity?.montantTotal ?? 0,
  montantPaye: props.entity?.montantPaye ?? 0,
  dateEmission: props.entity?.dateEmission ?? new Date().toISOString().split('T')[0],
  dateEcheance: props.entity?.dateEcheance ?? '',
  statutFacture: props.entity?.statutFacture ?? 'Emise',
  numeroFacture: props.entity?.numeroFacture ?? '',
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

const statutOptions = STATUTS_FACTURE.map(s => ({
  label: t(`finances.facture.statut.${s}`),
  value: s,
}))

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateFacturePayload)
    }
    else {
      emit('submit', state as CreateFacturePayload)
    }
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <UForm
    :schema="schema"
    :state="state"
    class="space-y-4"
    @submit="onSubmit"
  >
    <UFormField
      :label="t('finances.facture.typeId')"
      name="typeId"
      required
    >
      <USelect
        v-model="state.typeId"
        :items="typeOptions"
        value-key="value"
        :placeholder="t('finances.facture.typePlaceholder')"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('finances.facture.numeroFacture')"
      name="numeroFacture"
      required
    >
      <UInput
        v-model="state.numeroFacture"
        :placeholder="t('finances.facture.numeroFacturePlaceholder')"
        class="w-full"
      />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('finances.facture.dateEmission')"
        name="dateEmission"
        required
      >
        <UInput
          v-model="state.dateEmission"
          type="date"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('finances.facture.dateEcheance')"
        name="dateEcheance"
        required
      >
        <UInput
          v-model="state.dateEcheance"
          type="date"
          class="w-full"
        />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('finances.facture.montantTotal')"
        name="montantTotal"
        required
      >
        <UInput
          v-model.number="state.montantTotal"
          type="number"
          min="0"
          step="1"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('finances.facture.montantPaye')"
        name="montantPaye"
      >
        <UInput
          v-model.number="state.montantPaye"
          type="number"
          min="0"
          step="1"
          class="w-full"
        />
      </UFormField>
    </div>

    <UFormField
      :label="t('finances.facture.statutFacture')"
      name="statutFacture"
      required
    >
      <USelect
        v-model="state.statutFacture"
        :items="statutOptions"
        value-key="value"
        class="w-full"
      />
    </UFormField>

    <div class="flex justify-end gap-3 pt-4">
      <UButton
        variant="outline"
        @click="emit('cancel')"
      >
        {{ $t('actions.cancel') }}
      </UButton>
      <UButton
        type="submit"
        color="primary"
        :loading="saving"
      >
        {{ isEdit ? $t('actions.save') : $t('actions.create') }}
      </UButton>
    </div>
  </UForm>
</template>
