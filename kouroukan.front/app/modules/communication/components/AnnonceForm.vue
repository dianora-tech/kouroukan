<script setup lang="ts">
import { z } from 'zod'
import type { Annonce, CreateAnnoncePayload, UpdateAnnoncePayload } from '../types/annonce.types'
import { CIBLES_AUDIENCE } from '../types/annonce.types'
import { useAnnonceStore } from '../stores/annonce.store'

const props = defineProps<{
  entity?: Annonce | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateAnnoncePayload | UpdateAnnoncePayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const store = useAnnonceStore()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  contenu: z.string().min(1, t('validation.required')),
  dateDebut: z.string().min(1, t('validation.required')),
  dateFin: z.string().optional().or(z.literal('')),
  estActive: z.boolean(),
  cibleAudience: z.string().min(1, t('validation.required')),
  priorite: z.number({ required_error: t('validation.required') }).int().min(1).max(5),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  contenu: props.entity?.contenu ?? '',
  dateDebut: props.entity?.dateDebut ?? '',
  dateFin: props.entity?.dateFin ?? '',
  estActive: props.entity?.estActive ?? true,
  cibleAudience: props.entity?.cibleAudience ?? undefined,
  priorite: props.entity?.priorite ?? 3,
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

const cibleOptions = CIBLES_AUDIENCE.map(c => ({
  label: t(`communication.annonce.cible.${c}`),
  value: c,
}))

const prioriteOptions = [1, 2, 3, 4, 5].map(p => ({
  label: t(`communication.annonce.priorite.${p}`),
  value: p,
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
      emit('submit', { id: props.entity.id, ...state } as UpdateAnnoncePayload)
    }
    else {
      emit('submit', state as CreateAnnoncePayload)
    }
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <UForm :schema="schema" :state="state" class="space-y-4" @submit="onSubmit">
    <UFormField :label="t('communication.annonce.contenu')" name="contenu" required>
      <UTextarea v-model="state.contenu" :placeholder="t('communication.annonce.contenuPlaceholder')" :rows="5" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('communication.annonce.dateDebut')" name="dateDebut" required>
        <UInput v-model="state.dateDebut" type="date" class="w-full" />
      </UFormField>

      <UFormField :label="t('communication.annonce.dateFin')" name="dateFin">
        <UInput v-model="state.dateFin" type="date" class="w-full" />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('communication.annonce.cibleAudience')" name="cibleAudience" required>
        <USelect v-model="state.cibleAudience" :items="cibleOptions" value-key="value" class="w-full" />
      </UFormField>

      <UFormField :label="t('communication.annonce.typeId')" name="typeId" required>
        <USelect v-model="state.typeId" :items="typeOptions" value-key="value" class="w-full" />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('communication.annonce.prioriteLabel')" name="priorite" required>
        <USelect v-model="state.priorite" :items="prioriteOptions" value-key="value" class="w-full" />
      </UFormField>

      <UFormField :label="t('communication.annonce.estActive')" name="estActive">
        <UToggle v-model="state.estActive" />
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
