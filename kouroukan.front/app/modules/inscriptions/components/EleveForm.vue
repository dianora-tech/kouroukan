<script setup lang="ts">
import { z } from 'zod'
import type { Eleve, CreateElevePayload, UpdateElevePayload } from '../types/eleve.types'
import { STATUTS_INSCRIPTION_ELEVE, GENRES } from '../types/eleve.types'

const props = defineProps<{
  entity?: Eleve | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateElevePayload | UpdateElevePayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  firstName: z.string().min(1, t('validation.required')).max(100),
  lastName: z.string().min(1, t('validation.required')).max(100),
  dateNaissance: z.string().min(1, t('validation.required')),
  lieuNaissance: z.string().min(1, t('validation.required')).max(200),
  genre: z.enum(['M', 'F'], { required_error: t('validation.required') }),
  nationalite: z.string().min(1, t('validation.required')).max(50),
  adresse: z.string().max(500).optional().or(z.literal('')),
  photoUrl: z.string().url().optional().or(z.literal('')),
  numeroMatricule: z.string().min(1, t('validation.required')).max(50),
  niveauClasseId: z.number({ required_error: t('validation.required') }).int().positive(),
  classeId: z.number().int().positive().optional().nullable(),
  parentId: z.number().int().positive().optional().nullable(),
  statutInscription: z.string().min(1, t('validation.required')),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  firstName: props.entity?.firstName ?? '',
  lastName: props.entity?.lastName ?? '',
  dateNaissance: props.entity?.dateNaissance ?? '',
  lieuNaissance: props.entity?.lieuNaissance ?? '',
  genre: (props.entity?.genre as 'M' | 'F') ?? undefined,
  nationalite: props.entity?.nationalite ?? 'Guineenne',
  adresse: props.entity?.adresse ?? '',
  photoUrl: props.entity?.photoUrl ?? '',
  numeroMatricule: props.entity?.numeroMatricule ?? '',
  niveauClasseId: props.entity?.niveauClasseId ?? undefined,
  classeId: props.entity?.classeId ?? undefined,
  parentId: props.entity?.parentId ?? undefined,
  statutInscription: props.entity?.statutInscription ?? 'Prospect',
})

const saving = ref(false)

const genreOptions = GENRES.map(g => ({
  label: t(`inscriptions.eleve.genre${g}`),
  value: g,
}))

const statutOptions = STATUTS_INSCRIPTION_ELEVE.map(s => ({
  label: t(`inscriptions.eleve.statut.${s}`),
  value: s,
}))

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateElevePayload)
    }
    else {
      emit('submit', state as CreateElevePayload)
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
      <UFormField :label="t('inscriptions.eleve.firstName')" name="firstName" required>
        <UInput v-model="state.firstName" :placeholder="t('inscriptions.eleve.firstNamePlaceholder')" class="w-full" />
      </UFormField>

      <UFormField :label="t('inscriptions.eleve.lastName')" name="lastName" required>
        <UInput v-model="state.lastName" :placeholder="t('inscriptions.eleve.lastNamePlaceholder')" class="w-full" />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('inscriptions.eleve.dateNaissance')" name="dateNaissance" required>
        <UInput v-model="state.dateNaissance" type="date" class="w-full" />
      </UFormField>

      <UFormField :label="t('inscriptions.eleve.lieuNaissance')" name="lieuNaissance" required>
        <UInput v-model="state.lieuNaissance" :placeholder="t('inscriptions.eleve.lieuNaissancePlaceholder')" class="w-full" />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('inscriptions.eleve.genre')" name="genre" required>
        <USelect v-model="state.genre" :items="genreOptions" value-key="value" class="w-full" />
      </UFormField>

      <UFormField :label="t('inscriptions.eleve.nationalite')" name="nationalite" required>
        <UInput v-model="state.nationalite" :placeholder="t('inscriptions.eleve.nationalitePlaceholder')" class="w-full" />
      </UFormField>
    </div>

    <UFormField :label="t('inscriptions.eleve.adresse')" name="adresse">
      <UTextarea v-model="state.adresse" :placeholder="t('inscriptions.eleve.adressePlaceholder')" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('inscriptions.eleve.numeroMatricule')" name="numeroMatricule" required>
        <UInput v-model="state.numeroMatricule" :placeholder="t('inscriptions.eleve.matriculePlaceholder')" class="w-full" />
      </UFormField>

      <UFormField :label="t('inscriptions.eleve.statutInscription')" name="statutInscription" required>
        <USelect v-model="state.statutInscription" :items="statutOptions" value-key="value" class="w-full" />
      </UFormField>
    </div>

    <UFormField :label="t('inscriptions.eleve.photoUrl')" name="photoUrl">
      <UInput v-model="state.photoUrl" type="url" :placeholder="t('inscriptions.eleve.photoUrlPlaceholder')" class="w-full" />
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
