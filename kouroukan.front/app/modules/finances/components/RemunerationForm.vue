<script setup lang="ts">
import { z } from 'zod'
import type { RemunerationEnseignant, CreateRemunerationPayload, UpdateRemunerationPayload } from '../types/remuneration.types'
import { STATUTS_REMUNERATION, MODES_REMUNERATION, MOIS_OPTIONS } from '../types/remuneration.types'

const props = defineProps<{
  entity?: RemunerationEnseignant | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateRemunerationPayload | UpdateRemunerationPayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()

const isEdit = computed(() => !!props.entity?.id)

const currentYear = new Date().getFullYear()
const currentMonth = new Date().getMonth() + 1

const schema = z.object({
  enseignantId: z.number({ required_error: t('validation.required') }).int().positive(),
  mois: z.number({ required_error: t('validation.required') }).int().min(1).max(12),
  annee: z.number({ required_error: t('validation.required') }).int().min(2020).max(2050),
  modeRemuneration: z.string().min(1, t('validation.required')),
  montantForfait: z.number().min(0).optional().nullable(),
  nombreHeures: z.number().min(0).optional().nullable(),
  tauxHoraire: z.number().min(0).optional().nullable(),
  montantTotal: z.number({ required_error: t('validation.required') }).min(0),
  statutPaiement: z.string().min(1, t('validation.required')),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  enseignantId: props.entity?.enseignantId ?? undefined,
  mois: props.entity?.mois ?? currentMonth,
  annee: props.entity?.annee ?? currentYear,
  modeRemuneration: props.entity?.modeRemuneration ?? '',
  montantForfait: props.entity?.montantForfait ?? undefined,
  nombreHeures: props.entity?.nombreHeures ?? undefined,
  tauxHoraire: props.entity?.tauxHoraire ?? undefined,
  montantTotal: props.entity?.montantTotal ?? 0,
  statutPaiement: props.entity?.statutPaiement ?? 'EnAttente',
})

const saving = ref(false)

const showForfait = computed(() =>
  state.modeRemuneration === 'Forfait' || state.modeRemuneration === 'Mixte',
)

const showHeures = computed(() =>
  state.modeRemuneration === 'Heures' || state.modeRemuneration === 'Mixte',
)

const statutOptions = STATUTS_REMUNERATION.map(s => ({
  label: t(`finances.remuneration.statut.${s}`),
  value: s,
}))

const modeOptions = MODES_REMUNERATION.map(m => ({
  label: t(`finances.remuneration.mode.${m}`),
  value: m,
}))

const moisOptions = MOIS_OPTIONS.map(m => ({
  label: t(`finances.remuneration.mois.${m.value}`),
  value: m.value,
}))

const anneeOptions = Array.from({ length: 6 }, (_, i) => ({
  label: String(currentYear - 1 + i),
  value: currentYear - 1 + i,
}))

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateRemunerationPayload)
    }
    else {
      emit('submit', state as CreateRemunerationPayload)
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
    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('finances.remuneration.mois_label')"
        name="mois"
        required
      >
        <USelect
          v-model="state.mois"
          :items="moisOptions"
          value-key="value"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('finances.remuneration.annee')"
        name="annee"
        required
      >
        <USelect
          v-model="state.annee"
          :items="anneeOptions"
          value-key="value"
          class="w-full"
        />
      </UFormField>
    </div>

    <UFormField
      :label="t('finances.remuneration.modeRemuneration')"
      name="modeRemuneration"
      required
    >
      <USelect
        v-model="state.modeRemuneration"
        :items="modeOptions"
        value-key="value"
        class="w-full"
      />
    </UFormField>

    <div
      v-if="showForfait"
      class="grid grid-cols-1 gap-4 sm:grid-cols-1"
    >
      <UFormField
        :label="t('finances.remuneration.montantForfait')"
        name="montantForfait"
      >
        <UInput
          v-model.number="state.montantForfait"
          type="number"
          min="0"
          step="1"
          class="w-full"
        />
      </UFormField>
    </div>

    <div
      v-if="showHeures"
      class="grid grid-cols-1 gap-4 sm:grid-cols-2"
    >
      <UFormField
        :label="t('finances.remuneration.nombreHeures')"
        name="nombreHeures"
      >
        <UInput
          v-model.number="state.nombreHeures"
          type="number"
          min="0"
          step="0.5"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('finances.remuneration.tauxHoraire')"
        name="tauxHoraire"
      >
        <UInput
          v-model.number="state.tauxHoraire"
          type="number"
          min="0"
          step="1"
          class="w-full"
        />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('finances.remuneration.montantTotal')"
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
        :label="t('finances.remuneration.statutPaiement')"
        name="statutPaiement"
        required
      >
        <USelect
          v-model="state.statutPaiement"
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
