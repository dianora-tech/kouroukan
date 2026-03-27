<script setup lang="ts">
import { z } from 'zod'
import type { AnneeScolaire, CreateAnneeScolairePayload, UpdateAnneeScolairePayload } from '../types/annee-scolaire.types'

const props = defineProps<{
  entity?: AnneeScolaire | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateAnneeScolairePayload | UpdateAnneeScolairePayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()

const isEdit = computed(() => !!props.entity?.id)

const statutOptions = [
  { label: 'En préparation', value: 'preparation' },
  { label: 'Active', value: 'active' },
  { label: 'Clôturée', value: 'cloturee' },
  { label: 'Archivée', value: 'archivee' },
]

const typePeriodeOptions = [
  { label: 'Trimestre', value: 'trimestre' },
  { label: 'Semestre', value: 'semestre' },
]

const schema = z.object({
  libelle: z.string().min(1, t('validation.required')).max(20),
  dateDebut: z.string().min(1, t('validation.required')),
  dateFin: z.string().min(1, t('validation.required')),
  code: z.string().min(1, t('validation.required')).max(20),
  description: z.string().max(500).optional().or(z.literal('')),
  statut: z.string().min(1, t('validation.required')),
  dateRentree: z.string().optional().or(z.literal('')),
  nombrePeriodes: z.coerce.number().min(1).default(3),
  typePeriode: z.string().min(1, t('validation.required')),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  libelle: props.entity?.libelle ?? '',
  dateDebut: props.entity?.dateDebut ?? '',
  dateFin: props.entity?.dateFin ?? '',
  code: props.entity?.code ?? '',
  description: props.entity?.description ?? '',
  statut: props.entity?.statut ?? 'preparation',
  dateRentree: props.entity?.dateRentree ?? '',
  nombrePeriodes: props.entity?.nombrePeriodes ?? 3,
  typePeriode: props.entity?.typePeriode ?? 'trimestre',
})

// Sync state when entity is loaded asynchronously (edit mode)
watch(() => props.entity, (entity) => {
  if (entity) {
    state.libelle = entity.libelle ?? ''
    state.dateDebut = entity.dateDebut ?? ''
    state.dateFin = entity.dateFin ?? ''
    state.code = entity.code ?? ''
    state.description = entity.description ?? ''
    state.statut = entity.statut ?? 'preparation'
    state.dateRentree = entity.dateRentree ?? ''
    state.nombrePeriodes = entity.nombrePeriodes ?? 3
    state.typePeriode = entity.typePeriode ?? 'trimestre'
  }
}, { immediate: true })

const saving = ref(false)

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    // Clean empty strings to null for optional date fields
    const payload = {
      ...state,
      dateRentree: state.dateRentree || null,
      description: state.description || null,
    }
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...payload } as UpdateAnneeScolairePayload)
    }
    else {
      emit('submit', payload as CreateAnneeScolairePayload)
    }
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <UForm :schema="schema" :state="state" class="space-y-4" @submit="onSubmit">
    <UFormField :label="t('inscriptions.anneeScolaire.libelle')" name="libelle" required>
      <UInput v-model="state.libelle" :placeholder="t('inscriptions.anneeScolaire.libellePlaceholder')" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('inscriptions.anneeScolaire.dateDebut')" name="dateDebut" required>
        <UInput v-model="state.dateDebut" type="date" class="w-full" />
      </UFormField>

      <UFormField :label="t('inscriptions.anneeScolaire.dateFin')" name="dateFin" required>
        <UInput v-model="state.dateFin" type="date" class="w-full" />
      </UFormField>
    </div>

    <UFormField label="Code" name="code" required>
      <UInput v-model="state.code" placeholder="Ex: 2024-2025" maxlength="20" class="w-full" />
    </UFormField>

    <UFormField label="Description" name="description">
      <UTextarea v-model="state.description" maxlength="500" class="w-full" />
    </UFormField>

    <UFormField label="Statut" name="statut" required>
      <USelect v-model="state.statut" :items="statutOptions" class="w-full" />
    </UFormField>

    <UFormField label="Date de rentrée" name="dateRentree">
      <UInput v-model="state.dateRentree" type="date" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField label="Nombre de périodes" name="nombrePeriodes" required>
        <UInput v-model.number="state.nombrePeriodes" type="number" min="1" class="w-full" />
      </UFormField>

      <UFormField label="Type de période" name="typePeriode" required>
        <USelect v-model="state.typePeriode" :items="typePeriodeOptions" class="w-full" />
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
