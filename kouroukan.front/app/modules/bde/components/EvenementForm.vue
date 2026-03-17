<script setup lang="ts">
import { z } from 'zod'
import type { Evenement, CreateEvenementPayload, UpdateEvenementPayload } from '../types/evenement.types'
import { STATUTS_EVENEMENT } from '../types/evenement.types'
import { useEvenement } from '../composables/useEvenement'
import { useAssociation } from '../composables/useAssociation'

const props = defineProps<{
  entity?: Evenement | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateEvenementPayload | UpdateEvenementPayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const { typeOptions } = useEvenement()
const { items: associations } = useAssociation()

const isEdit = computed(() => !!props.entity?.id)

const associationOptions = computed(() =>
  associations.value.map(a => ({ label: a.name, value: a.id })),
)

const schema = z.object({
  name: z.string().min(3, t('validation.required')),
  description: z.string().optional().nullable().or(z.literal('')),
  associationId: z.number({ required_error: t('validation.required') }).int().positive(),
  dateEvenement: z.string().min(1, t('validation.required')),
  lieu: z.string().min(1, t('validation.required')),
  capacite: z.number().int().min(0).optional().nullable(),
  tarifEntree: z.number().min(0).optional().nullable(),
  nombreInscrits: z.number({ required_error: t('validation.required') }).int().min(0),
  statutEvenement: z.string().min(1, t('validation.required')),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  name: props.entity?.name ?? '',
  description: props.entity?.description ?? '',
  associationId: props.entity?.associationId ?? undefined,
  dateEvenement: props.entity?.dateEvenement ? props.entity.dateEvenement.split('T')[0] : '',
  lieu: props.entity?.lieu ?? '',
  capacite: props.entity?.capacite ?? undefined,
  tarifEntree: props.entity?.tarifEntree ?? undefined,
  nombreInscrits: props.entity?.nombreInscrits ?? 0,
  statutEvenement: props.entity?.statutEvenement ?? 'Planifie',
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

const statutOptions = STATUTS_EVENEMENT.map(s => ({
  label: t(`bde.evenement.statut.${s}`),
  value: s,
}))

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateEvenementPayload)
    }
    else {
      emit('submit', state as CreateEvenementPayload)
    }
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <UForm :schema="schema" :state="state" class="space-y-4" @submit="onSubmit">
    <UFormField :label="t('bde.evenement.typeId')" name="typeId" required>
      <USelect v-model="state.typeId" :items="typeOptions" value-key="value" :placeholder="t('bde.evenement.typePlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('bde.evenement.associationId')" name="associationId" required>
      <USelect v-model="state.associationId" :items="associationOptions" value-key="value" :placeholder="t('bde.evenement.associationPlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('bde.evenement.name')" name="name" required>
      <UInput v-model="state.name" :placeholder="t('bde.evenement.namePlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('bde.evenement.description')" name="description">
      <UTextarea v-model="state.description" :placeholder="t('bde.evenement.descriptionPlaceholder')" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('bde.evenement.dateEvenement')" name="dateEvenement" required>
        <UInput v-model="state.dateEvenement" type="date" class="w-full" />
      </UFormField>

      <UFormField :label="t('bde.evenement.lieu')" name="lieu" required>
        <UInput v-model="state.lieu" :placeholder="t('bde.evenement.lieuPlaceholder')" class="w-full" />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-3">
      <UFormField :label="t('bde.evenement.capacite')" name="capacite">
        <UInput v-model.number="state.capacite" type="number" min="0" class="w-full" />
      </UFormField>

      <UFormField :label="t('bde.evenement.tarifEntree')" name="tarifEntree">
        <UInput v-model.number="state.tarifEntree" type="number" min="0" step="1" class="w-full" />
      </UFormField>

      <UFormField :label="t('bde.evenement.nombreInscrits')" name="nombreInscrits" required>
        <UInput v-model.number="state.nombreInscrits" type="number" min="0" class="w-full" />
      </UFormField>
    </div>

    <UFormField :label="t('bde.evenement.statutEvenement')" name="statutEvenement" required>
      <USelect v-model="state.statutEvenement" :items="statutOptions" value-key="value" class="w-full" />
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
