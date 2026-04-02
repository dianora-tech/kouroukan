<script setup lang="ts">
import { z } from 'zod'
import type { CahierTextes, CreateCahierTextesPayload, UpdateCahierTextesPayload } from '../types/cahierTextes.types'
import { useSeanceStore } from '../stores/seance.store'

const props = defineProps<{
  entity?: CahierTextes | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateCahierTextesPayload | UpdateCahierTextesPayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const seanceStore = useSeanceStore()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  seanceId: z.number({ required_error: t('validation.required') }).int().positive(),
  contenu: z.string().min(1, t('validation.required')),
  dateSeance: z.string().min(1, t('validation.required')),
  travailAFaire: z.string().optional().or(z.literal('')),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  seanceId: props.entity?.seanceId ?? undefined,
  contenu: props.entity?.contenu ?? '',
  dateSeance: props.entity?.dateSeance ?? '',
  travailAFaire: props.entity?.travailAFaire ?? '',
})

const saving = ref(false)

const seanceOptions = computed(() =>
  seanceStore.items.map(s => ({
    label: `${s.matiereName ?? ''} - ${s.classeName ?? ''} (${t(`pedagogie.seance.jour.${s.jourSemaine}`)} ${s.heureDebut})`,
    value: s.id,
  })),
)

onMounted(() => {
  if (seanceStore.items.length === 0) seanceStore.fetchAll({ pageSize: 100 })
})

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateCahierTextesPayload)
    }
    else {
      emit('submit', state as CreateCahierTextesPayload)
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
      :label="t('pedagogie.cahierTextes.seance')"
      name="seanceId"
      required
    >
      <USelect
        v-model="state.seanceId"
        :items="seanceOptions"
        value-key="value"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('pedagogie.cahierTextes.dateSeance')"
      name="dateSeance"
      required
    >
      <UInput
        v-model="state.dateSeance"
        type="date"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('pedagogie.cahierTextes.contenu')"
      name="contenu"
      required
    >
      <UTextarea
        v-model="state.contenu"
        :placeholder="t('pedagogie.cahierTextes.contenuPlaceholder')"
        :rows="6"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('pedagogie.cahierTextes.travailAFaire')"
      name="travailAFaire"
    >
      <UTextarea
        v-model="state.travailAFaire"
        :placeholder="t('pedagogie.cahierTextes.travailPlaceholder')"
        :rows="4"
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
