<script setup lang="ts">
import { z } from 'zod'
import type { Message, CreateMessagePayload, UpdateMessagePayload } from '../types/message.types'
import { TYPES_MESSAGE } from '../types/message.types'
import { useMessageStore } from '../stores/message.store'

const props = defineProps<{
  entity?: Message | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateMessagePayload | UpdateMessagePayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const store = useMessageStore()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  expediteurId: z.number({ required_error: t('validation.required') }).int().positive(),
  destinataireId: z.number().int().positive().optional().nullable(),
  sujet: z.string().min(1, t('validation.required')).max(200),
  contenu: z.string().min(1, t('validation.required')),
  groupeDestinataire: z.string().max(100).optional().or(z.literal('')),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  expediteurId: props.entity?.expediteurId ?? undefined,
  destinataireId: props.entity?.destinataireId ?? undefined,
  sujet: props.entity?.sujet ?? '',
  contenu: props.entity?.contenu ?? '',
  groupeDestinataire: props.entity?.groupeDestinataire ?? '',
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

const typeOptions = computed(() =>
  store.types.map(t => ({
    label: t.name,
    value: t.id,
  })),
)

onMounted(() => {
  if (store.types.length === 0) {
    store.fetchTypes()
  }
})

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateMessagePayload)
    }
    else {
      emit('submit', state as CreateMessagePayload)
    }
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <UForm :schema="schema" :state="state" class="space-y-4" @submit="onSubmit">
    <UFormField :label="t('communication.message.sujet')" name="sujet" required>
      <UInput v-model="state.sujet" :placeholder="t('communication.message.sujetPlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('communication.message.contenu')" name="contenu" required>
      <UTextarea v-model="state.contenu" :placeholder="t('communication.message.contenuPlaceholder')" :rows="5" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('communication.message.expediteurId')" name="expediteurId" required>
        <UInput v-model.number="state.expediteurId" type="number" :placeholder="t('communication.message.expediteurIdPlaceholder')" class="w-full" />
      </UFormField>

      <UFormField :label="t('communication.message.destinataireId')" name="destinataireId">
        <UInput v-model.number="state.destinataireId" type="number" :placeholder="t('communication.message.destinataireIdPlaceholder')" class="w-full" />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('communication.message.typeId')" name="typeId" required>
        <USelect v-model="state.typeId" :items="typeOptions" value-key="value" class="w-full" />
      </UFormField>

      <UFormField :label="t('communication.message.groupeDestinataire')" name="groupeDestinataire">
        <UInput v-model="state.groupeDestinataire" :placeholder="t('communication.message.groupeDestinatairePlaceholder')" class="w-full" />
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
