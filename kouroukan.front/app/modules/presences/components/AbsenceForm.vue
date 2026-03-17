<script setup lang="ts">
import { z } from 'zod'
import type { Absence, CreateAbsencePayload, UpdateAbsencePayload } from '../types/absence.types'
import { useAbsence } from '../composables/useAbsence'

const props = defineProps<{
  entity?: Absence | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateAbsencePayload | UpdateAbsencePayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const { typeOptions } = useAbsence()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  eleveId: z.number({ required_error: t('validation.required') }).int().positive(),
  appelId: z.number().int().positive().nullable().optional(),
  dateAbsence: z.string().min(1, t('validation.required')),
  heureDebut: z.string().nullable().optional(),
  heureFin: z.string().nullable().optional(),
  estJustifiee: z.boolean(),
  motifJustification: z.string().nullable().optional(),
  pieceJointeUrl: z.string().nullable().optional(),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  eleveId: props.entity?.eleveId ?? undefined,
  appelId: props.entity?.appelId ?? null,
  dateAbsence: props.entity?.dateAbsence ?? new Date().toISOString().split('T')[0],
  heureDebut: props.entity?.heureDebut ?? null,
  heureFin: props.entity?.heureFin ?? null,
  estJustifiee: props.entity?.estJustifiee ?? false,
  motifJustification: props.entity?.motifJustification ?? null,
  pieceJointeUrl: props.entity?.pieceJointeUrl ?? null,
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateAbsencePayload)
    }
    else {
      emit('submit', state as CreateAbsencePayload)
    }
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <UForm :schema="schema" :state="state" class="space-y-4" @submit="onSubmit">
    <UFormField :label="t('presences.absence.typeId')" name="typeId" required>
      <USelect v-model="state.typeId" :items="typeOptions" value-key="value" :placeholder="t('presences.absence.typePlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('presences.absence.eleveId')" name="eleveId" required>
      <UInput v-model.number="state.eleveId" type="number" :placeholder="t('presences.absence.elevePlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('presences.absence.appelId')" name="appelId">
      <UInput v-model.number="state.appelId" type="number" :placeholder="t('presences.absence.appelPlaceholder')" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-3">
      <UFormField :label="t('presences.absence.dateAbsence')" name="dateAbsence" required>
        <UInput v-model="state.dateAbsence" type="date" class="w-full" />
      </UFormField>

      <UFormField :label="t('presences.absence.heureDebut')" name="heureDebut">
        <UInput v-model="state.heureDebut" type="time" class="w-full" />
      </UFormField>

      <UFormField :label="t('presences.absence.heureFin')" name="heureFin">
        <UInput v-model="state.heureFin" type="time" class="w-full" />
      </UFormField>
    </div>

    <UFormField :label="t('presences.absence.estJustifiee')" name="estJustifiee">
      <UToggle v-model="state.estJustifiee" />
    </UFormField>

    <UFormField v-if="state.estJustifiee" :label="t('presences.absence.motifJustification')" name="motifJustification">
      <UTextarea v-model="state.motifJustification" :placeholder="t('presences.absence.motifPlaceholder')" class="w-full" />
    </UFormField>

    <UFormField v-if="state.estJustifiee" :label="t('presences.absence.pieceJointeUrl')" name="pieceJointeUrl">
      <UInput v-model="state.pieceJointeUrl" :placeholder="t('presences.absence.pieceJointePlaceholder')" class="w-full" />
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
