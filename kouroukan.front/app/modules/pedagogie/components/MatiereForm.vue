<script setup lang="ts">
import { z } from 'zod'
import type { Matiere, CreateMatierePayload, UpdateMatierePayload } from '../types/matiere.types'
import { useMatiereStore } from '../stores/matiere.store'
import { useNiveauClasseStore } from '../stores/niveauClasse.store'

const props = defineProps<{
  entity?: Matiere | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateMatierePayload | UpdateMatierePayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const matiereStore = useMatiereStore()
const niveauClasseStore = useNiveauClasseStore()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  name: z.string().min(1, t('validation.required')).max(100),
  code: z.string().min(1, t('validation.required')).max(20),
  coefficient: z.number({ required_error: t('validation.required') }).positive(),
  nombreHeures: z.number({ required_error: t('validation.required') }).int().positive(),
  niveauClasseId: z.number({ required_error: t('validation.required') }).int().positive(),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  name: props.entity?.name ?? '',
  code: props.entity?.code ?? '',
  coefficient: props.entity?.coefficient ?? undefined,
  nombreHeures: props.entity?.nombreHeures ?? undefined,
  niveauClasseId: props.entity?.niveauClasseId ?? undefined,
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

const typeOptions = computed(() =>
  matiereStore.types.map(t => ({
    label: t.name,
    value: t.id,
  })),
)

const niveauClasseOptions = computed(() =>
  niveauClasseStore.items.map(n => ({
    label: `${n.code} - ${n.name}`,
    value: n.id,
  })),
)

onMounted(() => {
  if (matiereStore.types.length === 0) matiereStore.fetchTypes()
  if (niveauClasseStore.items.length === 0) niveauClasseStore.fetchAll({ pageSize: 100 })
})

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateMatierePayload)
    }
    else {
      emit('submit', state as CreateMatierePayload)
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
      <UFormField :label="t('pedagogie.matiere.name')" name="name" required>
        <UInput v-model="state.name" :placeholder="t('pedagogie.matiere.namePlaceholder')" class="w-full" />
      </UFormField>

      <UFormField :label="t('pedagogie.matiere.code')" name="code" required>
        <UInput v-model="state.code" :placeholder="t('pedagogie.matiere.codePlaceholder')" class="w-full" />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('pedagogie.matiere.coefficient')" name="coefficient" required>
        <UInput v-model.number="state.coefficient" type="number" step="0.5" :placeholder="t('pedagogie.matiere.coefficientPlaceholder')" class="w-full" />
      </UFormField>

      <UFormField :label="t('pedagogie.matiere.nombreHeures')" name="nombreHeures" required>
        <UInput v-model.number="state.nombreHeures" type="number" :placeholder="t('pedagogie.matiere.heuresPlaceholder')" class="w-full" />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('pedagogie.matiere.niveauClasse')" name="niveauClasseId" required>
        <USelect v-model="state.niveauClasseId" :items="niveauClasseOptions" value-key="value" class="w-full" />
      </UFormField>

      <UFormField :label="t('pedagogie.matiere.type')" name="typeId" required>
        <USelect v-model="state.typeId" :items="typeOptions" value-key="value" class="w-full" />
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
