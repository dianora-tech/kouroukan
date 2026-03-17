<script setup lang="ts">
import { z } from 'zod'
import type { DepenseBDE, CreateDepenseBDEPayload, UpdateDepenseBDEPayload } from '../types/depense-bde.types'
import { STATUTS_VALIDATION_BDE, CATEGORIES_DEPENSE_BDE } from '../types/depense-bde.types'
import { useDepenseBDE } from '../composables/useDepenseBDE'
import { useAssociation } from '../composables/useAssociation'

const props = defineProps<{
  entity?: DepenseBDE | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateDepenseBDEPayload | UpdateDepenseBDEPayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const { typeOptions } = useDepenseBDE()
const { items: associations } = useAssociation()

const isEdit = computed(() => !!props.entity?.id)

const associationOptions = computed(() =>
  associations.value.map(a => ({ label: a.name, value: a.id })),
)

const schema = z.object({
  name: z.string().min(3, t('validation.required')),
  description: z.string().optional().nullable().or(z.literal('')),
  associationId: z.number({ required_error: t('validation.required') }).int().positive(),
  montant: z.number({ required_error: t('validation.required') }).min(0),
  motif: z.string().min(3, t('validation.required')),
  categorie: z.string().min(1, t('validation.required')),
  statutValidation: z.string().min(1, t('validation.required')),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  name: props.entity?.name ?? '',
  description: props.entity?.description ?? '',
  associationId: props.entity?.associationId ?? undefined,
  montant: props.entity?.montant ?? 0,
  motif: props.entity?.motif ?? '',
  categorie: props.entity?.categorie ?? '',
  statutValidation: props.entity?.statutValidation ?? 'Demandee',
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

const statutOptions = STATUTS_VALIDATION_BDE.map(s => ({
  label: t(`bde.depenseBde.statut.${s}`),
  value: s,
}))

const categorieOptions = CATEGORIES_DEPENSE_BDE.map(c => ({
  label: t(`bde.depenseBde.categorie.${c}`),
  value: c,
}))

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateDepenseBDEPayload)
    }
    else {
      emit('submit', state as CreateDepenseBDEPayload)
    }
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <UForm :schema="schema" :state="state" class="space-y-4" @submit="onSubmit">
    <UFormField :label="t('bde.depenseBde.typeId')" name="typeId" required>
      <USelect v-model="state.typeId" :items="typeOptions" value-key="value" :placeholder="t('bde.depenseBde.typePlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('bde.depenseBde.associationId')" name="associationId" required>
      <USelect v-model="state.associationId" :items="associationOptions" value-key="value" :placeholder="t('bde.depenseBde.associationPlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('bde.depenseBde.name')" name="name" required>
      <UInput v-model="state.name" :placeholder="t('bde.depenseBde.namePlaceholder')" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('bde.depenseBde.montant')" name="montant" required>
        <UInput v-model.number="state.montant" type="number" min="0" step="1" class="w-full" />
      </UFormField>

      <UFormField :label="t('bde.depenseBde.categorie_label')" name="categorie" required>
        <USelect v-model="state.categorie" :items="categorieOptions" value-key="value" class="w-full" />
      </UFormField>
    </div>

    <UFormField :label="t('bde.depenseBde.motif')" name="motif" required>
      <UTextarea v-model="state.motif" :placeholder="t('bde.depenseBde.motifPlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('bde.depenseBde.statutValidation')" name="statutValidation" required>
      <USelect v-model="state.statutValidation" :items="statutOptions" value-key="value" class="w-full" />
    </UFormField>

    <UFormField :label="t('bde.depenseBde.description')" name="description">
      <UTextarea v-model="state.description" :placeholder="t('bde.depenseBde.descriptionPlaceholder')" class="w-full" />
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
