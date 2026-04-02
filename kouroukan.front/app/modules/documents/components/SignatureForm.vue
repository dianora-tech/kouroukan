<script setup lang="ts">
import { z } from 'zod'
import type { Signature, CreateSignaturePayload, UpdateSignaturePayload } from '../types/signature.types'
import { STATUTS_SIGNATURE, NIVEAUX_SIGNATURE } from '../types/signature.types'
import { useSignature } from '../composables/useSignature'

const props = defineProps<{
  entity?: Signature | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateSignaturePayload | UpdateSignaturePayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const { typeOptions } = useSignature()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  name: z.string().min(3, t('validation.required')),
  description: z.string().optional().nullable().or(z.literal('')),
  documentGenereId: z.number({ required_error: t('validation.required') }).int().positive(),
  signataireId: z.number({ required_error: t('validation.required') }).int().positive(),
  ordreSignature: z.number({ required_error: t('validation.required') }).int().min(1),
  statutSignature: z.string().min(1, t('validation.required')),
  niveauSignature: z.string().min(1, t('validation.required')),
  motifRefus: z.string().optional().nullable().or(z.literal('')),
  estValidee: z.boolean(),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  name: props.entity?.name ?? '',
  description: props.entity?.description ?? '',
  documentGenereId: props.entity?.documentGenereId ?? undefined,
  signataireId: props.entity?.signataireId ?? undefined,
  ordreSignature: props.entity?.ordreSignature ?? 1,
  statutSignature: props.entity?.statutSignature ?? 'EnAttente',
  niveauSignature: props.entity?.niveauSignature ?? 'Basique',
  motifRefus: props.entity?.motifRefus ?? '',
  estValidee: props.entity?.estValidee ?? false,
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

const statutOptions = STATUTS_SIGNATURE.map(s => ({
  label: t(`documents.signature.statut.${s}`),
  value: s,
}))

const niveauOptions = NIVEAUX_SIGNATURE.map(n => ({
  label: t(`documents.signature.niveau.${n}`),
  value: n,
}))

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateSignaturePayload)
    }
    else {
      emit('submit', state as CreateSignaturePayload)
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
      :label="t('documents.signature.typeId')"
      name="typeId"
      required
    >
      <USelect
        v-model="state.typeId"
        :items="typeOptions"
        value-key="value"
        :placeholder="t('documents.signature.typePlaceholder')"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('documents.signature.name')"
      name="name"
      required
    >
      <UInput
        v-model="state.name"
        :placeholder="t('documents.signature.namePlaceholder')"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('documents.signature.description')"
      name="description"
    >
      <UTextarea
        v-model="state.description"
        :placeholder="t('documents.signature.descriptionPlaceholder')"
        class="w-full"
      />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('documents.signature.documentGenereId')"
        name="documentGenereId"
        required
      >
        <UInput
          v-model.number="state.documentGenereId"
          type="number"
          min="1"
          :placeholder="t('documents.signature.documentGenereIdPlaceholder')"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('documents.signature.signataireId')"
        name="signataireId"
        required
      >
        <UInput
          v-model.number="state.signataireId"
          type="number"
          min="1"
          :placeholder="t('documents.signature.signataireIdPlaceholder')"
          class="w-full"
        />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('documents.signature.ordreSignature')"
        name="ordreSignature"
        required
      >
        <UInput
          v-model.number="state.ordreSignature"
          type="number"
          min="1"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('documents.signature.niveauSignature')"
        name="niveauSignature"
        required
      >
        <USelect
          v-model="state.niveauSignature"
          :items="niveauOptions"
          value-key="value"
          class="w-full"
        />
      </UFormField>
    </div>

    <UFormField
      :label="t('documents.signature.statutSignature')"
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

    <UFormField
      v-if="state.statutSignature === 'Refuse'"
      :label="t('documents.signature.motifRefus')"
      name="motifRefus"
    >
      <UTextarea
        v-model="state.motifRefus"
        :placeholder="t('documents.signature.motifRefusPlaceholder')"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('documents.signature.estValidee')"
      name="estValidee"
    >
      <UToggle v-model="state.estValidee" />
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
