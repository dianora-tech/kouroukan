<script setup lang="ts">
import { z } from 'zod'
import type { Enseignant, CreateEnseignantPayload, UpdateEnseignantPayload } from '../types/enseignant.types'
import { STATUTS_ENSEIGNANT, MODES_REMUNERATION } from '../types/enseignant.types'
import { useEnseignant } from '../composables/useEnseignant'

const props = defineProps<{
  entity?: Enseignant | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateEnseignantPayload | UpdateEnseignantPayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const { typeOptions } = useEnseignant()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  matricule: z.string().min(1, t('validation.required')),
  specialite: z.string().min(1, t('validation.required')),
  dateEmbauche: z.string().min(1, t('validation.required')),
  modeRemuneration: z.string().min(1, t('validation.required')),
  montantForfait: z.number().min(0).nullable().optional(),
  telephone: z.string().min(1, t('validation.required')),
  email: z.string().email(t('validation.email')).nullable().optional().or(z.literal('')),
  statutEnseignant: z.string().min(1, t('validation.required')),
  soldeCongesAnnuel: z.number({ required_error: t('validation.required') }).int().min(0),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  matricule: props.entity?.matricule ?? '',
  specialite: props.entity?.specialite ?? '',
  dateEmbauche: props.entity?.dateEmbauche ?? new Date().toISOString().split('T')[0],
  modeRemuneration: props.entity?.modeRemuneration ?? 'Forfait',
  montantForfait: props.entity?.montantForfait ?? undefined,
  telephone: props.entity?.telephone ?? '',
  email: props.entity?.email ?? '',
  statutEnseignant: props.entity?.statutEnseignant ?? 'Actif',
  soldeCongesAnnuel: props.entity?.soldeCongesAnnuel ?? 30,
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

const statutOptions = STATUTS_ENSEIGNANT.map(s => ({
  label: t(`personnel.enseignant.statut.${s}`),
  value: s,
}))

const modeRemunerationOptions = MODES_REMUNERATION.map(m => ({
  label: t(`personnel.enseignant.modeRemuneration.${m}`),
  value: m,
}))

const showMontantForfait = computed(() =>
  state.modeRemuneration === 'Forfait' || state.modeRemuneration === 'Mixte',
)

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateEnseignantPayload)
    }
    else {
      emit('submit', state as CreateEnseignantPayload)
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
      :label="t('personnel.enseignant.typeId')"
      name="typeId"
      required
    >
      <USelect
        v-model="state.typeId"
        :items="typeOptions"
        value-key="value"
        :placeholder="t('personnel.enseignant.typePlaceholder')"
        class="w-full"
      />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('personnel.enseignant.matricule')"
        name="matricule"
        required
      >
        <UInput
          v-model="state.matricule"
          :placeholder="t('personnel.enseignant.matriculePlaceholder')"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('personnel.enseignant.telephone')"
        name="telephone"
        required
      >
        <PhoneInput v-model="state.telephone" />
      </UFormField>
    </div>

    <UFormField
      :label="t('personnel.enseignant.specialite')"
      name="specialite"
      required
    >
      <UInput
        v-model="state.specialite"
        :placeholder="t('personnel.enseignant.specialitePlaceholder')"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('personnel.enseignant.email')"
      name="email"
    >
      <UInput
        v-model="state.email"
        type="email"
        :placeholder="t('personnel.enseignant.emailPlaceholder')"
        class="w-full"
      />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('personnel.enseignant.dateEmbauche')"
        name="dateEmbauche"
        required
      >
        <UInput
          v-model="state.dateEmbauche"
          type="date"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('personnel.enseignant.statutEnseignantLabel')"
        name="statutEnseignant"
        required
      >
        <USelect
          v-model="state.statutEnseignant"
          :items="statutOptions"
          value-key="value"
          class="w-full"
        />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('personnel.enseignant.modeRemunerationLabel')"
        name="modeRemuneration"
        required
      >
        <USelect
          v-model="state.modeRemuneration"
          :items="modeRemunerationOptions"
          value-key="value"
          class="w-full"
        />
      </UFormField>

      <UFormField
        v-if="showMontantForfait"
        :label="t('personnel.enseignant.montantForfait')"
        name="montantForfait"
      >
        <UInput
          v-model.number="state.montantForfait"
          type="number"
          min="0"
          step="1000"
          :placeholder="t('personnel.enseignant.montantForfaitPlaceholder')"
          class="w-full"
        />
      </UFormField>
    </div>

    <UFormField
      :label="t('personnel.enseignant.soldeCongesAnnuel')"
      name="soldeCongesAnnuel"
      required
    >
      <UInput
        v-model.number="state.soldeCongesAnnuel"
        type="number"
        min="0"
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
