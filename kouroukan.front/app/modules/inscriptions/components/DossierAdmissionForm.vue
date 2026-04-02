<script setup lang="ts">
import { z } from 'zod'
import type { DossierAdmission, CreateDossierAdmissionPayload, UpdateDossierAdmissionPayload } from '../types/dossier-admission.types'
import { STATUTS_DOSSIER, ETAPES_WORKFLOW } from '../types/dossier-admission.types'
import { useDossierAdmission } from '../composables/useDossierAdmission'

const props = defineProps<{
  entity?: DossierAdmission | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateDossierAdmissionPayload | UpdateDossierAdmissionPayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const { typeOptions } = useDossierAdmission()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  eleveId: z.number({ required_error: t('validation.required') }).int().positive(),
  anneeScolaireId: z.number({ required_error: t('validation.required') }).int().positive(),
  statutDossier: z.string().min(1, t('validation.required')),
  etapeActuelle: z.string().min(1, t('validation.required')),
  dateDemande: z.string().min(1, t('validation.required')),
  dateDecision: z.string().optional().or(z.literal('')),
  motifRefus: z.string().max(500).optional().or(z.literal('')),
  scoringInterne: z.number().min(0).max(100).optional().nullable(),
  commentaires: z.string().optional().or(z.literal('')),
  responsableAdmissionId: z.number().int().positive().optional().nullable(),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  eleveId: props.entity?.eleveId ?? undefined,
  anneeScolaireId: props.entity?.anneeScolaireId ?? undefined,
  statutDossier: props.entity?.statutDossier ?? 'Prospect',
  etapeActuelle: props.entity?.etapeActuelle ?? 'DepotDossier',
  dateDemande: props.entity?.dateDemande ?? new Date().toISOString().split('T')[0],
  dateDecision: props.entity?.dateDecision ?? '',
  motifRefus: props.entity?.motifRefus ?? '',
  scoringInterne: props.entity?.scoringInterne ?? undefined,
  commentaires: props.entity?.commentaires ?? '',
  responsableAdmissionId: props.entity?.responsableAdmissionId ?? undefined,
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

const statutOptions = STATUTS_DOSSIER.map(s => ({
  label: t(`inscriptions.dossierAdmission.statut.${s}`),
  value: s,
}))

const etapeOptions = ETAPES_WORKFLOW.map(e => ({
  label: t(`inscriptions.dossierAdmission.etape.${e}`),
  value: e,
}))

const showMotifRefus = computed(() => state.statutDossier === 'Refuse')

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateDossierAdmissionPayload)
    }
    else {
      emit('submit', state as CreateDossierAdmissionPayload)
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
      :label="t('inscriptions.dossierAdmission.typeId')"
      name="typeId"
      required
    >
      <USelect
        v-model="state.typeId"
        :items="typeOptions"
        value-key="value"
        :placeholder="t('inscriptions.dossierAdmission.typePlaceholder')"
        class="w-full"
      />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('inscriptions.dossierAdmission.statutDossier')"
        name="statutDossier"
        required
      >
        <USelect
          v-model="state.statutDossier"
          :items="statutOptions"
          value-key="value"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('inscriptions.dossierAdmission.etapeActuelle')"
        name="etapeActuelle"
        required
      >
        <USelect
          v-model="state.etapeActuelle"
          :items="etapeOptions"
          value-key="value"
          class="w-full"
        />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('inscriptions.dossierAdmission.dateDemande')"
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
        :label="t('inscriptions.dossierAdmission.dateDecision')"
        name="dateDecision"
      >
        <UInput
          v-model="state.dateDecision"
          type="date"
          class="w-full"
        />
      </UFormField>
    </div>

    <UFormField
      :label="t('inscriptions.dossierAdmission.scoringInterne')"
      name="scoringInterne"
    >
      <UInput
        v-model.number="state.scoringInterne"
        type="number"
        min="0"
        max="100"
        step="0.01"
        class="w-full"
      />
    </UFormField>

    <UFormField
      v-if="showMotifRefus"
      :label="t('inscriptions.dossierAdmission.motifRefus')"
      name="motifRefus"
    >
      <UTextarea
        v-model="state.motifRefus"
        :placeholder="t('inscriptions.dossierAdmission.motifRefusPlaceholder')"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('inscriptions.dossierAdmission.commentaires')"
      name="commentaires"
    >
      <UTextarea
        v-model="state.commentaires"
        :placeholder="t('inscriptions.dossierAdmission.commentairesPlaceholder')"
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
