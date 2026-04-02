<script setup lang="ts">
import { z } from 'zod'
import type { Notification, CreateNotificationPayload, UpdateNotificationPayload } from '../types/notification.types'
import { CANAUX_NOTIFICATION } from '../types/notification.types'
import { useNotificationStore } from '../stores/notification.store'

const props = defineProps<{
  entity?: Notification | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateNotificationPayload | UpdateNotificationPayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const store = useNotificationStore()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  destinatairesIds: z.string().min(1, t('validation.required')),
  contenu: z.string().min(1, t('validation.required')).max(500),
  canal: z.string().min(1, t('validation.required')),
  lienAction: z.string().url().optional().or(z.literal('')),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  destinatairesIds: props.entity?.destinatairesIds ?? '',
  contenu: props.entity?.contenu ?? '',
  canal: props.entity?.canal ?? undefined,
  lienAction: props.entity?.lienAction ?? '',
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

const canalOptions = CANAUX_NOTIFICATION.map(c => ({
  label: t(`communication.notification.canal.${c}`),
  value: c,
}))

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
      emit('submit', { id: props.entity.id, ...state } as UpdateNotificationPayload)
    }
    else {
      emit('submit', state as CreateNotificationPayload)
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
      :label="t('communication.notification.contenu')"
      name="contenu"
      required
    >
      <UTextarea
        v-model="state.contenu"
        :placeholder="t('communication.notification.contenuPlaceholder')"
        :rows="3"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('communication.notification.destinatairesIds')"
      name="destinatairesIds"
      required
    >
      <UTextarea
        v-model="state.destinatairesIds"
        :placeholder="t('communication.notification.destinatairesIdsPlaceholder')"
        :rows="2"
        class="w-full"
      />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('communication.notification.canal')"
        name="canal"
        required
      >
        <USelect
          v-model="state.canal"
          :items="canalOptions"
          value-key="value"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('communication.notification.typeId')"
        name="typeId"
        required
      >
        <USelect
          v-model="state.typeId"
          :items="typeOptions"
          value-key="value"
          class="w-full"
        />
      </UFormField>
    </div>

    <UFormField
      :label="t('communication.notification.lienAction')"
      name="lienAction"
    >
      <UInput
        v-model="state.lienAction"
        type="url"
        :placeholder="t('communication.notification.lienActionPlaceholder')"
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
