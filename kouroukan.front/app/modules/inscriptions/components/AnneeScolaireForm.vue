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

const schema = z.object({
  libelle: z.string().min(1, t('validation.required')).max(20),
  dateDebut: z.string().min(1, t('validation.required')),
  dateFin: z.string().min(1, t('validation.required')),
  estActive: z.boolean(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  libelle: props.entity?.libelle ?? '',
  dateDebut: props.entity?.dateDebut ?? '',
  dateFin: props.entity?.dateFin ?? '',
  estActive: props.entity?.estActive ?? false,
})

const saving = ref(false)

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateAnneeScolairePayload)
    }
    else {
      emit('submit', state as CreateAnneeScolairePayload)
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

    <UFormField :label="t('inscriptions.anneeScolaire.estActive')" name="estActive">
      <UToggle v-model="state.estActive" />
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
