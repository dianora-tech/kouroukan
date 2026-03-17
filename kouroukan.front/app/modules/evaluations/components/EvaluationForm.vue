<script setup lang="ts">
import { z } from 'zod'
import type { Evaluation, CreateEvaluationPayload, UpdateEvaluationPayload } from '../types/evaluation.types'
import { TRIMESTRES } from '../types/evaluation.types'
import { useEvaluation } from '../composables/useEvaluation'

const props = defineProps<{
  entity?: Evaluation | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateEvaluationPayload | UpdateEvaluationPayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const { typeOptions } = useEvaluation()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  matiereId: z.number({ required_error: t('validation.required') }).int().positive(),
  classeId: z.number({ required_error: t('validation.required') }).int().positive(),
  enseignantId: z.number({ required_error: t('validation.required') }).int().positive(),
  dateEvaluation: z.string().min(1, t('validation.required')),
  coefficient: z.number({ required_error: t('validation.required') }).min(0.01),
  noteMaximale: z.number({ required_error: t('validation.required') }).min(1),
  trimestre: z.number({ required_error: t('validation.required') }).int().min(1).max(3),
  anneeScolaireId: z.number({ required_error: t('validation.required') }).int().positive(),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  matiereId: props.entity?.matiereId ?? undefined,
  classeId: props.entity?.classeId ?? undefined,
  enseignantId: props.entity?.enseignantId ?? undefined,
  dateEvaluation: props.entity?.dateEvaluation ?? new Date().toISOString().split('T')[0],
  coefficient: props.entity?.coefficient ?? 1,
  noteMaximale: props.entity?.noteMaximale ?? 20,
  trimestre: props.entity?.trimestre ?? 1,
  anneeScolaireId: props.entity?.anneeScolaireId ?? undefined,
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

const trimestreOptions = TRIMESTRES.map(tr => ({
  label: t(`evaluations.evaluation.trimestre.${tr}`),
  value: tr,
}))

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateEvaluationPayload)
    }
    else {
      emit('submit', state as CreateEvaluationPayload)
    }
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <UForm :schema="schema" :state="state" class="space-y-4" @submit="onSubmit">
    <UFormField :label="t('evaluations.evaluation.typeId')" name="typeId" required>
      <USelect v-model="state.typeId" :items="typeOptions" value-key="value" :placeholder="t('evaluations.evaluation.typePlaceholder')" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('evaluations.evaluation.dateEvaluation')" name="dateEvaluation" required>
        <UInput v-model="state.dateEvaluation" type="date" class="w-full" />
      </UFormField>

      <UFormField :label="t('evaluations.evaluation.trimestre')" name="trimestre" required>
        <USelect v-model="state.trimestre" :items="trimestreOptions" value-key="value" class="w-full" />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('evaluations.evaluation.coefficient')" name="coefficient" required>
        <UInput v-model.number="state.coefficient" type="number" min="0.01" step="0.01" class="w-full" />
      </UFormField>

      <UFormField :label="t('evaluations.evaluation.noteMaximale')" name="noteMaximale" required>
        <UInput v-model.number="state.noteMaximale" type="number" min="1" step="0.5" class="w-full" />
      </UFormField>
    </div>

    <UFormField :label="t('evaluations.evaluation.matiereId')" name="matiereId" required>
      <UInput v-model.number="state.matiereId" type="number" :placeholder="t('evaluations.evaluation.matierePlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('evaluations.evaluation.classeId')" name="classeId" required>
      <UInput v-model.number="state.classeId" type="number" :placeholder="t('evaluations.evaluation.classePlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('evaluations.evaluation.enseignantId')" name="enseignantId" required>
      <UInput v-model.number="state.enseignantId" type="number" :placeholder="t('evaluations.evaluation.enseignantPlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('evaluations.evaluation.anneeScolaireId')" name="anneeScolaireId" required>
      <UInput v-model.number="state.anneeScolaireId" type="number" :placeholder="t('evaluations.evaluation.anneeScolairePlaceholder')" class="w-full" />
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
