<script setup lang="ts">
import { z } from 'zod'
import type { Seance, CreateSeancePayload, UpdateSeancePayload } from '../types/seance.types'
import { JOURS_SEMAINE } from '../types/seance.types'
import { useClasseStore } from '../stores/classe.store'
import { useMatiereStore } from '../stores/matiere.store'
import { useSalleStore } from '../stores/salle.store'

const props = defineProps<{
  entity?: Seance | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateSeancePayload | UpdateSeancePayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const classeStore = useClasseStore()
const matiereStore = useMatiereStore()
const salleStore = useSalleStore()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  matiereId: z.number({ required_error: t('validation.required') }).int().positive(),
  classeId: z.number({ required_error: t('validation.required') }).int().positive(),
  enseignantId: z.number({ required_error: t('validation.required') }).int().positive(),
  salleId: z.number({ required_error: t('validation.required') }).int().positive(),
  jourSemaine: z.number({ required_error: t('validation.required') }).int().min(1).max(6),
  heureDebut: z.string().min(1, t('validation.required')),
  heureFin: z.string().min(1, t('validation.required')),
  anneeScolaireId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  matiereId: props.entity?.matiereId ?? undefined,
  classeId: props.entity?.classeId ?? undefined,
  enseignantId: props.entity?.enseignantId ?? undefined,
  salleId: props.entity?.salleId ?? undefined,
  jourSemaine: props.entity?.jourSemaine ?? undefined,
  heureDebut: props.entity?.heureDebut ?? '',
  heureFin: props.entity?.heureFin ?? '',
  anneeScolaireId: props.entity?.anneeScolaireId ?? undefined,
})

const saving = ref(false)

const jourOptions = JOURS_SEMAINE.map(j => ({
  label: t(`pedagogie.seance.jour.${j.value}`),
  value: j.value,
}))

const classeOptions = computed(() =>
  classeStore.items.map(c => ({
    label: c.name,
    value: c.id,
  })),
)

const matiereOptions = computed(() =>
  matiereStore.items.map(m => ({
    label: `${m.code} - ${m.name}`,
    value: m.id,
  })),
)

const salleOptions = computed(() =>
  salleStore.items.filter(s => s.estDisponible).map(s => ({
    label: s.name,
    value: s.id,
  })),
)

onMounted(() => {
  if (classeStore.items.length === 0) classeStore.fetchAll({ pageSize: 100 })
  if (matiereStore.items.length === 0) matiereStore.fetchAll({ pageSize: 100 })
  if (salleStore.items.length === 0) salleStore.fetchAll({ pageSize: 100 })
})

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateSeancePayload)
    }
    else {
      emit('submit', state as CreateSeancePayload)
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
      <UFormField :label="t('pedagogie.seance.matiere')" name="matiereId" required>
        <USelect v-model="state.matiereId" :items="matiereOptions" value-key="value" class="w-full" />
      </UFormField>

      <UFormField :label="t('pedagogie.seance.classe')" name="classeId" required>
        <USelect v-model="state.classeId" :items="classeOptions" value-key="value" class="w-full" />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('pedagogie.seance.enseignant')" name="enseignantId" required>
        <UInput v-model.number="state.enseignantId" type="number" :placeholder="t('pedagogie.seance.enseignantPlaceholder')" class="w-full" />
      </UFormField>

      <UFormField :label="t('pedagogie.seance.salle')" name="salleId" required>
        <USelect v-model="state.salleId" :items="salleOptions" value-key="value" class="w-full" />
      </UFormField>
    </div>

    <UFormField :label="t('pedagogie.seance.jourSemaine')" name="jourSemaine" required>
      <USelect v-model="state.jourSemaine" :items="jourOptions" value-key="value" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('pedagogie.seance.heureDebut')" name="heureDebut" required>
        <UInput v-model="state.heureDebut" type="time" class="w-full" />
      </UFormField>

      <UFormField :label="t('pedagogie.seance.heureFin')" name="heureFin" required>
        <UInput v-model="state.heureFin" type="time" class="w-full" />
      </UFormField>
    </div>

    <UFormField :label="t('pedagogie.seance.anneeScolaire')" name="anneeScolaireId" required>
      <UInput v-model.number="state.anneeScolaireId" type="number" :placeholder="t('pedagogie.seance.anneeScolairePlaceholder')" class="w-full" />
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
