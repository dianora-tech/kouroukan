<script setup lang="ts">
import { z } from 'zod'
import type { Inscription, CreateInscriptionPayload, UpdateInscriptionPayload } from '../types/inscription.types'
import { STATUTS_INSCRIPTION, TYPES_ETABLISSEMENT, SERIES_BAC } from '../types/inscription.types'
import { useInscription } from '../composables/useInscription'

const props = defineProps<{
  entity?: Inscription | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateInscriptionPayload | UpdateInscriptionPayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const { typeOptions } = useInscription()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  eleveId: z.number({ required_error: t('validation.required') }).int().positive(),
  classeId: z.number({ required_error: t('validation.required') }).int().positive(),
  anneeScolaireId: z.number({ required_error: t('validation.required') }).int().positive(),
  dateInscription: z.string().min(1, t('validation.required')),
  montantInscription: z.number({ required_error: t('validation.required') }).min(0),
  estPaye: z.boolean(),
  estRedoublant: z.boolean(),
  typeEtablissement: z.string().optional().or(z.literal('')),
  serieBac: z.string().optional().or(z.literal('')),
  statutInscription: z.string().min(1, t('validation.required')),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  eleveId: props.entity?.eleveId ?? undefined,
  classeId: props.entity?.classeId ?? undefined,
  anneeScolaireId: props.entity?.anneeScolaireId ?? undefined,
  dateInscription: props.entity?.dateInscription ?? new Date().toISOString().split('T')[0],
  montantInscription: props.entity?.montantInscription ?? 0,
  estPaye: props.entity?.estPaye ?? false,
  estRedoublant: props.entity?.estRedoublant ?? false,
  typeEtablissement: props.entity?.typeEtablissement ?? '',
  serieBac: props.entity?.serieBac ?? '',
  statutInscription: props.entity?.statutInscription ?? 'EnAttente',
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

const statutOptions = STATUTS_INSCRIPTION.map(s => ({
  label: t(`inscriptions.inscription.statut.${s}`),
  value: s,
}))

const typeEtablissementOptions = [
  { label: t('inscriptions.inscription.filters.allTypes'), value: '' },
  ...TYPES_ETABLISSEMENT.map(te => ({
    label: t(`inscriptions.inscription.typeEtab.${te}`),
    value: te,
  })),
]

const serieBacOptions = [
  { label: t('inscriptions.inscription.filters.allSeries'), value: '' },
  ...SERIES_BAC.map(s => ({
    label: s,
    value: s,
  })),
]

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateInscriptionPayload)
    }
    else {
      emit('submit', state as CreateInscriptionPayload)
    }
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <UForm :schema="schema" :state="state" class="space-y-4" @submit="onSubmit">
    <UFormField :label="t('inscriptions.inscription.typeId')" name="typeId" required>
      <USelect v-model="state.typeId" :items="typeOptions" value-key="value" :placeholder="t('inscriptions.inscription.typePlaceholder')" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('inscriptions.inscription.dateInscription')" name="dateInscription" required>
        <UInput v-model="state.dateInscription" type="date" class="w-full" />
      </UFormField>

      <UFormField :label="t('inscriptions.inscription.statutInscription')" name="statutInscription" required>
        <USelect v-model="state.statutInscription" :items="statutOptions" value-key="value" class="w-full" />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('inscriptions.inscription.montantInscription')" name="montantInscription" required>
        <UInput v-model.number="state.montantInscription" type="number" min="0" step="0.01" class="w-full" />
      </UFormField>

      <UFormField :label="t('inscriptions.inscription.typeEtablissement')" name="typeEtablissement">
        <USelect v-model="state.typeEtablissement" :items="typeEtablissementOptions" value-key="value" class="w-full" />
      </UFormField>
    </div>

    <UFormField :label="t('inscriptions.inscription.serieBac')" name="serieBac">
      <USelect v-model="state.serieBac" :items="serieBacOptions" value-key="value" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('inscriptions.inscription.estPaye')" name="estPaye">
        <UToggle v-model="state.estPaye" />
      </UFormField>

      <UFormField :label="t('inscriptions.inscription.estRedoublant')" name="estRedoublant">
        <UToggle v-model="state.estRedoublant" />
      </UFormField>
    </div>

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
