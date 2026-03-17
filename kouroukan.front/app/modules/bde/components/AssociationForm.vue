<script setup lang="ts">
import { z } from 'zod'
import type { Association, CreateAssociationPayload, UpdateAssociationPayload } from '../types/association.types'
import { STATUTS_ASSOCIATION } from '../types/association.types'
import { useAssociation } from '../composables/useAssociation'

const props = defineProps<{
  entity?: Association | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateAssociationPayload | UpdateAssociationPayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const { typeOptions } = useAssociation()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  name: z.string().min(3, t('validation.required')),
  description: z.string().optional().nullable().or(z.literal('')),
  sigle: z.string().optional().nullable().or(z.literal('')),
  anneeScolaire: z.string().min(1, t('validation.required')),
  statut: z.string().min(1, t('validation.required')),
  budgetAnnuel: z.number({ required_error: t('validation.required') }).min(0),
  superviseurId: z.number().optional().nullable(),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  name: props.entity?.name ?? '',
  description: props.entity?.description ?? '',
  sigle: props.entity?.sigle ?? '',
  anneeScolaire: props.entity?.anneeScolaire ?? '',
  statut: props.entity?.statut ?? 'Active',
  budgetAnnuel: props.entity?.budgetAnnuel ?? 0,
  superviseurId: props.entity?.superviseurId ?? undefined,
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

const statutOptions = STATUTS_ASSOCIATION.map(s => ({
  label: t(`bde.association.statut.${s}`),
  value: s,
}))

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateAssociationPayload)
    }
    else {
      emit('submit', state as CreateAssociationPayload)
    }
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <UForm :schema="schema" :state="state" class="space-y-4" @submit="onSubmit">
    <UFormField :label="t('bde.association.typeId')" name="typeId" required>
      <USelect v-model="state.typeId" :items="typeOptions" value-key="value" :placeholder="t('bde.association.typePlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('bde.association.name')" name="name" required>
      <UInput v-model="state.name" :placeholder="t('bde.association.namePlaceholder')" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('bde.association.sigle')" name="sigle">
        <UInput v-model="state.sigle" :placeholder="t('bde.association.siglePlaceholder')" class="w-full" />
      </UFormField>

      <UFormField :label="t('bde.association.anneeScolaire')" name="anneeScolaire" required>
        <UInput v-model="state.anneeScolaire" :placeholder="t('bde.association.anneeScolairePlaceholder')" class="w-full" />
      </UFormField>
    </div>

    <UFormField :label="t('bde.association.description')" name="description">
      <UTextarea v-model="state.description" :placeholder="t('bde.association.descriptionPlaceholder')" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('bde.association.budgetAnnuel')" name="budgetAnnuel" required>
        <UInput v-model.number="state.budgetAnnuel" type="number" min="0" step="1" class="w-full" />
      </UFormField>

      <UFormField :label="t('bde.association.statut_label')" name="statut" required>
        <USelect v-model="state.statut" :items="statutOptions" value-key="value" class="w-full" />
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
