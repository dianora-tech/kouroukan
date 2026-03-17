<script setup lang="ts">
import { z } from 'zod'
import type { DemandeConge, CreateDemandeCongePayload, UpdateDemandeCongePayload } from '../types/demandeConge.types'
import { STATUTS_DEMANDE_CONGE } from '../types/demandeConge.types'
import { useDemandeConge } from '../composables/useDemandeConge'

const props = defineProps<{
  entity?: DemandeConge | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateDemandeCongePayload | UpdateDemandeCongePayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const { typeOptions } = useDemandeConge()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  enseignantId: z.number({ required_error: t('validation.required') }).int().positive(),
  dateDebut: z.string().min(1, t('validation.required')),
  dateFin: z.string().min(1, t('validation.required')),
  motif: z.string().min(3, t('validation.minLength', { min: 3 })),
  statutDemande: z.string().min(1, t('validation.required')),
  pieceJointeUrl: z.string().nullable().optional().or(z.literal('')),
  commentaireValidateur: z.string().nullable().optional().or(z.literal('')),
  impactPaie: z.boolean(),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  enseignantId: props.entity?.enseignantId ?? undefined,
  dateDebut: props.entity?.dateDebut ?? new Date().toISOString().split('T')[0],
  dateFin: props.entity?.dateFin ?? '',
  motif: props.entity?.motif ?? '',
  statutDemande: props.entity?.statutDemande ?? 'Soumise',
  pieceJointeUrl: props.entity?.pieceJointeUrl ?? '',
  commentaireValidateur: props.entity?.commentaireValidateur ?? '',
  impactPaie: props.entity?.impactPaie ?? false,
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

const statutOptions = STATUTS_DEMANDE_CONGE.map(s => ({
  label: t(`personnel.demandeConge.statut.${s}`),
  value: s,
}))

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateDemandeCongePayload)
    }
    else {
      emit('submit', state as CreateDemandeCongePayload)
    }
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <UForm :schema="schema" :state="state" class="space-y-4" @submit="onSubmit">
    <UFormField :label="t('personnel.demandeConge.typeId')" name="typeId" required>
      <USelect v-model="state.typeId" :items="typeOptions" value-key="value" :placeholder="t('personnel.demandeConge.typePlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('personnel.demandeConge.enseignantId')" name="enseignantId" required>
      <UInput v-model.number="state.enseignantId" type="number" :placeholder="t('personnel.demandeConge.enseignantPlaceholder')" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('personnel.demandeConge.dateDebut')" name="dateDebut" required>
        <UInput v-model="state.dateDebut" type="date" class="w-full" />
      </UFormField>

      <UFormField :label="t('personnel.demandeConge.dateFin')" name="dateFin" required>
        <UInput v-model="state.dateFin" type="date" class="w-full" />
      </UFormField>
    </div>

    <UFormField :label="t('personnel.demandeConge.motif')" name="motif" required>
      <UTextarea v-model="state.motif" :placeholder="t('personnel.demandeConge.motifPlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('personnel.demandeConge.statutDemandeLabel')" name="statutDemande" required>
      <USelect v-model="state.statutDemande" :items="statutOptions" value-key="value" class="w-full" />
    </UFormField>

    <UFormField :label="t('personnel.demandeConge.pieceJointeUrl')" name="pieceJointeUrl">
      <UInput v-model="state.pieceJointeUrl" :placeholder="t('personnel.demandeConge.pieceJointePlaceholder')" class="w-full" />
    </UFormField>

    <UFormField v-if="isEdit" :label="t('personnel.demandeConge.commentaireValidateur')" name="commentaireValidateur">
      <UTextarea v-model="state.commentaireValidateur" :placeholder="t('personnel.demandeConge.commentairePlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('personnel.demandeConge.impactPaie')" name="impactPaie">
      <UToggle v-model="state.impactPaie" />
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
