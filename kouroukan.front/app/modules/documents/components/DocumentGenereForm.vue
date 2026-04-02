<script setup lang="ts">
import { z } from 'zod'
import type { DocumentGenere, CreateDocumentGenerePayload, UpdateDocumentGenerePayload } from '../types/document-genere.types'
import { STATUTS_SIGNATURE_DOCUMENT } from '../types/document-genere.types'
import { useDocumentGenere } from '../composables/useDocumentGenere'

const props = defineProps<{
  entity?: DocumentGenere | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateDocumentGenerePayload | UpdateDocumentGenerePayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const { typeOptions } = useDocumentGenere()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  name: z.string().min(3, t('validation.required')),
  description: z.string().optional().nullable().or(z.literal('')),
  modeleDocumentId: z.number({ required_error: t('validation.required') }).int().positive(),
  eleveId: z.number().int().positive().optional().nullable(),
  enseignantId: z.number().int().positive().optional().nullable(),
  donneesJson: z.string().min(1, t('validation.required')),
  dateGeneration: z.string().min(1, t('validation.required')),
  statutSignature: z.string().min(1, t('validation.required')),
  cheminFichier: z.string().optional().nullable().or(z.literal('')),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  name: props.entity?.name ?? '',
  description: props.entity?.description ?? '',
  modeleDocumentId: props.entity?.modeleDocumentId ?? undefined,
  eleveId: props.entity?.eleveId ?? null,
  enseignantId: props.entity?.enseignantId ?? null,
  donneesJson: props.entity?.donneesJson ?? '{}',
  dateGeneration: props.entity?.dateGeneration?.split('T')[0] ?? new Date().toISOString().split('T')[0],
  statutSignature: props.entity?.statutSignature ?? 'EnAttente',
  cheminFichier: props.entity?.cheminFichier ?? '',
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

const statutOptions = STATUTS_SIGNATURE_DOCUMENT.map(s => ({
  label: t(`documents.documentGenere.statut.${s}`),
  value: s,
}))

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateDocumentGenerePayload)
    }
    else {
      emit('submit', state as CreateDocumentGenerePayload)
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
      :label="t('documents.documentGenere.typeId')"
      name="typeId"
      required
    >
      <USelect
        v-model="state.typeId"
        :items="typeOptions"
        value-key="value"
        :placeholder="t('documents.documentGenere.typePlaceholder')"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('documents.documentGenere.name')"
      name="name"
      required
    >
      <UInput
        v-model="state.name"
        :placeholder="t('documents.documentGenere.namePlaceholder')"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('documents.documentGenere.description')"
      name="description"
    >
      <UTextarea
        v-model="state.description"
        :placeholder="t('documents.documentGenere.descriptionPlaceholder')"
        class="w-full"
      />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('documents.documentGenere.modeleDocumentId')"
        name="modeleDocumentId"
        required
      >
        <UInput
          v-model.number="state.modeleDocumentId"
          type="number"
          min="1"
          :placeholder="t('documents.documentGenere.modeleDocumentIdPlaceholder')"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('documents.documentGenere.dateGeneration')"
        name="dateGeneration"
        required
      >
        <UInput
          v-model="state.dateGeneration"
          type="date"
          class="w-full"
        />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('documents.documentGenere.eleveId')"
        name="eleveId"
      >
        <UInput
          v-model.number="state.eleveId"
          type="number"
          min="1"
          :placeholder="t('documents.documentGenere.eleveIdPlaceholder')"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('documents.documentGenere.enseignantId')"
        name="enseignantId"
      >
        <UInput
          v-model.number="state.enseignantId"
          type="number"
          min="1"
          :placeholder="t('documents.documentGenere.enseignantIdPlaceholder')"
          class="w-full"
        />
      </UFormField>
    </div>

    <UFormField
      :label="t('documents.documentGenere.donneesJson')"
      name="donneesJson"
      required
    >
      <UTextarea
        v-model="state.donneesJson"
        :placeholder="t('documents.documentGenere.donneesJsonPlaceholder')"
        :rows="6"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('documents.documentGenere.statutSignature')"
      name="statutSignature"
      required
    >
      <USelect
        v-model="state.statutSignature"
        :items="statutOptions"
        value-key="value"
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
