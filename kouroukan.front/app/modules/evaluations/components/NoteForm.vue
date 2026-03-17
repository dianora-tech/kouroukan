<script setup lang="ts">
import { z } from 'zod'
import type { Note, CreateNotePayload, UpdateNotePayload } from '../types/note.types'

const props = defineProps<{
  entity?: Note | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateNotePayload | UpdateNotePayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  evaluationId: z.number({ required_error: t('validation.required') }).int().positive(),
  eleveId: z.number({ required_error: t('validation.required') }).int().positive(),
  valeur: z.number({ required_error: t('validation.required') }).min(0),
  commentaire: z.string().optional().or(z.literal('')),
  dateSaisie: z.string().min(1, t('validation.required')),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  evaluationId: props.entity?.evaluationId ?? undefined,
  eleveId: props.entity?.eleveId ?? undefined,
  valeur: props.entity?.valeur ?? 0,
  commentaire: props.entity?.commentaire ?? '',
  dateSaisie: props.entity?.dateSaisie?.split('T')[0] ?? new Date().toISOString().split('T')[0],
})

const saving = ref(false)

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateNotePayload)
    }
    else {
      emit('submit', state as CreateNotePayload)
    }
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <UForm :schema="schema" :state="state" class="space-y-4" @submit="onSubmit">
    <UFormField :label="t('evaluations.note.evaluationId')" name="evaluationId" required>
      <UInput v-model.number="state.evaluationId" type="number" :placeholder="t('evaluations.note.evaluationPlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('evaluations.note.eleveId')" name="eleveId" required>
      <UInput v-model.number="state.eleveId" type="number" :placeholder="t('evaluations.note.elevePlaceholder')" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('evaluations.note.valeur')" name="valeur" required>
        <UInput v-model.number="state.valeur" type="number" min="0" step="0.25" class="w-full" />
      </UFormField>

      <UFormField :label="t('evaluations.note.dateSaisie')" name="dateSaisie" required>
        <UInput v-model="state.dateSaisie" type="date" class="w-full" />
      </UFormField>
    </div>

    <UFormField :label="t('evaluations.note.commentaire')" name="commentaire">
      <UTextarea v-model="state.commentaire" :placeholder="t('evaluations.note.commentairePlaceholder')" class="w-full" />
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
