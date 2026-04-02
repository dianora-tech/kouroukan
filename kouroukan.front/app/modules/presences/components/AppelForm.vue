<script setup lang="ts">
import { z } from 'zod'
import type { Appel, CreateAppelPayload, UpdateAppelPayload } from '../types/appel.types'

const props = defineProps<{
  entity?: Appel | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateAppelPayload | UpdateAppelPayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  classeId: z.number({ required_error: t('validation.required') }).int().positive(),
  enseignantId: z.number({ required_error: t('validation.required') }).int().positive(),
  seanceId: z.number().int().positive().nullable().optional(),
  dateAppel: z.string().min(1, t('validation.required')),
  heureAppel: z.string().min(1, t('validation.required')),
  estCloture: z.boolean(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  classeId: props.entity?.classeId ?? undefined,
  enseignantId: props.entity?.enseignantId ?? undefined,
  seanceId: props.entity?.seanceId ?? null,
  dateAppel: props.entity?.dateAppel ?? new Date().toISOString().split('T')[0],
  heureAppel: props.entity?.heureAppel ?? new Date().toTimeString().slice(0, 5),
  estCloture: props.entity?.estCloture ?? false,
})

const saving = ref(false)

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateAppelPayload)
    }
    else {
      emit('submit', state as CreateAppelPayload)
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
      :label="t('presences.appel.classeId')"
      name="classeId"
      required
    >
      <UInput
        v-model.number="state.classeId"
        type="number"
        :placeholder="t('presences.appel.classePlaceholder')"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('presences.appel.enseignantId')"
      name="enseignantId"
      required
    >
      <UInput
        v-model.number="state.enseignantId"
        type="number"
        :placeholder="t('presences.appel.enseignantPlaceholder')"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('presences.appel.seanceId')"
      name="seanceId"
    >
      <UInput
        v-model.number="state.seanceId"
        type="number"
        :placeholder="t('presences.appel.seancePlaceholder')"
        class="w-full"
      />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('presences.appel.dateAppel')"
        name="dateAppel"
        required
      >
        <UInput
          v-model="state.dateAppel"
          type="date"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('presences.appel.heureAppel')"
        name="heureAppel"
        required
      >
        <UInput
          v-model="state.heureAppel"
          type="time"
          class="w-full"
        />
      </UFormField>
    </div>

    <UFormField
      :label="t('presences.appel.estCloture')"
      name="estCloture"
    >
      <UToggle v-model="state.estCloture" />
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
