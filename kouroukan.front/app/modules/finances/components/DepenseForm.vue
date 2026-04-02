<script setup lang="ts">
import { z } from 'zod'
import type { Depense, CreateDepensePayload, UpdateDepensePayload } from '../types/depense.types'
import { STATUTS_DEPENSE, CATEGORIES_DEPENSE } from '../types/depense.types'
import { useDepense } from '../composables/useDepense'

const props = defineProps<{
  entity?: Depense | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateDepensePayload | UpdateDepensePayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const { typeOptions } = useDepense()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  montant: z.number({ required_error: t('validation.required') }).min(0),
  motifDepense: z.string().min(3, t('validation.required')),
  categorie: z.string().min(1, t('validation.required')),
  beneficiaireNom: z.string().min(3, t('validation.required')),
  beneficiaireTelephone: z.string().optional().nullable().or(z.literal('')),
  beneficiaireNIF: z.string().optional().nullable().or(z.literal('')),
  statutDepense: z.string().min(1, t('validation.required')),
  dateDemande: z.string().min(1, t('validation.required')),
  pieceJointeUrl: z.string().optional().nullable().or(z.literal('')),
  numeroJustificatif: z.string().min(1, t('validation.required')),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  montant: props.entity?.montant ?? 0,
  motifDepense: props.entity?.motifDepense ?? '',
  categorie: props.entity?.categorie ?? '',
  beneficiaireNom: props.entity?.beneficiaireNom ?? '',
  beneficiaireTelephone: props.entity?.beneficiaireTelephone ?? '',
  beneficiaireNIF: props.entity?.beneficiaireNIF ?? '',
  statutDepense: props.entity?.statutDepense ?? 'Demande',
  dateDemande: props.entity?.dateDemande ?? new Date().toISOString().split('T')[0],
  pieceJointeUrl: props.entity?.pieceJointeUrl ?? '',
  numeroJustificatif: props.entity?.numeroJustificatif ?? '',
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

const statutOptions = STATUTS_DEPENSE.map(s => ({
  label: t(`finances.depense.statut.${s}`),
  value: s,
}))

const categorieOptions = CATEGORIES_DEPENSE.map(c => ({
  label: t(`finances.depense.categorie.${c}`),
  value: c,
}))

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateDepensePayload)
    }
    else {
      emit('submit', state as CreateDepensePayload)
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
      :label="t('finances.depense.typeId')"
      name="typeId"
      required
    >
      <USelect
        v-model="state.typeId"
        :items="typeOptions"
        value-key="value"
        :placeholder="t('finances.depense.typePlaceholder')"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('finances.depense.numeroJustificatif')"
      name="numeroJustificatif"
      required
    >
      <UInput
        v-model="state.numeroJustificatif"
        :placeholder="t('finances.depense.numeroJustificatifPlaceholder')"
        class="w-full"
      />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('finances.depense.montant')"
        name="montant"
        required
      >
        <UInput
          v-model.number="state.montant"
          type="number"
          min="0"
          step="1"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('finances.depense.categorie')"
        name="categorie"
        required
      >
        <USelect
          v-model="state.categorie"
          :items="categorieOptions"
          value-key="value"
          class="w-full"
        />
      </UFormField>
    </div>

    <UFormField
      :label="t('finances.depense.motifDepense')"
      name="motifDepense"
      required
    >
      <UTextarea
        v-model="state.motifDepense"
        :placeholder="t('finances.depense.motifPlaceholder')"
        class="w-full"
      />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('finances.depense.beneficiaireNom')"
        name="beneficiaireNom"
        required
      >
        <UInput
          v-model="state.beneficiaireNom"
          :placeholder="t('finances.depense.beneficiaireNomPlaceholder')"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('finances.depense.beneficiaireTelephone')"
        name="beneficiaireTelephone"
      >
        <PhoneInput v-model="state.beneficiaireTelephone" />
      </UFormField>
    </div>

    <UFormField
      :label="t('finances.depense.beneficiaireNIF')"
      name="beneficiaireNIF"
    >
      <UInput
        v-model="state.beneficiaireNIF"
        :placeholder="t('finances.depense.nifPlaceholder')"
        class="w-full"
      />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('finances.depense.dateDemande')"
        name="dateDemande"
        required
      >
        <UInput
          v-model="state.dateDemande"
          type="date"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('finances.depense.statutDepense')"
        name="statutDepense"
        required
      >
        <USelect
          v-model="state.statutDepense"
          :items="statutOptions"
          value-key="value"
          class="w-full"
        />
      </UFormField>
    </div>

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
