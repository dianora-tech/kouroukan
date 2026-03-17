<script setup lang="ts">
import { z } from 'zod'
import type { NiveauClasse, CreateNiveauClassePayload, UpdateNiveauClassePayload } from '../types/niveauClasse.types'
import { CYCLES_ETUDE, MINISTERES_TUTELLE, EXAMENS_SORTIE } from '../types/niveauClasse.types'

const props = defineProps<{
  entity?: NiveauClasse | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateNiveauClassePayload | UpdateNiveauClassePayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  name: z.string().min(1, t('validation.required')).max(100),
  code: z.string().min(1, t('validation.required')).max(20),
  ordre: z.number({ required_error: t('validation.required') }).int().positive(),
  cycleEtude: z.string().min(1, t('validation.required')),
  ageOfficielEntree: z.number().int().positive().optional().nullable(),
  ministereTutelle: z.string().optional().or(z.literal('')),
  examenSortie: z.string().optional().or(z.literal('')),
  tauxHoraireEnseignant: z.number().positive().optional().nullable(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  name: props.entity?.name ?? '',
  code: props.entity?.code ?? '',
  ordre: props.entity?.ordre ?? undefined,
  cycleEtude: props.entity?.cycleEtude ?? '',
  ageOfficielEntree: props.entity?.ageOfficielEntree ?? undefined,
  ministereTutelle: props.entity?.ministereTutelle ?? '',
  examenSortie: props.entity?.examenSortie ?? '',
  tauxHoraireEnseignant: props.entity?.tauxHoraireEnseignant ?? undefined,
})

const saving = ref(false)

const cycleOptions = CYCLES_ETUDE.map(c => ({
  label: t(`pedagogie.niveauClasse.cycle.${c}`),
  value: c,
}))

const ministereOptions = [
  { label: t('pedagogie.niveauClasse.filters.allMinisteres'), value: '' },
  ...MINISTERES_TUTELLE.map(m => ({
    label: m,
    value: m,
  })),
]

const examenOptions = [
  { label: t('pedagogie.niveauClasse.filters.noExamen'), value: '' },
  ...EXAMENS_SORTIE.map(e => ({
    label: e,
    value: e,
  })),
]

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateNiveauClassePayload)
    }
    else {
      emit('submit', state as CreateNiveauClassePayload)
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
      <UFormField :label="t('pedagogie.niveauClasse.name')" name="name" required>
        <UInput v-model="state.name" :placeholder="t('pedagogie.niveauClasse.namePlaceholder')" class="w-full" />
      </UFormField>

      <UFormField :label="t('pedagogie.niveauClasse.code')" name="code" required>
        <UInput v-model="state.code" :placeholder="t('pedagogie.niveauClasse.codePlaceholder')" class="w-full" />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('pedagogie.niveauClasse.ordre')" name="ordre" required>
        <UInput v-model.number="state.ordre" type="number" :placeholder="t('pedagogie.niveauClasse.ordrePlaceholder')" class="w-full" />
      </UFormField>

      <UFormField :label="t('pedagogie.niveauClasse.cycleEtude')" name="cycleEtude" required>
        <USelect v-model="state.cycleEtude" :items="cycleOptions" value-key="value" class="w-full" />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('pedagogie.niveauClasse.ageOfficielEntree')" name="ageOfficielEntree">
        <UInput v-model.number="state.ageOfficielEntree" type="number" :placeholder="t('pedagogie.niveauClasse.agePlaceholder')" class="w-full" />
      </UFormField>

      <UFormField :label="t('pedagogie.niveauClasse.ministereTutelle')" name="ministereTutelle">
        <USelect v-model="state.ministereTutelle" :items="ministereOptions" value-key="value" class="w-full" />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('pedagogie.niveauClasse.examenSortie')" name="examenSortie">
        <USelect v-model="state.examenSortie" :items="examenOptions" value-key="value" class="w-full" />
      </UFormField>

      <UFormField :label="t('pedagogie.niveauClasse.tauxHoraireEnseignant')" name="tauxHoraireEnseignant">
        <UInput v-model.number="state.tauxHoraireEnseignant" type="number" :placeholder="t('pedagogie.niveauClasse.tauxPlaceholder')" class="w-full" />
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
