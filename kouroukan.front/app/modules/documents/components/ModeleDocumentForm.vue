<script setup lang="ts">
import { z } from 'zod'
import type { ModeleDocument, CreateModeleDocumentPayload, UpdateModeleDocumentPayload } from '../types/modele-document.types'
import { useModeleDocument } from '../composables/useModeleDocument'

const props = defineProps<{
  entity?: ModeleDocument | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateModeleDocumentPayload | UpdateModeleDocumentPayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const { typeOptions } = useModeleDocument()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  name: z.string().min(3, t('validation.required')),
  description: z.string().optional().nullable().or(z.literal('')),
  code: z.string().min(1, t('validation.required')),
  contenu: z.string().min(1, t('validation.required')),
  logoUrl: z.string().optional().nullable().or(z.literal('')),
  couleurPrimaire: z.string().optional().nullable().or(z.literal('')),
  textePiedPage: z.string().optional().nullable().or(z.literal('')),
  estActif: z.boolean(),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  name: props.entity?.name ?? '',
  description: props.entity?.description ?? '',
  code: props.entity?.code ?? '',
  contenu: props.entity?.contenu ?? '',
  logoUrl: props.entity?.logoUrl ?? '',
  couleurPrimaire: props.entity?.couleurPrimaire ?? '#16a34a',
  textePiedPage: props.entity?.textePiedPage ?? '',
  estActif: props.entity?.estActif ?? true,
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateModeleDocumentPayload)
    }
    else {
      emit('submit', state as CreateModeleDocumentPayload)
    }
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <UForm :schema="schema" :state="state" class="space-y-4" @submit="onSubmit">
    <UFormField :label="t('documents.modeleDocument.typeId')" name="typeId" required>
      <USelect v-model="state.typeId" :items="typeOptions" value-key="value" :placeholder="t('documents.modeleDocument.typePlaceholder')" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('documents.modeleDocument.name')" name="name" required>
        <UInput v-model="state.name" :placeholder="t('documents.modeleDocument.namePlaceholder')" class="w-full" />
      </UFormField>

      <UFormField :label="t('documents.modeleDocument.code')" name="code" required>
        <UInput v-model="state.code" :placeholder="t('documents.modeleDocument.codePlaceholder')" class="w-full" />
      </UFormField>
    </div>

    <UFormField :label="t('documents.modeleDocument.description')" name="description">
      <UTextarea v-model="state.description" :placeholder="t('documents.modeleDocument.descriptionPlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('documents.modeleDocument.contenu')" name="contenu" required>
      <UTextarea v-model="state.contenu" :placeholder="t('documents.modeleDocument.contenuPlaceholder')" :rows="8" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('documents.modeleDocument.logoUrl')" name="logoUrl">
        <UInput v-model="state.logoUrl" :placeholder="t('documents.modeleDocument.logoUrlPlaceholder')" class="w-full" />
      </UFormField>

      <UFormField :label="t('documents.modeleDocument.couleurPrimaire')" name="couleurPrimaire">
        <UInput v-model="state.couleurPrimaire" type="color" class="w-full" />
      </UFormField>
    </div>

    <UFormField :label="t('documents.modeleDocument.textePiedPage')" name="textePiedPage">
      <UInput v-model="state.textePiedPage" :placeholder="t('documents.modeleDocument.textePiedPagePlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('documents.modeleDocument.estActif')" name="estActif">
      <UToggle v-model="state.estActif" />
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
