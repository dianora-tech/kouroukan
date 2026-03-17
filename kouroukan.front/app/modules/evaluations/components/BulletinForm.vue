<script setup lang="ts">
import { z } from 'zod'
import type { Bulletin, CreateBulletinPayload, UpdateBulletinPayload } from '../types/bulletin.types'
import { TRIMESTRES_BULLETIN } from '../types/bulletin.types'

const props = defineProps<{
  entity?: Bulletin | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateBulletinPayload | UpdateBulletinPayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  eleveId: z.number({ required_error: t('validation.required') }).int().positive(),
  classeId: z.number({ required_error: t('validation.required') }).int().positive(),
  trimestre: z.number({ required_error: t('validation.required') }).int().min(1).max(3),
  anneeScolaireId: z.number({ required_error: t('validation.required') }).int().positive(),
  moyenneGenerale: z.number({ required_error: t('validation.required') }).min(0).max(20),
  rang: z.number().int().positive().optional().or(z.literal(0)),
  appreciation: z.string().optional().or(z.literal('')),
  estPublie: z.boolean(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  eleveId: props.entity?.eleveId ?? undefined,
  classeId: props.entity?.classeId ?? undefined,
  trimestre: props.entity?.trimestre ?? 1,
  anneeScolaireId: props.entity?.anneeScolaireId ?? undefined,
  moyenneGenerale: props.entity?.moyenneGenerale ?? 0,
  rang: props.entity?.rang ?? undefined,
  appreciation: props.entity?.appreciation ?? '',
  estPublie: props.entity?.estPublie ?? false,
})

const saving = ref(false)

const trimestreOptions = TRIMESTRES_BULLETIN.map(tr => ({
  label: t(`evaluations.bulletin.trimestre.${tr}`),
  value: tr,
}))

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateBulletinPayload)
    }
    else {
      emit('submit', state as CreateBulletinPayload)
    }
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <UForm :schema="schema" :state="state" class="space-y-4" @submit="onSubmit">
    <UFormField :label="t('evaluations.bulletin.eleveId')" name="eleveId" required>
      <UInput v-model.number="state.eleveId" type="number" :placeholder="t('evaluations.bulletin.elevePlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('evaluations.bulletin.classeId')" name="classeId" required>
      <UInput v-model.number="state.classeId" type="number" :placeholder="t('evaluations.bulletin.classePlaceholder')" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('evaluations.bulletin.trimestre')" name="trimestre" required>
        <USelect v-model="state.trimestre" :items="trimestreOptions" value-key="value" class="w-full" />
      </UFormField>

      <UFormField :label="t('evaluations.bulletin.anneeScolaireId')" name="anneeScolaireId" required>
        <UInput v-model.number="state.anneeScolaireId" type="number" :placeholder="t('evaluations.bulletin.anneeScolairePlaceholder')" class="w-full" />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('evaluations.bulletin.moyenneGenerale')" name="moyenneGenerale" required>
        <UInput v-model.number="state.moyenneGenerale" type="number" min="0" max="20" step="0.01" class="w-full" />
      </UFormField>

      <UFormField :label="t('evaluations.bulletin.rang')" name="rang">
        <UInput v-model.number="state.rang" type="number" min="1" :placeholder="t('evaluations.bulletin.rangPlaceholder')" class="w-full" />
      </UFormField>
    </div>

    <UFormField :label="t('evaluations.bulletin.appreciation')" name="appreciation">
      <UTextarea v-model="state.appreciation" :placeholder="t('evaluations.bulletin.appreciationPlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('evaluations.bulletin.estPublie')" name="estPublie">
      <UToggle v-model="state.estPublie" />
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
