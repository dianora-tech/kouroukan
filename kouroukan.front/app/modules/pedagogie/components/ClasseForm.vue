<script setup lang="ts">
import { z } from 'zod'
import type { Classe, CreateClassePayload, UpdateClassePayload } from '../types/classe.types'
import { useNiveauClasseStore } from '../stores/niveauClasse.store'

const props = defineProps<{
  entity?: Classe | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateClassePayload | UpdateClassePayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const niveauClasseStore = useNiveauClasseStore()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  name: z.string().min(1, t('validation.required')).max(100),
  niveauClasseId: z.number({ required_error: t('validation.required') }).int().positive(),
  capacite: z.number({ required_error: t('validation.required') }).int().positive(),
  anneeScolaireId: z.number({ required_error: t('validation.required') }).int().positive(),
  enseignantPrincipalId: z.number().int().positive().optional().nullable(),
  effectif: z.number({ required_error: t('validation.required') }).int().min(0),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  name: props.entity?.name ?? '',
  niveauClasseId: props.entity?.niveauClasseId ?? undefined,
  capacite: props.entity?.capacite ?? undefined,
  anneeScolaireId: props.entity?.anneeScolaireId ?? undefined,
  enseignantPrincipalId: props.entity?.enseignantPrincipalId ?? undefined,
  effectif: props.entity?.effectif ?? 0,
})

const saving = ref(false)

const niveauClasseOptions = computed(() =>
  niveauClasseStore.items.map(n => ({
    label: `${n.code} - ${n.name}`,
    value: n.id,
  })),
)

onMounted(() => {
  if (niveauClasseStore.items.length === 0) {
    niveauClasseStore.fetchAll({ pageSize: 100 })
  }
})

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateClassePayload)
    }
    else {
      emit('submit', state as CreateClassePayload)
    }
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <UForm :schema="schema" :state="state" class="space-y-4" @submit="onSubmit">
    <UFormField :label="t('pedagogie.classe.name')" name="name" required>
      <UInput v-model="state.name" :placeholder="t('pedagogie.classe.namePlaceholder')" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('pedagogie.classe.niveauClasse')" name="niveauClasseId" required>
        <USelect v-model="state.niveauClasseId" :items="niveauClasseOptions" value-key="value" class="w-full" />
      </UFormField>

      <UFormField :label="t('pedagogie.classe.anneeScolaire')" name="anneeScolaireId" required>
        <UInput v-model.number="state.anneeScolaireId" type="number" :placeholder="t('pedagogie.classe.anneeScolairePlaceholder')" class="w-full" />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('pedagogie.classe.capacite')" name="capacite" required>
        <UInput v-model.number="state.capacite" type="number" :placeholder="t('pedagogie.classe.capacitePlaceholder')" class="w-full" />
      </UFormField>

      <UFormField :label="t('pedagogie.classe.effectif')" name="effectif" required>
        <UInput v-model.number="state.effectif" type="number" :placeholder="t('pedagogie.classe.effectifPlaceholder')" class="w-full" />
      </UFormField>
    </div>

    <UFormField :label="t('pedagogie.classe.enseignantPrincipal')" name="enseignantPrincipalId">
      <UInput v-model.number="state.enseignantPrincipalId" type="number" :placeholder="t('pedagogie.classe.enseignantPlaceholder')" class="w-full" />
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
