<script setup lang="ts">
import { z } from 'zod'
import type { Salle, CreateSallePayload, UpdateSallePayload } from '../types/salle.types'
import { useSalleStore } from '../stores/salle.store'

const props = defineProps<{
  entity?: Salle | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateSallePayload | UpdateSallePayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const salleStore = useSalleStore()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  name: z.string().min(1, t('validation.required')).max(100),
  capacite: z.number({ required_error: t('validation.required') }).int().positive(),
  batiment: z.string().max(100).optional().or(z.literal('')),
  equipements: z.string().optional().or(z.literal('')),
  estDisponible: z.boolean(),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  name: props.entity?.name ?? '',
  capacite: props.entity?.capacite ?? undefined,
  batiment: props.entity?.batiment ?? '',
  equipements: props.entity?.equipements ?? '',
  estDisponible: props.entity?.estDisponible ?? true,
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

const typeOptions = computed(() =>
  salleStore.types.map(t => ({
    label: t.name,
    value: t.id,
  })),
)

onMounted(() => {
  if (salleStore.types.length === 0) salleStore.fetchTypes()
})

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateSallePayload)
    }
    else {
      emit('submit', state as CreateSallePayload)
    }
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <UForm :schema="schema" :state="state" class="space-y-4" @submit="onSubmit">
    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('pedagogie.salle.name')" name="name" required>
        <UInput v-model="state.name" :placeholder="t('pedagogie.salle.namePlaceholder')" class="w-full" />
      </UFormField>

      <UFormField :label="t('pedagogie.salle.type')" name="typeId" required>
        <USelect v-model="state.typeId" :items="typeOptions" value-key="value" class="w-full" />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('pedagogie.salle.capacite')" name="capacite" required>
        <UInput v-model.number="state.capacite" type="number" :placeholder="t('pedagogie.salle.capacitePlaceholder')" class="w-full" />
      </UFormField>

      <UFormField :label="t('pedagogie.salle.batiment')" name="batiment">
        <UInput v-model="state.batiment" :placeholder="t('pedagogie.salle.batimentPlaceholder')" class="w-full" />
      </UFormField>
    </div>

    <UFormField :label="t('pedagogie.salle.equipements')" name="equipements">
      <UTextarea v-model="state.equipements" :placeholder="t('pedagogie.salle.equipementsPlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('pedagogie.salle.estDisponible')" name="estDisponible">
      <UToggle v-model="state.estDisponible" />
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
